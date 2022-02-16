using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Meniga.MassTransit.Common.Bus;
using Microsoft.Extensions.Hosting;

namespace Meniga.MassTransit.Runner
{
    public class Worker : BackgroundService
    {
        private readonly IEnumerable<IBusPublisher<EventOne>> _producers;

        public Worker(IEnumerable<IBusPublisher<EventOne>> producers)
        {
            _producers = producers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var producer in _producers)
            {
                await producer.PublishAsync(new EventOne { Text = "Demo" }, stoppingToken);
            }
        }
    }
}
