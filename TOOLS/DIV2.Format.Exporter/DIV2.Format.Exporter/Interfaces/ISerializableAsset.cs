using System.IO;

namespace DIV2.Format.Exporter.Interfaces
{
    public interface ISerializableAsset
    {
        #region Methods & Functions
        byte[] Serialize();
        void Write(BinaryWriter stream);
        #endregion
    }
}
