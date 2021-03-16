using Grpc.Net.Client;
using Orcastrate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContainerOrchestrator.Scheduler.ServiceHandlers
{
    public static class ConnectionHandler
    {
        private static Orcastrater.OrcastraterClient orcastrater = null;
        private static GrpcChannel channel = null;
        public static void CreateConnection(string serverAddress)
        {
            if (channel == null)
                channel = GrpcChannel.ForAddress(serverAddress);
        }

        public static async Task CloseChannelAsync()
        {
            await channel.ShutdownAsync();
        }

        public static Orcastrater.OrcastraterClient GetOrchasterClient(string serverAddress)
        {
            if (channel == null)
                CreateConnection(serverAddress);

            return orcastrater != null ? orcastrater : new Orcastrater.OrcastraterClient(channel);
        }
    }
}
