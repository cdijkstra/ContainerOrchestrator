using ContainerOrchestrator.Base;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
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

        public async Task<List<Node>> GetNodeStatusAsync()
        {
            var client = ConnectionHandler.GetOrchasterClient(serverAddress);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(500));
            using var reply = client.GetNodeCapacities(new Empty(), cancellationToken: cts.Token);
            List<Node> nodes = null;
            try
            {
                await foreach (var nodeStatus in reply.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
                {
                    nodes = JsonSerializer.Deserialize<List<Node>>(nodeStatus.Content);
                    return nodes;
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream timed out, stream cancelled.");
            }
            return nodes;
        }
    }
}
