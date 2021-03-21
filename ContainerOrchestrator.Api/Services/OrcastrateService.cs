using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ContainerOrchestrator.Api.Services;
using ContainerOrchestrator.Base;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orcastrate;

namespace ContainerOrchestrator.Api
{
    public class OrcastrateService : Orcastrater.OrcastraterBase
    {
        private readonly ILogger<OrcastrateService> _logger;
        private SubscribeHandler _subscribeHandler;

        public OrcastrateService(ILogger<OrcastrateService> logger)
        {
            _logger = logger;
            _subscribeHandler = new SubscribeHandler();
        }

        public override Task<NodesResponse> GetNodeCapacities(Empty request, ServerCallContext context)
        {
            var dummyNodes = new List<Node>(){
                    new Node{
                        IpAddress= "1.1.1.1",
                        Name="node1",
                        AllCPU= 300,
                        AllMemory= 2000,
                        FreeCPU=150,
                        FreeMemory=1000 },
                    new Node{
                        IpAddress= "2.2.2.2",
                        Name="node2",
                        AllCPU= 222,
                        AllMemory= 2222,
                        FreeCPU=1111,
                        FreeMemory=1111 }
                };
            _logger.LogInformation("Sending Node Capacities");

            var nodesResponse = new NodesResponse();

#warning find a better way to transfare values
            foreach (var node in dummyNodes)
            {
                nodesResponse.Nodes.Add(node);
            }

            return Task.FromResult(nodesResponse);
        }

        public override async Task Reconcile(IAsyncStreamReader<RegisterRequest> requestStream, IServerStreamWriter<PodsResponse> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("Collecting all the pending nodes and sending it to the Scheduler");

            _subscribeHandler.Join(string.Empty, responseStream);
            do
            {
                await SendDummyPodsAsync();
            } while (await requestStream.MoveNext());

            _subscribeHandler.Remove(context.Peer);

            throw new NotImplementedException();
        }

        public override async Task<GenericReply> SchedulePods(PodsRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting the scheduled pods and saving them");

            foreach (var pod in request.Pods)
            {
                Console.WriteLine(pod.PrettyPrint());
            }

            _logger.LogInformation("Send dummy pods again");

            //for testing purpose it sleeps 5s before it sends a new dummy request
            await SendDummyPodsAsync();

            return new GenericReply() { ResponseCode = 200, ResponseMessage="yeeeeah" };
        }

        private async Task SendDummyPodsAsync()
        {
            Thread.Sleep(5000);
            var pods = new List<Pod>();
            var podDummmyId = new Random().Next(1, 13);
            pods.Add(new Pod() { Name = "p" + podDummmyId + "-a", Image = "nginx" });
            pods.Add(new Pod() { Name = "p" + podDummmyId + "-b", Image = "nginx" });
            pods.Add(new Pod() { Name = "p" + podDummmyId + "-c-pending-forever", Image = "nginx", Request = new Limitation() { CPU = 10000, Memory = 100 } });

            await _subscribeHandler.BroadcastMessageAsync(pods);
        }
    }
}

