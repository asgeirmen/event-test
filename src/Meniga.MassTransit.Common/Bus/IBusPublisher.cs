using System.Threading;
using System.Threading.Tasks;

namespace Meniga.MassTransit.Common.Bus
{
    public interface IBusPublisher<T>
    {
        Task PublishAsync(T message, CancellationToken token);
    }
}
