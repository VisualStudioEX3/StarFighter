using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace DIV2.Format.Exporter
{
    class PNG2BMP
    {
        #region Constants
        const int RGB_PAL_LENGTH = 256;

        const int BMP_HEADER_LENGTH = 54;
        const int BMP_PALETTE_LENGTH = 1024; // 256 double WORD (4 bytes) colors.
        const int BMP_FIRST_PIXEL = PNG2BMP.BMP_HEADER_LENGTH + PNG2BMP.BMP_PALETTE_LENGTH;
        #endregion

        #region Internal vars
        static BmpEncoder _encoder;
        #endregion

        #region Methods & Functions
        public static void SetupBMPEncoder(PAL palette)
        {
            var rgb = palette.DAC2RGB();
            var colors = new Color[PNG2BMP.RGB_PAL_LENGTH];
            int index = -1;

            for (int i = 0; i < PNG2BMP.RGB_PAL_LENGTH; i++)
            {
                colors[i] = Color.FromRgb(rgb[++index], rgb[++index], rgb[++index]);
            }

            PNG2BMP._encoder = new BmpEncoder()
            {
                BitsPerPixel = BmpBitsPerPixel.Pixel8,
                Quantizer = new PaletteQuantizer(new ReadOnlyMemory<Color>(colors), new QuantizerOptions() { Dither = null, MaxColors = PNG2BMP.RGB_PAL_LENGTH })
            };
        }

        public static void Convert(string pngFilename, out byte[] pixels, out short width, out short height)
        {
            PNG2BMP.Convert(File.ReadAllBytes(pngFilename), out pixels, out width, out height);
        }

        public static void Convert(byte[] pngFileData, out byte[] pixels, out short width, out short height)
        {
            using (var png = Image.Load(pngFileData))
            {
                pixels = new byte[png.Width * png.Height];
                width = (short)png.Width;
                height = (short)png.Height;
                
                png.Mutate(x => x.Flip(FlipMode.Vertical)); // Resolves the BMP inverse pixel order.

                using (var stream = new MemoryStream())
                {
                    png.Save(stream, PNG2BMP._encoder);

                    stream.Position = PNG2BMP.BMP_FIRST_PIXEL;
                    int readed = stream.Read(pixels, 0, pixels.Length);

                    if (readed != pixels.Length)
                    {
                        throw new OutOfMemoryException($"Error reading pixels from BMP. Expected readed bytes: {pixels.Length}, readed bytes: {readed}.");
                    }
                }
            }
        } 
        #endregion
    }
}
