using System;
using System.Collections.Generic;
using System.IO;
using DIV2Tools.MethodExtensions;

namespace DIV2Tools.Helpers
{
    /// <summary>
    /// PCX importer for 256 color images.
    /// </summary>
    public class PCX
    {
        #region Structures
        struct Header // 128 bytes:
        {
            #region Constants
            const byte ZSOFT_ID = 0x0A;
            const byte VERSION = 5;
            const byte BPP = 8;
            const byte NUMBER_OF_PLANES = 1;
            const byte HEADER_INTERPRETATION = 1;

            const int HEADER_PALETTE_LENGTH = 48;
            const int HEADER_UNUSED_BYTES = 54;

            /// <summary>
            /// Header length.
            /// </summary>
            public const int LENGTH = 128;
            #endregion

            #region Public vars
            public readonly byte zSoftId;
            public readonly byte version;
            public readonly byte encoding;
            public readonly byte bpp;
            public readonly short xMin;
            public readonly short yMin;
            public readonly short xMax;
            public readonly short yMax;
            public readonly short horizontalResolution;
            public readonly short verticalResolution;
            public readonly byte reserved;
            public readonly byte numberOfPlanes;
            public readonly short bytesPerLine;
            public readonly short headerInterpretation;
            public readonly short horizontalVideScreenSize;
            public readonly short verticalVideoScreenSize;
            #endregion

            #region Properties
            /// <summary>
            /// RLE compressed pixel array length.
            /// </summary>
            public int CompressedImageDataLength { get; private set; } 
            #endregion

            #region Constructor
            public Header(BinaryReader file)
            {
                this.zSoftId = file.ReadByte();
                this.version = file.ReadByte();
                this.encoding = file.ReadByte();
                this.bpp = file.ReadByte();
                this.xMin = file.ReadInt16();
                this.yMin = file.ReadInt16();
                this.xMax = file.ReadInt16();
                this.yMax = file.ReadInt16();
                this.horizontalResolution = file.ReadInt16();
                this.verticalResolution = file.ReadInt16();
                file.AdvanceReadPosition(Header.HEADER_PALETTE_LENGTH); // Unused EGA/CGA palette format.
                this.reserved = file.ReadByte();
                this.numberOfPlanes = file.ReadByte();
                this.bytesPerLine = file.ReadInt16();
                this.headerInterpretation = file.ReadInt16();
                this.horizontalVideScreenSize = file.ReadInt16();
                this.verticalVideoScreenSize = file.ReadInt16();
                file.AdvanceReadPosition(Header.HEADER_UNUSED_BYTES); // Unused bytes to fill the header length.

                this.CompressedImageDataLength = (int)file.GetLength() - (Header.LENGTH + EOFPalette.FULL_LENGTH);
            }
            #endregion

            #region Methods & Functions
            /// <summary>
            /// Checks for 256 color palette PCX format.
            /// </summary>
            /// <returns>Returns <see cref="true"/> if the header data match the 256 color format.</returns>
            public bool Check()
            {
                return this.zSoftId == Header.ZSOFT_ID &&
                       this.version == Header.VERSION &&
                       this.bpp == Header.BPP &&
                       this.numberOfPlanes == Header.NUMBER_OF_PLANES &&
                       this.headerInterpretation == Header.HEADER_INTERPRETATION;
            }

            public override string ToString()
            {
                return $"ZSoft Id: {this.zSoftId} (usually {Header.ZSOFT_ID})\n" +
                       $"Version: {this.version} (256 colors must be version {Header.VERSION})\n" +
                       $"RLE Encoding: {this.encoding} (usually 1)\n" +
                       $"Bits Per Pixel: {this.bpp} (256 colors must be {Header.BPP} bits per pixel)\n" +
                       $"Image Dimensions: (xMin: {this.xMin}, yMin: {this.yMin}, xMax: {this.xMax}, yMax: {this.yMax}) Using xMax + 1 and xMin + 1 to get Width and Height.\n" +
                       $"Resolution: {this.horizontalResolution} x {this.verticalResolution}\n" +
                       $"Header palette: (48 bytes for unused EGA/CGA palette format)\n" +
                       $"Reserved: {this.reserved} (unused, always 0)\n" +
                       $"Number Of Planes: {this.numberOfPlanes} (256 colors must be {Header.NUMBER_OF_PLANES})\n" +
                       $"Bytes Per Line: {this.bytesPerLine}\n" +
                       $"Header Interpretation: {this.headerInterpretation} (1 - Color or Black & White, 2 - Greyscale, 256 colors must be {Header.HEADER_INTERPRETATION})\n" +
                       $"Screen Size: {this.horizontalVideScreenSize} x {this.verticalVideoScreenSize}\n\n" +
                       $"Compressed Image Data Length: {this.CompressedImageDataLength}";
            }
            #endregion
        } 

        struct RLE8BppBitmap
        {
            #region Constants
            const int COUNTER_MASK = 0xC0; // Mask for check if bits 6 and 7 ar set.
            const int CLEAR_MASK = 0x3F;  // Mask for clear bits 6 and 7.
            #endregion

