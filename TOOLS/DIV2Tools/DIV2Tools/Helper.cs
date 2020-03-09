using System;
using System.Collections.Generic;
using System.Text;

namespace DIV2Tools
{
    public static class Helper
    {
        public static void Log(string message, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine(message);
            }
        }

        public static string GetNullTerminatedASCIIString(byte[] buffer)
        {
            int len = 0;
            do { len++; } while (len < buffer.Length && buffer[len] != 0);
            return Encoding.ASCII.GetString(buffer, 0, len);
        }
    }
}
