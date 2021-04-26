using System;
using System.Threading;
using System.Threading.Tasks;
using ContainerOrchestrator.Kubelet;
using Kubelet;

namespace ContainerOrchestrator.Kubelet.ServiceHandlers
{
    public class KubeletService
    {
        private string _serverAddress;
        private global::Kubelet.Kubelet.KubeletClient _client;

        public KubeletService(string serverAddress)
        {
            _serverAddress = serverAddress;
            _client = ConnectionHandler.GetClient(serverAddress);
        }

        public async Task HandlePodRequestsAsync()
        {
            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(500));
                var response = _client.RunPod(cancellationToken: cts.Token);

                while (await response.ResponseStream.MoveNext(cancellationToken: cts.Token))
                {
                    Pod PodRequest = response.ResponseStream.Current;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}