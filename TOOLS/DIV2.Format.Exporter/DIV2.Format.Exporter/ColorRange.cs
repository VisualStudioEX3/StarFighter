using DIV2.Format.Exporter.ExtensionMethods;
using DIV2.Format.Exporter.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DIV2.Format.Exporter
{
    class ColorRangeEnumerator : IEnumerator<byte>
    {
        #region Internal vars
        IList<byte> _items;
        int _currentIndex;
        #endregion

        #region Properties
        public byte Current { get; private set; }
        object IEnumerator.Current => this.Current;
        #endregion

        #region Constructor & Destructor
        public ColorRangeEnumerator(IList<byte> items)
        {
            this._items = items;
            this.Current = default(byte);
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
    /// Color range values.
    /// </summary>
    public sealed class ColorRange : ISerializableAsset, IEnumerable<byte>
    {
        #region Constants
        readonly static IndexOutOfRangeException INDEX_OUT_OF_RANGE_EXCEPTION =
            new IndexOutOfRangeException($"The index value must be a value beteween 0 and {LENGTH}.");

        public const RangeColors DEFAULT_RANGE_COLORS = RangeColors._8;
        public const RangeTypes DEFAULT_TYPE = RangeTypes.Direct;
        public const bool DEFAULT_FIXED_STATE = false;
        public const int DEFAULT_BLACK_COLOR = 0;
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
        public enum RangeColors : byte
        {
            /// <summary>
            /// 8 colors.
            /// </summary>
            _8 = 8,
            /// <summary>
            /// 16 colors.
            /// </summary>
            _16 = 16,
            /// <summary>
            /// 32 colors.
            /// </summary>
            _32 = 32
        }

        /// <summary>
        /// Available range types.
        /// </summary>
        public enum RangeTypes : byte
        {
            /// <summary>
            /// Direct from palette.
            /// </summary>
            Direct = 0,
            /// <summary>
            /// Editable each color.
            /// </summary>
            Edit1 = 1,
            /// <summary>
            /// Editable each 2 colors.
            /// </summary>
            Edit2 = 2,
            /// <summary>
            /// Editable each 4 colors.
            /// </summary>
            Edit4 = 4,
            /// <summary>
            /// Editable each 8 colors.
            /// </summary>
            Edit8 = 8
        }
        #endregion

        #region Public vars
        byte[] _rangeColors;

        /// <summary>
        /// Ammount of colors for the range.
        /// </summary>
        public RangeColors colors;
        /// <summary>
        /// Range type.
        /// </summary>
        public RangeTypes type;
        /// <summary>
        /// Defines if the range is editable (false) or not (true). By default is false.
        /// </summary>
        public bool isFixed;
        /// <summary>
        /// Index of the black color. Default is zero.
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
            if (a.colors == b.colors &&
                a.type == b.type &&
                a.isFixed == b.isFixed &&
                a.blackColor == b.blackColor)
            {
                for (int i = 0; i < LENGTH; i++)
                    if (a[i] != b[i])
                        return false;

                return true;
            }
            else
                return false;
        }

        public static bool operator !=(ColorRange a, ColorRange b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new <see cref="ColorRange"/> instance with default values.
        /// </summary>
        /// <param name="startColorIndex">The start color index to setup the range.</param>
        /// <remarks>By default, when initialize the 16 ranges in a <see cref="PAL"/>, 
        /// the idea is create a ramp values from 0 to 255 values and repeat until completes all 16 ranges. 
        /// This process is automatically setup in <see cref="ColorRangeTable"/> default constructor.</remarks>
        public ColorRange(ref byte startColorIndex)
        {
            this.colors = DEFAULT_RANGE_COLORS;
            this.type = DEFAULT_TYPE;
            this.isFixed = DEFAULT_FIXED_STATE;
            this.blackColor = DEFAULT_BLACK_COLOR;

            this._rangeColors = new byte[LENGTH];

            byte range = startColorIndex;

            for (int i = 0; i < LENGTH; i++)
            {
                this._rangeColors[i] = range;
                if (++range > byte.MaxValue)
                    range = 0;
            }

            startColorIndex = range;
        }

        /// <summary>
        /// Creates a new color range from memory.
        /// </summary>
        /// <param name="buffer">A <see cref="byte"/> array that contains the <see cref="ColorRange"/> data.</param>
        public ColorRange(byte[] buffer)
        {
            if (buffer.Length != SIZE)
                throw new ArgumentOutOfRangeException($"The buffer must be contains a {SIZE} array length.");

            using (var stream = new BinaryReader(new MemoryStream(buffer)))
            {
                this.colors = (RangeColors)stream.ReadByte();
                this.type = (RangeTypes)stream.ReadByte();
                this.isFixed = stream.ReadBoolean();
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
                stream.Write(this.isFixed);
                stream.Write(this.blackColor);
                stream.Write(this._rangeColors);

                return (stream.BaseStream as MemoryStream).ToArray();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return new ColorRangeEnumerator(this._rangeColors);
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
            return this.Serialize().CalculateChecksum().GetSecureHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var index in this._rangeColors)
                sb.Append($"{index}, ");

            string rangeValues = sb.ToString();
            rangeValues = sb.ToString().Substring(0, rangeValues.Length - 2);

            sb = new StringBuilder();

            sb.Append($"{{ {nameof(ColorRange)}: ");
            sb.Append($"{{ Hash: {this.GetHashCode()}, ");
            sb.Append($"Colors: {(int)this.colors}, ");
            sb.Append($"Type: {(int)this.type}, ");
            sb.Append($"Is fixed: {this.isFixed}, ");
            sb.Append($"Black color index: {this.blackColor}, ");
            sb.Append($"Range: [ {rangeValues} ] }}");
            sb.Append(" }");

            return sb.ToString();
        }
        #endregion
    }
}
