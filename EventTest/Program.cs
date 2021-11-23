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
        private static IServiceProvider _serviceProvider;

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

            //await host.RunAsync();

            //IConfiguration Configuration = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddEnvironmentVariables()
            //    .AddCommandLine(args)
            //    .Build();

            //RegisterServices();
            //IServiceScope scope = _serviceProvider.CreateScope();
            //scope.ServiceProvider.GetRequiredService<ConsoleApplication>().Run();
            //DisposeServices();

            //await Parser.Default.ParseArguments<PublishCommand, ConsumeCommand>(args)
            //    .WithParsedAsync<ICommand>(t => t.Execute());
        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            //services.AddSingleton<ICustomer, Customer>();
            //services.AddSingleton<ConsoleApplication>();
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
