# Overview
This repository is a POC on how to support both RabbitMq and Kafka by the same service. This is done by implementing an abstraction layer on top of MassTransit, similar to what was done in [Meniga Messaging Library](https://github.com/meniga/messaging).

MassTransit already supports Kafka via *Riders*, see [MassTransit documentation](https://masstransit-project.com/usage/riders/kafka.html). Allowing usage of both RabbitMq and Kafka in a service requires a code that is both aware of Kafka via Riders and RabbitMq via traditional patterns. The purpose of this POC is to create an abstration layer that makes the service code independent of the transport. Changing the transport between Kafka and RabbitMQ can be done with a simple config change.

The assumption is that all services have corresponding `BusConfig` that be read from a config file, environment variables or from a centralized location. The structure is as follows:

```json
{
  "BusConfig": {
    "Transport": "Kafka", 
    "RabbitMq": {
      "Host": "localhost",
      "Port": 5672
    },
    "Kafka": {
      "BootstrapServers": "localhost:9092"
    }
  }
}
```

Setting up the bus with publishers and consumers can be done with a single service extension as following:

```c#
using EventTest.Bus;
{
    ...

    public Task Register(IServiceCollection services, IConfiguration config, ILogger logger)
    {

        var busConfig = config.GetSection("BusConfig").Get<BusConfig>();
        services.ConfigureEventBus(busConfig, logger, 
            new[] { // List of consumers
                    new ConsumerConfig<ValueEntered, ValueEnteredEventConsumer>()
                    {
                        ConsumerGroup =  "my-consumer-group"
                    }
                }, 
            new [] { // List of publishers
                typeof(ValueEntered) 
            }
        );

        return Task.CompletedTask;
    }
}
```
This code connects to an event bus and configures one consumer and one publisher that are reading and writing `ValueEntered` messages.  `ValueEnteredEventConsumer` is a consumer that gets the `ValueEntered` messages:
```c#
using System;
using System.Threading.Tasks;
using MassTransit;

namespace EventTest
{
    public class ValueEnteredEventConsumer : IConsumer<ValueEntered>
    {
        public Task Consume(ConsumeContext<ValueEntered> context)
        {
            // Do something with the message
            return Task.CompletedTask;
        }
    }
}
```

To publish `ValueEntered` messages, the publisher can get a hold of `IBusPublisher<ValueEntered>` from the service provider, e.g. by having it as an argument in a publisher constructor or getting it directly from a service provider like follows:
```c#
var publisher = serviceProvider.GetRequiredService<IBusPublisher<ValueEntered>>();
await publisher.Publish(new ValueEntered()
    {
        Value = "my message value"
    });
```

## Getting started
Read following article to test the POC with Kafka 
* [Getting started with Kafka](kafka-getting-started.md)



 


