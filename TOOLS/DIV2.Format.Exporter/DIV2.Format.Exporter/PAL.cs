using DIV2.Format.Exporter.Converters;
using DIV2.Format.Importer;
using System;
using System.IO;

namespace DIV2.Format.Exporter
{
    /// <summary>
    /// PAL importer.
    /// </summary>
    public class PAL : DIVFormatCommonBase
    {
        #region Constants
        public const int COLOR_TABLE_LENGTH = 768;
        public const int RANGE_TABLE_LENGHT = 576;
        #endregion

        #region Properties
        /// <summary>
        /// Palette colors in DAC format [0..63].
        /// </summary>
        public byte[] ColorTable { get; private set; }
        /// <summary>
        /// Color ranges table.
        /// </summary>
        public byte[] RangeTable { get; private set; }
        #endregion

        #region Operators
        public static bool operator ==(PAL a, PAL b)
        {
            return a.GetHashCode() == b.GetHashCode();
        }

        public static bool operator !=(PAL a, PAL b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructors
        internal PAL() : base("pal")
        {
            this.ColorTable = new byte[PAL.COLOR_TABLE_LENGTH];
            this.RangeTable = new byte[PAL.RANGE_TABLE_LENGHT];
        }

        /// <summary>
        /// Imports a <see cref="PAL"/> file data.
        /// </summary>
        /// <param name="filename"><see cref="PAL"/> filename.</param>
        public PAL(string filename) : this()
        {
            using (var file = new BinaryReader(File.OpenRead(filename)))
            {
                if (!this.Validate(file))
                {
                    throw new FormatException("Invalid PAL file.");
                }

                this.ColorTable = file.ReadBytes(PAL.COLOR_TABLE_LENGTH);
                this.RangeTable = file.ReadBytes(PAL.RANGE_TABLE_LENGHT);
            }
        }

        /// <summary>
        /// Creates new <see cref="PAL"/> instance from color array.
        /// </summary>
        /// <param name="colors"><see cref="byte"/> array of RGB colors.</param>
        /// <param name="convertToDAC">If the source is full RGB range [0..255], convert all values to DAC format [0..63]. By default is false.</param>
        public PAL(byte[] colors, bool convertToDAC = false) : this()
        {
            if (colors.Length != PAL.COLOR_TABLE_LENGTH)
            {
                throw new ArgumentException(nameof(colors), $"The array must be a {PAL.COLOR_TABLE_LENGTH} length (RGB components only).");
            }

            colors.CopyTo(this.ColorTable, 0);

            if (convertToDAC)
            {
                for (int i = 0; i < PAL.COLOR_TABLE_LENGTH; i++)
                {
                    this.ColorTable[i] = (byte)(this.ColorTable[i] > 0 ? this.ColorTable[i] / 4 : 0);
                }
            }

            // Create default range table:
            using (var stream = new BinaryWriter(new MemoryStream(this.RangeTable)))
            {
                int range = 0;
                for (int i = 0; i < 16; i++)
                {
                    stream.Write((byte)16);
                    stream.Write((byte)0);
                    stream.Write(false);
                    stream.Write((byte)0);
                    for (int j = 0; j < 32; j++)
                    {
                        stream.Write((byte)range);
                        if (++range > byte.MaxValue)
                        {
                            range = 0;
                        }
                    }
                }
            }
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Create a <see cref="PAL"/> instance from PNG image.
        /// </summary>
        /// <param name="filename">Supported image file used to create the palette.</param>
        /// <returns>Returns a new <see cref="PAL"/> instance with the supported image file, converted to 256 color PNG format, color palette data.</returns>
        /// <remarks>Supported formats are Jpeg, Png, Bmp, Gif and Tga.</remarks>
        public static PAL FromImage(string filename)
        {
            byte[] file = File.ReadAllBytes(filename);
            MemoryStream stream = PNG2PAL.ConvertToIndexedPNG(file);
            return PNG2PAL.CreatePalette(stream);
        }

        /// <summary>
        /// Create a <see cref="PAL"/> instance from PCX image.
        /// </summary>
        /// <param name="filename">PCX file used to create the palette.</param>
        /// <returns>Returns a new <see cref="PAL"/> instance with the PCX color palette data.</returns>
        /// <remarks>Only supported 256 colors PCX images.</remarks>
        public static PAL FromPCX(string filename)
        {
            return PCX.CreatePalette(new BinaryReader(new MemoryStream(File.ReadAllBytes(filename))));
        }

        /// <summary>
        /// Create a <see cref="PAL"/> instance from <see cref="MAP"/> image.
        /// </summary>
        /// <param name="filename"><see cref="MAP"/> file used to create the palette.</param>
        /// <returns>Returns a new <see cref="PAL"/> instance with the <see cref="MAP"/> color palette data.</returns>
        public static PAL FromMAP(string filename)
        {
            return PAL.FromDIVFile(filename, MAP.HEADER_LENGTH);
        }

        /// <summary>
        /// Create a <see cref="PAL"/> instance from <see cref="FPG"/> image.
        /// </summary>
        /// <param name="filename"><see cref="FPG"/> file used to create the palette.</param>
        /// <returns>Returns a new <see cref="PAL"/> instance with the <see cref="FPG"/> color palette data.</returns>
        public static PAL FromFPG(string filename)
        {
            return PAL.FromDIVFile(filename, DIVFormatCommonBase.BASE_HEADER_LENGTH);
        }

        static PAL FromDIVFile(string filename, int position)
        {
            using (var file = new BinaryReader(new MemoryStream(File.ReadAllBytes(filename))))
            {
                file.BaseStream.Position = position;

                return new PAL()
                {
                    ColorTable = file.ReadBytes(PAL.COLOR_TABLE_LENGTH),
                    RangeTable = file.ReadBytes(PAL.RANGE_TABLE_LENGHT)
                };
            }
        }

        /// <summary>
        /// Converts all color values from DAC format [0..63] to full RGB format [0..255].
        /// </summary>
        /// <returns>Returns new array with the RGB values.</returns>
        public byte[] DAC2RGB()
        {
            var rgb = new byte[PAL.COLOR_TABLE_LENGTH];

            for (int i = 0; i < PAL.COLOR_TABLE_LENGTH; i++)
            {
                rgb[i] = (byte)(this.ColorTable[i] * 4);
            }

            return rgb;
        }

        /// <summary>
        /// Writes all palette data to the file stream.
        /// </summary>
        /// <param name="stream">File to write.</param>
        internal override void Write(BinaryWriter stream)
        {
            base.Write(stream);
            stream.Write(this.ColorTable);
            stream.Write(this.RangeTable);
        }

        /// <summary>
        /// Writes all palette data to the file stream skiping the header data.
        /// </summary>
        /// <param name="stream">File to write.</param>
        internal void WriteEmbebed(BinaryWriter stream)
        {
            stream.Write(this.ColorTable);
            stream.Write(this.RangeTable);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PAL)) return false;

            return this == (PAL)obj;
        }

        public override int GetHashCode()
        {
            return BitConverter.ToString(this.ColorTable).GetHashCode();
        }
        #endregion
    }
}
