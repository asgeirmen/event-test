using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EventTest.EventBus
{
    public class RabbitMqConfig
    {
        public string Address { get; set; }
        public string VirtualHost { get; set; }
        public BusTransports Transport { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public bool UseSsl { get; set; }
        public string SslServerName { get; set; }
        public string CertificateIdentifier { get; set; }
        public string PrivateKeyPassword { get; set; }
        public SslProtocols SslProtocols { get; }
        public StoreName StoreName { get; set; }
        public StoreLocation StoreLocation { get; set; }
        public X509FindType FindType { get; set; }
        public CertificateProvider CertificateProvider { get; set; }
    }

    public enum BusTransports
    {
        InMemory,
        RabbitMq
    }

    public enum CertificateProvider
    {
        None,
        Disk,
        Store,
    }

}
