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

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));

                x.AddRider(rider =>
                {

                    var method = rider.GetType().GetMethods().FirstOrDefault(m => m.Name == "AddConsumer" && m.IsGenericMethod);
                    MethodInfo generic = method.MakeGenericMethod(typeof(ValueEnteredEventConsumer));
                    generic.Invoke(rider, new object[]{null});
                    //rider.AddConsumer<ValueEnteredEventConsumer>();


                    rider.UsingKafka((context, k) =>
                    {

                        k.Host(_host + ":" + _port);
                        k.TopicEndpoint<ValueEntered>("quickstart-events", "consumer-group-name", e =>
                        {
                            e.EnableAutoOffsetStore = true;
                            e.AutoOffsetReset = AutoOffsetReset.Latest;

                            e.ConfigureConsumer(context, typeof(ValueEnteredEventConsumer));
                        });



                    });
                });
            });
            var provider = services.BuildServiceProvider();

            var riderConfig = provider.GetService<IRiderRegistrationConfigurator>();
            var busConfig = provider.GetService<IBusRegistrationConfigurator>();
            //riderConfig.AddConsumer<ValueEnteredEventConsumer>();
            var kafkaFactoryConfig = provider.GetService<IKafkaFactoryConfigurator>();
            var riderContext = provider.GetService<IRiderRegistrationContext>();
            var regContext = provider.GetService<IBusRegistrationContext>();
            var endpoints = regContext.GetConfigureReceiveEndpoints();
            //busControl.ConnectReceiveEndpoint<>()

            _busControl = provider.GetRequiredService<IBusControl>();
            _busControl.ConnectConsumer<ValueEnteredEventConsumer>();
        }

        public override async Task Publish<T>(T message)
        {
            await _busControl.Publish(message);
        }


        public override async Task StartAsync(ServiceCollection services)
        {


            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await _busControl.StartAsync(source.Token);
        }

        public override Task StopAsync()
        {
            return _busControl.StopAsync();
        }
    }
}
