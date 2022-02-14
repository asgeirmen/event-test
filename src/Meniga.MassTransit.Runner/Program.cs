using System.Threading.Tasks;
using Meniga.MassTransit.Common.Configuration;
using Meniga.MassTransit.RabbitMq;
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
                services.AddOptions<MassTransitConfiguration>()
                        .BindConfiguration(nameof(MassTransitConfiguration));

                services.ConfigureMassTransitRabbitMqBus(hostContext.Configuration.GetSection(nameof(MassTransitConfiguration)).Get<MassTransitConfiguration>().RabbitMqConfiguration);
                //services.ConfigureMassTransitKafkaRider(hostContext.Configuration.Get<MassTransitConfiguration>().KafkaConfiguration);
            })
            .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
    }
}
