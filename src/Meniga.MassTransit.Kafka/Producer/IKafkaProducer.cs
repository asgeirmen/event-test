using Meniga.MassTransit.Common.Bus;

namespace Meniga.MassTransit.Runner.Kafka.Producers
{
    public interface IKafkaProducer<T> : IBusPublisher<T> where T : class
    {
    }
}
