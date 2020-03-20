using System;
using System.Collections.Generic;
using System.IO;

namespace DIV2Tools
{
    /// <summary>
    /// PCX importer for 256 color images.
    /// </summary>
    public class PCX
    {
        #region Constants
        const int PALETTE_LENGTH = 768;
        const byte PALETTE_MARKER = 0x0C;
        #endregion

        #region Structures
        struct Header // 128 bytes:
        {
            #region Constants
            const byte ZSOFT_ID = 0x0A;
            const byte VERSION = 5;
            const byte BPP = 8;
            const byte NUMBER_OF_PLANES = 1;
            const byte HEADER_INTERPRETATION = 1;

            public const int LENGTH = 128;
            #endregion

            #region Public vars
            public readonly byte zSoftId;
            public readonly byte version;
            public readonly byte encoding;
            public readonly byte bpp;
            public readonly short[] imageDimensions; // 8 bytes (4 shorts)
            public readonly short horizontalResolution;
            public readonly short verticalResolution;
            public readonly byte[] headerPallete; // 48 bytes.
            public readonly byte reserved; // Always 0.
            public readonly byte numberOfPlanes;
            public readonly short bytesPerLine;
            public readonly short headerInterpretation; // 01 or 02 values.
            public readonly short horizontalVideScreenSize;
            public readonly short verticalVideoScreenSize;
            public readonly byte[] blanks; // 54 unused bytes. 
            #endregion

            #region Constructor
            public Header(BinaryReader file)
            {
                this.zSoftId = file.ReadByte();
                this.version = file.ReadByte();
                this.encoding = file.ReadByte();
                this.bpp = file.ReadByte();
                this.imageDimensions = new short[4];
                for (int i = 0; i < this.imageDimensions.Length; i++)
                {
                    this.imageDimensions[i] = file.ReadInt16();
                }
                this.horizontalResolution = file.ReadInt16();
                this.verticalResolution = file.ReadInt16();
                this.headerPallete = file.ReadBytes(48);
                this.reserved = file.ReadByte();
                this.numberOfPlanes = file.ReadByte();
                this.bytesPerLine = file.ReadInt16();
                this.headerInterpretation = file.ReadInt16();
                this.horizontalVideScreenSize = file.ReadInt16();
                this.verticalVideoScreenSize = file.ReadInt16();
                this.blanks = file.ReadBytes(54);
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
                       $"Bits per pixel: {this.bpp} (256 colors must be 8 bits per pixel)\n" +
                       $"Image dimensions: (xMin: {this.imageDimensions[0]}, yMin: {this.imageDimensions[1]}, xMax: {this.imageDimensions[2]}, yMax: {this.imageDimensions[2]}) Using xMax + 1 and xMin + 1 to get Width and Height.\n" +
                       $"Resolution: {this.horizontalResolution} x {this.verticalResolution}\n" +
                       $"Header palette: (48 bytes for unused EGA/CGA palette format)\n" +
                       $"Reserved: {this.reserved} (unused, always 0)\n" +
                       $"Number of planes: {this.numberOfPlanes} (256 colors must be 1)\n" +
                       $"Bytes per line: {this.bytesPerLine}\n" +
                       $"Header interpretation: {this.headerInterpretation} (1 - Color or Black & White, 2 - Greyscale, 256 colors must be 1)\n" +
                       $"Screen size: {this.horizontalVideScreenSize} x {this.verticalVideoScreenSize}\n" +
                       $"Blanks: (54 unused bytes to fill header size)\n";
            }
            #endregion
        } 
        #endregion

        #region Internal vars
        Header _header;
        #endregion

        #region Properties
        public short Width { get; private set; }
        public short Height { get; private set; }
        public byte[] Pixels { get; private set; }
        public byte[] Palette { get; private set; }
        #endregion

        #region Constructor
        public PCX(string filename, bool verbose = true)
        {
            using (var file = new BinaryReader(File.OpenRead(filename)))
            {
                this._header = new Header(file);
                Helper.Log(this._header.ToString(), verbose);

                if (this._header.Check())
                {
                    // Read RLE pixels:
                    int rleLenght = (int)file.BaseStream.Length - Header.LENGTH - PCX.PALETTE_LENGTH - 1;
                    byte[] rlePixels = file.ReadBytes(rleLenght);

                    // Read 256 color palette:
                    if (file.ReadByte() == PCX.PALETTE_MARKER)
                    {
                        this.Palette = file.ReadBytes(PCX.PALETTE_LENGTH);
                    }
                    else
                    {
                        throw new FormatException("Error to read color palette. Palette byte marker not found.");
                    }

                    //int lenght = (this._header.imageDimensions[2] + 1) * (this._header.imageDimensions[3] + 1);
                    //var pixels = new byte[lenght];
                    //Helper.RLEDecompress(rlePixels, pixels, (uint)rleLenght);

                    //int columns = 0;
                    //for (int i = 0; i < pixels.Length; i++)
                    //{
                    //    Console.Write($"#{i:000000}:{pixels[i]:000} ");
                    //    if (++columns == 18)
                    //    {
                    //        columns = 0;
                    //        Console.WriteLine();
                    //    }
                    //}
                    //Console.WriteLine();

                    //byte palFlag = file.ReadByte();
                    //Console.WriteLine($"Palette flag: {palFlag} ({palFlag == 0x0C})"); 
                }
                else
                {
                    throw new FormatException("The PCX loaded is not valid.");
                }
            }
        } 
        #endregion

        // TODO: Implement RLE decoder function.
    }
}
