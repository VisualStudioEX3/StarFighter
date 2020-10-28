using DIV2.Format.Exporter.CLI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIV2.Format.Exporter.CLI.Commands
{
    class MAPCommand : ICommand
    {
        public void PrintHelp()
        {
            throw new NotImplementedException();
        }

        public int Run(params string[] args)
        {
            /* Arguments draft:
             * "-s" source PNG filename
             * "-o" output MAP filename
             * "-p" PAL filename
             * "-i" graphId
             * "-d" description
             * "-m" JSON file with MAP metadata (this ignore values from -i or -d arguments).
             * */

            return 0;
        }
    }
}
