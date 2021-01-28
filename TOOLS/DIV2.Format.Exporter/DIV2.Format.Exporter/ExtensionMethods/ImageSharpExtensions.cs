using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Linq;
using System.Numerics;

namespace DIV2.Format.Exporter.ExtensionMethods
{
    /// <summary>
    /// Extension methods to interoperate between DIV2.Format.Exporter and ImageSharp formats.
    /// </summary>
    public static class ImageSharpExtensions
    {
        #region Methods & Functions
        internal static SixLabors.ImageSharp.Color[] ToImageSharpColors(this PAL instance)
        {
            return ToImageSharpColors(instance.ToRGB());
        }

        internal static SixLabors.ImageSharp.Color[] ToImageSharpColors(this Color[] palette)
        {
            return palette.Select(e => SixLabors.ImageSharp.Color.FromRgb(e.red, e.green, e.blue)).ToArray();
        }

        internal static SixLabors.ImageSharp.Color[] ToImageSharpColors(this byte[] palette)
        {
            return ToImageSharpColors(palette.ToColorArray());
        }

        internal static void ComposeBitmap(this Image<Rgb24> instance, byte[] pixels, SixLabors.ImageSharp.Color[] palette)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                int y = i / instance.Width;
                int x = i - (instance.Width * y);
                instance[x, y] = palette[pixels[i]];
            }
        }

        internal static Vector3 ToHSV(this Color color, ColorFormat colorType)
        {
            float max = (float)colorType;
            var rgb = new Rgb(color.red / max, color.green / max, color.blue / max);
            Hsv hsv = new ColorSpaceConverter().ToHsv(rgb);

            return new Vector3(hsv.H, hsv.S, hsv.V);
        }

        internal static Vector3 ToHSL(this Color color, ColorFormat colorType)
        {
            float max = (float)colorType;
            var rgb = new Rgb(color.red / max, color.green / max, color.blue / max);
            Hsl hsl = new ColorSpaceConverter().ToHsl(rgb);

            return new Vector3(hsl.H, hsl.S, hsl.L);
        }
        #endregion
    }
}
