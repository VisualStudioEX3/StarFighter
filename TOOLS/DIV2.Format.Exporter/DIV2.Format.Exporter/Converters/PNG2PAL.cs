using DIV2.Format.Exporter.MethodExtensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.IO;

namespace DIV2.Format.Exporter.Converters
{
    // PNG color palette interpretation: http://www.libpng.org/pub/png/spec/iso/index-object.html#11PLTE
    static class PNG2PAL
    {
        #region Constants
        const int PNG_PLTE_CHUNK_POSITION = 33;

        static readonly byte[] PNG_PLTE_SIGNATURE = { 80, 76, 84, 69 };

        readonly static PngEncoder UNCOMPRESSED_256_COLORS_PNG_ENCODER = new PngEncoder()
        {
            BitDepth = PngBitDepth.Bit8,
            ColorType = PngColorType.Palette,
            CompressionLevel = PngCompressionLevel.NoCompression
        };
        #endregion

        #region Methods & Functions
        internal static MemoryStream ConvertToIndexedPNG(byte[] buffer)
        {
            using (var image = Image.Load(buffer, out IImageFormat mime))
            {
                if(image.IsSupportedFormat(ImageSharpExtensions.SupportedMimeTypes.PNG, mime))
                {
                    return new MemoryStream(buffer);
                }
                else
                {
                    using (var stream = new MemoryStream())
                    {
                        image.Save(stream, PNG2PAL.UNCOMPRESSED_256_COLORS_PNG_ENCODER);
                        return stream;
                    }
                }
            }
        }

        internal static PAL CreatePalette(MemoryStream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                byte[] colors = new byte[PAL.COLOR_TABLE_LENGTH];

                reader.BaseStream.Position = PNG2PAL.PNG_PLTE_CHUNK_POSITION;

                int paletteSize = (int)reader.ReadUInt32(true);

                if (reader.CompareSignatures(PNG2PAL.PNG_PLTE_SIGNATURE)) // Checks if the chunk signature is the PLTE chunk.
                {
                    reader.ReadBytes(paletteSize).CopyTo(colors, 0);
                    return new PAL(colors, true);
                }
                else
                {
                    throw new FormatException("The PNG image not contains a 256 color palette (PLTE chunk not found).");
                }
            }
        } 
        #endregion
    }
}
