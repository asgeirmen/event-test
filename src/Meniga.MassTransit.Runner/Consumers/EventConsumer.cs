using System.Threading.Tasks;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Meniga.MassTransit.Runner.RabbitMq.Producers;
using Microsoft.Extensions.Logging;

namespace Meniga.MassTransit.Runner.Consumers
{
    public class EventConsumer : IConsumer<IEvent>
    {
        private readonly ILogger<EventConsumer> _logger;
        private readonly RabbitEventTwoProducer<EventTwo> _producer;
        public EventConsumer(ILogger<EventConsumer> logger, RabbitEventTwoProducer<EventTwo> producer)
        {
            _logger = logger;
            _producer = producer;
        }

        public async Task Consume(ConsumeContext<IEvent> context)
        {
            _logger.LogDebug(context.Message.Text);

            await _producer.PublishAsync(new EventTwo { Text = context.Message.Text }, context.CancellationToken);
        }
    }

    public class EventConsumerDefinition :
        ConsumerDefinition<EventConsumer>
    {
        public EventConsumerDefinition()
        {
            ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, 
            IConsumerConfigurator<EventConsumer> consumerConfigurator)
        {
            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }
    }
}
