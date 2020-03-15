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
        #region Constants
        const string HEADER_ID = "pal";
        static readonly byte[] HEADER_SIGNATURE = { 0x1A, 0x0D, 0x0A, 0x00 };
        const byte HEADER_VERSION = 0;
        #endregion

        #region Structs
        /// <summary>
        /// Header description.
        /// </summary>
        struct Header // 8 bytes.
        {
            #region Public vars
            public char[] id;           // 3 bytes.
            public byte[] signature;    // 4 bytes.
            public byte version;
            #endregion

            #region Constructor
            public Header(BinaryReader file)
            {
                this.id = file.ReadChars(3);
                this.signature = file.ReadBytes(4);
                this.version = file.ReadByte();
            }
            #endregion

            #region Methods & Functions
            public bool Check()
            {
                return new string(this.id).Equals(PAL.HEADER_ID) &&
                       BitConverter.ToUInt32(this.signature) == BitConverter.ToUInt32(PAL.HEADER_SIGNATURE) &&
                       this.version == PAL.HEADER_VERSION;
            }

            public override string ToString()
            {
                return $"PAL Header:\n- Id: {new string(this.id)}\n- Signature: {BitConverter.ToString(this.signature)}\n- Version: {this.version}\n";
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
        public class ColorPallete
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

            #region Constructor
            /// <summary>
            /// Read all colors from a pallete.
            /// </summary>
            /// <param name="file"><see cref="BinaryReader"/> instance of the PAL or compatible file format that contain pallete data.</param>
            /// <remarks>The <see cref="BinaryReader"/> instance must be setup in the first byte of the pallete array in the opened file.</remarks>
            public ColorPallete(BinaryReader file)
            {
                this._colors = new Color[ColorPallete.LENGTH];
                for (int i = 0; i < ColorPallete.LENGTH; i++)
                {
                    this._colors[i] = new Color(file);
                }
            }
            #endregion

            #region Methods & Functions
            public ColorPallete()
            {
                this._colors = new Color[ColorPallete.LENGTH];
            }

            /// <summary>
            /// Read all colors from a 256 colors PCX image.
            /// </summary>
            /// <param name="file"><see cref="byte"/> array with the content of a PCX file.</param>
            /// <returns>Returns a new instance of <see cref="ColorPallete"/> with all 256 colors.</returns>
            /// <remarks>The PCX must be a 256 color indexed format.</remarks>
            public static ColorPallete ReadPalleteFromPCXFile(byte[] file)
            {
                const int PAL_LENGTH = 768;
                const byte PAL_MARKER = 0x0C;

                int index = (file.Length - PAL_LENGTH) - 1;
                var colors = new ColorPallete();

                // Check if PCX is 256 color indexed:
                if (file[3] != 8 && file[index] != PAL_MARKER)
                {
                    throw new FormatException("The PCX file readed not is a 256 color indexed PCX image and not contain a 8 bit color pallete at the end of file to read.");
                }

                for (int i = 0; i < ColorPallete.LENGTH; i++)
                {
                    colors[i] = new Color(file[++index], file[++index], file[++index]);
                }

                return colors;
            }

            public void Write(BinaryWriter file)
            {
                foreach (var color in this._colors)
                {
                    color.Write(file);
                }
            }

            public int[] ToInt32Array()
            {
                var values = new int[ColorPallete.LENGTH];

                for (int i = 0; i < ColorPallete.LENGTH; i++)
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
                for (int i = 0; i < ColorPallete.LENGTH; i++)
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
            #region Public vars
            public byte colors;
            public byte type;
            public bool isFixed;
            public byte blackColor;
            public byte[] colorRanges; // 32 bytes.
            #endregion

            #region Constructor
            public ColorRange(BinaryReader file)
            {
                this.colors = file.ReadByte();
                this.type = file.ReadByte();
                this.isFixed = file.ReadBoolean();
                this.blackColor = file.ReadByte();
                this.colorRanges = file.ReadBytes(32);
            }
            #endregion

            #region Method & Functions
            public void Write(BinaryWriter file)
            {
                file.Write(this.colors);
                file.Write(this.type);
                file.Write(this.isFixed);
                file.Write(this.blackColor);
                file.Write(this.colorRanges);
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"- Colors: {this.colors}\n- Type: {this.type}\n- Fixed: {this.isFixed}\n- Black color: {this.blackColor}\n");

                sb.Append("- Color Ranges:\n");
                int column = 0;
                foreach (byte color in this.colorRanges)
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

            #region Constructor
            /// <summary>
            /// Read the 16 color ranges associated to a color pallete.
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
        ColorPallete _pallete;
        ColorRangeTable _colorRanges;
        #endregion

        #region Properties
        public ColorPallete Pallete => this._pallete;
        public ColorRangeTable Ranges => this._colorRanges;
        #endregion

        #region Constructor
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
                    this._pallete = new ColorPallete(file);
                    Helper.Log(this._pallete.ToString(), verbose);

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
        /// Convert pixel colors from source pallete to destiny pallete.
        /// </summary>
        /// <param name="pixels"><see cref="byte"/> array that contain the pixels to adapt to DIV PAL.</param>
        /// <param name="sourcePal">Original pixels pallete.</param>
        /// <param name="destPal">Pallete model to adapt pixels.</param>
        /// <returns>Returns a new <see cref="byte"/> array with adapted pixels.</returns>
        public static byte[] Convert(byte[] pixels, ColorPallete sourcePal, ColorPallete destPal)
        {
            var newPixels = new byte[pixels.Length];
            var sourceColors = sourcePal.ToInt32Array();
            var destColors = destPal.ToInt32Array();

            for (int i = 0; i < pixels.Length; i++)
            {
                newPixels[i] = PAL.GetNearColor(sourceColors[pixels[i]], destColors);
            }

            return newPixels;
        }

        static byte GetNearColor(int color, int[] pal)
        {
            int lastDiff = 0;
            int index = 0;

            for (int i = 0; i < 256; i++)
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
        #endregion
    }
}
