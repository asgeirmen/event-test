using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace EventTest
{
    public class ValueEnteredEventConsumer :
        IConsumer<ValueEntered>
    {
        public Task Consume(ConsumeContext<ValueEntered> context)
        {
            Console.WriteLine("Value entered: {0}", context.Message.Value);
            return Task.CompletedTask;
        }
    }
}
