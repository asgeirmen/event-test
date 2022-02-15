using System.Collections.Generic;
using Meniga.MassTransit.Common.Bus;

namespace Meniga.MassTransit.Common
{
    public class ConsumersRegistry : IConsumersRegistry
    {
        public ConsumersRegistry()
        {
            RegisteredConsumeres = new List<ConsumerConfig>();
        }

        public IList<ConsumerConfig> RegisteredConsumeres { get; }

        public void Register(ConsumerConfig consumer)
        {
            RegisteredConsumeres.Add(consumer);
        }
    }

    public interface IConsumersRegistry
    {
        IList<ConsumerConfig> RegisteredConsumeres { get; }
        void Register(ConsumerConfig consumer);
    }
}
