using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public sealed class ExternalMergeSort<T> : IExternalMergeSort<T> where T : class, IParseable<T>, new()
    {
        public void Merge(IEnumerable<string> fileNames, string sortedFilePath, IComparer<T> comparer)
        {
            if (fileNames == null)
            {
                throw new ArgumentNullException("fileNames");
            }

            if (!fileNames.Any())
            {
                return;
            }

            if (string.IsNullOrEmpty(sortedFilePath))
            {
                throw new ArgumentException("Sorted file path is null or empty.", "sortedFilePath");
            }

            var fileReaders = fileNames
                .Select(path => new StreamReader(path))
                .Where(fileReader => !fileReader.EndOfStream);

            var dictionary = new SortedDictionary<T, StreamReader>(comparer);

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

        private T GetNextUniqueRecord(StreamReader fileReader,
            SortedDictionary<T, StreamReader> dictionary)
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

                var current = new T();
                current.Parse(nextLine);

                if (!dictionary.ContainsKey(current))
                {
                    return current;
                }
            }

            return null;
        }
    }
}
