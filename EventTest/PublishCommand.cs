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
using Microsoft.Extensions.DependencyInjection;

namespace EventTest
{
    [Verb("publish", HelpText = "Publish one or multiple messages to queue")]
    public class PublishCommand : ICommand
    {
        [Option('c', "count", Required = false, Default = 0, HelpText = "Number of messages generated and published")]
        public int Count { get; set; }

        [Option('b', "broker", Required = false, Default = "rabbitmq", HelpText = "The type of broker to use: rabbitmq or kafka")]
        public string BrokerType { get; set; }

        public async Task Execute()
        {
            var services = new ServiceCollection();
            IServiceProvider provider = null;
            ITopicProducer<ValueEntered> producer = null;

            IBusControl busControl;
            if (BrokerType == "kafka")
            {
                services.AddMassTransit(x =>
                {
                    x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));

                    x.AddRider(rider =>
                    {
                        rider.AddProducer<ValueEntered>("quickstart-events");

                        rider.UsingKafka((context, k) => { k.Host("localhost:9092"); });
                    });
                });

                provider = services.BuildServiceProvider();
            }
            else
            {
                services.AddMassTransit(x =>
                {
                    x.UsingRabbitMq((context, cfg) => cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    }));
                });
                provider = services.BuildServiceProvider();
            }

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            busControl = provider.GetRequiredService<IBusControl>();
            await busControl.StartAsync(source.Token);

            if (BrokerType == "kafka")
            {
                producer = provider.GetRequiredService<ITopicProducer<ValueEntered>>();
            }

            try
            {
                if (Count > 0)
                {
                    for (int ind = 0; ind < Count; ind++)
                    {
                        string messageValue = "Message " + ind + ": " + new string('*', 10000);
                        var stopwatch = Stopwatch.StartNew();


                        if (BrokerType == "kafka")
                        {
                            await producer.Produce(new
                            {
                                Value = messageValue
                            });
                        }
                        else
                        {
                            await busControl.Publish<ValueEntered>(new
                            {
                                Value = messageValue
                            });
                        }

                        Console.WriteLine($"Publish took {stopwatch.ElapsedMilliseconds} ms: " + "Message " + ind + " + 10000 more");
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

                        if (BrokerType == "kafka")
                        {
                            await producer.Produce(new
                            {
                                Value = messageValue
                            });
                        }
                        else
                        {
                            await busControl.Publish<ValueEntered>(new
                            {
                                Value = messageValue
                            });
                        }
                    }
                }
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
