using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using Confluent.Kafka;
using MassTransit;
using MassTransit.KafkaIntegration;
using MassTransit.RabbitMqTransport;
using MassTransit.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace EventTest
{
    public class KafkaBroker : Broker
    {
        private readonly string _host;
        private readonly ushort _port;
        private IBusControl _busControl;

        public KafkaBroker(ServiceCollection services, string host, ushort port)
        {
            _host = host;
            _port = port == 0 ? (ushort) 9092 : _port;


            //_busControl.ConnectConsumer<ValueEnteredEventConsumer>();
        }

        public override async Task Publish<T>(T message)
        {
            await _busControl.Publish(message);
        }


        public override async Task StartAsync(ServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));

                x.AddRider(rider =>
                {
                    
                    foreach (var consumer in _consumers)
                    {
                        var method = rider.GetType().GetMethods().FirstOrDefault(m => m.Name == "AddConsumer" && m.IsGenericMethod);
                        MethodInfo generic = method.MakeGenericMethod(consumer.Type);
                        generic.Invoke(rider, new object[] { null });

                    }
                   // rider.AddConsumer<ValueEnteredEventConsumer>();


                    rider.UsingKafka((context, k) =>
                    {

                        k.Host(_host + ":" + _port);

                        foreach (var consumer in _consumers)
                        {

                            var eventType = consumer.Type.GetInterfaces().First(i => i.Name.StartsWith("IConsumer") && i.IsGenericType).GetGenericArguments().First();
                            
                            var method = typeof(KafkaConfiguratorExtensions).GetMethods().FirstOrDefault(m => m.Name == "TopicEndpoint" && m.IsGenericMethod);
                            MethodInfo generic = method.MakeGenericMethod(eventType);

                            Action<IKafkaTopicReceiveEndpointConfigurator> action = e =>
                            {
                                e.EnableAutoOffsetStore = true;
                                e.AutoOffsetReset = AutoOffsetReset.Earliest;

                                e.ConfigureConsumer(context, consumer.Type);
                            };

                            generic.Invoke(null, new object[] { k, eventType.FullName, consumer.GroupName, action });

                            //k.TopicEndpoint<ValueEntered>(eventType.FullName, consumer.GroupName, e =>
                            //{
                            //    e.EnableAutoOffsetStore = true;
                            //    e.AutoOffsetReset = AutoOffsetReset.Latest;

                            //    e.ConfigureConsumer(context, typeof(ValueEnteredEventConsumer));
                            //});


                        }
                    });
                });
            });
            var provider = services.BuildServiceProvider();


            _busControl = provider.GetRequiredService<IBusControl>();


            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await _busControl.StartAsync(source.Token);
        }

        public override Task StopAsync()
        {
            return _busControl.StopAsync();
        }
    }
}
