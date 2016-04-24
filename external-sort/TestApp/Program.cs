using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var targetDirectory = ConfigurationManager.AppSettings["SourceDirectory"];
            var files = GetFiles(targetDirectory);

            var sortedFilePath = Path.Combine(targetDirectory, "sorted_all.txt");
            var merger = new ExternalMergeSort<DummyRecord>();
            merger.Merge(files, sortedFilePath, new DummyRecordComparer());

            //TODO: Matcher presumably cleans up files
            Console.ReadKey();
        }

        private static IEnumerable<string> GetFiles(string targetDirectory)
        {
            if (string.IsNullOrEmpty(targetDirectory))
            {
                throw new ArgumentException("Target directory is null or empty.", targetDirectory);
            }

            var di = new DirectoryInfo(targetDirectory);

            return di.GetFiles().Select(fi => fi.FullName);
        }
    }
}