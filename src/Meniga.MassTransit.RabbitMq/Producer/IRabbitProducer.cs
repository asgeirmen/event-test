using System.Threading;
using System.Threading.Tasks;
using Meniga.MassTransit.Common.Bus;

namespace Meniga.MassTransit.RabbitMq.Producer
{
    public interface IRabbitMqProducer<T> : IBusPublisher<T> 
    {
        Task SendAsync(T message, CancellationToken token);
    }
}
