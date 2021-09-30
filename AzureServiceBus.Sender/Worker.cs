using AzureServiceBus.Sender.Services;
using Bogus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static Bogus.DataSets.Name;

namespace AzureServiceBus.Sender
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IQueueService queueService;        
        private Faker<Shared.Models.Person> fakePerson;
        public Worker(ILogger<Worker> logger, IQueueService queueService)
        {
            _logger = logger;
            this.queueService = queueService;
            fakePerson = new Faker<Shared.Models.Person>()
                //.CustomInstantiator(f => new Shared.Models.Person() { Id = userId++ })
                .RuleFor(u => u.FirstName, (f, p) => f.Name.FirstName(f.PickRandom<Gender>()))
                .RuleFor(u => u.Lastname, (f, p) => f.Name.LastName(f.PickRandom<Gender>()))
                .RuleFor(u => u.Email, (f, p) => f.Internet.Email(p.FirstName, p.Lastname))
                .RuleFor(u => u.Id, (f, p) => f.UniqueIndex);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var person = fakePerson.Generate();

                Console.WriteLine($"Person generated {JsonSerializer.Serialize(person)}");
                await queueService.SendMessageAsync(person, "test");

                await Task.Delay(100, stoppingToken);
            }
        }
    }
}
