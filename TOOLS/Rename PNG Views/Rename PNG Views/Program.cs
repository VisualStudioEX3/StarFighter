using System;
using System.IO;

namespace DIV2GJAM_Rename_Views_BMPs
{
    class Program
    {
        static void Main(string[] args)
        {
            char index = Path.GetFileName(Directory.GetCurrentDirectory())[0];

            for (int i = 0; i < 5; i++)
            {
                Delete($"{i}");
                Rename($"{i}", $"{index}0{i}");
            }

            for (int i = 60, j = 14; i < 64; i++, j--)
            {
                Delete($"{i}");
                Rename($"{i}", $"{index}{j}");
            }
        }

        static void Rename(string oldFilename, string newFilename)
        {
            File.Move($"{oldFilename}.png", $"{newFilename}.png");
        }

        static void Delete(string filename)
        {
            var filenameExt = $"{filename}.png.meta";
            if (File.Exists(filenameExt))
            {
                File.Delete(filenameExt);
            }
        }
    }
}