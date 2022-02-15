using MassTransit;
using Meniga.MassTransit.Common;
using Meniga.MassTransit.Common.Bus;
using Meniga.MassTransit.Common.Configuration;
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
            var consumerRegistry = provider.GetRequiredService<IConsumersRegistry>();
            services.AddMassTransit(config =>
            {
                if (configuration?.RabbitMqConfiguration != null)
                {
                    config.ConfigureMassTransitRabbitMqBus(configuration.RabbitMqConfiguration, consumerRegistry);
                }

                if (configuration?.KafkaConfiguration != null)
                {
                    config.ConfigureMassTransitKafkaRider(configuration.KafkaConfiguration, consumerRegistry);
                }
            });

            services.AddTransient(typeof(IBusPublisher<>), typeof(MassTransitPublisher<>));

            services.AddMassTransitHostedService();
        }
    }
}
