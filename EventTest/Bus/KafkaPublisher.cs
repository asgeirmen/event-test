using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MassTransit.KafkaIntegration;

namespace EventTest.Bus
{
    public class KafkaPublisher<T> : IBusPublisher<T>  where T : class 
    {
        private readonly ITopicProducer<T> _producer;

        public KafkaPublisher(ITopicProducer<T> producer)
        {
            _producer = producer;
        }

        public Task Publish(T message)
        {
            return _producer.Produce(message);
        }
    }
}
