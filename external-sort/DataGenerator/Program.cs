using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            BuildFiles();
            Console.WriteLine();
        }

        static void BuildFiles()
        {
            const string extension = ".txt";
            var random = new Random((int)DateTime.UtcNow.Ticks);

            for (int i = 0; i < _fileCount; i++)
            {
                using (var writer = new StreamWriter(_directory + "/file_" + i + extension))
                {
                    for (int j = 0; j < random.Next(_maxRecordCount); j++)
                    {
                        var start = new DateTime(1995, 1, 1);
                        int range = (DateTime.Today - start).Days;
                        var randomDate = start.AddDays(random.Next(range));

                        writer.WriteLine("{0},{1}", randomDate, Guid.NewGuid());
                    }
                }
            }            
        }
    }
}
