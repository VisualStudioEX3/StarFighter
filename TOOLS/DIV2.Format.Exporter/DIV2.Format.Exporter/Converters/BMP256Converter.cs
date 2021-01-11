using DIV2.Format.Exporter.ExtensionMethods;
using DIV2.Format.Exporter.Processors.Images;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System.IO;

namespace DIV2.Format.Exporter.Converters
{
    /// <summary>
    /// Base converter for all supported image formats to BMP 256 colors.
    /// </summary>
    /// <remarks>This class is the core of the image conversions to DIV Games Studio formats like <see cref="PAL"/> and <see cref="MAP"/> files.</remarks>
    class BMP256Converter
    {
        #region Constants
        const int BMP_HEADER_LENGTH = 54;
        const int BMP_HEADER_PALETTE_COLORS_OFFSET = 46;
        const int RGB_PALETTE_LENGTH = 768;
        #endregion

        #region Internal vars
        static int _currentPaletteHash = 0;
        static BmpEncoder _encoder;
        #endregion

        readonly static BmpEncoder DefaultEncoder = new BmpEncoder
        {
            BitsPerPixel = BmpBitsPerPixel.Pixel8
        };

        #region Methods & Functions
        static void SetupPalette(byte[] palette)
        {
            if (palette.GetHashCode() != _currentPaletteHash)
            {
                _encoder = new BmpEncoder
                {
                    BitsPerPixel = BmpBitsPerPixel.Pixel8,
                    Quantizer = new PaletteQuantizer(palette.ToImageSharpColors(), new QuantizerOptions { Dither = null })
                };

                _currentPaletteHash = palette.GetHashCode();
            }
        }

        /// <summary>
        /// Converts supported image format to a 256 colors BMP.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array that contais the image data to convert.</param>
        /// <param name="palette">The palette extracted from the converted image.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bitmap">The bitmap data of the converted image referencing the palette values.</param>
        public static void Convert(byte[] buffer, out byte[] palette, out short width, out short height, out byte[] bitmap)
        {
            Process(buffer, DefaultEncoder, out palette, out width, out height, out bitmap);
        }

        /// <summary>
        /// Converts supported image format to a 256 colors BMP using an existing color palette.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array that contais the image data to convert.</param>
        /// <param name="palette">The palette that contains the colors that must be used in conversion process.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bitmap">The bitmap data of the converted image referencing the palette values.</param>
        public static void ConvertTo(byte[] buffer, byte[] palette, out short width, out short height, out byte[] bitmap)
        {
            SetupPalette(palette);
            Process(buffer, _encoder, out _, out width, out height, out bitmap);
        }

        static void Process(byte[] buffer, BmpEncoder encoder, out byte[] palette, out short width, out short height, out byte[] bitmap)
        {
            using (Image image = ImageProcessor.ProcessImage(buffer, out IImageFormat mime))
            {
                width = (short)image.Width;
                height = (short)image.Height;

                using (var stream = new MemoryStream())
                {
                    if (mime.DefaultMimeType != BmpFormat.Instance.DefaultMimeType)
                        image.Mutate(x => x.Flip(FlipMode.Vertical)); // Resolves the BMP inverse pixel order.

                    image.Save(stream, encoder);

                    using (var reader = new BinaryReader(stream))
                    {
                        reader.BaseStream.Position = BMP_HEADER_PALETTE_COLORS_OFFSET;
                        int colors = reader.ReadInt32(); // 0 means the default 256 colors.
                        int paletteLength = (colors != 0 ? colors : RGB_PALETTE_LENGTH) * 4;

                        if (encoder == DefaultEncoder)
                        {
                            reader.BaseStream.Position = BMP_HEADER_LENGTH;
                            byte[] colorTable = reader.ReadBytes(paletteLength);
                            palette = ExtractPalette(colorTable);
                        }
                        else
                        {
                            palette = null;
                            reader.BaseStream.Position = BMP_HEADER_LENGTH + paletteLength;
                        }

                        bitmap = reader.ReadBytes(width * height);
                    }
                }
            }
        }

        static byte[] ExtractPalette(byte[] buffer)
        {
            var palette = new byte[RGB_PALETTE_LENGTH];

            for (int i = 0, j = 0; i < palette.Length; i += 4)
            {
                // BMP file stored the color table values in this order: blue, green, red, 0x00
                palette[j++] = buffer[i + 2];
                palette[j++] = buffer[i + 1];
                palette[j++] = buffer[i];
            }

            return palette;
        }
        #endregion
    }
}
