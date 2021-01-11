namespace DIV2.Format.Exporter.Interfaces
{
    public interface IFormatValidable
    {
        #region Methods & Functions
        bool Validate(byte[] buffer);
        #endregion
    }
}
