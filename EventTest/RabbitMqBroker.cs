using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MassTransit.ConsumeConfigurators;

namespace EventTest
{
    public class RabbitMqBroker : Broker
    {
        private readonly string _host;
        private readonly ushort _port;
        private readonly string _virtualHost;
        private readonly string _username;
        private readonly string _password;
        private  IBusControl _busControl;

        public RabbitMqBroker(ServiceCollection services, string host, ushort port, string virtualHost, string username, string password)
        {
            _host = host;
            _port = port;
            _virtualHost = virtualHost;
            _username = username;
            _password = password;

        }

        public override async Task Publish<T>(T message) 
        {
            await _busControl.Publish(message);
        }


        public override async Task StartAsync(ServiceCollection services)
        {

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(_host, _port == 0 ? (ushort)5672 : _port, _virtualHost, h =>
                        {
                            h.Username(_username);
                            h.Password(_password);
                        });
                    }
                );
            });

            var provider = services.BuildServiceProvider();
            _busControl = provider.GetRequiredService<IBusControl>();

            var consumerMethod = typeof(ConsumerExtensions).GetMethods().FirstOrDefault(m => m.Name == "Consumer" && m.IsGenericMethod && m.GetParameters().Length == 2);
            foreach (var consumer in _consumers)
            {
                MethodInfo consumerGenericMethod = consumerMethod.MakeGenericMethod(consumer.Type);
                _busControl.ConnectReceiveEndpoint(consumer.GroupName, e => consumerGenericMethod.Invoke(null, new object[] {e, null}) );

            }

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await _busControl.StartAsync(source.Token);
        }

        public override Task StopAsync()
        {
            return _busControl.StopAsync();
        }
    }
}
