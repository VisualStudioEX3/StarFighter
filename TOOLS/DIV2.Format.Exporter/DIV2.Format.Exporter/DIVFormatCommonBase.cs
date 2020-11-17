using DIV2.Format.Exporter.MethodExtensions;
using System.IO;

namespace DIV2.Format.Exporter
{
    public abstract class DIVFormatCommonBase
    {
        #region Constants
        static readonly byte[] MAGIC_NUMBER = { 0x1A, 0x0D, 0x0A, 0x00 };
        const int ID_LENGTH = 3;
        const byte VERSION = 0;

        internal const int BASE_HEADER_LENGTH = 8;
        #endregion

        #region Internal vars
        internal readonly string _id;
        #endregion

        #region Constructor
        internal DIVFormatCommonBase(string id)
        {
            this._id = id;
        } 
        #endregion

        #region Methods & Functions
        void WriteHeader(BinaryWriter file)
        {
            file.Write(this.GetType().Name.ToLower().ToByteArray());
            file.Write(DIVFormatCommonBase.MAGIC_NUMBER);
            file.Write(DIVFormatCommonBase.VERSION);
        }

        protected bool CheckHeader(BinaryReader file)
        {
            bool id = file.ReadBytes(DIVFormatCommonBase.ID_LENGTH).ToASCIIString() == this._id;
            bool magicNumber = file.CompareSignatures(DIVFormatCommonBase.MAGIC_NUMBER);
            bool version = file.ReadByte() == DIVFormatCommonBase.VERSION;

            return id && magicNumber && version;
        }

        /// <summary>
        /// Check the file format header.
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> instance of the file to check.</param>
        /// <returns>Returns true if the file header is valid.</returns>
        public bool CheckHeader(Stream stream)
        {
            return this.CheckHeader(new BinaryReader(stream));
        }

        /// <summary>
        /// Check the file format header.
        /// </summary>
        /// <param name="stream"><see cref="byte"/> array data of the file to check.</param>
        /// <returns>Returns true if the file header is valid.</returns>
        public bool CheckHeader(byte[] buffer)
        {
            return this.CheckHeader(new MemoryStream(buffer));
        }

        /// <summary>
        /// Check the file format header.
        /// </summary>
        /// <param name="stream">Filename of the file to check.</param>
        /// <returns>Returns true if the file header is valid.</returns>
        public bool CheckHeader(string filename)
        {
            return this.CheckHeader(File.OpenRead(filename));
        }

        internal virtual void Write(BinaryWriter file)
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
