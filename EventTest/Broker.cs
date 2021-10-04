using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace EventTest
{
    public abstract class Broker
    {
        protected List<ConsumerConfig> _consumers = new List<ConsumerConfig>();
        protected List<Type> _publisherTypes = new List<Type>();

        public abstract Task Publish<T>(T message);

        public void AddConsumer(string consumerGroupName, Type type)
        {
            _consumers.Add(new ConsumerConfig()
            {
                GroupName = consumerGroupName,
                Type = type
            });
        }

        public void AddPublisher<T>()
        {
            _publisherTypes.Add(typeof(T));
        }

        public abstract Task StartAsync(ServiceCollection servicesCollection);
        public abstract Task StopAsync();
    }

    public class ConsumerConfig
    {
        public string GroupName;
        public Type Type;
    }
}
