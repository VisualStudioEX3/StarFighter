using DIV2.Format.Exporter.MethodExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DIV2.Format.Exporter
{
    /// <summary>
    /// Color range values.
    /// </summary>
    public sealed class ColorRange : ISerializableAsset, IEnumerable<byte>
    {
        #region Constants
        readonly static IndexOutOfRangeException INDEX_OUT_OF_RANGE_EXCEPTION = new IndexOutOfRangeException($"The index value must be a value beteween 0 and {LENGTH}.");

        /// <summary>
        /// Number of color index entries in the range.
        /// </summary>
        public const int LENGTH = 32;
        /// <summary>
        /// Memory size of the range.
        /// </summary>
        public const int SIZE = (sizeof(byte) * 4) + (sizeof(byte) * LENGTH);
        #endregion

        #region Enumerations
        /// <summary>
        /// Available ammount of colors for the range.
        /// </summary>
        public enum ColorAmmount : byte
        {
            _8 = 8,
            _16 = 16,
            _32 = 32
        }

        /// <summary>
        /// Available range types.
        /// </summary>
        public enum RangeTypes : byte
        {
            _0 = 0,
            _1 = 1,
            _2 = 2,
            _4 = 4,
            _8 = 8
        }
        #endregion

        #region Public vars
        byte[] _rangeColors;

        /// <summary>
        /// Ammount of colors for the range.
        /// </summary>
        public ColorAmmount colors;
        /// <summary>
        /// Range type.
        /// </summary>
        public RangeTypes type;
        /// <summary>
        /// Is a fixed range?
        /// </summary>
        public bool isfixed;
        /// <summary>
        /// Index of the black color. Defaults is zero.
        /// </summary>
        public byte blackColor;
        #endregion

        #region Properties
        /// <summary>
        /// Get or set the range entry value.
        /// </summary>
        /// <param name="index">Index of the entry.</param>
        /// <returns>Returns the range entry value.</returns>
        public byte this[int index]
        {
            get
            {
                if (!index.IsClamped(0, LENGTH))
                    throw INDEX_OUT_OF_RANGE_EXCEPTION;

                return this._rangeColors[index];
            }
            set
            {
                if (!index.IsClamped(0, LENGTH))
                    throw INDEX_OUT_OF_RANGE_EXCEPTION;

                this._rangeColors[index] = value;
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(ColorRange a, ColorRange b)
        {
            return a.GetHashCode() == b.GetHashCode();
        }

        public static bool operator !=(ColorRange a, ColorRange b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new color range with default values.
        /// </summary>
        /// <param name="startColorIndex">The start color index to setup the range.</param>
        /// <remarks>By default, when initialize the 16 ranges in <see cref="PAL"/>, the idea is create a ramp values from 0 to 255 values and repeat until completes all 16 ranges. This process is automatically setup in <see cref="ColorRangeTable"/> default constructor.</remarks>
        public ColorRange(ref byte startColorIndex)
        {
            this.colors = ColorAmmount._8;
            this.type = RangeTypes._0;
            this.isfixed = false;
            this.blackColor = 0;

            this._rangeColors = new byte[LENGTH];

            byte range = startColorIndex;

            for (int i = 0; i < LENGTH; i++)
            {
                this._rangeColors[i] = range;
                if (++range > byte.MaxValue)
                    range = 0;
            }
        }

        /// <summary>
        /// Creates a new color range from memory.
        /// </summary>
        /// <param name="buffer"></param>
        public ColorRange(byte[] buffer)
        {
            if (buffer.Length != SIZE)
                throw new ArgumentOutOfRangeException($"The buffer must be contains a {SIZE} array length.");

            using (var stream = new BinaryReader(new MemoryStream(buffer)))
            {
                this.colors = (ColorAmmount)stream.ReadByte();
                this.type = (RangeTypes)stream.ReadByte();
                this.isfixed = stream.ReadBoolean();
                this.blackColor = stream.ReadByte();
                this._rangeColors = stream.ReadBytes(LENGTH);
            }
        }
        #endregion

        #region Methods & Functions
        public byte[] Serialize()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                stream.Write((byte)this.colors);
                stream.Write((byte)this.type);
                stream.Write(this.isfixed);
                stream.Write(this.blackColor);
                stream.Write(this._rangeColors);

                return (stream.BaseStream as MemoryStream).GetBuffer();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return this._rangeColors.GetEnumerator() as IEnumerator<byte>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ColorRange)) return false;

            return this == (ColorRange)obj;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            foreach (var color in this._rangeColors)
                hash ^= color;

            return hash;
        }
        #endregion
    }
}
