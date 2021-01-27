using DIV2.Format.Exporter.Interfaces;

namespace DIV2.Format.Exporter.Processors.Palettes
{
    class FpgPaletteProcessor : IPaletteProcessor
    {
        #region Properties
        public static FpgPaletteProcessor Instance => new FpgPaletteProcessor();
        #endregion

        #region Methods & Functions
        public bool Validate(byte[] buffer)
        {
            return FPG.ValidateFormat(buffer);
        }

        public PAL Process(byte[] buffer)
        {
            var fpg = new FPG(buffer);
            return fpg.Palette;
        } 
        #endregion
    }
}
