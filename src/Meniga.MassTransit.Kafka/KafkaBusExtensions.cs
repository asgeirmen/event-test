using System.Collections.Generic;
using MassTransit;
using MassTransit.MultiBus;
using Meniga.MassTransit.Common.Bus;
using Meniga.MassTransit.Common.Configuration.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Meniga.MassTransit.RabbitMq
{
    public static class KafkaBusExtensions
    {
        public static void ConfigureMassTransitKafkaRider(this IServiceCollection services, IList<ConsumerConfig> registeredConsumeres, KafkaConfiguration kafkaConfiguration)
        {
            services.AddMassTransit(config =>
            {
            });
        }
    }
}
