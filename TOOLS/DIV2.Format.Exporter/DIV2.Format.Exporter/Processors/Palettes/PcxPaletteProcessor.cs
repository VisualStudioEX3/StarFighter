using DIV2.Format.Exporter.ExtensionMethods;
using DIV2.Format.Exporter.Interfaces;
using DIV2.Format.Importer;
using System.IO;

namespace DIV2.Format.Exporter.Processors.Palettes
{
    class PcxPaletteProcessor : IPaletteProcessor
    {
        #region Properties
        public static PcxPaletteProcessor Instance => new PcxPaletteProcessor();
        #endregion

        #region Methods & Functions
        public bool Validate(byte[] buffer)
        {
            return PCX.Instance.Validate(buffer);
        }

        public PAL Process(byte[] buffer)
        {
            byte[] components = PCX.ExtractPalette(new BinaryReader(new MemoryStream(buffer)));
            return new PAL(components.ToColorArray().ToDAC());
        } 
        #endregion
    }
}
