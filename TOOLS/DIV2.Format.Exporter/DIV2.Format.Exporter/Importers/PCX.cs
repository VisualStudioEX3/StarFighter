using DIV2.Format.Exporter.ExtensionMethods;
using DIV2.Format.Exporter.Interfaces;
using System;
using System.IO;

namespace DIV2.Format.Importer
{
    /// <summary>
    /// PCX 256 color importer.
    /// </summary>
    class PCX : IFormatValidable
    {
        #region Constants
        const int HEADER_LENGTH = 128;

        const byte HEADER_SIGNATURE = 0x0A;
        const byte HEADER_MIN_VERSION = 0;
        const byte HEADER_MAX_VERSION = 5;
        const byte HEADER_UNCOMPRESSED = 0;
        const byte HEADER_RLE_ENCODED = 1;
        const byte HEADER_BPP_8 = 8;

        const int HEADER_WIDTH_POSITION = 8;
        const int HEADER_HEIGHT_POSITION = 10;

        const byte RLE_COUNTER_MASK = 0xC0; // Mask for check if bits 6 and 7 ar set (to check if is a counter byte).
        const byte RLE_CLEAR_MASK = 0x3F; // Mask for clear bits 6 and 7 (to get the counter value).
        const byte PALETTE_MARKER = 0x0C; // Marker of the 256 color palette at the end of image data.
        const int PALETTE_LENGTH = 768;

        static readonly FormatException NOT_256_COLORS_EXCEPTION = new FormatException("The PCX image is not a 256 color image.");
        #endregion

        #region Properties
        public short Width { get; }
        public short Height { get; }
        public byte[] Bitmap { get; }
        public byte[] Colors { get; }
        public static PCX Instance => new PCX();
        #endregion

        #region Constructor
        PCX()
        {
        }

        public PCX(byte[] buffer)
        {
            if (this.Validate(buffer))
            {
                using (var stream = new BinaryReader(new MemoryStream(buffer)))
                {
                    stream.BaseStream.Position = HEADER_WIDTH_POSITION;
                    this.Width = (short)(stream.ReadInt16() + 1);

                    stream.BaseStream.Position = HEADER_HEIGHT_POSITION;
                    this.Height = (short)(stream.ReadInt16() + 1);

                    int imageSize = (int)(stream.BaseStream.Length - (HEADER_LENGTH + (PALETTE_LENGTH + 1)));
                    byte value, write;
                    int index = 0;

                    // Read and decompress RLE image data:
                    this.Bitmap = new byte[this.Width * this.Height];

                    stream.BaseStream.Position = HEADER_LENGTH;

                    for (int i = 0; i < imageSize; i++)
                    {
                        value = stream.ReadByte();
                        if ((value & RLE_COUNTER_MASK) == RLE_COUNTER_MASK) // Checks if is a counter byte:
                        {
                            value = value.ClearBits(RLE_CLEAR_MASK); // Clear bits 6 and 7 to get the counter value.
                            write = stream.ReadByte(); // Next byte is the pixel value to write.
                            for (byte j = 0; j < value; j++) // Write n times the pixel value:
                            {
                                this.Bitmap[index] = write;
                                index++;
                            }
                            i++;
                        }
                        else // Single pixel value:
                        {
                            this.Bitmap[index] = value;
                            index++;
                        }
                    }

                    this.Colors = ExtractPalette(stream);
                }
            }
            else
            {
                throw NOT_256_COLORS_EXCEPTION;
            }
        }
        #endregion

        #region Methods & Functions
        public bool Validate(byte[] buffer)
        {
            bool signature =        buffer[0] == HEADER_SIGNATURE;
            bool version =          buffer[1].IsClamped(HEADER_MIN_VERSION, HEADER_MAX_VERSION);
            bool compress =         buffer[2].IsClamped(HEADER_UNCOMPRESSED, HEADER_RLE_ENCODED);
            bool isBpp8 =           buffer[3] == HEADER_BPP_8;
            bool paletteMarker =    buffer[buffer.Length - PALETTE_LENGTH - 1] == PALETTE_MARKER;

            return signature && version && compress && isBpp8 && paletteMarker;
        }

        public static byte[] ExtractPalette(byte[] buffer)
        {
            return ExtractPalette(new BinaryReader(new MemoryStream(buffer)));
        }

        public static byte[] ExtractPalette(BinaryReader stream)
        {
            stream.BaseStream.Position = stream.BaseStream.Length - PALETTE_LENGTH - 1;

            if (stream.ReadByte() == PALETTE_MARKER)
                return stream.ReadBytes(PALETTE_LENGTH);
            else
                throw NOT_256_COLORS_EXCEPTION;
        }
        #endregion
    }
}
