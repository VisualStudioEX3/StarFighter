using System;
using System.IO;
using DIV2.Format.Exporter;

namespace DIV2Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateMAPTest();
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

        static void CreateMAPTest()
        {
            string asset = GetAssetFilename("PLAYER.PNG");
            string output = GetOutputFilename("TEST.MAP");

            Console.WriteLine($"Create \"{output}\":");
            var map = new MAP(asset, 123, "Test MAP file.");
            {
                map.AddControlPoint(128, 128);
                map.AddControlPoint(255, 255);
                map.AddControlPoint(64, 64);
                
                map.Save(output);

                Console.WriteLine($"\"{output}\" created!\n");
            }
        }

        static void CreateFPGTest()
        {
            string[] asset = Directory.GetFiles(@"Assets\ENEMY\");
            string output = GetOutputFilename("TEST.FPG");

            var getId = new Func<string, int>((filename) => int.Parse(Path.GetFileNameWithoutExtension(filename)));

            Console.WriteLine($"Create \"{output}\":");
            var fpg = new FPG();
            {
                foreach (var png in asset)
                {
                    fpg.AddMap(new FPG.PNGImportDefinition()
                    {
                        filename = png,
                        graphId = getId(png),
                        description = $"Test FPG: {getId(png)}"
                    });
                    Console.WriteLine($"- Added \"{png}\" definition...");
                }

                Console.WriteLine("Writing FPG data, wait...");

                fpg.Save(output);

                Console.WriteLine($"\"{output}\" created!\n");
            }
        }
    }
}
