using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImageMagick;
using DIV2Tools.Helpers;
using DIV2Tools.MethodExtensions;

namespace DIV2Tools.DIVFormats
{
    /// <summary>
    /// MAP file.
    /// </summary>
    public class MAP
    {
        #region Structs
        /// <summary>
        /// Control Point value.
        /// </summary>
        public struct ControlPoint
        {
            #region Public vars
            public short x, y;
            #endregion

            #region Constructor
            public ControlPoint(BinaryReader file)
            {
                x = file.ReadInt16();
                y = file.ReadInt16();
            }
            #endregion

            #region Methods & Functions
            public void Write(BinaryWriter file)
            {
                file.Write(this.x);
                file.Write(this.y);
            }

            public override string ToString()
            {
                return $"[{x:0000}.{y:0000}]";
            }
            #endregion
        }
        #endregion

        #region Classes
        class Header : DIVFormatBaseHeader // 48 bytes.
        {
            #region Constants
            const string HEADER_ID = "map";
            #endregion

            #region Internal vars
            BaseInfo _info;
            #endregion

            #region Public vars
            public short Width { get { return this._info.Width; } set { this._info.Width = value; } }
            public short Height { get { return this._info.Height; } set { this._info.Height = value; } }
            public int GraphId { get { return this._info.GraphId; } set { this._info.GraphId = value; } }
            public string Description { get { return this._info.Description; } set { this._info.Description = value; } }
            #endregion

            #region Constructor
            public Header() : base(Header.HEADER_ID)
            {
                this._info = new BaseInfo();
            }

            public Header(short width, short height, short graphId, string description) : base(Header.HEADER_ID)
            {
                this._info = new BaseInfo(width, height, graphId, description);
            }

            public Header(BinaryReader file) : base(Header.HEADER_ID, file)
            {
                this._info = new BaseInfo(file, false);
            }
            #endregion

            #region Methods & Functions
            public override void Write(BinaryWriter file)
            {
                base.Write(file);
                this._info.Write(file);
            }

            public override string ToString()
            {
                return $"{base.ToString()}\n{this._info.ToString()}";
            }
            #endregion
        }

        /// <summary>
        /// <see cref="MAP"/> base info data and logic shared with <see cref="FPG.MAPRegister"/> class.
        /// </summary>
        public class BaseInfo
        {
            #region Constants
            public const int GRAPHID_MIN = 1;
            public const int GRAPHID_MAX = 999;
            public const int DESCRIPTION_LENGTH = 32;
            public const int FILENAME_LENGTH = 12;
            #endregion

            #region Internal vars
            readonly bool _isFPG;
            int _graphId;
            string _description;
            string _filename;

            #endregion

            #region Properties
            public short Width { get; set; }
            public short Height { get; set; }
            public int GraphId
            {
                get
                {
                    return this._graphId;
                }
                set
                {
                    if (!value.IsClamped(BaseInfo.GRAPHID_MIN, BaseInfo.GRAPHID_MAX))
                    {
                        throw new ArgumentOutOfRangeException($"GraphId must be a value between {BaseInfo.GRAPHID_MIN} and {BaseInfo.GRAPHID_MAX}.");
                    }

                    this._graphId = value;
                }
            }
            public string Description
            {
                get
                {
                    return this._description;
                }
                set
                {
                    this._description = value.GetFixedLengthString(BaseInfo.DESCRIPTION_LENGTH);
                }
            }
            public string Filename
            {
                get
                {
                    return this._filename;
                }
                set
                {
                    this._filename = value.GetFixedLengthString(BaseInfo.FILENAME_LENGTH);
                }
            }
            public int Length { get; set; }
            #endregion

            #region Constructors
            public BaseInfo()
            {
                this._isFPG = false;
            }

            public BaseInfo(BinaryReader file, bool isFPG)
            {
                this._isFPG = isFPG;

                if (!isFPG)
                {
                    // MAP format order:
                    this.Width = file.ReadInt16();
                    this.Height = file.ReadInt16();
                    this.GraphId = file.ReadInt32();
                    this.Description = file.ReadBytes(BaseInfo.DESCRIPTION_LENGTH).GetNullTerminatedASCIIString();
                }
                else
                {
                    // FPG format order:
                    this.GraphId = file.ReadInt32();
                    this.Length = file.ReadInt32();
                    this.Description = file.ReadBytes(BaseInfo.DESCRIPTION_LENGTH).GetNullTerminatedASCIIString();
                    this.Filename = this.Description = file.ReadBytes(BaseInfo.FILENAME_LENGTH).GetNullTerminatedASCIIString();
                    this.Width = file.ReadInt16();
                    this.Height = file.ReadInt16();
                }
            }

            public BaseInfo(short width, short height, int graphId, string description)
            {
                this._isFPG = false;

                this.Width = width;
                this.Height = height;
                this.GraphId = graphId;
                this.Description = description;
            }

