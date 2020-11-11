using DIV2.Format.Exporter.Processors.Palettes;
using System;
using System.IO;

namespace DIV2.Format.Exporter
{
    /// <summary>
    /// PAL importer & exporter.
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

        internal PAL(byte[] colors, byte[] rangeTable) : this()
        {
            this.ColorTable = colors;
            this.RangeTable = rangeTable;
        }

        /// <summary>
        /// Imports a <see cref="PAL"/> file data.
        /// </summary>
        /// <param name="filename"><see cref="PAL"/> filename.</param>
        public PAL(string filename) : this(File.ReadAllBytes(filename))
        {
        }

        /// <summary>
        /// Imports a <see cref="PAL"/> file data.
        /// </summary>
        /// <param name="buffer"><see cref="PAL"/> file data.</param>
        public PAL(byte[] buffer) : this()
        {
            using (var file = new BinaryReader(new MemoryStream(buffer)))
            {
                if (!this.Validate(file))
                {
                    throw new FormatException("Invalid PAL file.");
                }

                this.ColorTable = file.ReadBytes(PAL.COLOR_TABLE_LENGTH);
                this.RangeTable = file.ReadBytes(PAL.RANGE_TABLE_LENGHT);
            }
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Creates new <see cref="PAL"/> instance from color array.
        /// </summary>
        /// <param name="colors"><see cref="byte"/> array of RGB colors.</param>
        /// <param name="convertToDAC">If the source is full RGB range [0..255], convert all values to DAC format [0..63].</param>
        public static PAL CreatePalette(byte[] colors, bool convertToDAC)
        {
            if (colors.Length != PAL.COLOR_TABLE_LENGTH)
            {
                throw new ArgumentException(nameof(colors), $"The array must be a {PAL.COLOR_TABLE_LENGTH} length (RGB components only).");
            }

            var palette = new PAL();

            colors.CopyTo(palette.ColorTable, 0);

            if (convertToDAC)
            {
                for (int i = 0; i < PAL.COLOR_TABLE_LENGTH; i++)
                {
                    palette.ColorTable[i] = (byte)(palette.ColorTable[i] > 0 ? palette.ColorTable[i] / 4 : 0);
                }
            }

            // Create default range table:
            using (var stream = new BinaryWriter(new MemoryStream(palette.RangeTable)))
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

            return palette;
        }

        /// <summary>
        /// Create a <see cref="PAL"/> instance from PNG image.
        /// </summary>
        /// <param name="filename">Supported image file used to create the palette.</param>
        /// <returns>Returns a new <see cref="PAL"/> instance with the supported image file, converted to 256 color PNG format, color palette data.</returns>
        /// <remarks>Supported formats are JPEG, PNG, BMP, GIF and TGA. Also supported 256 color PCX images and <see cref="MAP"/> files.</remarks>
        public static PAL FromImage(string filename)
        {
            return PAL.FromImage(File.ReadAllBytes(filename));
        }

        /// <summary>
        /// Create a <see cref="PAL"/> instance from PNG image.
        /// </summary>
        /// <param name="buffer">Supported image file data used to create the palette.</param>
        /// <returns>Returns a new <see cref="PAL"/> instance with the supported image file, converted to 256 color PNG format, color palette data.</returns>
        /// <remarks>Supported formats are JPEG, PNG, BMP, GIF and TGA. Also supported 256 color PCX images and <see cref="MAP"/> files.</remarks>
        public static PAL FromImage(byte[] buffer)
        {
            return PaletteProcessor.ProcessPalette(buffer);
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
