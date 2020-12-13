using DIV2.Format.Exporter.MethodExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DIV2.Format.Exporter
{
    /// <summary>
    /// A representation of a 256 indexed color palette.
    /// </summary>
    public sealed class ColorPalette : ISerializableAsset, IEnumerable<Color>
    {
        #region Constants
        const byte MIN_DAC_VALUE = 0;
        const byte MAX_DAC_VALUE = 63;

        readonly static IndexOutOfRangeException INDEX_OUT_OF_RANGE_EXCEPTION = 
            new IndexOutOfRangeException($"The index value must be a value beteween 0 and {LENGTH}.");
        readonly static string[] COLOR_FIELD_NAMES = { "Red", "Green", "Blue" };
        const string DAC_VALUE_OUT_OF_RANGE_EXCEPTION_MESSAGE = "The {0} value must be a DAC range value [{1}..{2}].";

        /// <summary>
        /// Number of colors.
        /// </summary>
        public const int LENGTH = 256;
        /// <summary>
        /// Memory size of the palette.
        /// </summary>
        public const int SIZE = LENGTH * Color.SIZE;
        #endregion

        #region Internal vars
        Color[] _colors;
        #endregion

        #region Properties
        /// <summary>
        /// Get or set a <see cref="Color"/> value.
        /// </summary>
        /// <param name="index">Index in palette.</param>
        /// <returns>Returns the <see cref="color"/> value.</returns>
        public Color this[int index]
        {
            get
            {
                if (!index.IsClamped(0, LENGTH))
                    throw INDEX_OUT_OF_RANGE_EXCEPTION;

                return this._colors[index];
            }
            set
            {
                if (!index.IsClamped(0, LENGTH))
                    throw INDEX_OUT_OF_RANGE_EXCEPTION;

                for (int i = 0; i < Color.LENGTH; i++)
                    if (!value[i].IsClamped(MIN_DAC_VALUE, MAX_DAC_VALUE))
                        throw new ArgumentOutOfRangeException(string.Format(DAC_VALUE_OUT_OF_RANGE_EXCEPTION_MESSAGE, 
                                                                            COLOR_FIELD_NAMES[i], 
                                                                            MIN_DAC_VALUE, 
                                                                            MAX_DAC_VALUE));

                this._colors[index] = value;
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(ColorPalette a, ColorPalette b)
        {
            return a.GetHashCode() == b.GetHashCode();
        }

        public static bool operator !=(ColorPalette a, ColorPalette b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Color"/> palette.
        /// </summary>
        public ColorPalette()
        {
            this._colors = new Color[LENGTH];
        }

        /// <summary>
        /// Creates a new <see cref="Color"/> palette from memory.
        /// </summary>
        /// <param name="buffer">A 768 length array that contains the data of the 256 <see cref="Color"/> values.</param>
        public ColorPalette(byte[] buffer)
            : this()
        {
            if (buffer.Length != SIZE)
                throw new ArgumentOutOfRangeException($"The buffer must be contains a {SIZE} array length, with RGB colors in DAC format [{MIN_DAC_VALUE}..{MAX_DAC_VALUE}].");

            int index = -1;
            for (int i = 0; i < LENGTH; i++)
                this._colors[i] = new Color(buffer[index++], buffer[index++], buffer[index++]);
        }

        /// <summary>
        /// Creates a new <see cref="Color"/> palette from memory.
        /// </summary>
        /// <param name="stream">A <see cref="BinaryReader"/> stream that contains the data of the 256 <see cref="Color"/> values.</param>
        public ColorPalette(BinaryReader stream)
            : this()
        {
            for (int i = 0; i < LENGTH; i++)
                this._colors[i] = new Color(stream);
        }
        #endregion

        #region Methods & Functions
        public byte[] Serialize()
        {
            using (var buffer = new MemoryStream())
            {
                foreach (var color in this._colors)
                    buffer.Write(color.Serialize(), 0, Color.SIZE);

                return buffer.GetBuffer();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        /// <summary>
        /// Get a full RGB range values [0..255].
        /// </summary>
        /// <returns></returns>
        public Color[] ToRGB()
        {
            var rgb = new Color[LENGTH];

            for (int i = 0; i < LENGTH; i++)
                rgb[i] = this[i].ToRGB();

            return rgb;
        }

        public IEnumerator<Color> GetEnumerator()
        {
            return this._colors.GetEnumerator() as IEnumerator<Color>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ColorPalette)) return false;

            return this == (ColorPalette)obj;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            foreach (var color in this._colors)
                hash ^= color.GetHashCode();

            return hash;
        }
        #endregion
    }
}
