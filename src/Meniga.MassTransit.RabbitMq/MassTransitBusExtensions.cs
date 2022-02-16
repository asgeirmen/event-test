using MassTransit;
using Meniga.MassTransit.Common.Bus;
using Meniga.MassTransit.Common.Configuration;
using Meniga.MassTransit.Common.Configuration.Kafka;
using Meniga.MassTransit.Common.Configuration.RabbitMq;
using Meniga.MassTransit.Infrastructure.Consumer;
using Meniga.MassTransit.Infrastructure.Producer;
using Meniga.MassTransit.Infrastructure.Rider.Kafka;
using Meniga.MassTransit.Infrastructure.Transport.RabbitMq;
using Microsoft.Extensions.DependencyInjection;

namespace Meniga.MassTransit.Infrastructure
{
    public static class MassTransitBusExtensions
    {
        public static void ConfigureMassTransit(this IServiceCollection services,
            MassTransitConfiguration configuration)
        {

            var provider = services.BuildServiceProvider();
            services.AddMassTransit(config =>
            {
            
                if (configuration?.RabbitMqConfiguration != null)
                {
                    config.ConfigureMassTransitRabbitMqBus(configuration.RabbitMqConfiguration,
                        provider.GetRequiredService<IConsumersRegistry<RabbitMqConfiguration>>());
                    services.AddTransient(typeof(IBusPublisher<>), typeof(RabbitMqPublisher<>));
                }

                if (configuration?.KafkaConfiguration != null)
                {
                    if (configuration.RabbitMqConfiguration == null)
                    {
                        config.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                    }

                    var producersRegistry = provider.GetRequiredService<IProducersRegistry>();
                    
                    config.ConfigureMassTransitKafkaRider(configuration.KafkaConfiguration,
                        provider.GetRequiredService<IConsumersRegistry<KafkaConfiguration>>(),
                        producersRegistry);
                    services.RegisterKafkaProducers(producersRegistry.RegisteredProducers);
                }
            });

            services.AddMassTransitHostedService();
        }
    }
}
