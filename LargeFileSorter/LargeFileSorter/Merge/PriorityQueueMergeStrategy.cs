using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LargeFileSorter
{
    public class PriorityQueueMergeStrategy : IMergeChunksStrategy
    {
        private IComparer<string> _comparer;

        public PriorityQueueMergeStrategy(IComparer<string> comparer)
        {
            _comparer = comparer;
        }

        public void MergeChunks(string outputFilePath, string[] chunkFiles)
        {
            using (var writer = File.CreateText(outputFilePath))
            {
                List<StreamReader> readers = chunkFiles.Select(s => new StreamReader(s, Encoding.UTF8)).ToList();

                var priorityQueue = new C5.IntervalHeap<string>(_comparer);
                var readerForHandle = new Dictionary<C5.IPriorityQueueHandle<string>, StreamReader>();

                // Fill priority queue with initial strings. Each string is associated with reader it came from
                readers.ForEach(reader => {
                    C5.IPriorityQueueHandle<string> handle = null;
                    priorityQueue.Add(ref handle, reader.ReadLine());
                    readerForHandle[handle] = reader;
                });

                // Get minimal element from queue and read the next line from respective stream
                // repeat until all streams are exhausted
                while (!priorityQueue.IsEmpty)
                {
                    C5.IPriorityQueueHandle<string> handleMin;
                    string line = priorityQueue.DeleteMin(out handleMin);
                    writer.WriteLine(line);

                    var reader = readerForHandle[handleMin];
                    string nextLine = reader.ReadLine();
                    if (nextLine != null)
                    {
                        C5.IPriorityQueueHandle<string> handle = null;
                        priorityQueue.Add(ref handle, nextLine);
                        readerForHandle[handle] = reader;
                    }
                }

                readers.ForEach(r => r.Dispose());
            }

            chunkFiles.ForEach(f => File.Delete(f));
        }
    }
}







