using DIV2.Format.Exporter.Processors;
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
        const int BPP_8 = 8;
        const int PALETTE_RGB_LENGTH = PAL.COLOR_TABLE_LENGTH / 3;
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
        internal static Color[] ToColors(this PAL instance)
        {
            var rgb = instance.DAC2RGB();
            var colors = new Color[ImageSharpExtensions.PALETTE_RGB_LENGTH];
            int index = -1;

            for (int i = 0; i < ImageSharpExtensions.PALETTE_RGB_LENGTH; i++)
            {
                colors[i] = Color.FromRgb(rgb[++index], rgb[++index], rgb[++index]);
            }

            return colors;
        }

        internal static void ComposeBitmap(this Image<Rgb24> instance, byte[] pixels, Color[] palette)
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
            return ImageSharpExtensions.SUPPORTED_MIME_TYPES[format] == mime.DefaultMimeType && instance.PixelType.BitsPerPixel == ImageSharpExtensions.BPP_8;
        } 
        #endregion
    }
}
