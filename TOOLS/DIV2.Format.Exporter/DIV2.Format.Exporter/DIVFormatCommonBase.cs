using System;
using System.IO;
using DIV2.Format.Exporter.MethodExtensions;

namespace DIV2.Format.Exporter
{
    public abstract class DIVFormatCommonBase
    {
        #region Constants
        static readonly byte[] MAGIC_NUMBER = new byte[] { 0x1A, 0x0D, 0x0A, 0x00 };
        #endregion

        #region Internal vars
        internal readonly string _id;
        #endregion

        internal DIVFormatCommonBase(string id)
        {
            this._id = id;
        }

        #region Methods & Functions
        void WriteHeader(BinaryWriter file)
        {
            file.Write(this._id.ToByteArray());
            file.Write(DIVFormatCommonBase.MAGIC_NUMBER);
            file.Write((byte)0);
        }

        internal bool Validate(BinaryReader file)
        {
            return file.ReadBytes(3).ToASCIIString() == this._id &&
                   BitConverter.ToInt32(DIVFormatCommonBase.MAGIC_NUMBER, 0) == BitConverter.ToInt32(file.ReadBytes(4), 0) &&
                   file.ReadByte() == 0;
        }

        /// <summary>
        /// Writes the data to file.
        /// </summary>
        /// <param name="file"><see cref="BinaryWriter"/> instance.</param>
        /// <remarks>Call base implementation to write the header data.</remarks>
        public virtual void Write(BinaryWriter file)
        {
            this.WriteHeader(file);
        }

        /// <summary>
        /// Save all data to file.
        /// </summary>
        /// <param name="filename">Filename to the new file to write all data.</param>
        /// <remarks>Overrides <see cref="Write(BinaryWriter)"/> method to implements the data to write in file.</remarks>
        public void Save(string filename)
        {
            using (var file = new BinaryWriter(File.OpenWrite(filename)))
            {
                this.Write(file);
            }
        }
        #endregion
    }
}
