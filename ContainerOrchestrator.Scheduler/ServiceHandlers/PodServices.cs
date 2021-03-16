using ContainerOrchestrator.Base;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Orcastrate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static Orcastrate.Orcastrater;

namespace ContainerOrchestrator.Scheduler.ServiceHandlers
{
    public class PodServices
    {
        private string serverAddress;
        private OrcastraterClient client;

        public PodServices(string serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        public async Task UpdatePodsAsync(List<Pod> pods)
        {
            client = ConnectionHandler.GetOrchasterClient(serverAddress);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(500));

            var request = new GenericMessage() { Content = JsonSerializer.Serialize(pods) };

            using var reply = client.UpdatePods(request, cancellationToken: cts.Token);
            try
            {
                await foreach (var nodeStatus in reply.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
                {
                    var resp = JsonSerializer.Deserialize<Base.GenericResponse>(nodeStatus.Content);
                    Console.WriteLine(resp.Successful);
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream timed out, stream cancelled.");
            }

        }

        public async Task RecievePodsFromApi()
        {
            try {
                client = ConnectionHandler.GetOrchasterClient(serverAddress);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(500));

                var response = client.RecievePods(cancellationToken: cts.Token);

                while (await response.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
                {
                    var currentContent = response.ResponseStream.Current;

                    var pendingPods = JsonSerializer.Deserialize<List<Pod>>(currentContent.Content);

                    var ns = new NodeServices(serverAddress);
                    var nodes = await ns.GetNodeStatusAsync();
                    Console.WriteLine("Nodes in a list:");
                    nodes.ForEach(x => { Console.WriteLine(x.ToString()); });
                    List<Pod> scheduledPods = SchedulePods(pendingPods, nodes);

                    Console.WriteLine("\n Scheduled pods:");
                    scheduledPods.ForEach(x => { Console.WriteLine(x.ToString()); });
                    var ps = new PodServices(serverAddress);
                    await ps.UpdatePodsAsync(scheduledPods);

                    Console.Write("Request was processed");

                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            finally
            {
                await ConnectionHandler.CloseChannelAsync();
            }
           
        }

        private List<Pod> SchedulePods(List<Pod> pendingPods, List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                foreach (var pendingPod in pendingPods)
                {
                    //Already scheduler to somewhere
                    if (!string.IsNullOrEmpty(pendingPod.NodeName)) { continue; }

                    pendingPod.Limit = SetLimitation(pendingPod.Limit, pendingPod.Request);
                    pendingPod.Request = SetLimitation(pendingPod.Request, pendingPod.Request);

                    if (node.FreeCPU > pendingPod.Limit.CPU && node.FreeMemory > pendingPod.Limit.Memory)
                    {
                        pendingPod.NodeName = node.Name;
                        node.FreeCPU -= pendingPod.Limit.CPU;
                        node.FreeMemory -= pendingPod.Limit.Memory;
                        pendingPod.Status = PodStatus.Creating;
                    }
                }
            }
            return pendingPods;
        }

        private Limitation SetLimitation(Limitation limit, Limitation request)
        {
            var ret = new Limitation();
            if (limit.CPU == 0)
            {
                if (request.CPU == 0)
                {
                    ret.CPU = Pod.BasicCPURequest;
                }
                else
                {
                    ret.CPU = request.CPU;
                }
            }
            else
            {
                ret.CPU = limit.CPU;
            }

            if (limit.Memory == 0)
            {
                if (request.Memory == 0)
                {
                    ret.Memory = Pod.BasicMemoryRequest;
                }
                else
                {
                    ret.Memory = request.Memory;
                }
            }
            else
            {
                ret.Memory = limit.Memory;
            }

            return ret;
        }

    }
}
