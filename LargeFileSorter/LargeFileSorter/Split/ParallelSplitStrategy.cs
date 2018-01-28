using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LargeFileSorter
{
    public class ParallelSplitStrategy : SplitStrategyBase, ISplitChunksStrategy
    {
        private readonly IFileChunkReader _chunkReader;
        private readonly IChunkDataSorter _sorter;
        private IComparer<string> _comparer;

        public ParallelSplitStrategy(IFileChunkReader chunkReader, IChunkDataSorter sorter, IComparer<string> comparer)
        {
            _chunkReader = chunkReader;
            _sorter = sorter;
            _comparer = comparer;
        }

        public string[] SplitFileToSortedChunks(string sortedFilePath, long bytesOfMemoryToConsume, Func<string, int, string> getChunkPath)
        {
            AdjustMemoryLimit(ref bytesOfMemoryToConsume);

            ConcurrentBag<string> chunkFiles = new ConcurrentBag<string>();

            List<Task> tasksStarted = new List<Task>();
            using (StreamReader reader = new StreamReader(sortedFilePath, Encoding.UTF8))
            {
                int chunkIndex = 0;
                while (!reader.EndOfStream)
                {
                    Console.Write($"Chunk:{chunkIndex} ");
                    List<string> chunk = _chunkReader.ReadFileChunk(bytesOfMemoryToConsume, reader);
                    Console.WriteLine($"loaded;");

                    var newTask = Task.Factory.StartNew((o) =>
                    {
                        var state = (Tuple<List<string>, int>)o;
                        List<string> chunkData = state.Item1;
                        int currentChunkIndex = state.Item2;

                        _sorter.SortChunk(chunkData, _comparer);

                        string chunkPath = getChunkPath(sortedFilePath, currentChunkIndex);
                        chunkFiles.Add(chunkPath);
                        File.WriteAllLines(chunkPath, chunk);

                    }, new Tuple<List<string>, int>(chunk, chunkIndex));


                    tasksStarted.Add(newTask);

                    if (tasksStarted.Count > Environment.ProcessorCount - 1)
                    {
                        Task.WaitAny(tasksStarted.ToArray());
                        tasksStarted.RemoveAll(t => t.IsCompleted || t.IsCanceled || t.IsFaulted);
                    }

                    chunkIndex++;
                }

                Task.WaitAll(tasksStarted.ToArray());
            }

            return chunkFiles.ToArray();
        }

        override protected long GetMemoryLimitForOneChunk()
        {
            return  base.GetMemoryLimitForOneChunk() / Environment.ProcessorCount; // chunks for each core are loaded to RAM simultaneously
        }
    }
}