using DIV2.Format.Exporter.MethodExtensions;
using System;
using System.IO;

namespace DIV2.Format.Exporter
{
    public abstract class DIVFormatCommonBase
    {
        #region Constants
        static readonly byte[] MAGIC_NUMBER = { 0x1A, 0x0D, 0x0A, 0x00 };

        internal const int BASE_HEADER_LENGTH = 8;
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
            file.Write(this.GetType().Name.ToLower().ToByteArray());
            file.Write(DIVFormatCommonBase.MAGIC_NUMBER);
            file.Write((byte)0);
        }

        /// <summary>
        /// Validates if the file is a valid file.
        /// </summary>
        /// <param name="file"><see cref="BinaryReader"/> instance, that contains the file data to validate format.</param>
        /// <returns>Returns true if the file contains a valid header format.</returns>
        public bool Validate(BinaryReader file)
        {
            return file.ReadBytes(3).ToASCIIString() == this._id &&
                   file.CompareSignatures(DIVFormatCommonBase.MAGIC_NUMBER) &&
                   file.ReadByte() == 0;
        }

        /// <summary>
        /// Validates if the file is a valid file.
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> instance, that contains the file data to validate format.</param>
        /// <returns>Returns true if the file contains a valid header format.</returns>
        public bool Validate(Stream stream)
        {
            return this.Validate(new BinaryReader(stream));
        }

        /// <summary>
        /// Validates if the file is a valid file.
        /// </summary>
        /// <param name="filename">File to validate format.</param>
        /// <returns>Returns true if the file contains a valid header format.</returns>
        public bool Validate(string filename)
        {
            return this.Validate(File.OpenRead(filename));
        }

        /// <summary>
        /// Validates if the file is a valid file.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array, that contains the file data to validate format.</param>
        /// <returns>Returns true if the file contains a valid header format.</returns>
        public bool Validate(byte[] buffer)
        {
            return this.Validate(new MemoryStream(buffer));
        }

        /// <summary>
        /// Writes the data to file.
        /// </summary>
        /// <param name="file"><see cref="BinaryWriter"/> instance.</param>
        /// <remarks>Call base implementation to write the common file header data.</remarks>
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
