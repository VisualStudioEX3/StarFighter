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
        public byte[] ColorTable { get; }
        public byte[] RangeTable { get; }
        #endregion

        #region Constructors
        internal PAL() : base("pal")
        {
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
        /// <param name="colors"><see cref="byte"/> array of RGB colors in DAC format (value ranges 0-63).</param>
        /// <remarks>Warning: Import array color with full RGB range (0-255) may cause bad behaviour in DIV Games Studio.
        /// Hint: If you need to adapt full RGB value to DAC format, divide by 4 each color component.</remarks>
        public PAL(byte[] colors) : this()
        {
            if (colors.Length != PAL.COLOR_TABLE_LENGTH)
            {
                throw new ArgumentException(nameof(colors), $"The array must be a {PAL.COLOR_TABLE_LENGTH} length (RGB components only).");
            }

            this.ColorTable = colors;
            this.RangeTable = new byte[PAL.RANGE_TABLE_LENGHT];

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
                        if (++range > 255)
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
        /// Validates if the file is a valid <see cref="PAL"/> file.
        /// </summary>
        /// <param name="filename"><see cref="PAL"/> filename.</param>
        /// <returns>Returns true if the file contains a valid <see cref="PAL"/> header format.</returns>
        public static bool Validate(string filename)
        {
            using (var file = new BinaryReader(File.OpenRead(filename)))
            {
                return new PAL().Validate(file);
            }
        }

        /// <summary>
        /// Converts all color values from DAC format to full RGB format.
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
        /// Writes all palette data to the file stream, except the file header.
        /// </summary>
        /// <param name="stream">File to write.</param>
        public override void Write(BinaryWriter stream)
        {
            stream.Write(this.ColorTable);
            stream.Write(this.RangeTable);
        }
        #endregion
    }
}
