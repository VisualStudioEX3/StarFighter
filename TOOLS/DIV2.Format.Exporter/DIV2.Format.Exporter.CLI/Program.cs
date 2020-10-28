using System;
using DIV2.Format.Exporter.CLI.Commands;

namespace DIV2.Format.Exporter.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            new VersionCommand().Run();
            Console.ReadKey();
        }
    }
}
