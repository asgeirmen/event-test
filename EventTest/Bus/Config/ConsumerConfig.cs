using System;
using System.Collections.Generic;
using System.Text;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.KafkaIntegration;

namespace EventTest.EventBus
{
    public abstract class ConsumerConfig
    {

    }

    public class ConsumerConfig<TMessage,TConsumer> : ConsumerConfig where TMessage : class where TConsumer : IConsumer<TMessage>
    {
        public string TopicName;
        public string GroupName;

        public Type ConsumerType => typeof(TConsumer);

        public Action<IConsumerConfigurator<TMessage>> Config;


    }
}
