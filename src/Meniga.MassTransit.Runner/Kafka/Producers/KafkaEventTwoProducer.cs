using System.Threading;
using System.Threading.Tasks;
using MassTransit.KafkaIntegration;

namespace Meniga.MassTransit.Runner.Kafka.Producers
{
    public class KafkaEventTwoProducer : IKafkaProducer<EventTwo>
    {
        private readonly ITopicProducer<EventTwo> _producer;

        public KafkaEventTwoProducer(ITopicProducer<EventTwo> producer)
        {
            _producer = producer;
        }

        public async Task PublishAsync(EventTwo message, CancellationToken token)
        {
            await _producer.Produce(message, token);
        }
    }
}
