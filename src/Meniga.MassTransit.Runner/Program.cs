using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Meniga.MassTransit.Common.Configuration;
using Meniga.MassTransit.Common.Configuration.Kafka;
using Meniga.MassTransit.Common.Configuration.RabbitMq;
using Meniga.MassTransit.Infrastructure;
using Meniga.MassTransit.Infrastructure.Consumer;
using Meniga.MassTransit.Infrastructure.Producer;
using Meniga.MassTransit.Infrastructure.Rider.Kafka;
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
            var host = CreateHostBuilder(args)
                 .Build();

            var busControl = host.Services.GetService<IBusControl>();
            if (busControl != null)
            {
                await busControl.StartAsync();
            }
            await host.RunAsync();

                return 0;
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddJsonFile("massTransit.json");
            })
            .ConfigureServices((hostContext, services) =>
            {
                var massTransitConfiguration = hostContext.Configuration
                    .GetSection(nameof(MassTransitConfiguration))
                    .Get<MassTransitConfiguration>();

                RegisterBusConsumers<KafkaConfiguration>(services, massTransitConfiguration.KafkaConfiguration.Consumers);
                RegisterBusConsumers<RabbitMqConfiguration>(services, massTransitConfiguration.RabbitMqConfiguration.Consumers);

                if (massTransitConfiguration.KafkaConfiguration != null)
                {
                    RegisterKafkaBusProducer(services, massTransitConfiguration.KafkaConfiguration.Producers);
                }

                services.ConfigureMassTransit(massTransitConfiguration);
                services.AddHostedService<Worker>();
            })
            .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

        private static void RegisterBusConsumers<T>(IServiceCollection services, IDictionary<string, string> consumers)
        {
            var consumersRegistry = new ConsumersRegistry<T>();
            consumersRegistry.Register(new ConsumerConfig<EventOne, EventOneConsumer>() 
            { 
                ConsumerGroup = consumers[nameof(EventOneConsumer)] 
            });
            consumersRegistry.Register(new ConsumerConfig<EventTwo, EventTwoConsumer>() 
            { 
                ConsumerGroup = consumers[nameof(EventTwoConsumer)]
            });

            services.AddSingleton(typeof(IConsumersRegistry<T>), consumersRegistry);
        }

        private static void RegisterKafkaBusProducer(IServiceCollection services, IDictionary<string, string> producers)
        {
            var producersRegistry = new ProducersRegistry();
            producersRegistry.Register(new ProducerConfig<EventOne>()
            {
                ConsumerGroup = producers[$"{nameof(EventOne)}"]
            });

            producersRegistry.Register(new ProducerConfig<EventTwo>()
            {
                ConsumerGroup = producers[$"{nameof(EventTwo)}"]
            });

            services.AddSingleton<IProducersRegistry>(producersRegistry);
        }
    }
}
 