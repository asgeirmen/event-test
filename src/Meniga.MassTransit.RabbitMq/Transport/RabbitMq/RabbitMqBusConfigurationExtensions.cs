using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Meniga.MassTransit.Common.Configuration.RabbitMq;
using Meniga.MassTransit.Infrastructure.Consumer;

namespace Meniga.MassTransit.Infrastructure.Transport.RabbitMq
{
    public static class RabbitMqBusConfigurationExtensions
    {
        public static void ConfigureMassTransitRabbitMqBus(this IServiceCollectionBusConfigurator configurator, 
            RabbitMqConfiguration rabbitMqConfiguration, 
            IConsumersRegistry<RabbitMqConfiguration> consumersRegistry)
        {
            configurator.AddRabbitMqConsumers(consumersRegistry.RegisteredConsumeres);

            configurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqConfiguration.HostName, rabbitMqConfiguration.VirtualHost, h =>
                {
                    h.Username(rabbitMqConfiguration.UserName);
                    h.Password(rabbitMqConfiguration.Password);
                });

                cfg.RegisterRabbitMqEndpoints(consumersRegistry.RegisteredConsumeres, context);
            });
        }

        private static void AddRabbitMqConsumers(this IServiceCollectionBusConfigurator config, IList<ConsumerConfig> consumers)
        {
            foreach (var consumer in consumers)
            {
                var consumerType = ((dynamic)consumer).ConsumerType;
                var consumerConfig = (Type)((dynamic)consumer).Config;
                var method = config.GetType().GetMethods()
                    .FirstOrDefault(m => m.Name == "AddConsumer" && m.IsGenericMethod);
                MethodInfo generic = method.MakeGenericMethod(consumerType);
                generic.Invoke(config, new object[] {
                    consumerConfig
                });
            }
        }

        private static void RegisterRabbitMqEndpoints(this IRabbitMqBusFactoryConfigurator cfg, IList<ConsumerConfig> consumers, IBusRegistrationContext context)
        {
            foreach (var consumer in consumers)
            {
                var consumerType = (Type)((dynamic)consumer).ConsumerType;
                var consumerGroupName = (string)((dynamic)consumer).ConsumerGroup;
                cfg.ReceiveEndpoint(consumerGroupName,
                    e =>
                    {
                        e.ConfigureConsumer(context, consumerType);
                    });
            }
        }
    }
}
