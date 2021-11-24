namespace EventTest.Bus.Config
{
    public class BusConfig
    {
        public BusTransports Transport { get; set; }

        /// <summary>
        /// Configuration for a Kafka broker
        /// </summary>
        public KafkaConfig Kafka { get; set; }

        /// <summary>
        /// Configuration for a RabbitMQ broker
        /// </summary>
        public RabbitMqConfig RabbitMq { get; set; }
    }

    public enum BusTransports
    {
        InMemory,
        RabbitMq,
        Kafka
    }
}
