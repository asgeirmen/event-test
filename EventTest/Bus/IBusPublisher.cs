using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventTest.Bus
{
    public interface IBusPublisher<T> where T : class
    {
        Task Publish(T message);
        Task Publish(object message);
    }
}
