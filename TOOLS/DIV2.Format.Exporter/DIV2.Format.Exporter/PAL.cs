using DIV2.Format.Exporter.MethodExtensions;
using DIV2.Format.Exporter.Processors.Palettes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DIV2.Format.Exporter
{
    /// <summary>
    /// A representation of a DIV Games Studio PAL file.
    /// </summary>
    /// <remarks>Implements functions to import and export color palettes.</remarks>
    public sealed class PAL : IAssetFile, IEnumerable<Color>
    {
        #region Constants
        readonly static DIVHeader PAL_FILE_HEADER = new DIVHeader('p', 'a', 'l');
        readonly static PAL VALIDATOR = new PAL();
        readonly static IndexOutOfRangeException INDEX_OUT_OF_RANGE_EXCEPTION = 
            new IndexOutOfRangeException($"The index value must be a value beteween 0 and {LENGTH}.");

        /// <summary>
        /// Number of colors.
        /// </summary>
        public const int LENGTH = ColorPalette.LENGTH;
        /// <summary>
        /// Memory size.
        /// </summary>
        public const int SIZE = DIVHeader.SIZE + ColorPalette.SIZE + ColorRangeTable.SIZE;
        #endregion

        #region Properties
        /// <summary>
        /// Palette colors, in DAC format.
        /// </summary>
        public ColorPalette Colors { get; private set; }
        /// <summary>
        /// Color range table.
        /// </summary>
        public ColorRangeTable Ranges { get; private set; }
        /// <summary>
        /// Get or set a <see cref="Color"/> value.
        /// </summary>
        /// <param name="index"><see cref="Color"/> index.</param>
        /// <returns>Return the <see cref="Color"/> value.</returns>
        public Color this[int index]
        {
            get
            {
                if (!index.IsClamped(0, LENGTH))
                    throw INDEX_OUT_OF_RANGE_EXCEPTION;

                return this.Colors[index];
            }
            set
            {
                if (!index.IsClamped(0, LENGTH))
                    throw INDEX_OUT_OF_RANGE_EXCEPTION;

                this.Colors[index] = value;
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(PAL a, PAL b)
        {
            return a.GetHashCode() == b.GetHashCode();
        }

        public static bool operator !=(PAL a, PAL b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="PAL"/> instance.
        /// </summary>
        public PAL()
        {
            this.Colors = new ColorPalette();
            this.Ranges = new ColorRangeTable();
        }

        /// <summary>
        /// Creates a new <see cref="PAL"/> instance.
        /// </summary>
        /// <param name="colors"><see cref="ColorPalette"/> instance.</param>
        public PAL(ColorPalette colors)
        {
            this.Colors = colors;
            this.Ranges = new ColorRangeTable();
        }

        /// <summary>
        /// Creates a new <see cref="PAL"/> instance.
        /// </summary>
        /// <param name="colors"><see cref="ColorPalette"/> instance.</param>
        /// <param name="ranges"><see cref="ColorRangeTable"/> instance.</param>
        public PAL(ColorPalette colors, ColorRangeTable ranges)
        {
            this.Colors = colors;
            this.Ranges = ranges;
        }

        /// <summary>
        /// Creates a new <see cref="PAL"/> instance from a <see cref="Color"/> array.
        /// </summary>
        /// <param name="colors">A 256 length <see cref="Color"/> array.</param>
        public PAL(Color[] colors)
            : this(new ColorPalette(colors))
        {
        }

        /// <summary>
        /// Loads a <see cref="PAL"/> file.
        /// </summary>
        /// <param name="filename"><see cref="PAL"/> filename to load.</param>
        public PAL(string filename)
            : this(File.ReadAllBytes(filename))
        {
        }

        /// <summary>
        /// Loads a <see cref="PAL"/> file.
        /// </summary>
        /// <param name="buffer">A memory buffer that contains <see cref="PAL"/> file.</param>
        public PAL(byte[] buffer)
            : this()
        {
            try
            {
                using (var stream = new BinaryReader(new MemoryStream(buffer)))
                {
                    if (PAL_FILE_HEADER.Validate(stream.ReadBytes(DIVHeader.SIZE)))
                    {
                        this.Colors = new ColorPalette(stream.ReadBytes(ColorPalette.SIZE));
                        this.Ranges = new ColorRangeTable(stream.ReadBytes(ColorRangeTable.SIZE));

                        this.GetHashCode();
                    }
                    else
                        throw new DIVFormatHeaderException();
                }
            }
            catch (Exception ex)
            {
                throw new DIVFileFormatException<PAL>(ex);
            }

        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Creates new <see cref="PAL"/> instance from a supporte image file.
        /// </summary>
        /// <param name="filename">Image file to load.</param>
        /// <returns>Returns a new <see cref="PAL"/> instance.</returns>
        /// <remarks>Supported image formats are JPEG, PNG, BMP, GIF and TGA. 
        /// Also supported 256 color PCX images and <see cref="MAP"/> files, 
        /// without the metadata info, that will be converted to the new setup <see cref="PAL"/>.</remarks>
        public static PAL FromImage(string filename)
        {
            return FromImage(File.ReadAllBytes(filename));
        }

        /// <summary>
        /// Creates new <see cref="PAL"/> instance from a supporte image file.
        /// </summary>
        /// <param name="buffer">Memory buffer that contains a supported image file.</param>
        /// <returns>Supported image formats are JPEG, PNG, BMP, GIF and TGA. 
        /// Also supported 256 color PCX images and <see cref="MAP"/> files, 
        /// without the metadata info, that will be converted to the new setup <see cref="PAL"/>.</returns>
        public static PAL FromImage(byte[] buffer)
        {
            return PaletteProcessor.ProcessPalette(buffer);
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="PAL"/> file.
        /// </summary>
        /// <param name="filename">File to validate.</param>
        /// <returns>Returns true if the file is a valid <see cref="PAL"/>.</returns>
        public static bool ValidateFormat(string filename)
        {
            return VALIDATOR.Validate(filename);
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="PAL"/> file.
        /// </summary>
        /// <param name="buffer">Memory buffer that contain a <see cref="PAL"/> file data.</param>
        /// <returns>Returns true if the file is a valid <see cref="PAL"/>.</returns>
        public static bool ValidateFormat(byte[] buffer)
        {
            return VALIDATOR.Validate(buffer);
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="PAL"/> file.
        /// </summary>
        /// <param name="filename">File to validate.</param>
        /// <returns>Returns true if the file is a valid <see cref="PAL"/>.</returns>
        public bool Validate(string filename)
        {
            return this.Validate(File.ReadAllBytes(filename));
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="PAL"/> file.
        /// </summary>
        /// <param name="buffer">Memory buffer that contain a <see cref="PAL"/> file data.</param>
        /// <returns>Returns true if the file is a valid <see cref="PAL"/>.</returns>
        public bool Validate(byte[] buffer)
        {
            return PAL_FILE_HEADER.Validate(buffer[0..DIVHeader.SIZE]) && this.TryToReadFile(buffer);
        }

        bool TryToReadFile(byte[] buffer)
        {
            try
            {
                using (var stream = new BinaryReader(new MemoryStream(buffer)))
                {
                    stream.ReadBytes(DIVHeader.SIZE); // DIV Header.
                    stream.ReadBytes(ColorPalette.SIZE); // Color palette.
                    stream.ReadBytes(ColorRangeTable.SIZE); // Color Range table.
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Serialize the <see cref="PAL"/> instance in a <see cref="byte"/> array.
        /// </summary>
        /// <returns>Returns the <see cref="byte"/> array with the <see cref="PAL"/> serialized data.</returns>
        /// <remarks>This function not include the file header data.</remarks>
        public byte[] Serialize()
        {
            using (var buffer = new MemoryStream())
            {
                buffer.Write(this.Colors.Serialize());
                buffer.Write(this.Ranges.Serialize());

                return buffer.ToArray();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        /// <summary>
        /// Save the instance in a <see cref="PAL"/> file.
        /// </summary>
        /// <param name="filename">Filename to write the data.</param>
        public void Save(string filename)
        {
            using (var stream = new BinaryWriter(File.OpenWrite(filename)))
            {
                PAL_FILE_HEADER.Write(stream);
                this.Write(stream);
            }
        }

        public IEnumerator<Color> GetEnumerator()
        {
            return this.Colors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PAL)) return false;

            return this == (PAL)obj;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            foreach (var color in this.Colors)
                hash ^= color.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Compare this instance with other <see cref="PAL"/> instance.
        /// </summary>
        /// <param name="other"><see cref="PAL"/> instnace to compare.</param>
        /// <returns>Returns true if the both instances are equals.</returns>
        /// <remarks>The == and != operators only compare the colors between <see cref="PAL"/> instances. 
        /// Use this function if you want to compare color tables and color range tables.</remarks>
        public bool Compare(PAL other)
        {
            return this.Colors == other.Colors &&
                   this.Ranges == other.Ranges;
        }

        /// <summary>
        /// Creates a copy of the <see cref="Color"/> array converted to full RGB format [0..255].
        /// </summary>
        /// <returns>Returns a new <see cref="Color"/> array in full RGB format [0..255].</returns>
        public Color[] ToRGB() => this.Colors.ToRGB();
        #endregion
    }
}
