using System;
using System.IO;
using DIV2.Format.Exporter.MethodExtensions;

namespace DIV2.Format.Exporter
{
    /// <summary>
    /// PAL importer.
    /// </summary>
    class PAL
    {
        #region Constants
        public const int COLOR_TABLE_LENGTH = 768;
        public const int RANGE_TABLE_LENGHT = 576;
        #endregion

        #region Properties
        public byte[] ColorTable { get; private set; }
        public byte[] RangeTable { get; private set; }
        #endregion

        #region Constructors
        public PAL(string filename, bool DAC = true)
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

        public PAL(byte[] colors)
        {
            if (colors.Length != PAL.COLOR_TABLE_LENGTH)
            {
                throw new ArgumentException(nameof(colors), $"The array must be a {PAL.COLOR_TABLE_LENGTH} length.");
            }

            this.ColorTable = colors;
            this.RangeTable = new byte[PAL.RANGE_TABLE_LENGHT];

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
        public void Write(BinaryWriter stream)
        {
            stream.Write(this.ColorTable);
            stream.Write(this.RangeTable);
        }
        #endregion
    }
}
