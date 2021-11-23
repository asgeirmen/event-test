using System;
using System.Collections.Generic;
using System.Text;

namespace EventTest.EventBus
{
    public class BusConfig
    {
        /// <summary>
        /// Configuration for a Kafka broker
        /// </summary>
        public KafkaConfig Kafka { get; set; }
    }
}
