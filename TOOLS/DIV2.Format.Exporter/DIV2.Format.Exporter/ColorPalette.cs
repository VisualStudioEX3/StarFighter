using DIV2.Format.Exporter.MethodExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DIV2.Format.Exporter
{
    public struct Color : ISerializableAsset
    {
        #region Constants
        const float DAC_TO_RGB_FACTOR = 4.05f; // Value used to convert DAC values [0..63] to RGB [0..255].
        const int RGB_TO_DAC_FACTOR = 4; // Value used to convert RGB values [0..255] to DAC [0..63].

        readonly static IndexOutOfRangeException INDEX_OUT_OF_RANGE_EXCEPTION =
            new IndexOutOfRangeException($"The index value must be a value beteween 0 and {LENGTH}.");

        /// <summary>
        /// Number of components.
        /// </summary>
        public const int LENGTH = 3;
        /// <summary>
        /// Memory size.
        /// </summary>
        public const int SIZE = sizeof(byte) * LENGTH;
        #endregion

        #region Public vars
        public byte red;
        public byte green;
        public byte blue;
        #endregion

        #region Properties
        /// <summary>
        /// Get or set the component color value.
        /// </summary>
        /// <param name="index">Index of the component.</param>
        /// <returns>Returns the component color value.</returns>
        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return this.red;
                    case 1: return this.green;
                    case 2: return this.blue;
                    default: throw INDEX_OUT_OF_RANGE_EXCEPTION;
                }
            }

            set
            {
                switch (index)
                {
                    case 0: this.red = value; break;
                    case 1: this.green = value; break;
                    case 2: this.blue = value; break;
                    default: throw INDEX_OUT_OF_RANGE_EXCEPTION;
                }
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(Color a, Color b)
        {
            return a.red == b.red &&
                   a.green == b.green &&
                   a.blue == b.blue;
        }

        public static bool operator !=(Color a, Color b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Color"/> value.
        /// </summary>
        /// <param name="red">Red component value.</param>
        /// <param name="green">Green component value.</param>
        /// <param name="blue">Blue component value.</param>
        public Color(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        /// <summary>
        /// Creates a new <see cref="Color"/> value.
        /// </summary>
        /// <param name="red">Red component value.</param>
        /// <param name="green">Green component value.</param>
        /// <param name="blue">Blue component value.</param>
        public Color(int red, int green, int blue)
        {
            this.red = (byte)Math.Clamp(red, byte.MinValue, byte.MaxValue);
            this.green = (byte)Math.Clamp(green, byte.MinValue, byte.MaxValue);
            this.blue = (byte)Math.Clamp(blue, byte.MinValue, byte.MaxValue);
        }

        /// <summary>
        /// Creates a new <see cref="Color"/> value.
        /// </summary>
        /// <param name="buffer">A 3 length array that contains the <see cref="Color"/> component values.</param>
        public Color(byte[] buffer)
        {
            if (buffer.Length != SIZE)
                throw new ArgumentOutOfRangeException($"The array must be {SIZE} bytes length.");

            this.red = buffer[0];
            this.green = buffer[1];
            this.blue = buffer[2];
        }

        /// <summary>
        /// Creates a new <see cref="Color"/> value.
        /// </summary>
        /// <param name="stream"><see cref="BinaryReader"/> stream that contains the <see cref="Color"/> value.</param>
        public Color(BinaryReader stream)
            : this(stream.ReadByte(), stream.ReadByte(), stream.ReadByte())
        {
        }
        #endregion

        #region Methods & Functions
        public byte[] Serialize()
        {
            return new byte[LENGTH] { this.red, this.green, this.blue };
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        /// <summary>
        /// Converts DAC values [0..63] to RGB range [0..255].
        /// </summary>
        /// <returns>Returns new <see cref="Color"/> value in RGB range [0..255].</returns>
        /// <remarks>DIV Games Studio and other software that works in old VESA modes, 
        /// using the DAC format for colors instead of full RGB format. 
        /// Use this function to adapt DAC values to RGB in order to work properly with modern implementations.</remarks>
        public Color ToRGB()
        {
            return new Color((byte)(this.red * DAC_TO_RGB_FACTOR),
                             (byte)(this.green * DAC_TO_RGB_FACTOR),
                             (byte)(this.blue * DAC_TO_RGB_FACTOR));
        }

        /// <summary>
        /// Converts RGB values [0..255] to DAC range [0..63].
        /// </summary>
        /// <returns>Returns new <see cref="Color"/> value in DAC range [0..63].</returns>
        /// <remarks>DIV Games Studio and other software that works in old VESA modes, 
        /// using the DAC format for colors instead of full RGB format. 
        /// Use this function to adapt RGB values to DAC in order to work properly with DIV Games Studio.</remarks>
        public Color ToDAC()
        {
            return new Color((byte)(this.red / RGB_TO_DAC_FACTOR),
                             (byte)(this.green / RGB_TO_DAC_FACTOR),
                             (byte)(this.blue / RGB_TO_DAC_FACTOR));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Color)) return false;

            return this == (Color)obj;
        }

        public override int GetHashCode()
        {
            return this.red ^ this.green ^ this.blue;
        }
        #endregion
    }

    class ColorPaletteEnumerator : IEnumerator<Color>
    {
        #region Internal vars
        IList<Color> _items;
        int _currentIndex;
        #endregion

        #region Properties
        public Color Current { get; private set; }
        object IEnumerator.Current => this.Current;
        #endregion

        #region Constructor & Destructor
        public ColorPaletteEnumerator(IList<Color> items)
        {
            this._items = items;
            this.Current = default(Color);
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
    /// A representation of a 256 indexed color palette.
    /// </summary>
    public sealed class ColorPalette : ISerializableAsset, IEnumerable<Color>
    {
        #region Constants
        readonly static IndexOutOfRangeException INDEX_OUT_OF_RANGE_EXCEPTION = 
            new IndexOutOfRangeException($"The index value must be a value beteween 0 and {LENGTH}.");
        readonly static string[] COLOR_FIELD_NAMES = { "Red", "Green", "Blue" };
        const string DAC_VALUE_OUT_OF_RANGE_EXCEPTION_MESSAGE = "The {0} value must be a DAC range value [{1}..{2}].";

        public const byte MIN_DAC_VALUE = 0;
        public const byte MAX_DAC_VALUE = 63;
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
            for (int i = 0; i < LENGTH; i++)
                if (a[i] != b[i])
                    return false;

            return true;
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

            int index = 0;
            for (int i = 0; i < LENGTH; i++)
                this._colors[i] = new Color(buffer[index++], buffer[index++], buffer[index++]);
        }

        /// <summary>
        /// Creates a new <see cref="Color"/> palette from memory.
        /// </summary>
        /// <param name="colors">A 256 length <see cref="Color"/> array.</param>
        public ColorPalette(Color[] colors)
            : this()
        {
            if (colors.Length != LENGTH)
                throw new ArgumentOutOfRangeException($"The color array must be contains a {LENGTH} array length, with RGB colors in DAC format [{MIN_DAC_VALUE}..{MAX_DAC_VALUE}].");

            this._colors = colors;
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

                return buffer.ToArray();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        /// <summary>
        /// Get a full RGB range values [0..255].
        /// </summary>
        /// <returns>Returns new <see cref="Color"/> array of full RGB values [0..255].</returns>
        public Color[] ToRGB()
        {
            var rgb = new Color[LENGTH];

            for (int i = 0; i < LENGTH; i++)
                rgb[i] = this[i].ToRGB();

            return rgb;
        }

        /// <summary>
        /// Get a DAC range values [0..63].
        /// </summary>
        /// <returns>Returns new <see cref="Color"/> array of DAC values [0..63].</returns>
        public Color[] ToDAC()
        {
            var dac = new Color[LENGTH];

            for (int i = 0; i < LENGTH; i++)
                dac[i] = this[i].ToDAC();

            return dac;
        }

        public IEnumerator<Color> GetEnumerator()
        {
            return new ColorPaletteEnumerator(this._colors);
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
            return base.GetHashCode();
        }
        #endregion
    }
}
