using DIV2.Format.Exporter.Processors.Images;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;

namespace DIV2.Format.Exporter.MethodExtensions
{
    public static class ImageSharpExtensions
    {
        #region Constants
        static readonly Dictionary<SupportedMimeTypes, string> SUPPORTED_MIME_TYPES = new Dictionary<SupportedMimeTypes, string>()
        {
            { SupportedMimeTypes.PCX, PcxFormat.Instance.DefaultMimeType },
            { SupportedMimeTypes.MAP, MapFormat.Instance.DefaultMimeType },
            { SupportedMimeTypes.BMP, BmpFormat.Instance.DefaultMimeType },
            { SupportedMimeTypes.PNG, PngFormat.Instance.DefaultMimeType }
        };
        #endregion

        #region Enums
        internal enum SupportedMimeTypes
        {
            PCX,
            MAP,
            BMP,
            PNG
        }
        #endregion

        #region Methods & Functions
        internal static SixLabors.ImageSharp.Color[] ToImageSharpColors(this PAL instance)
        {
            return ToImageSharpColors(instance.ToRGB());
        }

        internal static SixLabors.ImageSharp.Color[] ToImageSharpColors(this Color[] palette)
        {
            var colors = new SixLabors.ImageSharp.Color[PAL.LENGTH];

            for (int i = 0; i < PAL.LENGTH; i++)
                colors[i] = SixLabors.ImageSharp.Color.FromRgb(palette[i].red, palette[i].green, palette[i].blue);

            return colors;
        }

        internal static SixLabors.ImageSharp.Color[] ToImageSharpColors(this byte[] palette)
        {
            var colors = new SixLabors.ImageSharp.Color[PAL.LENGTH];

            for (int i = 0, j = 0; i < ColorPalette.LENGTH; i++)
                colors[i] = SixLabors.ImageSharp.Color.FromRgb(palette[j++], palette[j++], palette[j++]);

            return colors;
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

        internal static bool IsSupportedFormat(this Image instance, SupportedMimeTypes format, IImageFormat mime)
        {
            return SUPPORTED_MIME_TYPES[format] == mime.DefaultMimeType && instance.PixelType.BitsPerPixel == (int)BmpBitsPerPixel.Pixel8;
        } 
        #endregion
    }
}
