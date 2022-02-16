using System.Collections.Generic;

namespace Meniga.MassTransit.Infrastructure.Consumer
{
    public class ConsumersRegistry<T> : IConsumersRegistry<T>
    {
        public ConsumersRegistry()
        {
            RegisteredConsumeres = new List<ConsumerConfig>();
        }

        public IList<ConsumerConfig> RegisteredConsumeres { get; }

        public void Register(ConsumerConfig consumerConfig)
        {
            RegisteredConsumeres.Add(consumerConfig);
        }
    }

    public interface IConsumersRegistry<T>
    {
        IList<ConsumerConfig> RegisteredConsumeres { get; }
        void Register(ConsumerConfig consumerConfig);
    }
}
