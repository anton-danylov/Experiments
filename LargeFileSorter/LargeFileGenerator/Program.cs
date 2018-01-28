using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LargeFileGenerator
{
    public class Generator
    {
        static string[] _words = new[] { "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat", "Apple", "Cherry", "Banana", "Something" };

        private readonly int _reportStep = 5000000;

        [ThreadStatic]
        StringBuilder _builder = new StringBuilder();

        [ThreadStatic]
        Random _random = new Random(new Guid().GetHashCode());

        private string GenerateLine()
        {
            int number = _random.Next(-100000, 100000);
            string generatedString = GenerateString();

            return $"{number}. {generatedString}";
        }

        private string GenerateString()
        {
            int length = _random.Next(10) + 1;
            _builder.Clear();

            for (int i = 0; i < length; i++)
            {
                _builder.Append(_words[_random.Next(_words.Length)]).Append(' ');
            }

            return _builder.ToString();
        }

        private long GenerateFile(string filePath, long fileSizeInBytes, CancellationToken token, IProgress<long> progress)
        {
            long currentSize = 0;
            int reportCounter = 0;
            using (var stream = File.CreateText(filePath))
            {
                while (currentSize < fileSizeInBytes)
                {
                    var newLine = GenerateLine();
                    stream.WriteLine(newLine);

                    currentSize += (newLine.Length + Environment.NewLine.Length);

                    if (reportCounter % _reportStep == 0)
                    {
                        progress.Report(currentSize);
                    }

                    token.ThrowIfCancellationRequested();
                }
            }

            return currentSize;
        }

        public Task<long> GenerateFileAsync(string filePath, long fileSizeInBytes, CancellationToken token, IProgress<long> progress)
        {
            return Task.Run(() => { return GenerateFile(filePath, fileSizeInBytes, token, progress); }, token);
        }
    }

    class Program
    {
        async static void Generate(string targetPath, long targetSize)
        {
            Generator generator = new Generator();

            long reportedProgress = 0;
            var progress = new Progress<long>((currentSize) =>
                {
                    long currentProgress = currentSize * 100 / targetSize;

                    if (reportedProgress != currentProgress)
                    {
                        reportedProgress = currentProgress;
                        Console.Write($"{reportedProgress}% ");
                    }
                });

            CancellationTokenSource cts = new CancellationTokenSource();

            long actualSize = await generator.GenerateFileAsync(targetPath, targetSize, cts.Token, progress);

            Console.WriteLine();
            Console.WriteLine($"File created. Size: {actualSize}");
        }

        static void Main(string[] args)
        {
            string targetPath = null;
            long targetSize = -1;

            try
            {
                targetPath = args[0];
                targetSize = long.Parse(args[1]) * 1024 * 1024;
            }
            catch
            {
                Console.WriteLine("Large random text file generator");
                Console.WriteLine("Format of generated strings is \"{number}. {string}\"");
                Console.WriteLine("Usage: LargeFileGenerator.exe <path to generated file> <desired file size in megabytes>");

                WaitForAnyKey();
                return;
            }

            Generate(targetPath, targetSize);

            WaitForAnyKey();
        }

        private static void WaitForAnyKey()
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
