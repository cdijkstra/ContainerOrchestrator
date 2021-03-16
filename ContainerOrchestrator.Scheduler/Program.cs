using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ContainerOrchestrator.Base;
using ContainerOrchestrator.Scheduler.ServiceHandlers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

namespace ContainerOrchestrator.Scheduler
{
    public class Program
    {
        public const string SERVER_ADDRESS = "https://localhost:5001";

        static async Task Main(string[] args)
        {
            //var ns = new NodeServices(SERVER_ADDRESS);
            //await ns.GetNodeStatusAsync();

            //var ps = new PodServices(SERVER_ADDRESS);
            //var ll = new List<Pod>() { new Pod() { Name = "p1", Image = "nginx", NodeName = "node1" } };
            //await ps.UpdatePodsAsync(ll);

            //_ = Task.Run(async () =>
            //{
            var ps = new PodServices(SERVER_ADDRESS);
            await ps.RecievePodsFromApi();
            //});

            //Console.ReadKey();
        }
    }
}
