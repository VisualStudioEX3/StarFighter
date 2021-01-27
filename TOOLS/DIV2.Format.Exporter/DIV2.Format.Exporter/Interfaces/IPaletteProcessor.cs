namespace DIV2.Format.Exporter.Interfaces
{
    interface IPaletteProcessor : IFormatValidable
    {
        #region Methods & Functions
        PAL Process(byte[] buffer); 
        #endregion
    }
}
