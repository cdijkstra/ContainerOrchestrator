using System.Threading.Tasks;
using Grpc.Core;
using Kubelet;

namespace ContainerOrchestrator.Api.Services
{
    public class KubeletService : Kubelet.Kubelet.KubeletBase
    {
        public override Task RunPod(IAsyncStreamReader<PodRequest> requestStream, IServerStreamWriter<Pod> responseStream, ServerCallContext context)
        {
            return base.RunPod(requestStream, responseStream, context);
        }
    }
}