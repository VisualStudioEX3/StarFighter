namespace DIV2.Format.Exporter.Processors.Palettes
{
    class FpgPaletteProcessor : IPaletteProcessor
    {
        #region Properties
        public static FpgPaletteProcessor Instance { get; } = new FpgPaletteProcessor();
        #endregion

        #region Methods & Functions
        public bool CheckFormat(byte[] buffer)
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
