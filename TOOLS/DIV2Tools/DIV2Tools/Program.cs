using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DIV2Tools.DIVFormats;
using DIV2Tools.Helpers;
using DIV2Tools.MethodExtensions;

namespace DIV2Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestBitOperations();
            //TestBinaryReaderEOF();

            //LoadPALTest();
            //CreatePALTest();
            //ComparePALTest();
            //ComparePALsTest();

            //ImportPCXTest();
            //CreateMAPTest();

            //LoadFPGTest();
            CreateFPGTest();

            Console.Beep();
            Console.ReadKey();
        }

        static string GetAssetFilename(string filename)
        {
            return @$"Assets\{filename}";
        }

        static string GetOutputFilename(string filename)
        {
            const string OUTPUT_DIRECTORY = "Output";
            
            if (!Directory.Exists(OUTPUT_DIRECTORY))
            {
                Directory.CreateDirectory(OUTPUT_DIRECTORY);
            }

            string newFilename = @$"{OUTPUT_DIRECTORY}\{filename}";

            if (File.Exists(newFilename))
            {
                File.Delete(newFilename);
            }

            return newFilename;
        }

        static void TestBinaryReaderEOF()
        {
            string asset = GetAssetFilename("SPACE.PAL");
            var file = new BinaryReader(File.OpenRead(asset));
            file.AdvanceReadPosition(file.GetLength() + 3);
            Console.WriteLine($"Length: {file.GetLength()}, Position: {file.GetCurrentPosition()}, EOF: {file.EOF()}");
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
            Console.WriteLine(clear(255)); // Expected 63.
        }

        static void ImportPCXTest()
        {
            string asset = GetAssetFilename("SPACE.PAL");
            new PCX(GetAssetFilename("PLAYER.PCX"));
        }

        static void LoadPALTest()
        {
            string asset = GetAssetFilename("SPACE.PAL");
            new PAL(GetAssetFilename("SPACE.PAL"));
        }

        static void CreatePALTest()
        {
            string asset = GetAssetFilename("PLAYER.PCX");
            string output = GetOutputFilename("TEST.PAL");
            var pal = new PAL(new PCX(asset, false));
            {
                pal.Write(output);
            }

            new PAL(output);
        }

        static void ComparePALTest()
        {
            string[] asset = { GetAssetFilename("PLAYER.PAL"), GetAssetFilename("TEST.PAL") };

            if (!File.Exists(asset[1]))
            {
                CreatePALTest();
            }

            var player = new PAL(asset[0], false);
            var test = new PAL(asset[1], false);

            //Console.WriteLine($"Compare palettes: {player == test}");

            // To print the entire comparasion:
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
            string asset = GetAssetFilename("PLAYER.PNG");
            string output = GetOutputFilename("TEST.MAP");

            // Create new MAP from a PNG file:
            var map = new MAP();
            {
                map.ImportPNG(asset);
                map.GraphId = 123;
                map.Description = "Test MAP file.";
                map.ControlPoints.Add(128, 128);
                map.ControlPoints.Add(255, 255);
                map.ControlPoints.Add(64, 64);
                
                map.Write(output);
            }

            new MAP(output); // Check new MAP created.
        }

        static void ComparePALsTest()
        {
            string[] asset = { GetAssetFilename("SPACE.PAL"), GetAssetFilename("PLAYER.PCX") };

            var div = new PAL(asset[0], false);
            var pcx = new PAL(new PCX(asset[1]));

            for (int i = 0; i < 256; i++)
            {
                int index = div.FindColor(pcx.Colors[i], false);
                Console.WriteLine($"#{i:000}: {pcx.Colors[i].ToString()} : {div.Colors[index]} : {pcx.Colors[i] == div.Colors[index]}");
            }
        }

        static void LoadFPGTest()
        {
            string asset = GetAssetFilename("PLAYER.FPG");
            new FPG(asset);
        }

        static void CreateFPGTest()
        {
            string[] asset = Directory.GetFiles(@"Assets\ENEMY\");
            string output = GetOutputFilename("TEST.FPG");

            var getId = new Func<string, int>((filename) => int.Parse(Path.GetFileNameWithoutExtension(filename)));

            Console.WriteLine($"Create {output}:");
            var fpg = new FPG();
            {
                for (int i = 0; i < asset.Length; i++)
                {
                    if (i == 0)
                    {
                        PAL palette;
                        fpg.Maps.Add(asset[i], getId(asset[i]), $"FPG test: {asset[i]}", asset[i].GetFilenameFromPath(), out palette);
                        fpg.ImportPalette(palette);
                    }
                    else
                    {
                        fpg.Maps.Add(asset[i], getId(asset[i]), $"FPG test: {asset[i]}", asset[i].GetFilenameFromPath());
                    }
                    Console.WriteLine($"- Added {asset[i]}...");
                }

                fpg.Write(output);

                Console.WriteLine($"{output} created!\n");
            }

            new FPG(output);
        }
    }
}
