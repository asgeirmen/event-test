using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventTest
{
    public interface ICommand
    {
        Task Register(IServiceCollection services, IConfiguration config);
        Task Execute(IServiceProvider serviceProvider);
    }
}
