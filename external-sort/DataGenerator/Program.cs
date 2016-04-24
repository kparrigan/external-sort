using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp;
using TestApp.Common;

namespace DataGenerator
{
    class Program
    {
        private static string _directory;
        private static int _fileCount;
        private static int _maxRecordCount;

        static void Main(string[] args)
        {
            _directory = ConfigurationManager.AppSettings["directory"];
            int.TryParse(ConfigurationManager.AppSettings["fileCount"], out _fileCount);
            int.TryParse(ConfigurationManager.AppSettings["maxRecordCount"], out _maxRecordCount);

            var files = BuildFiles();
            PreSort(files); //NOTE - We won't need to do this step in the matcher since each file will be pre-sorted via SQL

            Console.WriteLine();
        }

        static IEnumerable<string> BuildFiles()
        {
            const string extension = ".txt";
            var random = new Random((int)DateTime.UtcNow.Ticks);
            var fileList = new List<string>();

            for (int i = 0; i < _fileCount; i++)
            {
                var fileName = _directory + "/file_" + i + extension;
                using (var writer = new StreamWriter(fileName))
                {
                    var recordCount = random.Next(_maxRecordCount);
                    for (int j = 0; j < recordCount; j++)
                    {
                        var start = new DateTime(1995, 1, 1);
                        int range = (DateTime.Today - start).Days;
                        var randomDate = start.AddDays(random.Next(range));

                        writer.WriteLine("{0},{1}", randomDate, Guid.NewGuid());
                    }
                }

                fileList.Add(fileName);
            }

            return fileList;
        }

        private static void PreSort(IEnumerable<string> fileNames)
        {
            var records = new List<DummyRecord>();

            foreach (var fileName in fileNames)
            {
                using (var sr = new StreamReader(fileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        records.Add(new DummyRecord(line));
                    }
                }

                var sortedRecords = records.OrderBy(r => r.DateTimeSent);

                using (var sw = new StreamWriter(fileName, false))
                {
                    foreach (var record in sortedRecords)
                    {
                        sw.WriteLine(record.ToString());
                    }
                }

                records.Clear();
            }
        }
    }
}
