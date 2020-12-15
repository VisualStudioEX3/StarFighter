namespace DIV2.Format.Exporter.Processors.Palettes
{
    interface IPaletteProcessor : IFormatValidable
    {
        #region Methods & Functions
        PAL Process(byte[] buffer); 
        #endregion
    }
}
