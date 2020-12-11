using System.IO;

namespace DIV2.Format.Exporter
{
    public interface ISerializableAsset
    {
        #region Methods & Functions
        byte[] Serialize();
        void Write(BinaryWriter stream);
        #endregion
    }
}
