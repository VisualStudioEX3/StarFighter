using System;
using System.IO;
using DIV2.Format.Exporter.MethodExtensions;

namespace DIV2.Format.Exporter
{
    /// <summary>
    /// PAL importer.
    /// </summary>
    public class PAL
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
        /// <summary>
        /// Imports a <see cref="PAL"/> file data.
        /// </summary>
        /// <param name="filename"><see cref="PAL"/> filename.</param>
        public PAL(string filename)
        {
            using (var file = new BinaryReader(File.OpenRead(filename)))
            {
                if (!(file.ReadBytes(3).ToASCIIString() == "pal" && 
                      BitConverter.ToInt32(DIVFormatCommonBase.magicNumber, 0) == BitConverter.ToInt32(file.ReadBytes(4), 0) && 
                      file.ReadByte() == 0))
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
        public PAL(byte[] colors)
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
        /// Writes all palette data to the file stream, except the file header.
        /// </summary>
        /// <param name="stream">File to write.</param>
        public void Write(BinaryWriter stream)
        {
            stream.Write(this.ColorTable);
            stream.Write(this.RangeTable);
        }
        #endregion
    }
}
