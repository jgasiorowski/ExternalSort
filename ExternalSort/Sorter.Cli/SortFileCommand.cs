using Spectre.Console.Cli;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Sorter.Cli
{
    internal class SortFileCommand : AsyncCommand<SortFileCommand.Settings>
    {
        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var inputFileInfo = new FileInfo(settings.InputPath);
            var inputPath = inputFileInfo.FullName;

            settings.OutputPath ??= inputFileInfo.FullName + "-sorted";
            settings.WorkDirectory ??= Path.Combine(inputFileInfo.Directory.FullName, "sort-work");

            PrepareWorkingDirectory(settings.WorkDirectory);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            await SplitSort(settings, inputFileInfo);
            MergeSort(settings);

            sw.Stop();
            Console.WriteLine($"Miliseconds taken: {sw.ElapsedMilliseconds}");

            if (settings.Compare)
            {
                CompareLines(inputFileInfo.FullName, settings.OutputPath);
            }

            return 0;
        }

        private static async Task SplitSort(Settings settings, FileInfo inputFileInfo)
        {
            var size = inputFileInfo.Length;
            var blockSize = size / settings.Threads;
            var tasks = new List<Task>();
            var begin = 0L;
            var end = blockSize;
            for (var i = 0; i < settings.Threads; i++)
            {
                var start = begin;
                var stop = Math.Min(end, size);
                tasks.Add(Task.Run(() => SplitSortFragment(settings, inputFileInfo.FullName, start, stop)));
                begin = end + 1;
                end = begin + blockSize;
            }

            await Task.WhenAll(tasks);
        }

        private static void PrepareWorkingDirectory(string workDirectory)
        {
            Directory.CreateDirectory(workDirectory);
            Array.ForEach(Directory.GetFiles(workDirectory), File.Delete);
        }

        private static void MergeSort(Settings settings)
        {
            var files = Directory.GetFiles(settings.WorkDirectory);
            var readers = files.Select(f => new StreamReader(f)).ToList();
            var finished = true;
            var outfile = new StreamWriter(settings.OutputPath);
            var sortingBuffer = new BinarySearchTree();
            do
            {
                finished = true;
                foreach (var reader in readers)
                {
                    var line = reader.ReadLine();
                    if (line is not null)
                    {
                        sortingBuffer.Insert(line);
                        finished = false;
                    }
                }

                foreach (var item  in sortingBuffer.Ordered)
                {
                    outfile.WriteLine(item.Data);
                }

                sortingBuffer = new BinarySearchTree();
            } while (!finished);

            outfile.Dispose();
        }

        private static async Task SplitSortFragment(Settings settings, string inputPath, long begin, long end)
        {
            var megaByte = 1024 * 1024;
            var fragmentSizeBytes = settings.FragmentSize * megaByte;
            var chunk = new BinarySearchTree();
            var chunkIndex = 0;
            long lastPositionIndexIncrease = 0;
            var tasks = new List<Task>();
            
            using var fileStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024);
            using var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 1024 * 1024);
            
            if (begin > 0)
            {
                streamReader.BaseStream.Seek(begin, SeekOrigin.Begin);
                var test = streamReader.ReadLine(); //discard
                lastPositionIndexIncrease = streamReader.BaseStream.Position;
            }

            do
            {
                chunk.Insert(streamReader.ReadLine());

                if (streamReader.BaseStream.Position - lastPositionIndexIncrease > fragmentSizeBytes || streamReader.BaseStream.Position >= end)
                {
                    lastPositionIndexIncrease = streamReader.BaseStream.Position;

                    var oldChunk = chunk;
                    chunk = new BinarySearchTree();
                    //StoreChunkQuue(r, ++chunkIndex, begin);
                    tasks.Add(Task.Run(() => StoreChunk(settings.WorkDirectory, oldChunk, ++chunkIndex, begin)));
                }

            } while (streamReader.BaseStream.Position < end);

            await Task.WhenAll(tasks);

        }

        static void StoreChunk(string workDirectory, BinarySearchTree structure, int index, long prefix)
        {
            using var streamWriter = new StreamWriter(Path.Combine(workDirectory, prefix.ToString() + "_" + index.ToString()));
            foreach (var item in structure.Ordered)
            {
                streamWriter.WriteLine(item.Data);
            }
        }

        private static void CompareLines(string inputPath, string outputPath)
        {
            var inputCounter = 0;
            using (var fileStream = new StreamReader(inputPath))
            {
                do
                {
                    fileStream.ReadLine();
                    inputCounter++;
                } while (!fileStream.EndOfStream);
            }

            var outputCounter = 0;
            using (var fileStream = new StreamReader(outputPath))
            {
                do
                {
                    fileStream.ReadLine();
                    outputCounter++;
                } while (!fileStream.EndOfStream);
            }

            Console.WriteLine($"{inputCounter}");
            Console.WriteLine($"{outputCounter}");
        }

        public sealed class Settings : CommandSettings
        {
            [Description("Full path to file which contents you want to sort")]
            [CommandArgument(0, "<input>")]
            public required string InputPath { get; init; }

            [Description("Full path for outpur file (with sorted data)")]
            [CommandOption("-o|--output")]
            public string? OutputPath { get; set; }

            [Description("Full path for outpur file (with sorted data)")]
            [CommandOption("-w|--work-dir")]
            public string? WorkDirectory { get; set; }

            [Description("(ALPHA) Number of threads used for process")]
            [CommandOption("-t|--threads")]
            [DefaultValue(1)]
            public int Threads { get; init; }

            [Description("Sets size used for generated sorted fragments (in MB)")]
            [CommandOption("-f|--fragment-size")]
            [DefaultValue(10)]
            public int FragmentSize { get; init; }

            [Description("Print number of lines in input and output file")]
            [CommandOption("-c|--compare")]
            [DefaultValue(false)]
            public bool Compare { get; init; }
        }
    }
}
