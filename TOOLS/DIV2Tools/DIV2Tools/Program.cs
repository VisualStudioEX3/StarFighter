using System;
using System.Collections.Generic;

namespace DIV2Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            //new PAL("SPACE.PAL");
            ImportPCXTest("PLAYER.PCX");
            //CreatePALTest();
            //ComparePALTest();
            //CreateMAPTest();

            Console.Beep();
            Console.ReadKey();
        }

        static void ImportPCXTest(string filename)
        {
            new PCX(filename);
        }

        static void CreatePALTest()
        {
            var pal = PAL.CreateFromPCX("PLAYER.PCX");
            {
                pal.Write("TEST.PAL");
            }

            new PAL("TEST.PAL");
        }

        static void ComparePALTest()
        {
            //Console.WriteLine($"Compare palettes: {PAL.CreateFromPCX("PLAYER.PCX") == new PAL("TEST.PAL", false)}");
            //Console.WriteLine($"Compare palettes: {PAL.Compare(PAL.CreateFromPCX("PLAYER.PCX"), new PAL("TEST.PAL", false))}");
            var player = new PAL("PLAYER.PAL", false);
            var test = new PAL("TEST.PAL", false);

            PAL.Color a, b;
            int coincidences = 0;

            for (int i = 0; i < 256; i++)
            {
                a = player.Palette[i];
                b = test.Palette[i];

                if (a == b) coincidences++;

                Console.WriteLine($"{i:000}: {a.ToString()} : {b.ToString()} = {a == b}");
            }

            Console.WriteLine($"\nAre palettes equal: {PAL.Compare(player, test)} ({coincidences} coincidences of 256)");
        }

        static void CreateMAPTest()
        {
            // Create new MAP by original PNG file and using existent PAL file:
            var map = new MAP();
            {
                map.GraphId = 123;
                map.Description = "Test MAP file.";
                map.ImportPalette("SPACE.PAL");
                map.ImportPNG("PLAYER.PNG");
                map.ControlPoints.Add(128, 128);
                map.ControlPoints.Add(255, 255);
                map.ControlPoints.Add(64, 64);
                
                map.Write("TEST.MAP");
            }

            new MAP("TEST.MAP"); // Check new MAP created.
        }
    }
}
