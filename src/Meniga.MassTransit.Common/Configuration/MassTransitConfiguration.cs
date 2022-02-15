using System.Collections.Generic;
using Meniga.MassTransit.Common.Configuration.Kafka;
using Meniga.MassTransit.Common.Configuration.RabbitMq;

namespace Meniga.MassTransit.Common.Configuration
{
    public class MassTransitConfiguration
    {
        public KafkaConfiguration KafkaConfiguration { get; set; }
        public RabbitMqConfiguration RabbitMqConfiguration { get; set; }
        public IDictionary<string,string> Consumers { get; set; }
    }
}