            public BaseInfo(short width, short height, int graphId, string description, string filename, int length) : this(width, height, graphId, description)
            {
                this._isFPG = true;

                this.Filename = filename;
                this.Length = length;
            }
            #endregion

            #region Methods & Functions
            public void Write(BinaryWriter file)
            {
                if (!this._isFPG)
                {
                    // MAP format order:
                    file.Write(this.Width);
                    file.Write(this.Height);
                    file.Write(this.GraphId);
                    file.Write(this.Description.GetASCIIBytes());
                }
                else
                {
                    // FPG format order:
                    file.Write(this.GraphId);
                    file.Write(this.Length);
                    file.Write(this.Description.GetASCIIBytes());
                    file.Write(this.Filename.GetASCIIBytes());
                    file.Write(this.Width);
                    file.Write(this.Height);
                }
            }

            public override string ToString()
            {
                return !this._isFPG ?
                       $"- Width: {this.Width}\n- Height: {this.Height}\n- Graph Id: {this.GraphId}\n- Description: {this.Description}\n" :
                       $"- Graph Id: {this.GraphId}\n- Length: {this.Length}\n- Description: {this.Description}\n- Filename: {this.Filename}\n- Width: {this.Width}\n- Height: {this.Height}\n";
            }
            #endregion
        }

        public class ControlPointList
        {
            #region Internal vars
            List<ControlPoint> _points;
            #endregion

            #region Properties
            public int Length => this._points.Count;

            public ControlPoint this[int index]
            {
                get { return this._points[index]; }
                set { this._points[index] = value; }
            }
            #endregion

            #region Constructor
            public ControlPointList()
            {
                this._points = new List<ControlPoint>();
            }

            /// <summary>
            /// Read the control point array.
            /// </summary>
            /// <param name="file"><see cref="BinaryReader"/> instance of the MAP or FPG file format that contain control point array data.</param>
            /// <remarks>The <see cref="BinaryReader"/> instance must be setup in the byte of the control point counter value.</remarks>
            public ControlPointList(BinaryReader file)
            {
                this._points = new List<ControlPoint>(file.ReadInt16());

                for (int i = 0; i < this._points.Capacity; i++)
                {
                    this._points.Add(new ControlPoint(file));
                }
            }
            #endregion

            #region Methods & Functions
            public void Add(short x, short y)
            {
                this._points.Add(new ControlPoint() { x = x, y = y });
            }

            public void Remove(int index)
            {
                this._points.RemoveAt(index);
            }

            public void Write(BinaryWriter file)
            {
                file.Write((short)this._points.Count);

                foreach (var point in this._points)
                {
                    point.Write(file);
                }
            }

            public override string ToString()
            {
                if (this._points.Count == 0)
                {
                    return "The MAP not contain control points.";
                }

                var sb = new StringBuilder();
                sb.AppendLine($"Control points:");

                int column = 0;
                for (int i = 0; i < this._points.Count; i++)
                {
                    sb.Append($"{i:000}:{this._points[i].ToString()} ");
                    if (++column == 6)
                    {
                        column = 0;
                        sb.AppendLine();
                    }
                }

                sb.AppendLine();

                return sb.ToString();
            } 
            #endregion
        }

        /// <summary>
        /// Represents the all pixels from a MAP.
        /// </summary>
        public class Bitmap
        {
            #region Internal vars
            byte[] _pixels;
            #endregion

            #region Properties
            public byte this[int index]
            {
                get { return this._pixels[index]; }
                set { this._pixels[index] = value; }
            }

            public byte this[int x, int y]
            {
                get { return this._pixels[this.GetIndex(x, y)]; }
                set { this._pixels[this.GetIndex(x, y)] = value; }
            }

            public int Width { get; private set; }
            public int Height { get; private set; }
            public int Count => this._pixels.Length;
            #endregion

            #region Constructors
            public Bitmap(int width, int height)
            {
                this.Width = width;
                this.Height = height;
                this._pixels = new byte[width * height];
            }

            public Bitmap(int width, int height, byte[] pixels) : this(width, height)
            {
                if (this._pixels.Length == pixels.Length)
                {
                    this._pixels = pixels;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(pixels), $"The {nameof(pixels)} array length not match with the Bitmap size ({this._pixels.Length}).");
                }
            }

            /// <summary>
            /// Reads all pixels from a MAP file.
            /// </summary>
            /// <param name="width">Width of the MAP.</param>
            /// <param name="height">Height of the MAP.</param>
            /// <param name="file"><see cref="BinaryReader"/> instance of the MAP or FPG file format that contain pixel array data.</param>
            /// <remarks>The <see cref="BinaryReader"/> instance must be setup in the byte of the first pixel.</remarks>
            public Bitmap(int width, int height, BinaryReader file) : this(width, height)
            {
                this._pixels = file.ReadBytes(this._pixels.Length);
            }
            #endregion

