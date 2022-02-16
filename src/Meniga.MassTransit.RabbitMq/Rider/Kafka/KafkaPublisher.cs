using System.Threading;
using System.Threading.Tasks;
using MassTransit.KafkaIntegration;
using Meniga.MassTransit.Common.Bus;

namespace Meniga.MassTransit.Infrastructure.Rider.Kafka
{
    public class KafkaPublisher<T> : IBusPublisher<T> where T : class
    {
        private readonly ITopicProducer<T> _topicProducer;

        public KafkaPublisher(ITopicProducer<T> topicProducer)
        {
            _topicProducer = topicProducer;
        }
        public Task PublishAsync(T message, CancellationToken token)
        {
            return _topicProducer.Produce(message, token);
        }
    }
}
