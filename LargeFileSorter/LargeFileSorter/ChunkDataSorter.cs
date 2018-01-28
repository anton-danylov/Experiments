using System.Collections.Generic;

namespace LargeFileSorter
{
    public class ChunkDataSorter : IChunkDataSorter
    {
        public void SortChunk(List<string> chunk, IComparer<string> comparer)
        {
            chunk.Sort(comparer);
        }
    }
}






