using DIV2.Format.Exporter.Converters;
using DIV2.Format.Exporter.MethodExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DIV2.Format.Exporter
{
    /// <summary>
    /// <see cref="MAP"/> Control Point data.
    /// </summary>
    [Serializable]
    public struct ControlPoint : ISerializableAsset
    {
        #region Constants
        /// <summary>
        /// Number of items.
        /// </summary>
        public const int LENGTH = 2;
        /// <summary>
        /// Memory size.
        /// </summary>
        public const int SIZE = sizeof(short) * LENGTH;
        #endregion

        #region Public vars
        /// <summary>
        /// Horizontal coordinate.
        /// </summary>
        public short x;
        /// <summary>
        /// Vertical coordinate.
        /// </summary>
        public short y;
        #endregion

        #region Properties
        /// <summary>
        /// Get or set the coordinate value.
        /// </summary>
        /// <param name="index">Index of the coordinate in the structure.</param>
        /// <returns>Returns the coordinate value.</returns>
        public short this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return this.x;
                    case 1: return this.y;
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                switch (index)
                {
                    case 0: this.x = value; break;
                    case 1: this.y = value; break;
                }

                throw new IndexOutOfRangeException();
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(ControlPoint a, ControlPoint b)
        {
            return a.x == b.x &&
                   a.y == b.y;
        }

        public static bool operator !=(ControlPoint a, ControlPoint b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructors
        public ControlPoint(short x, short y)
        {
            this.x = x;
            this.y = y;
        }

        public ControlPoint(int x, int y)
            : this((short)x, (short)y)
        {
        }

        public ControlPoint(float x, float y)
            : this((short)x, (short)y)
        {
        }

        public ControlPoint(double x, double y)
            : this((short)x, (short)y)
        {
        }
        #endregion

        #region Methods & Functions
        public byte[] Serialize()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                stream.Write(this.x);
                stream.Write(this.y);

                return (stream.BaseStream as MemoryStream).GetBuffer();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ControlPoint)) return false;

            return this == (ControlPoint)obj;
        }

        public override int GetHashCode()
        {
            return this.x ^ this.y;
        }
        #endregion
    }

    /// <summary>
    /// A representation of a DIV Games Studio MAP file.
    /// </summary>
    /// <remarks>Implements functions to import and export graphic maps.</remarks>
    public sealed class MAP : IAssetFile, IEnumerable<byte>
    {
        #region Constants
        const int HEADER_LENGTH = DIVHeader.SIZE + (sizeof(short) * 2) + sizeof(int) + DESCRIPTION_LENGTH;
        const int MIN_PIXEL_COUNT = 1;

        readonly static DIVHeader MAP_FILE_HEADER = new DIVHeader('m', 'a', 'p');
        readonly static MAP VALIDATOR = new MAP();
        readonly static string PIXEL_OUT_OF_RANGE_EXCEPTION_MESSAGE = "{0} min value accepted is " + MIN_PIXEL_COUNT;
        readonly static ArgumentOutOfRangeException GRAPHID_OUT_OF_RANGE = new ArgumentOutOfRangeException($"GraphId must be a value between {MIN_GRAPH_ID} and {MAX_GRAPH_ID}.");
        const string INDEX_OUT_OF_RANGE_EXCEPTION_MESSAGE = "The index value must be a value beteween 0 and {0}.";
        const string COORDINATE_OUT_OF_RANGE_EXCEPTION_MESSAGE = "{0} coordinate must be a value beteween 0 and {1}.";

        /// <summary>
        /// Max description character length.
        /// </summary>
        public const int DESCRIPTION_LENGTH = 32;
        /// <summary>
        /// Min allowed graph id value.
        /// </summary>
        public const int MIN_GRAPH_ID = 1;
        /// <summary>
        /// Max allowed graph id value.
        /// </summary>
        public const int MAX_GRAPH_ID = 999;
        /// <summary>
        /// Max supported control points value.
        /// </summary>
        public const int MAX_CONTROL_POINTS = 999;
        #endregion

        #region Internal vars
        int _hash = 0;
        int _graphId;
        byte[] _bitmap;
        #endregion

        #region Properties
        /// <summary>
        /// Width of the graphic map.
        /// </summary>
        public short Width { get; private set; }
        /// <summary>
        /// Height of the graphic map.
        /// </summary>
        public short Height { get; private set; }
        /// <summary>
        /// Graphic identifiers used in <see cref="FPG"/> files.
        /// </summary>
        public int GraphId
        {
            get => this._graphId;
            set
            {
                if (!value.IsClamped(MIN_GRAPH_ID, MAX_GRAPH_ID))
                    throw GRAPHID_OUT_OF_RANGE;

                this._graphId = value;
            }
        }
        /// <summary>
        /// Optional graphic description.
        /// </summary>
        /// <remarks>The description only allow a 32 length ASCII null terminated string.</remarks>
        public string Description { get; set; }
        /// <summary>
        /// Color palette used by this graphic map.
        /// </summary>
        public PAL Palette { get; private set; }
        /// <summary>
        /// Optional control point list.
        /// </summary>
        public List<ControlPoint> ControlPoints { get; private set; }
        /// <summary>
        /// Get or set the color index in the bitmap.
        /// </summary>
        /// <param name="index">Pixel index in the bitmap array.</param>
        /// <returns>Returns the color index in color palette of the pixel.</returns>
        public byte this[int index]
        {
            get
            {
                if (!index.IsClamped(0, this._bitmap.Length))
                    throw new IndexOutOfRangeException(string.Format(INDEX_OUT_OF_RANGE_EXCEPTION_MESSAGE, this._bitmap.Length));

                return this._bitmap[index];
            }
            private set
            {
                if (!index.IsClamped(0, this._bitmap.Length))
                    throw new IndexOutOfRangeException(string.Format(INDEX_OUT_OF_RANGE_EXCEPTION_MESSAGE, this._bitmap.Length));

                this._bitmap[index] = value;
            }
        }
        /// <summary>
        /// Get or set the color index in the bitmap.
        /// </summary>
        /// <param name="x">Horizontal coordinate of the pixel to read.</param>
        /// <param name="y">Vertical coordinate of the pixel to read.</param>
        /// <returns>Returns the color index in color palette of the pixel.</returns>
        public byte this[int x, int y]
        {
            get
            {
                if (!x.IsClamped(0, this.Width))
                    throw new IndexOutOfRangeException(string.Format(COORDINATE_OUT_OF_RANGE_EXCEPTION_MESSAGE, "X", this.Width));

                if (!y.IsClamped(0, this.Width))
                    throw new IndexOutOfRangeException(string.Format(COORDINATE_OUT_OF_RANGE_EXCEPTION_MESSAGE, "Y", this.Height));

                return this._bitmap[this.GetIndex(x, y)];
            }
            private set
            {
                if (!x.IsClamped(0, this.Width))
                    throw new IndexOutOfRangeException(string.Format(COORDINATE_OUT_OF_RANGE_EXCEPTION_MESSAGE, "X", this.Width));

                if (!y.IsClamped(0, this.Width))
                    throw new IndexOutOfRangeException(string.Format(COORDINATE_OUT_OF_RANGE_EXCEPTION_MESSAGE, "Y", this.Height));

                this._bitmap[this.GetIndex(x, y)] = value;
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(MAP a, MAP b)
        {
            return a.GetHashCode() == b.GetHashCode();
        }

        public static bool operator !=(MAP a, MAP b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructors
        MAP()
        {
            this.ControlPoints = new List<ControlPoint>();
        }

        /// <summary>
        /// Creates a new <see cref="MAP"/> instance.
        /// </summary>
        /// <param name="palette"><see cref="PAL"/> instance for this <see cref="MAP"/> instance.</param>
        /// <param name="width">Bitmap width.</param>
        /// <param name="height">Bitmap height.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic identifiers. By default is 1.</param>
        /// <param name="description">Optional <see cref="MAP"/> description.</param>
        public MAP(PAL palette, int width, int height, int graphId = MIN_GRAPH_ID, string description = "")
            : this()
        {
            if (width < MIN_PIXEL_COUNT)
                throw new ArgumentOutOfRangeException(string.Format(PIXEL_OUT_OF_RANGE_EXCEPTION_MESSAGE, "Width"));
            if (height < MIN_PIXEL_COUNT)
                throw new ArgumentOutOfRangeException(string.Format(PIXEL_OUT_OF_RANGE_EXCEPTION_MESSAGE, "Height"));
            if (!graphId.IsClamped(MIN_GRAPH_ID, MAX_GRAPH_ID))
                throw GRAPHID_OUT_OF_RANGE;

            this._bitmap = new byte[width * height];

            this.Palette = palette;
            this.Width = (short)width;
            this.Height = (short)height;
            this.GraphId = graphId;
            this.Description = description;
        }

        /// <summary>
        /// Loads a <see cref="MAP"/> file.
        /// </summary>
        /// <param name="filename">Filename to load.</param>
        public MAP(string filename)
            : this(File.ReadAllBytes(filename))
        {
        }

        /// <summary>
        /// Loads a <see cref="MAP"/> file from memory.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array that contain the <see cref="MAP"/> file data to load.</param>
        public MAP(byte[] buffer)
            : this()
        {
            using (var stream = new BinaryReader(new MemoryStream(buffer)))
            {
                if (MAP_FILE_HEADER.Validate(stream.ReadBytes(DIVHeader.SIZE)))
                {
                    this.Width = stream.ReadInt16();
                    this.Height = stream.ReadInt16();
                    this.GraphId = stream.ReadInt32();
                    this.Description = stream.ReadBytes(DESCRIPTION_LENGTH).ToASCIIString();
                    this.Palette = new PAL(new ColorPalette(stream.ReadBytes(ColorPalette.SIZE)),
                                           new ColorRangeTable(stream.ReadBytes(ColorRangeTable.SIZE)));

                    short points = stream.ReadInt16();
                    for (int i = 0; i < points; i++)
                        this.ControlPoints.Add(new ControlPoint(stream.ReadInt16(), stream.ReadInt16()));

                    this._bitmap = stream.ReadBytes(this.Width * this.Height);
                }
                else
                    throw new FormatException($"Error loading {nameof(MAP)} file.");
            }
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Creates a new <see cref="MAP"/> instance from a supported image format.
        /// </summary>
        /// <param name="filename">Image file to load.</param>
        /// <param name="palette"><see cref="PAL"/> instance to convert the loaded image.</param>
        /// <returns>Returns a new <see cref="MAP"/> instance from the loaded image.</returns>
        /// <remarks>Supported image formats are JPEG, PNG, BMP, GIF and TGA. Also supported 256 color PCX images and <see cref="MAP"/> files, without the metadata info, that will be converted to the new setup <see cref="PAL"/>.</remarks>
        public static MAP FromImage(string filename, PAL palette)
        {
            return FromImage(File.ReadAllBytes(filename), palette);
        }

        /// <summary>
        /// Creates a new <see cref="MAP"/> instance from a supported image format.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array that contain a supported image.</param>
        /// <param name="palette"><see cref="PAL"/> instance to convert the loaded image.</param>
        /// <returns>Returns a new <see cref="MAP"/> instance from the loaded image.</returns>
        /// <remarks>Supported image formats are JPEG, PNG, BMP, GIF and TGA. Also supported 256 color PCX images and <see cref="MAP"/> files, without the metadata info, that will be converted to the new setup <see cref="PAL"/>.</remarks>
        public static MAP FromImage(byte[] buffer, PAL palette)
        {
            BMP256Converter.Convert(buffer, out byte[] bitmap, out short width, out short height, palette);
            return new MAP(palette, width, height) { _bitmap = bitmap };
        }

        int GetIndex(int x, int y)
        {
            return (this.Width * y) + x;
        }

        /// <summary>
        /// Get the bitmap array data of this instance.
        /// </summary>
        /// <returns>Returns a <see cref="byte"/> array with all pixels with their color indexes from the <see cref="PAL"/> instance.</returns>
        public byte[] GetBitmapArray() => this._bitmap;

        /// <summary>
        /// Set the bitmap array data for this instance.
        /// </summary>
        /// <param name="pixels"><see cref="byte"/> array that contains pixel data for this instance.</param>
        public void SetBitmapArray(byte[] pixels)
        {
            if (pixels.Length != this._bitmap.Length)
                throw new ArgumentOutOfRangeException($"The pixel array must be had the same length that this bitmap instance ({this._bitmap.Length} bytes).");

            this._bitmap = pixels;
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="MAP"/> file.
        /// </summary>
        /// <param name="filename">File to validate.</param>
        /// <returns>Returns true if the file is a valid <see cref="MAP"/>.</returns>
        public static bool ValidateFormat(string filename)
        {
            return VALIDATOR.Validate(filename);
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="MAP"/> file.
        /// </summary>
        /// <param name="buffer">Memory buffer that contain a <see cref="MAP"/> file data.</param>
        /// <returns>Returns true if the file is a valid <see cref="MAP"/>.</returns>
        public static bool ValidateFormat(byte[] buffer)
        {
            return VALIDATOR.Validate(buffer);
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="MAP"/> file.
        /// </summary>
        /// <param name="filename">File to validate.</param>
        /// <returns>Returns true if the file is a valid <see cref="MAP"/>.</returns>
        public bool Validate(string filename)
        {
            return this.Validate(File.ReadAllBytes(filename));
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="MAP"/> file.
        /// </summary>
        /// <param name="buffer">Memory buffer that contain a <see cref="MAP"/> file data.</param>
        /// <returns>Returns true if the file is a valid <see cref="MAP"/>.</returns>
        public bool Validate(byte[] buffer)
        {
            return MAP_FILE_HEADER.Validate(buffer) && this.TryToReadFile(buffer);
        }

        bool TryToReadFile(byte[] buffer)
        {
            try
            {
                using (var stream = new BinaryReader(new MemoryStream(buffer)))
                {
                    stream.ReadBytes(DIVHeader.SIZE); // DIV Header.

                    short width = stream.ReadInt16();
                    short height = stream.ReadInt16();
                    int bitmapSize = width * height;

                    stream.ReadInt32(); // GraphId.
                    stream.ReadBytes(DESCRIPTION_LENGTH); // Description.
                    stream.ReadBytes(PAL.SIZE); // Palette.

                    short points = stream.ReadInt16(); // Control points counter.
                    if (points > 0)
                        stream.ReadBytes(points * ControlPoint.SIZE); // Control points list.

                    stream.ReadBytes(bitmapSize); // Bitmap data.
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Serialize the <see cref="MAP"/> instance in a <see cref="byte"/> array.
        /// </summary>
        /// <returns>Returns the <see cref="byte"/> array with the <see cref="MAP"/> serialized data.</returns>
        /// <remarks>This function not include the file header data.</remarks>
        public byte[] Serialize()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                stream.Write(this.Width);
                stream.Write(this.Height);
                stream.Write(this.GraphId);
                stream.Write(this.Description.GetASCIIZString(DESCRIPTION_LENGTH));

                this.Palette.Write(stream);

                var count = (short)Math.Min(this.ControlPoints.Count, MAX_CONTROL_POINTS);
                stream.Write(count);
                for (int i = 0; i < count; i++)
                    this.ControlPoints[i].Write(stream);

                stream.Write(this._bitmap);

                return (stream.BaseStream as MemoryStream).GetBuffer();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        /// <summary>
        /// Save the instance in a <see cref="MAP"/> file.
        /// </summary>
        /// <param name="filename">Filename to write the data.</param>
        public void Save(string filename)
        {
            using (var stream = new BinaryWriter(File.OpenWrite(filename)))
            {
                MAP_FILE_HEADER.Write(stream);
                this.Write(stream);
            }
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return this._bitmap.GetEnumerator() as IEnumerator<byte>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MAP)) return false;

            return this == (MAP)obj;
        }

        public override int GetHashCode()
        {
            if (this._hash == 0)
            {
                this._hash = this.Width ^ this.Height;
                foreach (var color in this._bitmap)
                    this._hash ^= color;
            }

            int hash = this._hash ^
                       this.GraphId ^
                       this.Description.GetHashCode() ^
                       this.Palette.GetHashCode();

            return hash;
        }
        #endregion
    }
}
