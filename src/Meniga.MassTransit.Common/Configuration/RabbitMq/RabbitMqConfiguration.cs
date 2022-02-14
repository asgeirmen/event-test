using System;
using System.Collections.Generic;
using System.Text;

namespace Meniga.MassTransit.Common.Configuration.RabbitMq
{
    public class RabbitMqConfiguration
    {
        public HostSettings Host { get; set; }
        public IEnumerable<RabbitMqConsumer> Consumers { get; set; }
    }
}
