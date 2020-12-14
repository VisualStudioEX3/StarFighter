using System;
using System.Reflection;
using DIV2.Format.Exporter.CLI.Interfaces;

namespace DIV2.Format.Exporter.CLI.Commands
{
    class VersionCommand : ICommand
    {
        #region Methods & Functions
        void PrintVersionData<T>()
        {
            Assembly a = typeof(T).Assembly;

            var p = a.GetCustomAttribute<AssemblyProductAttribute>();
            var v = a.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var c = a.GetCustomAttribute<AssemblyCopyrightAttribute>();

            string name = p.Product;
            string version = v.Version;
            string copyright = c.Copyright.Replace("©", "(C)"); // '©' character isn't shown correctly.

            Console.WriteLine($"{name} v{version}, {copyright}");
        }

        public void PrintHelp()
        {
            Console.WriteLine("Shows the version information of the library converter and command line interface tools.");
        }

        public int Run(params string[] args)
        {
            this.PrintVersionData<PAL>();
            this.PrintVersionData<VersionCommand>();

            return 0;
        }
        #endregion
    }
}
