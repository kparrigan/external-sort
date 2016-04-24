using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestApp.Common;

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

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            merger.Merge(files, sortedFilePath, new DummyRecordComparer());
            stopWatch.Stop();

            var ts = stopWatch.Elapsed;
            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("Merge Process took: " + elapsedTime);

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