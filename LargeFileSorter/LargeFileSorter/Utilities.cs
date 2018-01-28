using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeFileSorter
{
    public static class Utilities
    {
        public static string GetTempFileName(string sortedFilePath, int chunk)
        {
            return $"{Path.GetFileNameWithoutExtension(sortedFilePath)}.{chunk}";
        }

        public static string GetTempFilePath(string sortedFilePath, int chunk)
        {
            return Path.Combine(Path.GetDirectoryName(sortedFilePath), GetTempFileName(sortedFilePath, chunk));
        }

        public static void ForEach<T> (this IEnumerable<T> items, Action<T> action)
        {
            foreach(T item in items)
            {
                action(item);
            }
        }


        public static bool CheckFilesAreEqual(string file1, string file2)
        {
            using (var reader1 = new StreamReader(file1))
            using (var reader2 = new StreamReader(file2))
            {
                while (!reader1.EndOfStream && !reader2.EndOfStream)
                {
                    if (reader1.ReadLine() != reader2.ReadLine())
                        return false;
                }
            }

            return true;
        }
    }


    public class Benchmark : IDisposable
    {
        private readonly string _messageStart;
        private readonly string _messageEnd;
        private readonly Stopwatch _stopWatch;

        public Benchmark(string messageStart, string messageEnd)
        {
            _messageStart = messageStart;
            _messageEnd = messageEnd;
            _stopWatch = new Stopwatch();

            if (messageStart != null)
            {
                Console.WriteLine(_messageStart);
            }

            _stopWatch.Start();
        }

        public void Dispose()
        {
            _stopWatch.Stop();
            Console.WriteLine("{0}: {1}", _messageEnd ?? "Elapsed", _stopWatch.Elapsed);
        }
    }
}
