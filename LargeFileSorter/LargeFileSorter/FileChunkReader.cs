using System.Collections.Generic;
using System.IO;

namespace LargeFileSorter
{
    public class FileChunkReader : IFileChunkReader
    {
        public List<string> ReadFileChunk(long bytesOfMemoryToConsume, StreamReader stream)
        {
            List<string> result = new List<string>();
            long bytesCount = 0;
            string currentLine = null;
            while (bytesCount < bytesOfMemoryToConsume && (currentLine = stream.ReadLine()) != null)
            {
                result.Add(currentLine);
                bytesCount += currentLine.Length * sizeof(char);
            }

            return result;
        }
    }
}