            #region Properties
            public byte[] Pixels { get; private set; }
            #endregion

            #region Constructor
            /// <summary>
            /// Reads and decompress the RLE 8 bpp image data.
            /// </summary>
            /// <param name="file"><see cref="BinaryReader"/> instance. Must be point to the first byte of the image data array.</param>
            /// <param name="length">Image data compressed length.</param>
            /// <param name="width">Image width.</param>
            /// <param name="height">Image height.</param>
            public RLE8BppBitmap(BinaryReader file, int length, int width, int height)
            {
                // Lambda function to clear bits 6 and 7 in a byte value:
                var clearBits = new Func<byte, byte>((value) =>
                {
                    // .NET bit operations works in Int32 values. A conversion is needed to work with bytes.

                    int i = value;
                    return (byte)(i & RLE8BppBitmap.CLEAR_MASK);
                });

                this.Pixels = new byte[width * height];

                byte value, write;
                int index = 0;

                for (int i = 0; i < length; i++)
                {
                    value = file.ReadByte();
                    if ((value & RLE8BppBitmap.COUNTER_MASK) == RLE8BppBitmap.COUNTER_MASK) // Checks if is a counter byte:
                    {
                        value = clearBits(value); // Clear bits 6 and 7 to get the repetition value.
                        write = file.ReadByte(); // Next byte is the pixel value to write n times.
                        for (byte j = 0; j < value; j++)
                        {
                            this.Pixels[index] = write;
                            index++;
                        }
                        i++;
                    }
                    else // Single pixel value:
                    {
                        this.Pixels[index] = value;
                        index++;
                    }
                }
            }
            #endregion
        }

        struct EOFPalette
        {
            #region Constants
            /// <summary>
            /// Palette marker. This confirm the existence of the 8 bpp palette at the end of the file.
            /// </summary>
            public const byte MARKER = 0x0C;
            /// <summary>
            /// Color array length.
            /// </summary>
            public const int PALETTE_LENGTH = 768;
            /// <summary>
            /// Full palette structure length (marker byte + color array length).
            /// </summary>
            public const int FULL_LENGTH = EOFPalette.PALETTE_LENGTH + 1;
            #endregion

            #region Public vars
            public readonly byte marker;
            public readonly byte[] colors;
            #endregion

            #region Constructor
            /// <summary>
            /// Reads the 8 bpp color palette at the end of the file.
            /// </summary>
            /// <param name="file"><see cref="BinaryReader"/> instance. Must be point to the marker byte before the palette array.</param>
            public EOFPalette(BinaryReader file)
            {
                this.marker = file.ReadByte();

                if (this.marker == EOFPalette.MARKER)
                {
                    this.colors = new byte[EOFPalette.PALETTE_LENGTH];

                    for (int i = 0; i < EOFPalette.PALETTE_LENGTH; i++)
                    {
                        this.colors[i] = (byte)(file.ReadByte() / 4);
                    }
                }
                else
                {
                    throw new FormatException("Error to read color palette. Palette byte marker not found.");
                }
            } 
            #endregion
        }
        #endregion

        #region Internal vars
        Header _header;
        RLE8BppBitmap _bitmap;
        EOFPalette _palette;
        #endregion

        #region Properties
        public short Width => (short)(this._header.xMax + 1);
        public short Height => (short)(this._header.yMax + 1);
        /// <summary>
        /// Uncompressed pixels.
        /// </summary>
        public byte[] Pixels => this._bitmap.Pixels;
        /// <summary>
        /// 256 color palette (3 bytes per color in RGB format).
        /// </summary>
        public byte[] Palette => this._palette.colors;
        #endregion

        #region Constructor
        PCX(Stream stream, bool verbose)
        {
            using (var file = new BinaryReader(stream))
            {
                this._header = new Header(file);
                Helper.Log(this._header.ToString(), verbose);

                if (this._header.Check())
                {
                    // Read RLE data and decompress pixels:
                    this._bitmap = new RLE8BppBitmap(file, this._header.CompressedImageDataLength, this.Width, this.Height);
                    
                    // Read 256 color palette at end of file:
                    this._palette = new EOFPalette(file);

                    Helper.Log("PCX loaded!", verbose);
                }
                else
                {
                    throw new FormatException("The PCX loaded is not valid.");
                }
            }
        }

        /// <summary>
        /// Import PCX file.
        /// </summary>
        /// <param name="filename"><see cref="PCX"/> filename.</param>
        /// <param name="verbose">Log <see cref="PCX"/> import data to console. By default is <see cref="true"/>.</param>
        public PCX(string filename, bool verbose = true) : this(File.OpenRead(filename), verbose)
        {
        }

        /// <summary>
        /// Import PCX from <see cref="byte"/> array.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array that contain <see cref="PCX"/> file data.</param>
        /// <param name="verbose">Log <see cref="PCX"/> import data to console. By default is <see cref="true"/>.</param>
        public PCX(byte[] buffer, bool verbose = true) : this(new MemoryStream(buffer), verbose)
        {
        }
        #endregion
    }
}
