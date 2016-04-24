using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Common
{
    public interface IParseable<T>
    {
        void Parse(string text);
    }
}
