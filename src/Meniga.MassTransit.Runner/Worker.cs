using System.Threading;
using System.Threading.Tasks;
using Meniga.MassTransit.Common.Bus;
using Microsoft.Extensions.Hosting;

namespace Meniga.MassTransit.Runner
{
    public class Worker : BackgroundService
    {
        private readonly IBusPublisher<EventOne> _producer;
        public Worker(IBusPublisher<EventOne> producer)
        {
            _producer = producer;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _producer.PublishAsync(new EventOne { Text = "Demo" }, stoppingToken);
        }
    }
}
