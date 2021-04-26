using Grpc.Net.Client;
using System.Threading.Tasks;
using KubeletClient = Kubelet.Kubelet.KubeletClient;

namespace ContainerOrchestrator.Kubelet.ServiceHandlers
{
    public static class ConnectionHandler
    {
        private static KubeletClient orcastrater = null;
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

        public static KubeletClient GetClient(string serverAddress)
        {
            if (channel == null)
                CreateConnection(serverAddress);

            return orcastrater ?? new KubeletClient(channel);
        }
    }
}
