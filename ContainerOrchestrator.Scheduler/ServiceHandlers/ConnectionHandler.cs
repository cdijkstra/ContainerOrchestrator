using Grpc.Net.Client;
using Orcastrate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Orcastrate.Orcastrater;

namespace ContainerOrchestrator.Scheduler.ServiceHandlers
{
    public static class ConnectionHandler
    {
        private static OrcastraterClient orcastrater = null;
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

        public static OrcastraterClient GetClient(string serverAddress)
        {
            if (channel == null)
                CreateConnection(serverAddress);

            return orcastrater ?? new OrcastraterClient(channel);
        }
    }
}
