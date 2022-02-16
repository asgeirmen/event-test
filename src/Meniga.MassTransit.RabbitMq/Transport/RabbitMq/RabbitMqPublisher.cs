using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Meniga.MassTransit.Common.Bus;

namespace Meniga.MassTransit.Infrastructure.Transport.RabbitMq
{
    public class RabbitMqPublisher<T> : IBusPublisher<T>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RabbitMqPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        public Task PublishAsync(T message, CancellationToken token)
        {
            return _publishEndpoint.Publish(message, token);
        }
    }
}
