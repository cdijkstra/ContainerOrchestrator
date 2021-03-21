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
            var ps = new PodServices(SERVER_ADDRESS);
            await ps.ReconcileAsync();

            Console.ReadKey();
        }
    }
}
