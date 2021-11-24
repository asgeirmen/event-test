using System;
using System.Threading.Tasks;
using CommandLine;
using EventTest.Bus;
using EventTest.Bus.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventTest
{
    [Verb("consume", HelpText = "Save a code change")]
    public class ConsumeCommand : ICommand
    {
        [Option('g', "group", Required = false, Default = "default", HelpText = "Name of the consumer group ")]
        public string ConsumerGroup { get; set; }

        public  Task Register(IServiceCollection services, IConfiguration config)
        {
            var busConfig = config.GetSection("BusConfig").Get<BusConfig>();
            services.ConfigureEventBus(busConfig, null, new[]
                {
                    new ConsumerConfig<ValueEntered, ValueEnteredEventConsumer>()
                    {
                        ConsumerGroup =  ConsumerGroup
                    }
                }, 
                null);

            return Task.CompletedTask;
        }

        public async Task Execute(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Press enter to exit");

            await Task.Run(() => Console.ReadLine());
        }
    }
}
