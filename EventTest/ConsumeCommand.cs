using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace EventTest
{
    [Verb("consume", HelpText = "Save a code change")]
    public class ConsumeCommand : ICommand
    {
        [Option('n', "name", Required = false, Default = "default", HelpText = "Name of the consumer, that will be appended to a queue name starting with 'event-listener-' ")]
        public string Name { get; set; }

        [Option('b', "broker", Required = false, Default = "rabbitmq", HelpText = "The type of broker to use: rabbitmq or kafka")]
        public string BrokerType { get; set; }

        public async Task Execute()
        {
            Broker broker;

            var services = new ServiceCollection();
            if (BrokerType == "kafka")
            {
                broker = new KafkaBroker(services, "localhost", 0);

            }
            else
            {
                broker = new RabbitMqBroker(services, "localhost", 0, "/", "guest", "guest");
            }

            broker.AddConsumer("event-listener-" + Name, typeof(ValueEnteredEventConsumer));



            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await broker.StartAsync(services);
            //await busControl.StartAsync(source.Token);
            try
            {
                Console.WriteLine("Press enter to exit");

                await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await broker.StopAsync();
                //await busControl.StopAsync();
            }
        }
    }
}
