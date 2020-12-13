using DIV2.Format.Exporter.MethodExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DIV2.Format.Exporter
{
    /// <summary>
    /// A collection of 16 ranges that composes the <see cref="PAL"/> <see cref="ColorRange"/> table.
    /// </summary>
    /// <remarks>The color ranges are used only in DIV Games Studio color and drawing tools. Not are used in games. 
    /// By default is enough to creates a <see cref="ColorRangeTable"/> with default values.</remarks>
    public sealed class ColorRangeTable : ISerializableAsset, IEnumerable<ColorRange>
    {
        #region Constants
        readonly static IndexOutOfRangeException INDEX_OUT_OF_RANGE_EXCEPTION = 
            new IndexOutOfRangeException($"The index value must be a value beteween 0 and {LENGTH}.");

        /// <summary>
        /// Number of <see cref="ColorRange"/> in the table.
        /// </summary>
        public const int LENGTH = 16;
        /// <summary>
        /// Memory size of the color range table.
        /// </summary>
        public const int SIZE = LENGTH * ColorRange.SIZE;
        #endregion

        #region Internal vars
        ColorRange[] _ranges;
        #endregion

        #region Properties
        /// <summary>
        /// Get or set the <see cref="ColorRange"/> value.
        /// </summary>
        /// <param name="index">Index of the range.</param>
        /// <returns>Returns the <see cref="ColorRange"/> value.</returns>
        public ColorRange this[int index]
        {
            get
            {
                if (!index.IsClamped(0, LENGTH))
                    throw INDEX_OUT_OF_RANGE_EXCEPTION;

                return this._ranges[index];
            }
            set
            {
                if (!index.IsClamped(0, LENGTH))
                    throw INDEX_OUT_OF_RANGE_EXCEPTION;

                this._ranges[index] = value;
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(ColorRangeTable a, ColorRangeTable b)
        {
            return a.GetHashCode() == b.GetHashCode();
        }

        public static bool operator !=(ColorRangeTable a, ColorRangeTable b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="ColorRangeTable"/> with default <see cref="ColorRange"/> values.
        /// </summary>
        public ColorRangeTable()
        {
            byte range = 0;

            this._ranges = new ColorRange[LENGTH];

            for (int i = 0; i < LENGTH; i++)
                this._ranges[i] = new ColorRange(ref range);
        }

        /// <summary>
        /// Creates a new <see cref="ColorRangeTable"/> from memory.
        /// </summary>
        /// <param name="buffer">A <see cref="byte"/> array that contains a <see cref="ColorRangeTable"/> data.</param>
        public ColorRangeTable(byte[] buffer)
        {
            if (buffer.Length != SIZE)
                throw new ArgumentOutOfRangeException();

            using (var stream = new BinaryReader(new MemoryStream(buffer)))
            {
                for (int i = 0; i < LENGTH; i++)
                    this._ranges[i] = new ColorRange(stream.ReadBytes(ColorRange.SIZE));
            }
        }
        #endregion

        #region Methods & Functions
        public byte[] Serialize()
        {
            using (var buffer = new MemoryStream())
            {
                foreach (var range in this._ranges)
                    buffer.Write(range.Serialize());

                return buffer.GetBuffer();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        public IEnumerator<ColorRange> GetEnumerator()
        {
            return this._ranges.GetEnumerator() as IEnumerator<ColorRange>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ColorRangeTable)) return false;

            return this == (ColorRangeTable)obj;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            foreach (var color in this._ranges)
                hash ^= color.GetHashCode();

            return hash;
        }
        #endregion
    }
}
