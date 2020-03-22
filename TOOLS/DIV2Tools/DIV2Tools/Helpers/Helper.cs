using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DIV2Tools.Helpers
{
    public static class Helper
    {
        #region Methods & Functions
        public static void Log(string message, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine(message);
            }
        } 
        #endregion
    }
}
