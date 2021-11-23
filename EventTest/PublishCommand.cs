using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using MassTransit;
using MassTransit.Monitoring.Performance;
using MassTransit.KafkaIntegration;
using Confluent.Kafka;
using EventTest.Bus;
using EventTest.Bus.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EventTest.EventBus;

namespace EventTest
{
    [Verb("publish", HelpText = "Publish one or multiple messages to queue")]
    public class PublishCommand : ICommand
    {
        [Option('c', "count", Required = false, Default = 0, HelpText = "Number of messages generated and published")]
        public int Count { get; set; }

        public Task Register(IServiceCollection services, IConfiguration config)
        {

            var busConfig = config.GetSection("BusConfig").Get<BusConfig>();
            services.ConfigureEventBus(busConfig, null, null, new [] {typeof(ValueEntered) });

            return Task.CompletedTask;
        }

        public async Task Execute(IServiceProvider serviceProvider)
        {
            var publisher = serviceProvider.GetRequiredService<IBusPublisher<ValueEntered>>();

            if (Count > 0)
            {
                for (int ind = 0; ind < Count; ind++)
                {
                    string messageValue = "Message " + ind + ": " + new string('*', 300);
                    var stopwatch = Stopwatch.StartNew();

                    await publisher.Publish(new ValueEntered()
                    {
                        Value = messageValue
                    });

                    Console.WriteLine($"Publish took {stopwatch.ElapsedMilliseconds} ms: " + "Message " + ind + " + 300 more");
                }
            }
            else
            {
                while (true)
                {
                    string messageValue = await Task.Run(() =>
                    {
                        Console.WriteLine("Enter message (or quit to exit)");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if ("quit".Equals(messageValue, StringComparison.OrdinalIgnoreCase))
                        break;

                    await publisher.Publish(new ValueEntered()
                    {
                        Value = messageValue
                    });
                }
            }
        }
    }
}
