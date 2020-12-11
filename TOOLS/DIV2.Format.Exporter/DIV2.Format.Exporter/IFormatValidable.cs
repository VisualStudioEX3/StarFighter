namespace DIV2.Format.Exporter
{
    public interface IFormatValidable
    {
        #region Methods & Functions
        bool Validate(byte[] buffer);
        #endregion
    }
}
