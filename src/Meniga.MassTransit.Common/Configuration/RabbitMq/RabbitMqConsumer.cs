using System;
using System.Collections.Generic;
using System.Text;

namespace Meniga.MassTransit.Common.Configuration.RabbitMq
{
    public class RabbitMqConsumer
    {
        public string Type { get; set; }
        public string Definition { get; set; }
        public string Endpoint { get; set; }
    }
}
