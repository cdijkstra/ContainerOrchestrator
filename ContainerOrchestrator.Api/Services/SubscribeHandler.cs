using ContainerOrchestrator.Base;
using Grpc.Core;
using Orcastrate;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContainerOrchestrator.Api.Services
{
    public class SubscribeHandler
    {
        private static ConcurrentDictionary<string, IServerStreamWriter<Orcastrate.GenericMessage>> schedulers = new ConcurrentDictionary<string, IServerStreamWriter<Orcastrate.GenericMessage>>();

        public void Join(string name, IServerStreamWriter<Orcastrate.GenericMessage> response) => schedulers.TryAdd(name, response);

        public void Remove(string name) => schedulers.TryRemove(name, out var s);

        public async Task BroadcastMessageAsync(Orcastrate.GenericMessage message) => await BroadcastMessages(message);

        private async Task BroadcastMessages(Orcastrate.GenericMessage message)
        {
            foreach (var scheduler in schedulers)
            {
                var item = await SendMessageToSubscriber(scheduler, message);
                if (item != null)
                {
                    Remove(item?.Key);
                };
            }
        }

        private async Task<Nullable<KeyValuePair<string, IServerStreamWriter<Orcastrate.GenericMessage>>>> SendMessageToSubscriber(KeyValuePair<string, IServerStreamWriter<Orcastrate.GenericMessage>> scheduler, Orcastrate.GenericMessage message)
        {
            try
            {
                await scheduler.Value.WriteAsync(message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return scheduler;
            }
        }
    }
}
