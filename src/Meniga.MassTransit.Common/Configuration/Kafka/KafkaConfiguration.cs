using System.Collections.Generic;

namespace Meniga.MassTransit.Common.Configuration.Kafka
{
    public class KafkaConfiguration
    {
        public IEnumerable<string> Brokers { get; set; }
        public IDictionary<string,string> Producers { get; set; }
    }
}
