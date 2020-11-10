using DIV2.Format.Exporter.MethodExtensions;
using DIV2.Format.Exporter.Processors.Images;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.IO;

namespace DIV2.Format.Exporter.Converters
{
    class BMP256Converter
    {
        #region Constants
        const int BMP_HEADER_LENGTH = 54;
        const int BMP_PALETTE_LENGTH = 1024; // 256 double WORD (4 bytes) colors.
        const int BMP_FIRST_PIXEL = BMP256Converter.BMP_HEADER_LENGTH + BMP256Converter.BMP_PALETTE_LENGTH;
        #endregion

        #region Internal vars
        static int _currentPaletteHash = 0;
        static BmpEncoder _encoder;
        #endregion

        #region Methods & Functions
        static void SetupPalette(PAL palette)
        {
            if (palette.GetHashCode() != BMP256Converter._currentPaletteHash)
            {
                BMP256Converter._encoder = new BmpEncoder()
                {
                    BitsPerPixel = BmpBitsPerPixel.Pixel8,
                    Quantizer = new PaletteQuantizer(new ReadOnlyMemory<Color>(palette.ToColors()), new QuantizerOptions() { Dither = null })
                };

                BMP256Converter._currentPaletteHash = palette.GetHashCode();
            }
        }

        public static void Convert(byte[] buffer, out byte[] bitmap, out short width, out short height, PAL palette)
        {
            BMP256Converter.SetupPalette(palette);

            using (Image image = ImageProcessor.ProcessImage(buffer, out IImageFormat mime))
            {
                width = (short)image.Width;
                height = (short)image.Height;

                using (var stream = new MemoryStream())
                {
                    if (mime.DefaultMimeType != BmpFormat.Instance.DefaultMimeType)
                    {
                        image.Mutate(x => x.Flip(FlipMode.Vertical)); // Resolves the BMP inverse pixel order. 
                    }
                    image.Save(stream, BMP256Converter._encoder);

                    bitmap = new byte[width * height];
                    stream.Position = BMP256Converter.BMP_FIRST_PIXEL;

                    int readed = stream.Read(bitmap, 0, bitmap.Length);
                    
                    if (readed != bitmap.Length)
                    {
                        throw new OutOfMemoryException($"Error reading pixels from BMP. Expected readed bytes: {bitmap.Length}, readed bytes: {readed}.");
                    }
                } 
            }
        }
        #endregion
    }
}
