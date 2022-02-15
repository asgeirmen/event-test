using MassTransit.ExtensionsDependencyInjectionIntegration;
using Meniga.MassTransit.Common;
using Meniga.MassTransit.Common.Configuration.Kafka;

namespace Meniga.MassTransit.RabbitMq
{
    public static class KafkaBusConfigurationExtensions
    {
        public static void ConfigureMassTransitKafkaRider(this IServiceCollectionBusConfigurator configurator, KafkaConfiguration rabbitMqConfiguration, IConsumersRegistry consumersRegistry)
        {
            
        }
    }
}
