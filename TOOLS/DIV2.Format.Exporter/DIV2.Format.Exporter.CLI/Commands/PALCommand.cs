using DIV2.Format.Exporter.CLI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIV2.Format.Exporter.CLI.Commands
{
    class PALCommand : ICommand
    {
        public void PrintHelp()
        {
            throw new NotImplementedException();
        }

        public int Run(params string[] args)
        {
            /* Arguments draft:
             * "-s" source JSON file that contain the RGB color table (internally convert to DAC values).
             * "-o" output PAL filename
             * */

            return 0;
        }
    }
}
