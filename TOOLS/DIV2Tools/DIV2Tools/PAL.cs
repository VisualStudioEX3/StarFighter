using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace DIV2Tools
{
    /// <summary>
    /// PAL file.
    /// </summary>
    public class PAL
    {
        #region Structs
        /// <summary>
        /// Header description.
        /// </summary>
        class Header : DIVFormatBaseHeader
        {
            #region Constants
            const string HEADER_ID = "pal";
            #endregion

            #region Constructor
            public Header() : base(Header.HEADER_ID)
            {
            }

            public Header(BinaryReader file) : base(Header.HEADER_ID, file)
            {
            }
            #endregion

            #region Methods & Functions
            public override string ToString()
            {
                return $"{base.ToString()}\n";
            }
            #endregion
        }

        /// <summary>
        /// Color structure, with byte components in ranges from 0 to 63.
        /// </summary>
        public struct Color
        {
            #region Public vars
            public byte r, g, b;
            #endregion

            #region Operators
            public static bool operator ==(Color a, Color b)
            {
                return a.r == b.r &&
                       a.g == b.g &&
                       a.b == b.b;
            }

            public static bool operator !=(Color a, Color b)
            {
                return !(a == b);
            }
            #endregion

            #region Constructor
            public Color(BinaryReader file)
            {
                this.r = Color.Clamp(file.ReadByte());
                this.g = Color.Clamp(file.ReadByte());
                this.b = Color.Clamp(file.ReadByte());
            }

            public Color(byte r, byte g, byte b)
            {
                this.r = Color.Clamp(r);
                this.g = Color.Clamp(g);
                this.b = Color.Clamp(b);
            }
            #endregion

            #region Methods & Functions
            static byte Clamp(byte value)
            {
                return Math.Clamp(value, (byte)0, (byte)63);
            }

            public void Write(BinaryWriter file)
            {
                file.Write(Color.Clamp(this.r));
                file.Write(Color.Clamp(this.g));
                file.Write(Color.Clamp(this.b));
            }

            public override bool Equals(object obj)
            {
                return this == (Color)obj;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public int ToInt32()
            {
                return (this.r * 65536) + (this.g * 256) + this.b;
            }

            public override string ToString()
            {
                return $"[{Color.Clamp(this.r):00}.{Color.Clamp(this.g):00}.{Color.Clamp(this.b):00}]";
            }
            #endregion
        }

        /// <summary>
        /// 256 length-positions Color table.
        /// </summary>
        public class ColorPalette
        {
            #region Constants
            public const int LENGTH = 256;
            #endregion

            #region Internal vars
            Color[] _colors;
            #endregion

            #region Properties
            public Color this[int index]
            {
                get { return this._colors[index]; }
                set { this._colors[index] = value; }
            }
            #endregion

            #region Operators
            public static bool operator ==(ColorPalette a, ColorPalette b)
            {
                for (int i = 0; i < ColorPalette.LENGTH; i++)
                {
                    if (a[i] != b[i]) return false;
                }

                return true;
            }

            public static bool operator !=(ColorPalette a, ColorPalette b)
            {
                return !(a == b);
            }
            #endregion

            #region Constructors
            public ColorPalette()
            {
                this._colors = new Color[ColorPalette.LENGTH];
            }

            /// <summary>
            /// Read all colors from a palette.
            /// </summary>
            /// <param name="file"><see cref="BinaryReader"/> instance of the PAL or compatible file format that contain palette data.</param>
            /// <remarks>The <see cref="BinaryReader"/> instance must be setup in the first byte of the palette array in the opened file.</remarks>
            public ColorPalette(BinaryReader file)
            {
                this._colors = new Color[ColorPalette.LENGTH];
                for (int i = 0; i < ColorPalette.LENGTH; i++)
                {
                    this._colors[i] = new Color(file);
                }
            }
            #endregion

            #region Methods & Functions
            /// <summary>
            /// Read all colors from a 256 colors PCX image.
            /// </summary>
            /// <param name="file"><see cref="byte"/> array with the content of a PCX file.</param>
            /// <returns>Returns a new instance of <see cref="ColorPalette"/> with all 256 colors.</returns>
            /// <remarks>The PCX must be a 256 color indexed format.</remarks>
            public static ColorPalette ReadPaletteFromPCXFile(byte[] file)
            {
                // TODO: Uses new PCX class to import palette.

                const int PAL_LENGTH = 768;
                const byte PAL_MARKER = 0x0C;

                int index = (file.Length - PAL_LENGTH) - 1;
                var colors = new ColorPalette();

                // Check if PCX is 256 color indexed:
                if (file[3] != 8 && file[index] != PAL_MARKER)
                {
                    throw new FormatException("The PCX file readed not is a 256 color indexed PCX image and not contain a 8 bit color palette at the end of file to read.");
                }

                for (int i = 0; i < ColorPalette.LENGTH; i++)
                {
                    colors[i] = new Color((byte)(file[++index] / 4), (byte)(file[++index] / 4), (byte)(file[++index] / 4)); // Divide each component by 4 to map to DIV format (0-63 color range).
                }

                return colors;
            }

            /// <summary>
            /// Read all colors from a 256 colors PCX image.
            /// </summary>
            /// <param name="filename">PCX filename to read.</param>
            /// <returns>Returns a new instance of <see cref="ColorPalette"/> with all 256 colors.</returns>
            /// <remarks>The PCX must be a 256 color indexed format.</remarks>
            public static ColorPalette ReadPaletteFromPCXFile(string filename)
            {
                return ColorPalette.ReadPaletteFromPCXFile(File.ReadAllBytes(filename));
            }

            public void Write(BinaryWriter file)
            {
                foreach (var color in this._colors)
                {
                    color.Write(file);
                }
            }

            public override bool Equals(object obj)
            {
                return this == (ColorPalette)obj;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public int[] ToInt32Array()
            {
                var values = new int[ColorPalette.LENGTH];

                for (int i = 0; i < ColorPalette.LENGTH; i++)
                {
                    values[i] = this._colors[i].ToInt32();
                }

                return values;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Color table:");

                int column = 0;
                for (int i = 0; i < ColorPalette.LENGTH; i++)
                {
                    sb.Append($"{i:000}:{this._colors[i].ToString()} ");
                    if (++column == 8)
                    {
                        column = 0;
                        sb.AppendLine();
                    }
                }

                return sb.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Color Ranges.
        /// </summary>
        public struct ColorRange // 36 bytes.
        {
            #region Constants
            const int DEFAULT_COLORS = 16;
            const int DEFAULT_RANGE_REPETITIONS = 16;
            const int RANGE_LENGTH = 32;
            #endregion

            #region Properties
            /// <summary>
            /// Number of colors. Possible values: 8, 16 or 32.
            /// </summary>
            public byte Colors { get; private set; }

            /// <summary>
            /// Range type. Possible values are: 0 - direct; 1, 2, 4 or 8 - editable each n colors.
            /// </summary>
            public byte Type { get; private set; }

            /// <summary>
            /// Is fixed?
            /// </summary>
            public bool IsFixed { get; private set; }

            /// <summary>
            /// Index of black color.
            /// </summary>
            public byte BlackColor { get; private set; }

            /// <summary>
            /// Color ranges.
            /// </summary>
            public byte[] ColorRanges { get; private set; }
            #endregion

            #region Operators
            public static bool operator ==(ColorRange a, ColorRange b)
            {
                return a.Colors == b.Colors &&
                       a.Type == b.Type &&
                       a.IsFixed == b.IsFixed &&
                       a.BlackColor == b.BlackColor &&
                       new Func<bool>(() =>
                       {
                           for (int i = 0; i < ColorRange.RANGE_LENGTH; i++)
                           {
                               if (a.ColorRanges[i] != b.ColorRanges[i]) return false;
                           }
                           return true;
                       }).Invoke();
            }

            public static bool operator !=(ColorRange a, ColorRange b)
            {
                return !(a == b);
            }
            #endregion

            #region Constructor
            public ColorRange(BinaryReader file)
            {
                this.Colors = file.ReadByte();
                this.Type = file.ReadByte();
                this.IsFixed = file.ReadBoolean();
                this.BlackColor = file.ReadByte();
                this.ColorRanges = file.ReadBytes(ColorRange.RANGE_LENGTH);
            }
            #endregion

            #region Method & Functions
            public static ColorRange CreateDefaultRanges(byte rangeValue)
            {
                return new ColorRange()
                {
                    Colors = ColorRange.DEFAULT_COLORS,
                    Type = 0,
                    IsFixed = false,
                    BlackColor = 0,
                    ColorRanges = new Func<byte[]>(() =>
                    {
                        var ranges = new byte[ColorRange.RANGE_LENGTH];
                        for (int i = 0; i < ColorRange.DEFAULT_RANGE_REPETITIONS; i++)
                        {
                            ranges[i] = rangeValue;
                        }
                        return ranges;
                    }).Invoke()
                };
            }

            public void Write(BinaryWriter file)
            {
                file.Write(this.Colors);
                file.Write(this.Type);
                file.Write(this.IsFixed);
                file.Write(this.BlackColor);
                file.Write(this.ColorRanges);
            }

            public override bool Equals(object obj)
            {
                return this == (ColorRange)obj;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"- Colors: {this.Colors}\n- Type: {this.Type}\n- Fixed: {this.IsFixed}\n- Black color: {this.BlackColor}\n");

                sb.Append("- Color Ranges:\n");
                int column = 0;
                foreach (byte color in this.ColorRanges)
                {
                    sb.Append($"{color:000} ");
                    if (++column == 8)
                    {
                        column = 0;
                        sb.AppendLine();
                    }
                }
                return sb.ToString();
            }
            #endregion
        }

        /// <summary>
        /// 16 length-positions ColorRange table.
        /// </summary>
        public class ColorRangeTable
        {
            #region Constants
            public const int LENGTH = 16;
            #endregion

            #region Internal vars
            ColorRange[] _ranges;
            #endregion

            #region Properties
            public ColorRange this[int index]
            {
                get { return this._ranges[index]; }
                set { this._ranges[index] = value; }
            }
            #endregion

            #region Operators
            public static bool operator ==(ColorRangeTable a, ColorRangeTable b)
            {
                for (int i = 0; i < ColorRangeTable.LENGTH; i++)
                {
                    if (a[i] != b[i]) return false;
                }

                return true;
            }

            public static bool operator !=(ColorRangeTable a, ColorRangeTable b)
            {
                return !(a == b);
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Creates a default ColorRanges for a palette.
            /// </summary>
            public ColorRangeTable()
            {
                byte range = 0;

                this._ranges = new ColorRange[ColorRangeTable.LENGTH];

                for (int i = 0; i < ColorRangeTable.LENGTH; i++)
                {
                    this._ranges[i] = ColorRange.CreateDefaultRanges(range);
                    range += 16;
                }
            }

            /// <summary>
            /// Read the 16 color ranges associated to a color palette.
            /// </summary>
            /// <param name="file"><see cref="BinaryReader"/> instance of the PAL or compatible file format that contain color range array data.</param>
            /// <remarks>The <see cref="BinaryReader"/> instance must be setup in the first byte of the first color range structure in the opened file.</remarks>
            public ColorRangeTable(BinaryReader file)
            {
                this._ranges = new ColorRange[ColorRangeTable.LENGTH];
                for (int i = 0; i < ColorRangeTable.LENGTH; i++)
                {
                    this._ranges[i] = new ColorRange(file);
                }
            }
            #endregion

            #region Methods & Functions

            public void Write(BinaryWriter file)
            {
                foreach (var range in this._ranges)
                {
                    range.Write(file);
                }
            }

            public override bool Equals(object obj)
            {
                return this == (ColorRangeTable)obj;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Color Ranges:");

                for (int i = 0; i < ColorRangeTable.LENGTH; i++)
                {
                    sb.AppendLine($"Range {i:00}:\n{this._ranges[i].ToString()} ");
                }

                return sb.ToString();
            }
            #endregion
        }
        #endregion

        #region Internal vars
        Header _header;
        ColorPalette _palette;
        ColorRangeTable _colorRanges;
        #endregion

        #region Properties
        public ColorPalette Palette => this._palette;
        public ColorRangeTable Ranges => this._colorRanges;
        #endregion

        #region Operators
        public static bool operator ==(PAL a, PAL b)
        {
            return a.Palette == b.Palette &&
                   a.Ranges == b.Ranges;
        }

        public static bool operator !=(PAL a, PAL b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructor
        PAL()
        {
            this._header = new Header();
            this._palette = new ColorPalette();
            this._colorRanges = new ColorRangeTable();
        }

        /// <summary>
        /// Import a PAL file.
        /// </summary>
        /// <param name="filename">PAL file.</param>
        /// <param name="verbose">Log PAL import data to console. By default is true.</param>
        public PAL(string filename, bool verbose = true)
        {
            using (var file = new BinaryReader(File.OpenRead(filename)))
            {
                Helper.Log($"Reading \"{filename}\"...\n", verbose);

                this._header = new Header(file);
                Helper.Log(this._header.ToString(), verbose);

                if (this._header.Check())
                {
                    this._palette = new ColorPalette(file);
                    Helper.Log(this._palette.ToString(), verbose);

                    this._colorRanges = new ColorRangeTable(file);
                    Helper.Log(this._colorRanges.ToString(), verbose);
                }
                else
                {
                    throw new FormatException("Invalid PAL file!");
                }
            }
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Creates a <see cref="PAL"/> instance using the palette data from PCX image.
        /// </summary>
        /// <param name="filename">PCX filename.</param>
        /// <returns>Returns a new <see cref="PAL"/> instance with the PCX palette data.</returns>
        public static PAL CreateFromPCX(string filename)
        {
            var pal = new PAL();
            pal._palette = PAL.ColorPalette.ReadPaletteFromPCXFile(filename);

            return pal;
        }

        /// <summary>
        /// Write all data in a file.
        /// </summary>
        /// <param name="filename"><see cref="PAL"/> filename.</param>
        public void Write(string filename)
        {
            using (var file = new BinaryWriter(File.OpenWrite(filename)))
            {
                this._header.Write(file);
                this._palette.Write(file);
                this._colorRanges.Write(file);
            }
        }

        /// <summary>
        /// Convert pixel colors from source palette to destiny palette.
        /// </summary>
        /// <param name="pixels"><see cref="byte"/> array that contain the pixels to adapt to DIV PAL.</param>
        /// <param name="sourcePal">Original pixels palette.</param>
        /// <param name="destPal">palette model to adapt pixels.</param>
        /// <returns>Returns a new <see cref="byte"/> array with adapted pixels.</returns>
        public static MAP.Bitmap Convert(MAP.Bitmap pixels, ColorPalette sourcePal, ColorPalette destPal)
        {
            var newPixels = new MAP.Bitmap(pixels.Width, pixels.Height);
            var sourceColors = sourcePal.ToInt32Array();
            var destColors = destPal.ToInt32Array();

            for (int i = 0; i < pixels.Count; i++)
            {
                newPixels[i] = PAL.GetNearColor(sourceColors[pixels[i]], destColors);
            }

            return newPixels;
        }

        /// <summary>
        /// Compare 2 <see cref="PAL"/> instances with option to ignore <see cref="ColorRangeTable"/> comparison.
        /// </summary>
        /// <param name="a">The first <see cref="PAL"/> to compare.</param>
        /// <param name="b">The second <see cref="PAL"/> to compare.</param>
        /// <param name="ignoreColorRanges">Ignore <see cref="ColorRangeTable"/> in comparison process. By default is <see cref="true"/>.</param>
        /// <returns>Returns <see cref="true"/> if the 2 <see cref="PAL"/> instances are equal.</returns>
        public static bool Compare(PAL a, PAL b, bool ignoreColorRanges = true)
        {
            return (a.Palette == b.Palette) && (ignoreColorRanges ? true : a.Ranges == b.Ranges);
        }

        static byte GetNearColor(int color, int[] pal)
        {
            int lastDiff = 0;
            int index = 0;

            for (int i = 0; i < ColorPalette.LENGTH; i++)
            {
                if (color == pal[i]) return (byte)i;

                int currentDiff = Math.Abs(color - pal[i]);

                if (i == 0 || currentDiff < lastDiff)
                {
                    lastDiff = currentDiff;
                    index = i;
                }
            }

            return (byte)index;
        }

        public override bool Equals(object obj)
        {
            return this == (PAL)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
