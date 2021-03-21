using ContainerOrchestrator.Base;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Orcastrate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ContainerOrchestrator.Scheduler.ServiceHandlers
{
    public class NodeServices
    {
        private string serverAddress;

        public NodeServices(string serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        public async Task<IList<Node>> GetNodeStatusAsync()
        {
            var client = ConnectionHandler.GetClient(serverAddress);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(500));
            try
            {
                var reply = await client.GetNodeCapacitiesAsync(new Empty(), cancellationToken: cts.Token);
                return reply.Nodes;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream timed out, stream cancelled.");
            }
            return null;
        }
    }
}
