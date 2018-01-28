using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LargeFileSorter
{
    public class SyncSplitStrategy : SplitStrategyBase, ISplitChunksStrategy
    {
        private readonly IFileChunkReader _chunkReader;
        private readonly IChunkDataSorter _sorter;
        private IComparer<string> _comparer;

        public SyncSplitStrategy(IFileChunkReader chunkReader, IChunkDataSorter sorter, IComparer<string> comparer)
        {
            _chunkReader = chunkReader;
            _sorter = sorter;
            _comparer = comparer;
        }

        public string[] SplitFileToSortedChunks(string sortedFilePath, long bytesOfMemoryToConsume, Func<string, int, string> getChunkPath)
        {
            AdjustMemoryLimit(ref bytesOfMemoryToConsume);

            List<string> chunkFiles = new List<string>();

            using (StreamReader reader = new StreamReader(sortedFilePath, Encoding.UTF8))
            {
                int chunkIndex = 0;
                while (!reader.EndOfStream)
                {
                    Console.Write($"Chunk:{chunkIndex} ");
                    List<string> chunkData = _chunkReader.ReadFileChunk(bytesOfMemoryToConsume, reader);
                    Console.WriteLine($"loaded;");

                    _sorter.SortChunk(chunkData, _comparer);
                    string chunkPath = getChunkPath(sortedFilePath, chunkIndex);
                    chunkFiles.Add(chunkPath);
                    File.WriteAllLines(chunkPath, chunkData);

                    chunkIndex++;
                }
            }

            DoLargeObjectHeapCompaction();

            return chunkFiles.ToArray();
        }
    }
}
