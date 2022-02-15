using MassTransit;
using Meniga.MassTransit.Common.Bus;
using Meniga.MassTransit.Common.Configuration;
using Meniga.MassTransit.Infrastructure.Consumer;
using Meniga.MassTransit.Infrastructure.Producer;
using Meniga.MassTransit.Infrastructure.RabbitMq;
using Microsoft.Extensions.DependencyInjection;

namespace Meniga.MassTransit.RabbitMq
{
    public static class MassTransitBusExtensions
    {
        public static void ConfigureMassTransit(this IServiceCollection services, 
            MassTransitConfiguration configuration) {

            var provider = services.BuildServiceProvider();
            var consumersRegistry = provider.GetRequiredService<IConsumersRegistry>();
            services.AddMassTransit(config =>
            {
                if (configuration?.RabbitMqConfiguration != null)
                {
                    config.ConfigureMassTransitRabbitMqBus(configuration.RabbitMqConfiguration, consumersRegistry);
                    services.AddTransient(typeof(IBusPublisher<>), typeof(RabbitMqPublisher<>));
                }

                if (configuration?.KafkaConfiguration != null)
                {
                    var producersRegistry = provider.GetRequiredService<IProducersRegistry>();
                    config.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
                    config.ConfigureMassTransitKafkaRider(configuration.KafkaConfiguration, consumersRegistry, producersRegistry);
                    services.RegisterKafkaProducers(producersRegistry.RegisteredProducers);
                }
            });

            services.AddMassTransitHostedService();
        }
    }
}