            #region Methods & Functions
            int GetIndex(int x, int y)
            {
                return (this.Width * y) + x;
            }

            public void Write(BinaryWriter file)
            {
                file.Write(this._pixels);
            } 

            public byte[] ToByteArray()
            {
                return this._pixels;
            }
            #endregion
        }
        #endregion

        #region Internal vars
        Header _header;
        #endregion

        #region Properties
        public short Width => this._header.Width;
        public short Height => this._header.Height;
        public int GraphId { get { return this._header.GraphId; } set { this._header.GraphId = value; } }
        public string Description { get { return this._header.Description; } set { this._header.Description = value; } }
        public PAL Palette { get; private set; }
        public ControlPointList ControlPoints { get; private set; }
        public Bitmap Pixels { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new <see cref="MAP"/> empty instance.
        /// </summary>
        public MAP()
        {
            this._header = new Header();
            this.ControlPoints = new ControlPointList();
        }

        /// <summary>
        /// Import a <see cref="MAP"/> from a <see cref="BinaryReader"/> stream.
        /// </summary>
        /// <param name="file"><see cref="BinaryReader"/> instance.</param>
        /// <param name="fromFPG">Indicates if <see cref="MAP"/> is readed from a <see cref="FPG"/>. By default is <see cref="false"/>.</param>
        /// <param name="verbose">Log <see cref="MAP"/> import data in console. By default is <see cref="true"/>.</param>
        public MAP(BinaryReader file, bool fromFPG = false, bool verbose = true)
        {
            if (!fromFPG)
            {
                this._header = new Header(file);
                Helper.Log(this._header.ToString(), verbose); 
            }

            if (this._header.Check() || fromFPG)
            {
                if (!fromFPG)
                {
                    this.Palette = new PAL(file, true, verbose);
                    Helper.Log(this.Palette.ToString(), verbose); 
                }

                this.ControlPoints = new ControlPointList(file);
                Helper.Log(this.ControlPoints.ToString(), verbose);

                this.Pixels = new Bitmap(this._header.Width, this._header.Height, file);
                Helper.Log($"Readed {this.Pixels.Count} pixels in MAP.", verbose);
            }
            else
            {
                throw new FormatException("Invalid MAP file!");
            }
        }

        /// <summary>
        /// Import a <see cref="MAP"/> file.
        /// </summary>
        /// <param name="filename"><see cref="MAP"/> file.</param>
        /// <param name="verbose">Log <see cref="MAP"/> import data in console. By default is <see cref="true"/>.</param>
        public MAP(string filename, bool verbose = true) : this(new BinaryReader(File.OpenRead(filename)), false, verbose)
        {
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Import an external <see cref="PAL"/> file to use in this <see cref="MAP"/>.
        /// </summary>
        /// <param name="filename"><see cref="PAL"/> file to import.</param>
        public void ImportPalette(string filename)
        {
            this.Palette = new PAL(filename, true, false);
        }

        /// <summary>
        /// Convert PNG to <see cref="PCX"/> format in memory and import it as <see cref="MAP"/> format.
        /// </summary>
        /// <param name="pngfile">PNG file to convert to 256 color indexed <see cref="PCX"/> image.</param>
        public void ImportPNG(string pngfile)
        {
            // Load PNG image and convert to PCX using ImageMagick framework:
            using (MagickImage png = new MagickImage(pngfile))
            {
                png.ColorType = ColorType.Palette;
                png.Format = MagickFormat.Pcx;

                // Load the PCX image using custom importer and get color palette and uncompreseed pixel data:
                var pcx = new PCX(new MagickImage(png.ToByteArray()).ToByteArray(), false);
                {
                    this._header.Width = pcx.Width;
                    this._header.Height = pcx.Height;
                    this.Palette = new PAL(pcx, true);
                    this.Pixels = new Bitmap(this._header.Width, this._header.Height, pcx.Pixels);
                }
            }
        }

        /// <summary>
        /// Write all data in a file.
        /// </summary>
        /// <param name="filename"><see cref="MAP"/> filename.</param>
        public void Write(string filename)
        {
            if (this._header is null) throw new InvalidOperationException("Header is not initialized.");
            if (this.Palette is null) throw new InvalidOperationException("Palette is not initialized.");

            this.Write(new BinaryWriter(File.OpenWrite(filename)));
        }

        /// <summary>
        /// Write all data in a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="filename"><see cref="BinaryWriter"/> instance.</param>
        public void Write(BinaryWriter file)
        {
            this._header?.Write(file);
            this.Palette?.Write(file);
            this.ControlPoints.Write(file);
            this.Pixels.Write(file);
        }

        public override string ToString()
        {
            return $"{this._header?.ToString()}\n{this.Palette?.ToString()}\n{this.ControlPoints.ToString()}\n{this.Pixels.Count} pixels stored.";
        }
        #endregion
    }
}
