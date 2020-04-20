using System;
using System.IO;
using DIV2.Format.Exporter;

class Program
{
    static void Main(string[] args)
    {
        //CreateMAPTest();
        //CreateFPGFromFileTest();
        CreateFPGFromMemoryTest();

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
        string asset2 = GetAssetFilename("SPACE.PAL");
        string output = GetOutputFilename("TEST.MAP");

        Console.WriteLine($"Create \"{output}\":");
        var map = new MAP(asset, asset2, 123, "Test MAP file.");
        {
            map.AddControlPoint(128, 128);
            map.AddControlPoint(255, 255);
            map.AddControlPoint(64, 64);

            map.Save(output, asset2);

            Console.WriteLine($"\"{output}\" created!\n");
        }
    }

    static void CreateFPGFromFileTest()
    {
        string[] asset = Directory.GetFiles(@"Assets\ENEMY\");
        string asset2 = GetAssetFilename("SPACE.PAL");
        string output = GetOutputFilename("TEST.FPG");

        var getId = new Func<string, int>((filename) => int.Parse(Path.GetFileNameWithoutExtension(filename)));

        Console.WriteLine($"Create \"{output}\":");
        var fpg = new FPG();
        {
            foreach (var png in asset)
            {
                fpg.AddMap(png, getId(png), $"Test FPG: {getId(png)}");
                Console.WriteLine($"- Added \"{png}\" definition...");
            }

            Console.WriteLine("Writing FPG data to file, wait...");
            fpg.Save(output, asset2);
            Console.WriteLine($"\"{output}\" created!\n");
        }
    }

    static void CreateFPGFromMemoryTest()
    {
        string[] asset = Directory.GetFiles(@"Assets\ENEMY\");
        string asset2 = GetAssetFilename("SPACE.PAL");
        string output = GetOutputFilename("TEST.FPG");

        var getId = new Func<string, int>((filename) => int.Parse(Path.GetFileNameWithoutExtension(filename)));

        Console.WriteLine($"Create \"{output}\":");
        var fpg = new FPG();
        {
            foreach (var png in asset)
            {
                fpg.AddMap(File.ReadAllBytes(png), getId(png), $"Test FPG: {getId(png)}");
                Console.WriteLine($"- Added \"{png}\" definition (from memory)...");
            }

            Console.WriteLine("Writing FPG data to file, wait...");
            fpg.Save(output, asset2);
            Console.WriteLine($"\"{output}\" created!\n");
        }
    }
}
