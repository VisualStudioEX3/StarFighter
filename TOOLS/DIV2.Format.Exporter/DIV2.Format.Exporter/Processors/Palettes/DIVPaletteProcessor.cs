namespace DIV2.Format.Exporter.Processors.Palettes
{
    class DIVPaletteProcessor : IPaletteProcessor
    {
        #region Properties
        public static DIVPaletteProcessor Instance { get; } = new DIVPaletteProcessor();
        #endregion

        #region Methods & Functions
        public bool CheckFormat(byte[] buffer)
        {
            return MAP.ValidateFormat(buffer) || FPG.ValidateFormat(buffer);
        }

        public PAL Process(byte[] buffer)
        {
            try
            {
                return new MAP(buffer).Palette;
            }
            catch
            {
                return new FPG(buffer).Palette;
            }
        } 
        #endregion
    }
}
