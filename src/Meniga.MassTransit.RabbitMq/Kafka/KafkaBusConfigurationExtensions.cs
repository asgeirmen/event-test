using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.KafkaIntegration;
using MassTransit.Registration;
using Meniga.MassTransit.Common.Bus;
using Meniga.MassTransit.Common.Configuration.Kafka;
using Meniga.MassTransit.Infrastructure.Consumer;
using Meniga.MassTransit.Infrastructure.Kafka;
using Meniga.MassTransit.Infrastructure.Producer;
using Microsoft.Extensions.DependencyInjection;

namespace Meniga.MassTransit.RabbitMq
{
    public static class KafkaBusConfigurationExtensions
    {
        public static void ConfigureMassTransitKafkaRider(this IServiceCollectionBusConfigurator configurator,
            KafkaConfiguration kafkaConfiguration, 
            IConsumersRegistry consumersRegistry,
            IProducersRegistry producersRegistry)
        {
            configurator.AddRider(rider =>
            {
                rider.AddKafkaConsumers(consumersRegistry.RegisteredConsumeres);
                rider.AddKafkaProducers(producersRegistry.RegisteredProducers);

                rider.UsingKafka((context, config) =>
                {
                    config.Host(string.Join(",", kafkaConfiguration.Brokers));

                    config.RegisterTopicEndpoints(context, consumersRegistry.RegisteredConsumeres);
                });
            });
        }

        private static void AddKafkaConsumers(this IRiderRegistrationConfigurator config, IList<ConsumerConfig> consumers)
        {
            foreach (var consumer in consumers)
            {
                var consumerType = ((dynamic)consumer).ConsumerType;
                var method = config.GetType().GetMethods()
                    .FirstOrDefault(m => m.Name == "AddConsumer" && m.IsGenericMethod);
                MethodInfo generic = method.MakeGenericMethod(consumerType);
                generic.Invoke(config, new object[] { null });
            }
        }

        private static void AddKafkaProducers(this IRiderRegistrationConfigurator config, IList<ProducerConfig> producers)
        {
            foreach (var producer in producers)
            {
                var messageType = (Type)((dynamic)producer).MessageType;
                var method = typeof(KafkaProducerRegistrationExtensions).GetMethods()
                                    .FirstOrDefault(m => m.Name == "AddProducer" && m.IsGenericMethod);
                MethodInfo generic = method.MakeGenericMethod(messageType);
                generic.Invoke(null, new object[] { config, messageType.FullName, null });
            }
        }

        private static void RegisterTopicEndpoints(this IKafkaFactoryConfigurator config, IRiderRegistrationContext context, IList<ConsumerConfig> consumers)
        {
            foreach(var consumer in consumers)
            {
                var consumerType = (Type)((dynamic)consumer).ConsumerType;
                var eventType = consumerType.GetInterfaces()
                                        .First(i => i.Name.StartsWith("IConsumer") && i.IsGenericType)
                                        .GetGenericArguments()
                                        .First();

                var method = typeof(KafkaConfiguratorExtensions).GetMethods()
                    .FirstOrDefault(m => m.Name == "TopicEndpoint" && m.IsGenericMethod);
                MethodInfo generic = method.MakeGenericMethod(eventType);

                Action<IKafkaTopicReceiveEndpointConfigurator> action = e =>
                {
                    e.EnableAutoOffsetStore = true;
                    e.ConfigureConsumer(context, consumerType);
                };

                generic.Invoke(null,
                    new object[] { config, eventType.FullName, ((dynamic)consumer).ConsumerGroup, action });
            }
        }

        public static void RegisterKafkaProducers(this IServiceCollection services, IList<ProducerConfig> producers)
        {
            foreach(var producer in producers)
            {
                var messageType = (Type)((dynamic)producer).MessageType;
                var publisherInterface = typeof(IBusPublisher<>).MakeGenericType(messageType);
                var publisherImpl = typeof(KafkaPublisher<>).MakeGenericType(messageType);
                services.AddTransient(publisherInterface, publisherImpl);
            }
        }
    }
}
