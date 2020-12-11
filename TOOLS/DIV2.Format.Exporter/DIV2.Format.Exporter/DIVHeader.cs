﻿using DIV2.Format.Exporter.MethodExtensions;
using System;
using System.IO;
using System.Text;

namespace DIV2.Format.Exporter
{
    sealed class DIVHeader : ISerializableAsset, IFormatValidable
    {
        #region Constants
        const int MAGIC_NUMBER = 658714; // 0x1A, 0x0D, 0x0A, 0x00
        const byte VERSION = 0; // Version never changes from DIV Games Studio 1 to DIV Games Studio 2.

        public const int SIZE = (sizeof(byte) * 3) + sizeof(int) + sizeof(byte);
        #endregion

        #region Internal vars
        byte[] _id;
        int _magicNumber;
        byte _version;
        #endregion

        #region Constructors
        public DIVHeader(char x, char y, char z)
        {
            this._id = new char[] { x, y, z }.ToByteArray();
            this._magicNumber = MAGIC_NUMBER;
            this._version = VERSION;
        }

        public DIVHeader(byte[] buffer)
        {
            if (buffer.Length == SIZE)
                throw new ArgumentOutOfRangeException($"Error reading the {nameof(DIVHeader)}. The buffer length must be over {SIZE} bytes.");

            this._id = buffer[0..2];
            this._magicNumber = BitConverter.ToInt32(buffer[3..6]);
            this._version = buffer[7];
        }
        #endregion

        #region Methods & Functions
        public bool Validate(byte[] buffer)
        {
            if (buffer.Length == SIZE)
            {
                var header = new DIVHeader(buffer);

                bool id = header._id.ToASCIIString().Equals(this._id.ToASCIIString());
                bool magicNumber = (header._magicNumber == MAGIC_NUMBER);
                bool version = (header._version == VERSION);

                return id && magicNumber && version; 
            }

            return false;
        }

        public byte[] Serialize()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                stream.Write(this._id);
                stream.Write(MAGIC_NUMBER);
                stream.Write(VERSION);

                return (stream.BaseStream as MemoryStream).GetBuffer();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.BaseStream.Position = 0;
            stream.Write(this.Serialize());
        }
        #endregion
    }
}