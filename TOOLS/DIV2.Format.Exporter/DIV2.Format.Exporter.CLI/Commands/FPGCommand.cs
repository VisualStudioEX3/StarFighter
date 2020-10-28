using DIV2.Format.Exporter.CLI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIV2.Format.Exporter.CLI.Commands
{
    class FPGCommand : ICommand
    {
        public void PrintHelp()
        {
            throw new NotImplementedException();
        }

        public int Run(params string[] args)
        {
            /* Arguments draft:
             * "-s" source folder path that contain PNG files and JSON MAP metadata files.
             * "-o" output FPG filename
             * "-p" PAL filename
             * */

            return 0;
        }
    }
}
