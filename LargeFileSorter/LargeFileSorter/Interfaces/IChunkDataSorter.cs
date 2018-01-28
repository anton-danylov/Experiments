using System.Collections.Generic;

namespace LargeFileSorter
{
    public interface IChunkDataSorter
    {
        void SortChunk(List<string> chunk, IComparer<string> comparer);
    }
    }


/*
        public void MergeChunks(string outputFilePath)
        {
            using (var writer = File.CreateText(outputFilePath))
            {
                List<StreamReader> readers = _chunksPath.Select(s => new StreamReader(s, Encoding.UTF8)).ToList();

                C5.IntervalHeap<string> priorityQueue = new C5.IntervalHeap<string>(_comparer);

                bool isDataLeft = true;
                while(isDataLeft)
                {
                    isDataLeft = false;
                    foreach (var reader in readers)
                    {
                        if (reader.EndOfStream)
                            continue;

                        priorityQueue.Add(reader.ReadLine());
                        isDataLeft = true;
                    }

                    while(!priorityQueue.IsEmpty)
                    {
                        writer.WriteLine(priorityQueue.DeleteMin());
                    }
                }

                readers.ForEach(r => r.Dispose());
            }

            _chunksPath.ForEach(f => File.Delete(f));
        }
*/

/*
sw.Reset(); sw.Start();
                        SortChunk(chunk);
sw.Stop();

                        Console.Write($"sorted({sw.Elapsed})");

                        string chunkPath = GetTempFilePath(sortedFilePath, chunkIndex);
_chunksPath.Add(chunkPath);
                        File.WriteAllLines(chunkPath, chunk);

                        Console.WriteLine($"dumped({chunkPath})");
*/



/*

         public void MergeChunks(string outputFilePath, string[] chunkFiles)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var writer = File.CreateText(outputFilePath))
            {
                List<StreamReader> readers = null;
                try
                {
                    readers = chunkFiles.Select(s => new StreamReader(s, Encoding.UTF8)).ToList();
                    SortedDictionary<string, List<StreamReader>> kWayMerger
                        = new SortedDictionary<string, List<StreamReader>>(_comparer);

                    readers.ForEach(reader => AddLineForMerging(kWayMerger, reader, reader.ReadLine()));

                    while (kWayMerger.Count > 0)
                    {
                        var pair = kWayMerger.First();
                        kWayMerger.Remove(pair.Key);

                        string lineToWrite = pair.Key;
                        List<StreamReader> readersForLine = pair.Value;

                        foreach (var reader in readersForLine)
                        {
                            writer.WriteLine(lineToWrite);

                            AddLineForMerging(kWayMerger, reader, reader.ReadLine());
                        }
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

            sw.Stop();
            Console.WriteLine($"Merged:{sw.Elapsed}");
        }

        private static void AddLineForMerging(SortedDictionary<string, List<StreamReader>> kWayMerger, StreamReader reader, string line)
        {
            if (line == null)
                return;

            if (!kWayMerger.TryGetValue(line, out List<StreamReader> readersForLine))
            {
                kWayMerger[line] = new List<StreamReader>() { reader };
            }
            else
            {
                readersForLine.Add(reader);
            }
        }
     


            public void MergeChunks(string outputFilePath, string[] chunkFiles)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var writer = File.CreateText(outputFilePath))
            {
                List<StreamReader> readers = null;
                try
                {
                    readers = chunkFiles.Select(s => new StreamReader(s, Encoding.UTF8)).ToList();
                    SortedDictionary<string, StreamContainer> kWayMerger
                        = new SortedDictionary<string, StreamContainer>(_comparer);

                    readers.ForEach(reader => AddLineForMerging(kWayMerger, reader, reader.ReadLine()));

                    while (kWayMerger.Count > 0)
                    {
                        var pair = kWayMerger.First();
                        kWayMerger.Remove(pair.Key);

                        string lineToWrite = pair.Key;
                        StreamContainer readersForLine = pair.Value;

                        //writer.WriteLine(lineToWrite);
                        //AddLineForMerging(kWayMerger, readersForLine.FirstReader, readersForLine.FirstReader.ReadLine());

                        //if (readersForLine.Readers != null)
                        //{
                        //    foreach (var reader in readersForLine.Readers)
                        //    {
                        //        writer.WriteLine(lineToWrite);
                        //        AddLineForMerging(kWayMerger, reader, reader.ReadLine());
                        //    }
                        //}  
                        foreach (var reader in readersForLine.Readers)
                        {
                            writer.WriteLine(lineToWrite);
                            AddLineForMerging(kWayMerger, reader, reader.ReadLine());
                        }
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

            sw.Stop();
            Console.WriteLine($"Merged:{sw.Elapsed}");
        }

        private static void AddLineForMerging(SortedDictionary<string, StreamContainer> kWayMerger, StreamReader reader, string line)
        {
            if (line == null)
                return;

            if (kWayMerger.ContainsKey(line))
            {
                //StreamContainer container = kWayMerger[line];
                //container.AddReader(reader);

                //kWayMerger[line] = container;
                kWayMerger[line].AddReader(reader);
            }
            else
            {
                kWayMerger[line] = new StreamContainer().AddReader(reader);
            }
        }

        private class StreamContainer
        {
            public StreamReader FirstReader { get; set; }
            public LinkedList<StreamReader> Readers { get; set; }

            public StreamContainer AddReader(StreamReader reader)
            {
                if (Readers == null)
                {
                    Readers = new LinkedList<StreamReader>();
                }

                Readers.AddLast(reader);

                return this;
            }
        }



     */



