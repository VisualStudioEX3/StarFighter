using DIV2.Format.Exporter.ExtensionMethods;
using DIV2.Format.Exporter.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DIV2.Format.Exporter
{
    class ColorRangeTableEnumerator : IEnumerator<ColorRange>
    {
        #region Internal vars
        IList<ColorRange> _items;
        int _currentIndex;
        #endregion

        #region Properties
        public ColorRange Current { get; private set; }
        object IEnumerator.Current => this.Current;
        #endregion

        #region Constructor & Destructor
        public ColorRangeTableEnumerator(IList<ColorRange> items)
        {
            this._items = items;
            this.Current = default(ColorRange);
            this.Reset();
        }

        void IDisposable.Dispose()
        {
        }
        #endregion

        #region Methods & Functions
        public bool MoveNext()
        {
            if (++this._currentIndex >= this._items.Count)
                return false;
            else
                this.Current = this._items[this._currentIndex];

            return true;
        }

        public void Reset()
        {
            this._currentIndex = -1;
        }
        #endregion
    }

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
        /// Number of <see cref="ColorRange"/>s in the table.
        /// </summary>
        public const int LENGTH = 16;
        /// <summary>
        /// Memory size of the color range table.
        /// </summary>
        public const int SIZE = LENGTH * ColorRange.SIZE;
        #endregion

        #region Internal vars
        ColorRange[] _ranges = new ColorRange[LENGTH];
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
            for (int i = 0; i < LENGTH; i++)
                if (a[i] != b[i])
                    return false;

            return true;
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

                return buffer.ToArray();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        public IEnumerator<ColorRange> GetEnumerator()
        {
            return new ColorRangeTableEnumerator(this._ranges);
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
            return this.Serialize().CalculateChecksum().GetSecureHashCode();
        }

        public override string ToString()
        {
            return $"{{ {nameof(ColorRangeTable)}: {{ Hash: {this.GetHashCode()} }} }}";
        }
        #endregion
    }
}
