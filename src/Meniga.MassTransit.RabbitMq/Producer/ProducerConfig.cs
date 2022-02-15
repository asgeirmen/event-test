using System;
using Meniga.MassTransit.Common.Bus;

namespace Meniga.MassTransit.Infrastructure.Producer
{
    public abstract class ProducerConfig
    {
    }

    public class ProducerConfig<TMessage, TProducer> : ProducerConfig where TMessage : class where TProducer : IBusPublisher<TMessage>
    {
        public string Topic;
        public Type MessageType => typeof(TMessage);
    }
}
