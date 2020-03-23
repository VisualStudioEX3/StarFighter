using System;
using System.Collections;
using System.Collections.Generic;
using DIV2Tools.DIVFormats;
using DIV2Tools.Helpers;
using DIV2Tools.MethodExtensions;

namespace DIV2Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            //new PAL("SPACE.PAL");
            //TestBitOperations();
            //ImportPCXTest("PLAYER.PCX");
            //CreatePALTest();
            //ComparePALTest();
            //CreateMAPTest();
            ComparePALs();

            Console.Beep();
            Console.ReadKey();
        }

        static void TestBitOperations()
        {
            // Checks if bits 6 and 7 are set:
            var check = new Func<byte, bool>((value) => (value & 0xC0) == 0xC0);

            // Clear bits 6 and 7:
            var clear = new Func<byte, byte>((value) =>
            {
                int i = value;
                return (byte)(i & 0x3F);
            });

            Console.WriteLine(check(255)); // Expected true.
            Console.WriteLine(clear(255)); // Expetect 63.
        }

        static void ImportPCXTest(string filename)
        {
            new PCX(filename);
        }

        static void CreatePALTest()
        {
            var pal = new PAL(new PCX("PLAYER.PCX"));
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
                a = player.Colors[i];
                b = test.Colors[i];

                if (a == b) coincidences++;

                Console.WriteLine($"{i:000}: {a.ToString()} : {b.ToString()} = {a == b}");
            }

            Console.WriteLine($"\nAre palettes equal: {PAL.Compare(player, test)} ({coincidences} coincidences of 256)");
        }

        static void CreateMAPTest()
        {
            // Create new MAP using the a PNG file:
            var map = new MAP();
            {
                map.GraphId = 123;
                map.Description = "Test MAP file.";
                map.ImportPNG("PLAYER.PNG");
                map.ControlPoints.Add(128, 128);
                map.ControlPoints.Add(255, 255);
                map.ControlPoints.Add(64, 64);
                
                map.Write("TEST.MAP");
            }

            new MAP("TEST.MAP"); // Check new MAP created.
        }

        static void ComparePALs()
        {
            var div = new PAL("SPACE.PAL", false);
            var pcx = new PAL(new PCX("PLAYER.PCX", false));

            for (int i = 0; i < 256; i++)
            {
                int index = div.FindColor(pcx.Colors[i], false);
                Console.WriteLine($"#{i:000}: {pcx.Colors[i].ToString()} : {div.Colors[index]} : {pcx.Colors[i] == div.Colors[index]}");
            }
        }
    }
}
