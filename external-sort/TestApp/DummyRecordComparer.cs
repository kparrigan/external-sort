using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public sealed class DummyRecordComparer : IComparer<DummyRecord>
    {
        int IComparer<DummyRecord>.Compare(DummyRecord x, DummyRecord y)
        {
            return x.CompareTo(y);
        }
    }
}
