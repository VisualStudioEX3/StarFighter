namespace DIV2.Format.Exporter
{
    public interface IAssetFile : IFormatValidable, ISerializableAsset
    {
        bool Validate(string filename);
        void Save(string filename);
    }
}
