﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Confluent.Kafka;
using EventTest.Bus.Config;
using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventTest.Bus
{
    public static class BusSetupExtensions
    {
        public static IServiceCollection ConfigureEventBus(this IServiceCollection services,
            BusConfig busConfig, ILogger logger, IList<ConsumerConfig> consumers, IList<Type> publisherTypes)
        {
            if (busConfig.Transport == BusTransports.Kafka)
            {
                if (busConfig.Kafka == null)
                {
                    throw new Exception("Kafka config is missing");
                }

                services.AddMassTransit(x =>
                {
                    x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));

                    x.AddRider(rider =>
                    {
                        if (publisherTypes != null)
                        {
                            foreach (var publisher in publisherTypes)
                            {
                                var method = typeof(KafkaProducerRegistrationExtensions).GetMethods()
                                    .FirstOrDefault(m => m.Name == "AddProducer" && m.IsGenericMethod);
                                MethodInfo generic = method.MakeGenericMethod(publisher);
                                generic.Invoke(null, new object[] {rider, publisher.FullName, null});

                                var publisherInterface = typeof(IBusPublisher<>).MakeGenericType(publisher);
                                var publisherImpl = typeof(KafkaPublisher<>).MakeGenericType(publisher);
                                services.AddTransient(publisherInterface, publisherImpl);
                            }
                        }

                        if (consumers != null)
                        {
                            foreach (var consumer in consumers)
                            {
                                var consumerType = ((dynamic) consumer).ConsumerType;
                                var method = rider.GetType().GetMethods()
                                    .FirstOrDefault(m => m.Name == "AddConsumer" && m.IsGenericMethod);
                                MethodInfo generic = method.MakeGenericMethod(consumerType);
                                rider.AddConsumer(consumerType);
                                generic.Invoke(rider, new object[] {null});
                            }
                        }


                        rider.UsingKafka((context, k) =>
                        {
                            string[] bootstrapServers = busConfig.Kafka?.BootstrapServers.Split(",") ?? new []{"localhost:9092"};
                            k.Host(bootstrapServers, configurator =>
                            {
                                if (busConfig.Kafka.CancellationDelay != null) configurator.CancellationDelay(TimeSpan.FromMilliseconds(busConfig.Kafka.CancellationDelay.Value));
                                if (busConfig.Kafka.Ssl != null)
                                {
                                    var ssl = busConfig.Kafka.Ssl;
                                    configurator.UseSsl(sslConfig =>
                                    {
                                        if (ssl.CaLocation != null) sslConfig.CaLocation = ssl.CaLocation;
                                        if (ssl.CertificateLocation != null) sslConfig.CertificateLocation = ssl.CertificateLocation;
                                        if (ssl.CertificatePem != null) sslConfig.CertificatePem = ssl.CertificatePem;
                                        if (ssl.CipherSuites != null) sslConfig.CipherSuites = ssl.CipherSuites;
                                        if (ssl.CrlLocation != null) sslConfig.CrlLocation = ssl.CrlLocation;
                                        if (ssl.CurvesList != null) sslConfig.CurvesList = ssl.CurvesList;
                                        if (ssl.EndpointIdentificationAlgorithm != null) sslConfig.EndpointIdentificationAlgorithm = Enum.Parse<SslEndpointIdentificationAlgorithm>(ssl.EndpointIdentificationAlgorithm);
                                        if (ssl.KeyPassword != null) sslConfig.KeyPassword = ssl.KeyPassword;
                                        if (ssl.KeyPem != null) sslConfig.KeyPem = ssl.KeyPem;
                                        if (ssl.KeystoreLocation != null) sslConfig.KeystoreLocation = ssl.KeystoreLocation;
                                        if (ssl.KeyLocation != null) sslConfig.KeyLocation = ssl.KeyLocation;
                                        if (ssl.KeystorePassword != null) sslConfig.KeystorePassword = ssl.KeystorePassword;
                                        if (ssl.SigalgsList != null) sslConfig.SigalgsList = ssl.SigalgsList;
                                    });
                                    // TODO: Add SaslConfig
                                }
                                if (busConfig.Kafka.Sasl != null)
                                {
                                    var sasl = busConfig.Kafka.Sasl;
                                    configurator.UseSasl(saslConfig =>
                                    {
                                        if (sasl.KerberosKeytab != null) saslConfig.KerberosKeytab = sasl.KerberosKeytab;
                                        if (sasl.KerberosKinitCmd != null) saslConfig.KerberosKinitCmd = sasl.KerberosKinitCmd;
                                        if (sasl.KerberosPrincipal != null) saslConfig.KerberosPrincipal = sasl.KerberosPrincipal;
                                        if (sasl.KerberosServiceName != null) saslConfig.KerberosServiceName = sasl.KerberosServiceName;
                                        if (sasl.Mechanism != null) saslConfig.Mechanism = Enum.Parse<SaslMechanism>(sasl.Mechanism);
                                        if (sasl.OauthbearerConfig != null) saslConfig.OauthbearerConfig = sasl.OauthbearerConfig;
                                        if (sasl.Password != null) saslConfig.Password = sasl.Password;
                                        if (sasl.Username != null) saslConfig.Username = sasl.Username;
                                    });
                                }
                            });

                            if (consumers != null)
                            {
                                foreach (var consumer in consumers)
                                {
                                    var consumerType = (Type) ((dynamic) consumer).ConsumerType;

                                    var eventType = consumerType.GetInterfaces()
                                        .First(i => i.Name.StartsWith("IConsumer") && i.IsGenericType)
                                        .GetGenericArguments()
                                        .First();

                                    var method = typeof(KafkaConfiguratorExtensions).GetMethods()
                                        .FirstOrDefault(m => m.Name == "TopicEndpoint" && m.IsGenericMethod);
                                    MethodInfo generic = method.MakeGenericMethod(eventType);

                                    Action<IKafkaTopicReceiveEndpointConfigurator> action = e =>
                                    {
                                        e.EnableAutoOffsetStore = true;
                                        e.ConfigureConsumer(context, consumerType);
                                    };

                                    generic.Invoke(null,
                                        new object[] {k, eventType.FullName, ((dynamic) consumer).ConsumerGroup, action});
                                }
                            }
                        });
                    });
                });
            }
            else if (busConfig.Transport == BusTransports.RabbitMq)
            {
                if (busConfig.RabbitMq == null)
                {
                    throw new Exception("RabbitMq config is missing");
                }

                services.AddMassTransit(x =>
                {
                    if (consumers != null)
                    {
                        foreach (var consumer in consumers)
                        {
                            var consumerType = ((dynamic)consumer).ConsumerType;
                            var consumerConfig = (Type)((dynamic)consumer).Config;
                            var method = x.GetType().GetMethods()
                                .FirstOrDefault(m => m.Name == "AddConsumer" && m.IsGenericMethod);
                            MethodInfo generic = method.MakeGenericMethod(consumerType);
                            generic.Invoke(x, new object[] { consumerConfig });
                        }
                    }

                    x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(busConfig.RabbitMq.Host ?? "localhost", (ushort)(busConfig.RabbitMq.Port ?? 5672), busConfig.RabbitMq.VirtualHost ?? "/", h =>
                            {
                                if (busConfig.RabbitMq.Username != null) h.Username(busConfig.RabbitMq.Username);
                                if (busConfig.RabbitMq.Password != null) h.Password(busConfig.RabbitMq.Password);
                            });

                            if (consumers != null)
                            {
                                foreach (var consumer in consumers)
                                {
                                    var consumerType = (Type)((dynamic)consumer).ConsumerType;
                                    var consumerGroupName = (string) ((dynamic) consumer).ConsumerGroup;
                                    cfg.ReceiveEndpoint(consumerGroupName,
                                        e => { e.ConfigureConsumer(context, consumerType); });
                                }
                            }
                        }
                    );
                    if (publisherTypes != null)
                    {
                        foreach (var publisher in publisherTypes)
                        {
                            var publisherInterface = typeof(IBusPublisher<>).MakeGenericType(publisher);
                            var publisherImpl = typeof(RabbitMqPublisher<>).MakeGenericType(publisher);
                            services.AddTransient(publisherInterface, publisherImpl);
                        }
                    }
                });

            }
            else
            {
                throw new Exception("Transport not configured");
            }

            return services;
        }
    }
}
