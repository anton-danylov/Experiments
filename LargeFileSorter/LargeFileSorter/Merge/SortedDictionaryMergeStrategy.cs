using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace LargeFileSorter
{
    public class SortedDictionaryMergeStrategy : IMergeChunksStrategy
    {
        private IComparer<string> _comparer;

        public SortedDictionaryMergeStrategy(IComparer<string> comparer)
        {
            _comparer = comparer;
        }

        public void MergeChunks(string outputFilePath, string[] chunkFiles)
        {
            Console.WriteLine(nameof(SortedDictionaryMergeStrategy));
            using (var writer = File.CreateText(outputFilePath))
            {
                List<StreamReader> readers = null;
                try
                {
                    readers = chunkFiles.Select(s => new StreamReader(s, Encoding.UTF8)).ToList();
                    SortedDictionary<string, StreamContainer> kWayMerger
                        = new SortedDictionary<string, StreamContainer>(_comparer);

                    // Here SortedDictionary is used as a priority queue for k-Way merge. The key is string, and the value
                    // is a linked list of readers this string came from. Approach is similar as in PriorityQueueMergeStrategy,
                    // but the difference is that here there could be multiple readers for the same line. 
                    // It could be beneficial if there are many identical strings
                    // At first read a line from each stream
                    readers.ForEach(reader => AddLineForMerging(kWayMerger, reader, reader.ReadLine()));


                    while (kWayMerger.Count > 0)
                    {
                        var pair = kWayMerger.First();
                        kWayMerger.Remove(pair.Key);

                        string lineToWrite = pair.Key;
                        StreamContainer readersForLine = pair.Value;

                        // Write minimal line from the dictionary to the output for each reader in the list
                        do
                        {
                            StreamReader reader = readersForLine.Reader;
                            readersForLine = readersForLine.Container;

                            writer.WriteLine(lineToWrite);
                            AddLineForMerging(kWayMerger, reader, reader.ReadLine());
                        }
                        while (readersForLine != null);
                    }
                }
                finally
                {
                    readers?.ForEach(r => r.Dispose());
                }
            }

            foreach (var path in chunkFiles)
            {
                File.Delete(path);
            }
        }

        private static void AddLineForMerging(SortedDictionary<string, StreamContainer> kWayMerger, StreamReader reader, string line)
        {
            if (line == null)
                return;

            if (kWayMerger.ContainsKey(line))
            {
                kWayMerger[line].AddReader(reader);
            }
            else
            {
                kWayMerger[line] = new StreamContainer() { Reader = reader };
            }
        }

        private class StreamContainer
        {
            public StreamReader Reader { get; set; }
            public StreamContainer Container { get; set; }

            public void AddReader(StreamReader reader)
            {
                var head = new StreamContainer() { Reader = reader, Container = Container };
                Container = head;
            }
        }
    }
}
