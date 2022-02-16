using System.Collections.Generic;

namespace Meniga.MassTransit.Common.Configuration.RabbitMq
{
    public class RabbitMqConfiguration
    {
        public string HostName { get; set; }
        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public IDictionary<string,string> Consumers { get; set; }
    }
}
