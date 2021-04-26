using System;
using System.Threading.Tasks;
using ContainerOrchestrator.Kubelet.ServiceHandlers;

namespace ContainerOrchestrator.Kubelet
{
    class Program
    {
        public const string SERVER_ADDRESS = "https://localhost:5003";
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello Kotelet!");
            var kubeletService = new KubeletService(SERVER_ADDRESS);
            await kubeletService.HandlePodRequestsAsync();
        }
    }
}

// 100ms -> WhatshouldIdo
// Controller manager -> Prima, doorgaan
// Scheduler -> pod 1 -> node 1, pod 2 -> node 2
// Kubelet -> draait containers
// Docker image, port

