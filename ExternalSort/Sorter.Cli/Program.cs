using Spectre.Console.Cli;

namespace Sorter.Cli
{
    internal class Program
    {
        static Task<int> Main(string[] args)
        {
            var app = new CommandApp<SortFileCommand>();
            return app.RunAsync(args);
        }
    }
}
