using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public interface IExternalMergeSort<T>
    {
        void Merge(IEnumerable<string> fileNames, string sortedFilePath, IComparer<T> comparer, bool cleanup);
    }
}
