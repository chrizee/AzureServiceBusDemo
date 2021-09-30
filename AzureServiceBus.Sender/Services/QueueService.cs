using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureServiceBus.Sender.Services
{
    public class QueueService : IQueueService
    {
        private readonly IConfiguration configuration;

        public QueueService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendMessageAsync<T>(T serviceBusmessage, string queue)
        {
            var client = new ServiceBusClient(configuration.GetConnectionString("AzureServiceBus"));
            var message = new ServiceBusMessage(JsonSerializer.Serialize(serviceBusmessage));

            await client.CreateSender(queue).SendMessageAsync(message);
        }
    }
}
