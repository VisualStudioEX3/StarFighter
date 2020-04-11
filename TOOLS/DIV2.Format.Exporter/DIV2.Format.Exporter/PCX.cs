using System;
using System.IO;
using ImageMagick;
using DIV2.Format.Exporter.MethodExtensions;

namespace DIV2.Format.Exporter
{
    class PCX
    {
        #region Constants
        const int HEADER_LENGTH = 128;
        const int PALETTE_LENGTH = 768;
        const int RLE_COUNTER_MASK = 0xC0; // Mask for check if bits 6 and 7 ar set (to check if is a counter byte).
        const int RLE_CLEAR_MASK = 0x3F; // Mask for clear bits 6 and 7 (to get the counter value).
        #endregion

        #region Properties
        public short Width { get; private set; }
        public short Height { get; private set; }
        public byte[] Bitmap { get; private set; }
        public byte[] Palette { get; private set; }
        #endregion

        #region Constructor
        public PCX(byte[] pngData, bool skipPalette, bool convertPaletteToDAC) : this(new MagickImage(pngData), skipPalette, convertPaletteToDAC)
        {
        }

        public PCX(string pngFilename, bool skipPalette, bool convertPaletteToDAC) : this(new MagickImage(pngFilename), skipPalette, convertPaletteToDAC)
        { 
        }

        PCX(MagickImage pngImage, bool skipPalette, bool convertPaletteToDAC)
        {
            // Load PNG image and convert to PCX using ImageMagick framework:
            using (MagickImage png = pngImage)
            {
                this.Width = (short)png.Width;
                this.Height = (short)png.Height;
                this.Bitmap = new byte[this.Width * this.Height];

                png.ColorType = ColorType.Palette;
                png.Format = MagickFormat.Pcx;

                // Load the PCX image:
                using (var pcx = new MagickImage(png.ToByteArray()))
                {
                    using (var buffer = new BinaryReader(new MemoryStream(pcx.ToByteArray())))
                    {
                        // Lambda function to clear bits 6 and 7 in a byte value:
                        var clearBits = new Func<byte, byte>((value) =>
                        {
                            // .NET bit operations works in Int32 values. A conversion is needed to work with bytes.
                            int i = value;
                            return (byte)(i & PCX.RLE_CLEAR_MASK);
                        });

                        int imageSize = (int)(buffer.BaseStream.Length - (PCX.HEADER_LENGTH + (PCX.PALETTE_LENGTH + 1)));
                        byte value, write;
                        int index = 0;

                        // Read and decompress RLE image data:
                        buffer.BaseStream.Position = PCX.HEADER_LENGTH;
                        for (int i = 0; i < imageSize; i++)
                        {
                            value = buffer.ReadByte();
                            if ((value & PCX.RLE_COUNTER_MASK) == PCX.RLE_COUNTER_MASK) // Checks if is a counter byte:
                            {
                                value = clearBits(value); // Clear bits 6 and 7 to get the counter value.
                                write = buffer.ReadByte(); // Next byte is the pixel value to write.
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

                        // Read the PCX palette and convert to VGA format:
                        if (!skipPalette)
                        {
                            buffer.BaseStream.Position++;  // Skip palette marker byte.
                            if (convertPaletteToDAC)
                            {
                                this.Palette = new byte[PCX.PALETTE_LENGTH];
                                for (int i = 0; i < PCX.PALETTE_LENGTH; i++)
                                {
                                    this.Palette[i] = (byte)(buffer.ReadByte() / 4); // This convert from 0-255 range to VGA 0-63 range value.
                                } 
                            }
                            else
                            {
                                this.Palette = buffer.ReadBytes(PCX.PALETTE_LENGTH);
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
