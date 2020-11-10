namespace DIV2.Format.Exporter.Processors.Palettes
{
    interface IPaletteProcessor
    {
        #region Methods & Functions
        bool CheckFormat(byte[] buffer);
        PAL Process(byte[] buffer); 
        #endregion
    }
}
