using System.IO;
using DIV2.Format.Exporter.MethodExtensions;

namespace DIV2.Format.Exporter
{
    static class DIVFormatCommonBase
    {
        #region Public vars
        public static readonly byte[] magicNumber = new byte[] { 0x1A, 0x0D, 0x0A, 0x00 };
        #endregion

        #region Methods & Functions
        public static void WriteCommonHeader(BinaryWriter file, string id)
        {
            file.Write(id.ToByteArray());
            file.Write(DIVFormatCommonBase.magicNumber);
            file.Write((byte)0);
        }
        #endregion
    }
}
