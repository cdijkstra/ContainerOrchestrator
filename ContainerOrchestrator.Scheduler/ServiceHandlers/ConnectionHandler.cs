using Grpc.Net.Client;
using System.Threading.Tasks;
using static Orcastrate.Orcastrater;

namespace ContainerOrchestrator.Scheduler.ServiceHandlers
{
    public static class ConnectionHandler
    {
        private static readonly OrcastraterClient _orcastrater;
        private static GrpcChannel _channel;

        static ConnectionHandler()
        {
            _orcastrater = null;
            _channel = null;
        }
        
        public static void CreateConnection(string serverAddress)
        {
            if (_channel == null)
                _channel = GrpcChannel.ForAddress(serverAddress);
        }

        public static async Task CloseChannelAsync()
        {
            await _channel.ShutdownAsync();
        }

        public static OrcastraterClient GetClient(string serverAddress)
        {
            if (_channel == null)
                CreateConnection(serverAddress);

            return _orcastrater ?? new OrcastraterClient(_channel);
        }
    }
}
