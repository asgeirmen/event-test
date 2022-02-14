using MassTransit;
using MassTransit.MultiBus;
using Meniga.MassTransit.Common.Configuration.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Meniga.MassTransit.RabbitMq
{
    public static class KafkaBusExtensions
    {
        public static void ConfigureMassTransitKafkaRider(this IServiceCollection services, KafkaConfiguration kafkaConfiguration)
        {
            services.AddMassTransit(config =>
            {
            });
        }
    }
}
