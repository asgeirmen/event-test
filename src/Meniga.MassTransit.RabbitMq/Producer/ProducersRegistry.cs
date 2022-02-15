using System.Collections.Generic;

namespace Meniga.MassTransit.Infrastructure.Producer
{
    public class ProducersRegistry : IProducersRegistry
    {
        public ProducersRegistry()
        {
            RegisteredProducers = new List<ProducerConfig>();
        }

        public IList<ProducerConfig> RegisteredProducers { get; }

        public void Register(ProducerConfig producerConfig)
        {
            RegisteredProducers.Add(producerConfig);
        }
    }

    public interface IProducersRegistry
    {
        IList<ProducerConfig> RegisteredProducers { get; }
        void Register(ProducerConfig producerConfig);
    }
}
