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
        private static IComparer<DummyRecord> _comparer = new DummyRecordComparer();

        static void Main(string[] args)
        {
            var targetDirectory = ConfigurationManager.AppSettings["SourceDirectory"];
            var files = GetFiles(targetDirectory);

            PreSort(files); //NOTE - We won't need to do this step in the matcher since each file will be pre-sorted via SQL

            var sortedFilePath = Path.Combine(targetDirectory, "sorted_all.txt");
            Merge(files, sortedFilePath);

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

        //IGNORE THIS METHOD
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

        private static void Merge(IEnumerable<string> fileNames, string sortedFilePath)
        {
            var fileReaders = fileNames
                .Select(path => new StreamReader(path))
                .Where(fileReader => !fileReader.EndOfStream);

            var dictionary = new SortedDictionary<DummyRecord, StreamReader>(new DummyRecordComparer());

            foreach (var reader in fileReaders)
            {
                var record = GetNextUniqueRecord(reader, dictionary);
                dictionary.Add(record, reader);
            }

            using (var sw = new StreamWriter(sortedFilePath, false))
            {
                while (dictionary.Any())
                {
                    var record = dictionary.Keys.Last();
                    var fileReader = dictionary[record];
                    dictionary.Remove(record);

                    sw.WriteLine(record);

                    var nextRecord = GetNextUniqueRecord(fileReader, dictionary);

                    if (nextRecord != null)
                    {
                        dictionary.Add(nextRecord, fileReader);
                    }
                    else
                    {
                        fileReader.Dispose();
                    }
                }
            }
        }

        private static DummyRecord GetNextUniqueRecord(StreamReader fileReader,
            SortedDictionary<DummyRecord, StreamReader> dictionary)
        {
            if (fileReader == null)
            {
                throw new ArgumentNullException("fileReader");
            }

            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            while (!fileReader.EndOfStream)
            {
                var nextLine = fileReader.ReadLine();

                if (string.IsNullOrEmpty(nextLine))
                {
                    return null;
                }

                var current = new DummyRecord(nextLine);

                if (!dictionary.ContainsKey(current))
                {
                    return current;
                }
            }

            return null;
        }
    }
}