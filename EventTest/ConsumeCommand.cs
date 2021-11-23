using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Confluent.Kafka;
using EventTest.EventBus;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventTest
{
    [Verb("consume", HelpText = "Save a code change")]
    public class ConsumeCommand : ICommand
    {
        [Option('n', "name", Required = false, Default = "default", HelpText = "Name of the consumer, that will be appended to a queue name starting with 'event-listener-' ")]
        public string Name { get; set; }

        [Option('b', "broker", Required = false, Default = "kafka", HelpText = "The type of broker to use: rabbitmq or kafka")]
        public string BrokerType { get; set; }

        public  Task Register(IServiceCollection services, IConfiguration config)
        {
            var busConfig = config.GetSection("BusConfig").Get<BusConfig>();
            services.ConfigureEventBus(busConfig, null, new[]
                {
                    new ConsumerConfig<ValueEntered, ValueEnteredEventConsumer>()
                    {
                        GroupName = "event-listener-" + Name
                    }
                }, 
                null);

            return Task.CompletedTask;
        }

        public async Task Execute(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Press enter to exit");

            await Task.Run(() => Console.ReadLine());
        }
    }
}
