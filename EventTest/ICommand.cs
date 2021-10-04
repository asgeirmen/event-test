using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventTest
{
    public interface ICommand
    {
         Task Execute();
    }
}
