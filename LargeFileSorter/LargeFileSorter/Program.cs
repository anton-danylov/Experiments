using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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

            if (mergeStrategy == nameof(PriorityQueueMergeStrategy))
            {
                builder.RegisterType<PriorityQueueMergeStrategy>().As<IMergeChunksStrategy>();
            }
            else
            {
                builder.RegisterType<SortedDictionaryMergeStrategy>().As<IMergeChunksStrategy>();
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






