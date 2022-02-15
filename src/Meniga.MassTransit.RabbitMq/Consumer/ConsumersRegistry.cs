using System.Collections.Generic;

namespace Meniga.MassTransit.Infrastructure.Consumer
{
    public class ConsumersRegistry : IConsumersRegistry
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

    public interface IConsumersRegistry
    {
        IList<ConsumerConfig> RegisteredConsumeres { get; }
        void Register(ConsumerConfig consumerConfig);
    }
}
