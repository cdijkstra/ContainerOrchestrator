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
    public class OrcastrateService : Orcastrate.Orcastrater.OrcastraterBase
    {
        private readonly ILogger<OrcastrateService> _logger;
        private SubscribeHandler _subscribeHandler;

        public OrcastrateService(ILogger<OrcastrateService> logger)
        {
            _logger = logger;
            _subscribeHandler = new SubscribeHandler();
        }

        public override async Task UpdatePods(GenericMessage request, IServerStreamWriter<Orcastrate.GenericMessage> responseStream, ServerCallContext context)
        {
            var pods = JsonSerializer.Deserialize<List<Pod>>(request.Content);
            _logger.LogInformation("Sending GetNodeCapacities response");
            var response = new Base.GenericResponse() { ErrorMessage = string.Empty, Successful = true };
            var jsonString = JsonSerializer.Serialize(response);
            var ret = new Orcastrate.GenericMessage { Content = jsonString };

            await responseStream.WriteAsync(ret);
            _logger.LogInformation("Send dummy pods again");

            //for testing purpose it sleeps 5s before it sends a new dummy request
            Thread.Sleep(5000);
            await SendDummyPods();
        }

        public override async Task GetNodeCapacities(Empty _, IServerStreamWriter<Orcastrate.GenericMessage> responseStream, ServerCallContext context)
        {
            var forecast = new List<Node>(){
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
            var jsonString = JsonSerializer.Serialize(forecast);
            var ret = new Orcastrate.GenericMessage { Content = jsonString };

            await responseStream.WriteAsync(ret);
        }


        public override async Task RecievePods(IAsyncStreamReader<GenericMessage> requestStream, IServerStreamWriter<Orcastrate.GenericMessage> responseStream, ServerCallContext context)
        {
            //_subscribeHandler = new SubscribeHandler();
            _subscribeHandler.Join(string.Empty, responseStream);
            do
            {
                await SendDummyPods();
            } while (await requestStream.MoveNext());

            _subscribeHandler.Remove(context.Peer);
        }

        private async Task SendDummyPods()
        {
            List<Pod> pods = new List<Pod>();
            var podDummmyId = new Random().Next(1, 13);
            pods.Add(new Pod() { Name = "p" + podDummmyId + "-a", Image = "nginx" });
            pods.Add(new Pod() { Name = "p" + podDummmyId + "-b", Image = "nginx" });
            pods.Add(new Pod() { Name = "p" + podDummmyId + "-c-pending-forever", Image = "nginx", Request = new Limitation() { CPU = 10000, Memory = 100 } });
            var content = JsonSerializer.Serialize(pods);

            await _subscribeHandler.BroadcastMessageAsync(new GenericMessage() { Content = content });
        }
    }
}

