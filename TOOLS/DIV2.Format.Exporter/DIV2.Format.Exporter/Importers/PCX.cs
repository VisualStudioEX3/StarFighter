using DIV2.Format.Exporter;
using DIV2.Format.Exporter.MethodExtensions;
using System;
using System.IO;

namespace DIV2.Format.Importer
{
    class PCX
    {
        #region Constants
        const int HEADER_LENGTH = 128;

        const byte HEADER_SIGNATURE = 0x0A;
        const byte HEADER_MIN_VERSION = 0;
        const byte HEADER_MAX_VERSION = 5;
        const byte HEADER_UNCOMPRESSED = 0;
        const byte HEADER_RLE_ENCODED = 1;
        const byte HEADER_BPP_8 = 8;

        const int HEADER_BPP_POSITION = 3;
        const int HEADER_WIDTH_POSITION = 8;
        const int HEADER_HEIGHT_POSITION = 10;

        const int RLE_COUNTER_MASK = 0xC0; // Mask for check if bits 6 and 7 ar set (to check if is a counter byte).
        const int RLE_CLEAR_MASK = 0x3F; // Mask for clear bits 6 and 7 (to get the counter value).
        const int PALETTE_MARKER = 0x0C; // Marker of the 256 color palette at the end of image data.

        static readonly FormatException NOT_256_COLORS_EXCEPTION = new FormatException("The PCX image is not a 256 color image.");
        #endregion

        #region Constructor
        internal static bool IsPCX(byte[] buffer)
        {
            bool signature = buffer[0] == HEADER_SIGNATURE;
            bool version = buffer[1].IsClamped(HEADER_MIN_VERSION, HEADER_MAX_VERSION);
            bool compress = buffer[2].IsClamped(HEADER_UNCOMPRESSED, HEADER_RLE_ENCODED);

            return signature && version && compress;
        }

        internal static bool IsPCX256(byte[] buffer)
        {
            bool header = IsPCX(buffer);
            bool isBpp8 = buffer[3] == HEADER_BPP_8;
            bool paletteMarker = buffer[buffer.Length - ColorPalette.SIZE - 1] == PALETTE_MARKER;

            return header && isBpp8 && paletteMarker;
        }

        internal static void Import(byte[] buffer, out short width, out short height, out byte[] bitmap, out PAL palette)
        {
            if (IsPCX256(buffer))
            {
                using (var file = new BinaryReader(new MemoryStream(buffer)))
                {
                    file.BaseStream.Position = HEADER_BPP_POSITION;

                    file.BaseStream.Position = HEADER_WIDTH_POSITION;
                    width = file.ReadInt16();

                    file.BaseStream.Position = HEADER_HEIGHT_POSITION;
                    height = file.ReadInt16();

                    // Lambda function to clear bits 6 and 7 in a byte value:
                    Func<byte, byte> clearBits = (arg) =>
                    {
                        // .NET bit operations works in Int32 values. A conversion is needed to work with bytes.
                        int i = arg;
                        return (byte)(i & RLE_CLEAR_MASK);
                    };

                    int imageSize = (int)(file.BaseStream.Length - (HEADER_LENGTH + (ColorPalette.SIZE + 1)));
                    byte value, write;
                    int index = 0;

                    // Read and decompress RLE image data:
                    bitmap = new byte[imageSize];

                    file.BaseStream.Position = HEADER_LENGTH;
                    for (int i = 0; i < imageSize; i++)
                    {
                        value = file.ReadByte();
                        if ((value & RLE_COUNTER_MASK) == RLE_COUNTER_MASK) // Checks if is a counter byte:
                        {
                            value = clearBits(value); // Clear bits 6 and 7 to get the counter value.
                            write = file.ReadByte(); // Next byte is the pixel value to write.
                            for (byte j = 0; j < value; j++) // Write n times the pixel value:
                            {
                                bitmap[index] = write;
                                index++;
                            }
                            i++;
                        }
                        else // Single pixel value:
                        {
                            bitmap[index] = value;
                            index++;
                        }
                    }

                    palette = CreatePalette(file);
                }
            }
            else
            {
                throw NOT_256_COLORS_EXCEPTION;
            }
        }

        internal static PAL CreatePalette(BinaryReader file)
        {
            file.BaseStream.Position = file.BaseStream.Length - ColorPalette.SIZE - 1;

            if (file.ReadByte() == PALETTE_MARKER)
            {
                var colors = new ColorPalette(file.ReadBytes(ColorPalette.SIZE));
                return new PAL(colors);
            }
            else
                throw NOT_256_COLORS_EXCEPTION;
        }
        #endregion
    }
}
