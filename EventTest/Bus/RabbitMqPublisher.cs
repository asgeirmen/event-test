using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace EventTest.Bus
{
    public class RabbitMqPublisher<T> : IBusPublisher<T> where T : class
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RabbitMqPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task Publish(T message)
        {
            return _publishEndpoint.Publish<T>(message);
        }
    }
}
