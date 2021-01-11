namespace DIV2.Format.Exporter.Interfaces
{
    public interface IAssetFile : IFormatValidable, ISerializableAsset
    {
        bool Validate(string filename);
        void Save(string filename);
    }
}
