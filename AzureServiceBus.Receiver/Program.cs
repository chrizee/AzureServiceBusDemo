using Azure.Messaging.ServiceBus;
using AzureServiceBus.Shared.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureServiceBus.Receiver
{
    class Program
    {
        const string connectionString = "<connection-string>";
        const string queueName = "<queue>";

        static async Task Main(string[] args)
        {
            await using var client = new ServiceBusClient(connectionString);
            await using var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions { AutoCompleteMessages = false, MaxConcurrentCalls = 1 });
            
            processor.ProcessErrorAsync += ErrorHandler;
            processor.ProcessMessageAsync += MessageHandler;

            await processor.StartProcessingAsync();

            Console.ReadLine();

            await processor.StopProcessingAsync();
            await processor.CloseAsync();
        }

        private static async Task MessageHandler(ProcessMessageEventArgs arg)
        {
            var person = arg.Message.Body.ToObjectFromJson<Person>();

            Console.WriteLine(JsonSerializer.Serialize(person));

            await arg.CompleteMessageAsync(arg.Message);

        }

        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.ErrorSource);
            Console.WriteLine(args.FullyQualifiedNamespace);
            Console.WriteLine(args.EntityPath);
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
