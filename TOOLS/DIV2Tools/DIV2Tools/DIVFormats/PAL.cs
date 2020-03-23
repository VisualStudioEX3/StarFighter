using System;
using System.Text;
using System.IO;
using DIV2Tools.Helpers;

namespace DIV2Tools.DIVFormats
{
    /// <summary>
    /// PAL file.
    /// </summary>
    public class PAL
    {
        #region Structs
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
        #endregion

        #region Classes
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

            /// <summary>
            /// Read all colors from a <see cref="PCX"/> image.
            /// </summary>
            /// <param name="pcx"><see cref="PCX"/> instance.</param>
            public ColorPalette(PCX pcx)
            {
                int index = -1;
                this._colors = new Color[ColorPalette.LENGTH];

                for (int i = 0; i < ColorPalette.LENGTH; i++)
                {
                    this._colors[i] = new Color(pcx.Palette[++index], pcx.Palette[++index], pcx.Palette[++index]);
                }
            }
            #endregion

            #region Methods & Functions
            public int Find(Color color, bool exact = true)
            {
                if (exact)
                {
                    for (int i = 0; i < ColorPalette.LENGTH; i++)
                    {
                        if (color == this[i]) return i;
                    }

                    return -1;
                }
                else
                {
                    int lastDiff = 0;
                    int index = 0;

                    for (int i = 0; i < ColorPalette.LENGTH; i++)
                    {
                        if (color == this[i]) return i;

                        int currentDiff = Math.Abs(color.ToInt32() - this[i].ToInt32());

                        if (i == 0 || currentDiff < lastDiff)
                        {
                            lastDiff = currentDiff;
                            index = i;
                        }
                    }

                    return index;
                }
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
        #endregion

        #region Properties
        public ColorPalette Colors { get; private set; }
        public ColorRangeTable Ranges { get; private set; }
        #endregion

        #region Operators
        public static bool operator ==(PAL a, PAL b)
        {
            return a.Colors == b.Colors &&
                   a.Ranges == b.Ranges;
        }

        public static bool operator !=(PAL a, PAL b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Import a <see cref="PAL"/> file.
        /// </summary>
        /// <param name="file"><see cref="BinaryReader"/> instance.</param>
        /// <param name="skipHeader">Skip read header. Use this when import a palette in a <see cref="MAP"/> or <see cref="FPG"/> instance. By default is <see cref="false"/>.</param>
        /// <param name="verbose">Log <see cref="PAL"/> import data to console. By default is <see cref="true"/>.</param>
        public PAL(BinaryReader file, bool skipHeader = false, bool verbose = true)
        {
            if (!skipHeader)
            {
                this._header = new Header(file);
                Helper.Log(this._header.ToString(), verbose);
            }

            if (this._header.Check() || skipHeader)
            {
                this.Colors = new ColorPalette(file);
                Helper.Log(this.Colors.ToString(), verbose);

                this.Ranges = new ColorRangeTable(file);
                Helper.Log(this.Ranges.ToString(), verbose);
            }
            else
            {
                throw new FormatException("Invalid PAL file!");
            }
        }

        /// <summary>
        /// Import a <see cref="PAL"/> file.
        /// </summary>
        /// <param name="filename"><see cref="PAL"/> file.</param>
        /// <param name="skipHeader">Skip read header. Use this when import a palette in a <see cref="MAP"/> or <see cref="FPG"/> instance. By default is <see cref="false"/>.</param>
        /// <param name="verbose">Log <see cref="PAL"/> import data to console. By default is <see cref="true"/>.</param>
        public PAL(string filename, bool skipHeader, bool verbose = true) : this(new BinaryReader(File.OpenRead(filename)), skipHeader, verbose)
        {
        }

        /// <summary>
        /// Import a <see cref="PCX"/> palette.
        /// </summary>
        /// <param name="pcx"><see cref="PCX"/> instance.</param>
        /// <param name="skipHeader">Skip initialize header. Use this when import a palette in a <see cref="MAP"/> or <see cref="FPG"/> instance. By default is <see cref="false"/>.</param>
        public PAL(PCX pcx, bool skipHeader = false)
        {
            if (!skipHeader)
            {
                this._header = new Header(); 
            }
            this.Colors = new ColorPalette(pcx);
            this.Ranges = new ColorRangeTable();
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Search a <see cref="Color"/> value in palette.
        /// </summary>
        /// <param name="color"><see cref="Color"/> value to search.</param>
        /// <param name="exact">Flag to indicate if the search value must be the same or to get a next similar color.</param>
        /// <returns>Returns the index of the <see cref="Color"/> value or -1 if not found the same color when <paramref name="exact"/> is <see cref="true"/>.</returns>
        public int FindColor(Color color, bool exact = true)
        {
            return this.Colors.Find(color, exact);
        }

        /// <summary>
        /// Write all data in a file.
        /// </summary>
        /// <param name="filename"><see cref="PAL"/> filename.</param>
        public void Write(string filename)
        {
            this._header ??= new Header();
            this.Write(new BinaryWriter(File.OpenWrite(filename)));
        }

        /// <summary>
        /// Write all data in a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="file"><see cref="BinaryWriter"/> instance.</param>
        public void Write(BinaryWriter file)
        {
            this._header?.Write(file);
            this.Colors.Write(file);
            this.Ranges.Write(file);
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
            return (a.Colors == b.Colors) && (ignoreColorRanges ? true : a.Ranges == b.Ranges);
        }

        public override bool Equals(object obj)
        {
            return this == (PAL)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this._header?.ToString()}\n{this.Colors.ToString()}\n{this.Ranges.ToString()}";
        }
        #endregion
    }
}
