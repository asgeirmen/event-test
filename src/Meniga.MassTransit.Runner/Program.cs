using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Meniga.MassTransit.Common.Bus;
using Meniga.MassTransit.Common.Configuration;
using Meniga.MassTransit.Infrastructure.Consumer;
using Meniga.MassTransit.Infrastructure.Kafka;
using Meniga.MassTransit.Infrastructure.Producer;
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

                if (massTransitConfiguration.KafkaConfiguration != null)
                {
                    RegisterKafkaBusProducer(services, massTransitConfiguration.KafkaConfiguration.Producers);
                }

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

        private static void RegisterKafkaBusProducer(IServiceCollection services, IDictionary<string, string> producers)
        {
            var producersRegistry = new ProducersRegistry();
            producersRegistry.Register(new ProducerConfig<EventOne, KafkaPublisher<EventOne>>()
            {
                Topic = producers[$"{nameof(KafkaPublisher<EventOne>)};{nameof(EventOne)}"]
            });

            producersRegistry.Register(new ProducerConfig<EventTwo, KafkaPublisher<EventTwo>>()
            {
                Topic = producers[$"{nameof(KafkaPublisher<EventTwo>)};{nameof(EventTwo)}"]
            });

            services.AddSingleton<IProducersRegistry>(producersRegistry);
        }
    }
}
 