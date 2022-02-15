using System.Threading.Tasks;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;

namespace Meniga.MassTransit.Runner.Consumers
{
    public class EventOneConsumer : IConsumer<EventOne>
    {
        private readonly ILogger<EventOneConsumer> _logger;
        public EventOneConsumer(ILogger<EventOneConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<EventOne> context)
        {
            _logger.LogDebug(context.Message.Text);
            await context.Publish(new EventTwo { Text = context.Message.Text }, context.CancellationToken);
        }
    }

    public class EventConsumerDefinition :
        ConsumerDefinition<EventOneConsumer>
    {
        public EventConsumerDefinition()
        {
            ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, 
            IConsumerConfigurator<EventOneConsumer> consumerConfigurator)
        {
            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }
    }
}
