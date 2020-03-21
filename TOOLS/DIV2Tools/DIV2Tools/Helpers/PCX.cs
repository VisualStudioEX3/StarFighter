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
            public int RLEPixelArrayLength { get; private set; } 
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

                this.RLEPixelArrayLength = (int)file.BaseStream.Length - (Header.LENGTH + EOFPalette.FULL_LENGTH);
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
                return $"ZSoft Id: {this.zSoftId} (usually 0x0A:10)\n" +
                       $"Version: {this.version} (256 colors must be version 5)\n" +
                       $"RLE Encoding: {this.encoding} (usually 1)\n" +
                       $"Bits Per Pixel: {this.bpp} (256 colors must be 8 bits per pixel)\n" +
                       $"Image Dimensions: (xMin: {this.xMin}, yMin: {this.yMin}, xMax: {this.xMax}, yMax: {this.yMax}) Using xMax + 1 and xMin + 1 to get Width and Height.\n" +
                       $"Resolution: {this.horizontalResolution} x {this.verticalResolution}\n" +
                       $"Header palette: (48 bytes for unused EGA/CGA palette format)\n" +
                       $"Reserved: {this.reserved} (unused, always 0)\n" +
                       $"Number Of Planes: {this.numberOfPlanes} (256 colors must be 1)\n" +
                       $"Bytes Per Line: {this.bytesPerLine}\n" +
                       $"Header Interpretation: {this.headerInterpretation} (1 - Color or Black & White, 2 - Greyscale, 256 colors must be 1)\n" +
                       $"Screen Size: {this.horizontalVideScreenSize} x {this.verticalVideoScreenSize}\n\n" +
                       $"RLE Pixel Array Length: {this.RLEPixelArrayLength}";
            }
            #endregion
        } 

        struct RLEBitmap
        {
            #region Internal vars
            byte[] _buffer;
            #endregion

            #region Properties
            public byte this[int index] => this._buffer[index];
            public int Length => this._buffer.Length;
            #endregion

            #region Constructor
            public RLEBitmap(BinaryReader file, int length)
            {
                this._buffer = file.ReadBytes(length);
            }
            #endregion

            #region Methods & Functions
            public byte[] Decompress(int decompressedLength)
            {
                throw new NotImplementedException();

                // TODO: Implements a RLE decompression algorithm that checks the 6ª and 7ª bit to get counters (ref: http://www.fysnet.net/pcxfile.htm)
            }
            #endregion
        }

        struct EOFPalette
        {
            #region Constants
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
            public EOFPalette(BinaryReader file)
            {
                this.marker = file.ReadByte();

                if (this.marker == EOFPalette.MARKER)
                {
                    this.colors = file.ReadBytes(EOFPalette.PALETTE_LENGTH);
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
        RLEBitmap _bitmap;
        EOFPalette _palette;
        #endregion

        #region Properties
        public short Width => (short)(this._header.xMax + 1);
        public short Height => (short)(this._header.yMax + 1);
        /// <summary>
        /// Uncompressed pixels.
        /// </summary>
        public byte[] Pixels { get; private set; }
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
                    // Read RLE pixels:
                    this._bitmap = new RLEBitmap(file, this._header.RLEPixelArrayLength);
                    
                    // Decompress image:
                    this.Pixels = this._bitmap.Decompress(this.Width * this.Height);
                    
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
        /// Import PCX from <see cref="Byte"/> array.
        /// </summary>
        /// <param name="buffer"><see cref="Byte"/> array that contain <see cref="PCX"/> file data.</param>
        /// <param name="verbose">Log <see cref="PCX"/> import data to console. By default is <see cref="true"/>.</param>
        public PCX(byte[] buffer, bool verbose = true) : this(new MemoryStream(buffer), verbose)
        {
        }
        #endregion
    }
}
