using static ContainerOrchestrator.Base.ExtensionMethods;
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
using ContainerOrchestrator.Base;
using Google.Protobuf.Collections;

namespace ContainerOrchestrator.Scheduler.ServiceHandlers
{
    public class PodServices
    {
        private string serverAddress;
        private OrcastraterClient client;

        public PodServices(string serverAddress)
        {
            this.serverAddress = serverAddress;
            client = ConnectionHandler.GetClient(serverAddress);
        }

        private async Task UpdatePodsAsync(IList<Pod> pods)
        {
            var podsRequest = new PodsRequest();
#warning find a better way to transfare values
            foreach (var pod in pods)
            {
                podsRequest.Pods.Add(pod);
            }

            try
            {
                var reply = await client.SchedulePodsAsync(podsRequest, cancellationToken: new CancellationTokenSource(TimeSpan.FromSeconds(500)).Token);
                Console.WriteLine(reply.ResponseCode + " -- " + reply.ResponseMessage);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream timed out, stream cancelled.");
            }

        }

        public async Task ReconcileAsync()
        {
            try
            {
                //var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                var response = client.Reconcile(cancellationToken: CancellationToken.None);

                while (await response.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
                {
                    var pendingPods = response.ResponseStream.Current.Pods;

                    var ns = new NodeServices(serverAddress);
                    var nodes = await ns.GetNodeStatusAsync();

                    Console.WriteLine("Nodes in a list:");
                    foreach (var node in nodes)
                    {
                        Console.WriteLine(node.PrettyPrint());
                    }

                    var scheduledPods = SchedulingPods(pendingPods, nodes);

                    Console.WriteLine("\n Scheduled pods:");
                    foreach (var pod in scheduledPods)
                    {
                        Console.WriteLine(pod.PrettyPrint());
                    }

                    await UpdatePodsAsync(scheduledPods);

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

        private IList<Pod> SchedulingPods(IList<Pod> pendingPods, IList<Node> nodes)
        {
            foreach (var node in nodes)
            {
                foreach (var pendingPod in pendingPods)
                {
                    //Already scheduler to somewhere
                    if (!string.IsNullOrEmpty(pendingPod.NodeName)) { continue; }

                    if (pendingPod.Limit == null) { pendingPod.Limit = new Limitation(); }
                    if (pendingPod.Request == null) { pendingPod.Request = new Limitation(); }

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
                    ret.CPU = Config.BasicCPURequest;
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
                    ret.Memory = Config.BasicMemoryRequest;
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
