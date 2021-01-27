namespace DIV2.Format.Exporter.Interfaces
{
    public interface IAssetFile : IFormatValidable, ISerializableAsset
    {
        #region Methods & Functions
        bool Validate(string filename);
        void Save(string filename); 
        #endregion
    }
}
