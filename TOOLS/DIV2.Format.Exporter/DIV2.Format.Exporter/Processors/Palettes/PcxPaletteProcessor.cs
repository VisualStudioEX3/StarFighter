using DIV2.Format.Importer;
using System.IO;

namespace DIV2.Format.Exporter.Processors.Palettes
{
    class PcxPaletteProcessor : IPaletteProcessor
    {
        #region Properties
        public static PcxPaletteProcessor Instance { get; } = new PcxPaletteProcessor();
        #endregion

        #region Methods & Functions
        public bool CheckFormat(byte[] buffer)
        {
            return PCX.IsPCX256(buffer);
        }

        public PAL Process(byte[] buffer)
        {
            return PCX.CreatePalette(new BinaryReader(new MemoryStream(buffer)));
        } 
        #endregion
    }
}
