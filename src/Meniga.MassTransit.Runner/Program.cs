using System.Collections.Generic;
using System.Threading.Tasks;
using Meniga.MassTransit.Common;
using Meniga.MassTransit.Common.Bus;
using Meniga.MassTransit.Common.Configuration;
using Meniga.MassTransit.RabbitMq;
using Meniga.MassTransit.Runner.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Meniga.MassTransit.Runner
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            await CreateHostBuilder(args)
                 .Build()
                 .RunAsync();

            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("massTransit.json");
            })
            .ConfigureServices((hostContext, services) =>
            {
                var massTransitConfiguration = hostContext.Configuration
                    .GetSection(nameof(MassTransitConfiguration))
                    .Get<MassTransitConfiguration>();

                RegisterBusConsumers(services, massTransitConfiguration.Consumers);

                services.ConfigureMassTransit(massTransitConfiguration);
                services.AddHostedService<Worker>();
            })
            .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

        private static void RegisterBusConsumers(IServiceCollection services, IDictionary<string, string> consumers)
        {
            var consumersRegistry = new ConsumersRegistry();
            consumersRegistry.Register(new ConsumerConfig<EventOne, EventOneConsumer>() 
            { 
                ConsumerGroup = consumers[nameof(EventOneConsumer)] 
            });
            consumersRegistry.Register(new ConsumerConfig<EventTwo, EventTwoConsumer>() 
            { 
                ConsumerGroup = consumers[nameof(EventTwoConsumer)]
            });

            services.AddSingleton<IConsumersRegistry>(consumersRegistry);
        }
    }
}
 