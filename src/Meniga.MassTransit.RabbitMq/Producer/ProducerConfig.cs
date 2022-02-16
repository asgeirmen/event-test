using System;
using Meniga.MassTransit.Common.Bus;

namespace Meniga.MassTransit.Infrastructure.Producer
{
    public abstract class ProducerConfig
    {
    }

    public class ProducerConfig<TMessage> : ProducerConfig where TMessage : class
    {
        public string ConsumerGroup;
        public Type MessageType => typeof(TMessage);
    }
}
