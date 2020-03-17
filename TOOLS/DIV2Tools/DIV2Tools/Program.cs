using System;
using System.Collections.Generic;

namespace DIV2Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            //for (int i = 0; i < 255; i++)
            //{
            //    Console.WriteLine($"{i} - {Helper.Map((byte)i, 0, 255, 0, 63)}"); 
            //}

            //new PAL("SPACE.PAL");

            //CreatePALTest();
            ComparePALTest();
            //CreateMAPTest();

            Console.Beep();
            Console.ReadKey();
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
            Console.WriteLine($"Compare palettes: {PAL.Compare(PAL.CreateFromPCX("PLAYER.PCX"), new PAL("TEST.PAL", false))}");
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
