namespace DIV2.Format.Exporter.Processors.Palettes
{
    class MapPaletteProcessor : IPaletteProcessor
    {
        #region Properties
        public static MapPaletteProcessor Instance { get; } = new MapPaletteProcessor();
        #endregion

        #region Methods & Functions
        public bool CheckFormat(byte[] buffer)
        {
            return MAP.ValidateFormat(buffer);
        }

        public PAL Process(byte[] buffer)
        {
            var map = new MAP(buffer);
            return map.Palette;
        } 
        #endregion
    }
}
