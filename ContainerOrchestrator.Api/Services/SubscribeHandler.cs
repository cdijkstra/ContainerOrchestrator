using Grpc.Core;
using Orcastrate;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pod = Orcastrate.Pod;

namespace ContainerOrchestrator.Api.Services
{
    public class SubscribeHandler
    {
        private static ConcurrentDictionary<string, IServerStreamWriter<PodsResponse>> schedulers = new ConcurrentDictionary<string, IServerStreamWriter<PodsResponse>>();

        public void Join(string name, IServerStreamWriter<PodsResponse> response) => schedulers.TryAdd(name, response);

        public void Remove(string name) => schedulers.TryRemove(name, out var s);

        public async Task BroadcastMessageAsync(IList<Pod> message) => await BroadcastMessages(message);

        private async Task BroadcastMessages(IList<Pod> message)
        {
            foreach (var scheduler in schedulers)
            {
                var item = await SendMessageToSubscriber(scheduler, message);
                if (item != null)
                {
                    Remove(item?.Key);
                }
            }
        }
        
         private async Task<Nullable<KeyValuePair<string, IServerStreamWriter<PodsResponse>>>> SendMessageToSubscriber(KeyValuePair<string, IServerStreamWriter<PodsResponse>> scheduler, IList<Pod> message)
        {
            try
            {
                var podsResponse = new PodsResponse();
#warning find a better way to transfare values
                foreach (var pod in message)
                {
                    podsResponse.Pods.Add(pod);
                }
                await scheduler.Value.WriteAsync(podsResponse);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return scheduler;
            }
        }
         
         // public async Task BroadcastMessageAsync(Deployment deploymentMessage) => await BroadcastMessages(deploymentMessage);
         //
         // private async Task BroadcastMessages(Deployment deploymentMessage)
         // {
         //     var item = await SendMessageToSubscriber(api, deploymentMessage);
         //     if (item != null)
         //     {
         //         Remove(item?.Key);
         //     }
         // }
         
         // private async Task<Nullable<KeyValuePair<string, IServerStreamWriter<PodsResponse>>>> SendMessageToSubscriber(api, Deployment deploymentMessage)
         // {
         //     try
         //     {
         //         var deploymentResponse = new DeploymentResponse();
         //         await api.Value.WriteAsync(deploymentResponse);
         //         return null;
         //     }
         //     catch (Exception ex)
         //     {
         //         Console.WriteLine(ex);
         //         return api;
         //     }
         // }
    }
}
