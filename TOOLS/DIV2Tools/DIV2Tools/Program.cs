using System;
using System.Collections.Generic;

namespace DIV2Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create new MAP by original PNG file and using existent PAL file:
            var map = new MAP();
            {
                map.GraphId = 123;
                map.Description = "Test MAP file.";
                map.ImportPNG("PLAYER.PNG", "SPACE.PAL");
                map.ControlPoints.Add(128, 128);
                map.ControlPoints.Add(255, 255);
                map.ControlPoints.Add(64, 64);
                map.Write("TEST.MAP");
            }

            new MAP("TEST.MAP"); // Check new MAP created.

            Console.Beep();
            Console.ReadKey();
        }
    }
}
