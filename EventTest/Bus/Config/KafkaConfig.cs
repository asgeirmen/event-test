using System;
using System.Collections.Generic;
using System.Text;

namespace EventTest.Bus.Config
{
    public class KafkaConfig
    {
        /// <summary>
        /// Initial list of brokers as a CSV list of broker host or host:port
        /// </summary>
        public string BootstrapServers { get; set; }

        /// <summary>
        /// The maximum length of time (in milliseconds) before a cancellation request
        /// is acted on. Low values may result in measurably higher CPU usage.
        /// default: 100
        /// range: 1 &lt;= dotnet.cancellation.delay.max.ms &lt;= 10000
        /// importance: low
        /// </summary>
        public double? CancellationDelay { get; set; }

        /// <summary>
        /// Configure the use of SSL to connection to Kafka
        /// </summary>
        public KafkaSslConfig Ssl { get; set; }

        /// <summary>
        /// Configure the use of SASL to connection to Kafka
        /// </summary>
        public KafkaSaslConfig Sasl { get; set; }

    }
}
