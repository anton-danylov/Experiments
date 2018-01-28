using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LargeFileSorter
{
    public class LargeFileSorter
    {
        ISplitChunksStrategy _splitter;
        IMergeChunksStrategy _merger;

        public LargeFileSorter(ISplitChunksStrategy splitter, IMergeChunksStrategy merger)
        {
            _splitter = splitter;
            _merger = merger;
        }

        public void Sort(string sourceFilePath, string destFilePath, long memoryLimitOverride)
        {
            string[] chunkFilePaths;
            using (new Benchmark("Split started...", "Split finished in"))
            {
                chunkFilePaths = _splitter.SplitFileToSortedChunks(sourceFilePath, memoryLimitOverride, Utilities.GetTempFilePath);
            }

            using (new Benchmark("Merge started...", "Merge finished in"))
            {
                _merger.MergeChunks(destFilePath, chunkFilePaths);
            }
        }

    }

    class CommandLineProcessor
    {
        public static CommandLineArgs ProcessCommandLineArgs(string[] args)
        {
            Dictionary<string, string> arguments = ParseCmdLineArgs(args);

            CommandLineArgs result = null;
            if (arguments != null)
            {
                result = new CommandLineArgs(arguments);
            }

            return result;
        }

        public static void ShowHelp()
        {
            Console.WriteLine("Large text file line sorter");
            Console.WriteLine("Usage: LargeFileSorter.exe <path to source file> <path to destination file>");
            Console.WriteLine();
            Console.WriteLine("Optional parameters:");
            Console.WriteLine("/comparer:<comparer>.dll -- Custom assembly for comparing strings");
            Console.WriteLine("/memLimit:<number in megabytes> -- Amount of memory allowed to use by app. If not set then default value is calculated");
            Console.WriteLine("/splitStrategy:<SyncSplitStrategy or ParallelSplitStrategy> -- sort chunks in multiple threads or in one");
            Console.WriteLine("/mergeStrategy:<PriorityQueueMergeStrategy or SortedDictionaryMergeStrategy> -- k-Way merge is based on Sorted Dictionary or C5.PriorityQueue");
            Console.WriteLine("/compareWith:<path to another file> -- compare current sorted file with another for testing purposes");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine(@" LargeFileSorter.exe d:\large_file.txt "
                + @"d:\large_file_sorted.txt " 
                +"/comparer:\"UnsafeStringComparer.dll\" " 
                +"/memLimit:80 " 
                +"/splitStrategy:SyncSplitStrategy " 
                +"/mergeStrategy:SortedDictionaryMergeStrategy " 
                +"/compareWith:\"d:\\large_file_sorted0.txt\" ");
        }

        private static string Prepare(string s) => s?.ToUpperInvariant();

        private static Dictionary<string, string> ParseCmdLineArgs(string[] args)
        {
            if (args.Length < 2)
                return null;

            Dictionary<string, string> arguments = new Dictionary<string, string>();

            arguments[Prepare("SourceFilePath")] = args[0];
            arguments[Prepare("DestFilePath")] = args[1];

            Regex regex = new Regex(@"/([^\:]+)\:(.+)");


            for (int i = 2; i < args.Length; i++)
            {
                Match match = regex.Match(args[i]);

                if (!match.Success)
                    return null;

                arguments[Prepare(match.Groups[1].Value)] = match.Groups[2].Value;
            }


            return arguments;
        }

        public class CommandLineArgs
        {
            public string SourceFilePath { get; set; }
            public string DestFilePath { get; set; }
            public string Comparer { get; set; }

            /// <summary>
            /// Memory limit in megabytes. If set to less or equal to 0 value then effective limit will be 
            /// calculated by each split strategy depending on total and available RAM 
            /// </summary>
            public long MemLimit { get; set; }

            public string SplitStrategy { get; set; }
            public string MergeStrategy { get; set; }
            public string CompareWith { get; set; }

            private static string Get(Dictionary<string, string> arguments, string key) => arguments.TryGetValue(Prepare(key), out string val) ? val : null;

            public CommandLineArgs(Dictionary<string, string> args)
            {
                SourceFilePath = Get(args, nameof(SourceFilePath));
                DestFilePath = Get(args, nameof(DestFilePath));
                Comparer = Get(args, nameof(Comparer));
                SplitStrategy = Get(args, nameof(SplitStrategy));
                MergeStrategy = Get(args, nameof(MergeStrategy));
                CompareWith = Get(args, nameof(CompareWith));

                if (int.TryParse(Get(args, nameof(MemLimit)), out int memLimitInMegabytes))
                {
                    MemLimit = memLimitInMegabytes * 1024 * 1024;
                }
                else
                {
                    MemLimit = -1;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var cmdLineArgs = CommandLineProcessor.ProcessCommandLineArgs(args);
            if (cmdLineArgs == null)
            {
                CommandLineProcessor.ShowHelp();
                WaitForAnyKey();
                return;
            }

            var sourceFilePath = cmdLineArgs.SourceFilePath;
            var destFilePath = cmdLineArgs.DestFilePath;
            var comparerAssemblyRelativePath = cmdLineArgs.Comparer;
            long bytesOfMemoryToUse = cmdLineArgs.MemLimit;


            IContainer container = BuildDependencies(comparerAssemblyRelativePath
                , cmdLineArgs.SplitStrategy
                , cmdLineArgs.MergeStrategy);

            using (new Benchmark(null, "Total"))
            {
                container
                    .Resolve<LargeFileSorter>()
                    .Sort(sourceFilePath, destFilePath, bytesOfMemoryToUse);
            }

            if (!String.IsNullOrEmpty(cmdLineArgs.CompareWith))
            {
                bool equal = Utilities.CheckFilesAreEqual(destFilePath, cmdLineArgs.CompareWith);
                Console.WriteLine($"Files {destFilePath} and {cmdLineArgs.CompareWith} are {(equal ? "EQUAL" : "NOT EQUAL")}");
            }

            WaitForAnyKey();
        }

        private static void WaitForAnyKey()
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static IComparer<string> GetComparer(string comparerRelativeAssemblyPath)
        {
            var comparerAssemblyPath =
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), comparerRelativeAssemblyPath);

            var comparerTypes = Assembly.LoadFile(comparerAssemblyPath)
                .GetTypes();

            var comparerType = comparerTypes.FirstOrDefault(t => typeof(IComparer<string>).IsAssignableFrom(t));

            var comparer = (IComparer<string>)Activator.CreateInstance(comparerType);
            return comparer;
        }

        private static IContainer BuildDependencies(string comparerRelativeAssemblyPath
            , string splitStrategy = null
            , string mergeStrategy = null)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ChunkDataSorter>().As<IChunkDataSorter>();
            builder.RegisterType<FileChunkReader>().As<IFileChunkReader>();

            if (splitStrategy == nameof(SyncSplitStrategy))
            {
                builder.RegisterType<SyncSplitStrategy>().As<ISplitChunksStrategy>();
            }
            else
            {
                builder.RegisterType<ParallelSplitStrategy>().As<ISplitChunksStrategy>();
            }

            if (mergeStrategy == nameof(SortedDictionaryMergeStrategy))
            {
                builder.RegisterType<SortedDictionaryMergeStrategy>().As<IMergeChunksStrategy>();
            }
            else
            {
                builder.RegisterType<PriorityQueueMergeStrategy>().As<IMergeChunksStrategy>();
            }


            IComparer<string> comparerInstance =
                String.IsNullOrEmpty(comparerRelativeAssemblyPath)
                ? Comparer<string>.Default
                : GetComparer(comparerRelativeAssemblyPath);

            builder.RegisterInstance(comparerInstance).As<IComparer<string>>();


            builder.RegisterType<LargeFileSorter>();

            return builder.Build();
        }
    }
}






