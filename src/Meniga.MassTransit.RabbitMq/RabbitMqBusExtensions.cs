using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Meniga.MassTransit.Common.Configuration.RabbitMq;
using Microsoft.Extensions.DependencyInjection;

namespace Meniga.MassTransit.RabbitMq
{
    public static class RabbitMqBusExtensions
    {
        public static void ConfigureMassTransitRabbitMqBus(this IServiceCollection services, RabbitMqConfiguration rabbitMqConfiguration) {
            services.AddMassTransit(config =>
            {
                RegisterConsumers(config, rabbitMqConfiguration.Consumers);

                Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(rabbitMqConfiguration.Host.Host, rabbitMqConfiguration.Host.VirutalHost, h =>
                    {
                        h.Username(rabbitMqConfiguration.Host.UserName);
                        h.Password(rabbitMqConfiguration.Host.Password);
                    });

                    RegisterEndpoints(cfg, rabbitMqConfiguration.Consumers);
                });
            });

            services.AddMassTransitHostedService();
        }

        private static void RegisterConsumers(this IServiceCollectionBusConfigurator configurator, IEnumerable<RabbitMqConsumer> consumers)
        {
            foreach (var consumer in consumers)
            {
                if (consumer.Definition == default)
                {
                    throw new ArgumentNullException($"{nameof(consumer.Definition)} not defined for {consumer.Type}");
                }

                var addConsumer = configurator.GetType().GetMethods()
                    .FirstOrDefault(m => m.Name == "AddConsumer" && m.IsGenericMethod)
                    .MakeGenericMethod(new Type[] {
                        Type.GetType(consumer.Type)
                });

                addConsumer.Invoke(configurator, new object[] { consumer.Definition.GetType()});
            }
        }

        private static void RegisterEndpoints(this IRabbitMqBusFactoryConfigurator configurator, IEnumerable<RabbitMqConsumer> consumers)
        {
            foreach (var consumer in consumers)
                Expression.Call(typeof(IRabbitMqBusFactoryConfigurator)
                    .GetMethod("ReceiveEndpoint", new Type[] {
                        typeof(string),
                        Type.GetType(consumer.Type) }),
                        Expression.Constant(consumer.Endpoint)
                    );
        }
    }
}
