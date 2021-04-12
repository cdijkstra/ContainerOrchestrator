using Grpc.Net.Client;
using System.Threading.Tasks;
using Validatorapi;
using ValidatorClient = Validatorapi.ValidatorApi.ValidatorApiClient;

namespace ContainerOrchestrator.Scheduler.ServiceHandlers
{
    public static class ConnectionHandler
    {
        private static readonly ValidatorApi.ValidatorApiClient _validatorClient;
        private static GrpcChannel _channel;

        static ConnectionHandler()
        {
            _validatorClient = null;
            _channel = null;
        }
        
        public static void CreateConnection(string serverAddress)
        {
            if (_channel == null)
            {
                _channel = GrpcChannel.ForAddress(serverAddress);
            }
        }

        public static async Task CloseChannelAsync()
        {
            await _channel.ShutdownAsync();
        }

        public static ValidatorApi.ValidatorApiClient GetClient(string serverAddress)
        {
            if (_channel == null)
            {
                CreateConnection(serverAddress);
            }

            return _validatorClient ?? new ValidatorApi.ValidatorApiClient(_channel);
        }
    }
}
