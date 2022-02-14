using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Meniga.MassTransit.RabbitMq.Producer;

namespace Meniga.MassTransit.Runner.RabbitMq.Producers
{
    public class RabbitEventTwoProducer<IEventTwo> : IRabbitMqProducer<IEventTwo>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpoint _sendEndpoint;

        public RabbitEventTwoProducer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;      
        }

        public async Task PublishAsync(IEventTwo message, CancellationToken token)
        {
            await _publishEndpoint.Publish(message, token);
        }

        public async Task SendAsync(IEventTwo message, CancellationToken token)
        {
            await _sendEndpoint.Send(message, token);
        }
    }
}
