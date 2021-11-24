using System;
using MassTransit;
using MassTransit.ConsumeConfigurators;

namespace EventTest.Bus
{
    public abstract class ConsumerConfig
    {

    }

    public class ConsumerConfig<TMessage,TConsumer> : ConsumerConfig where TMessage : class where TConsumer : IConsumer<TMessage>
    {
        public string ConsumerGroup;

        public Type ConsumerType => typeof(TConsumer);

        public Action<IConsumerConfigurator<TMessage>> Config;


    }
}
