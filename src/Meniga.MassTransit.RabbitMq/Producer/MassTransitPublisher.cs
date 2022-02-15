using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Meniga.MassTransit.Common.Bus;

namespace Meniga.MassTransit.Infrastructure.Producer
{
    public class MassTransitPublisher<T> : IBusPublisher<T>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        public Task PublishAsync(T message, CancellationToken token)
        {
            return _publishEndpoint.Publish(message, token);
        }
    }
}
