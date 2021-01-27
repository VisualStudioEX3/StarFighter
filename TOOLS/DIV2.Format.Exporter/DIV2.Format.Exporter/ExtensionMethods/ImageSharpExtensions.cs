using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Linq;

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

        internal static Tuple<float, float, float> ToHSV(this Color color, bool fromDAC = true)
        {
            float max = fromDAC ? Color.MAX_DAC_VALUE : byte.MaxValue;
            var rgb = new Rgb(color.red / max, color.green / max, color.blue / max);
            Hsv hsv = new ColorSpaceConverter().ToHsv(rgb);

            return new Tuple<float, float, float>(hsv.H, hsv.S, hsv.V);
        }

        internal static Tuple<float, float, float> ToHSL(this Color color, bool fromDAC = true)
        {
            float max = fromDAC ? Color.MAX_DAC_VALUE : byte.MaxValue;
            var rgb = new Rgb(color.red / max, color.green / max, color.blue / max);
            Hsl hsl = new ColorSpaceConverter().ToHsl(rgb);

            return new Tuple<float, float, float>(hsl.H, hsl.S, hsl.L);
        }
        #endregion
    }
}
