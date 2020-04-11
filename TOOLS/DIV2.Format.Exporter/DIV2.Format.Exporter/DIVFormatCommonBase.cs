using System.IO;
using DIV2.Format.Exporter.MethodExtensions;

namespace DIV2.Format.Exporter
{
    static class DIVFormatCommonBase
    {
        public static void WriteCommonHeader(BinaryWriter file, string id)
        {
            file.Write(id.ToByteArray());
            file.Write(new byte[] { 0x1A, 0x0D, 0x0A, 0x00 });
            file.Write((byte)0);
        }

        public static void WritePalette(BinaryWriter file, byte[] palette)
        {
            file.Write(palette);

            // Write default palette color ranges:
            int range = 0;
            for (int i = 0; i < 16; i++)
            {
                file.Write((byte)16);
                file.Write((byte)0);
                file.Write(false);
                file.Write((byte)0);
                for (int j = 0; j < 32; j++)
                {
                    file.Write((byte)range);
                    if (++range > 255)
                    {
                        range = 0;
                    }
                }
            }
        }
    }
}
