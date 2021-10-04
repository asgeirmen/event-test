using System;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventTest
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            //IConfiguration Configuration = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddEnvironmentVariables()
            //    .AddCommandLine(args)
            //    .Build();

            //RegisterServices();
            //IServiceScope scope = _serviceProvider.CreateScope();
            //scope.ServiceProvider.GetRequiredService<ConsoleApplication>().Run();
            //DisposeServices();

            await Parser.Default.ParseArguments<PublishCommand, ConsumeCommand>(args)
                .WithParsedAsync<ICommand>(t => t.Execute());
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
