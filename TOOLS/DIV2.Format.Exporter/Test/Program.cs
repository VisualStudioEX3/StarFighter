using System;
using System.IO;
using System.Threading.Tasks;
using DIV2.Format.Exporter;

class Program
{
    static void Main(string[] args)
    {
        //CreateMAPTest();
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
                fpg.AddMap(png, getId(png), $"Test FPG: {getId(png)}");
                Console.WriteLine($"- Added \"{png}\" definition...");
            }

            Console.Write("Writing FPG data to file, wait");

            var progress = new AsyncOperationProgress();
            progress.onProgress += (p) => Console.Write('.');
            Task create = fpg.SaveAsync(output, progress);

            while (!create.IsCompleted) { }

            Console.WriteLine($"\n\"{output}\" created!\n");
        }
    }
}
