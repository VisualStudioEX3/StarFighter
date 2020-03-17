using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DIV2Tools
{
    /// <summary>
    /// Base header definition of all DIV file formats.
    /// </summary>
    public abstract class DIVFormatBaseHeader
    {
        #region Constants
        static readonly byte[] HEADER_SIGNATURE = { 0x1A, 0x0D, 0x0A, 0x00 };
        const byte HEADER_VERSION = 0;

        const string ID_LENGTH_EXCEPTION_MESSAGE = "The value must be a 3 characters length.";
        #endregion

        #region Internal vars
        string _id; 
        #endregion

        #region Properties
        public char[] Id { get; private set; }
        public byte[] Signature { get; private set; }
        public byte Version { get; private set; } 
        #endregion

        #region Constructors
        public DIVFormatBaseHeader(string id)
        {
            if (!this.SetId(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id), DIVFormatBaseHeader.ID_LENGTH_EXCEPTION_MESSAGE);
            }

            this.Id = this._id.ToCharArray();
            this.Signature = DIVFormatBaseHeader.HEADER_SIGNATURE;
            this.Version = DIVFormatBaseHeader.HEADER_VERSION;
        }

        public DIVFormatBaseHeader(string id, BinaryReader file)
        {
            if (!this.SetId(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id), DIVFormatBaseHeader.ID_LENGTH_EXCEPTION_MESSAGE);
            }

            this.Id = file.ReadChars(3);
            this.Signature = file.ReadBytes(4);
            this.Version = file.ReadByte();
        } 
        #endregion

        #region Methods & Functions
        bool SetId(string id)
        {
            this._id = id.ToLower();
            return (id.Length == 3);
        }

        public bool Check()
        {
            return new string(this.Id).Equals(this._id) &&
                   BitConverter.ToUInt32(this.Signature) == BitConverter.ToUInt32(DIVFormatBaseHeader.HEADER_SIGNATURE) &&
                   this.Version == DIVFormatBaseHeader.HEADER_VERSION;
        }

        public virtual void Write(BinaryWriter file)
        {
            file.Write(Encoding.ASCII.GetBytes(this.Id));
            file.Write(DIVFormatBaseHeader.HEADER_SIGNATURE);
            file.Write(DIVFormatBaseHeader.HEADER_VERSION);
        }

        public new virtual string ToString()
        {
            return $"Header:\n- Id: {new string(this.Id)}\n- Signature: {BitConverter.ToString(this.Signature)}\n- Version: {this.Version}";
        }
        #endregion
    }
}
