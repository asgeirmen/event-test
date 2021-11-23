using System;
using System.Threading.Tasks;
using CommandLine;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var parserResults = Parser.Default.ParseArguments<PublishCommand, ConsumeCommand>(args);

            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args)
                        .Build();

                    parserResults.WithParsedAsync<ICommand>(t => t.Register(services, config));
                })
                .Build();

            var busControl = host.Services.GetRequiredService<IBusControl>();
            await busControl.StartAsync();

            await parserResults.WithParsedAsync<ICommand>(t => t.Execute(host.Services));

            await busControl.StopAsync();
        }
    }
}
