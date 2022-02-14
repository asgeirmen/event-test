using System.Threading.Tasks;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;

namespace Meniga.MassTransit.Runner.Consumers
{
    public class EventTwoConsumer : IConsumer<EventTwo>
    {
        private readonly ILogger<EventTwoConsumer> _logger;
        public EventTwoConsumer(ILogger<EventTwoConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<EventTwo> context)
        {
            _logger.LogDebug(context.Message.Text);
            return Task.CompletedTask;
        }
    }

    public class EventTwoConsumerDefinition :
       ConsumerDefinition<EventTwoConsumer>
    {
        public EventTwoConsumerDefinition()
        {
            ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<EventTwoConsumer> consumerConfigurator)
        {
            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }
    }
}
