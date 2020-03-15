using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImageMagick;

namespace DIV2Tools
{
    /// <summary>
    /// MAP file.
    /// </summary>
    public class MAP
    {
        #region Constants
        const string HEADER_ID = "map";
        static readonly byte[] HEADER_SIGNATURE = { 0x1A, 0x0D, 0x0A, 0x00 };
        const byte HEADER_VERSION = 0;
        #endregion

        #region Structs
        struct Header // 48 bytes.
        {
            #region Public vars
            public char[] id;           // 3 bytes.
            public byte[] signature;    // 4 bytes.
            public byte version;
            
            public short width;         // 1 word (2 bytes)
            public short height;        // 1 word (2 bytes)
            public int graphId;         // 1 doble word (4 bytes)
            public string description;  // 32 bytes (ASCII)
            #endregion

            #region Constructor
            public Header(BinaryReader file)
            {
                this.id = file.ReadChars(3);
                this.signature = file.ReadBytes(4);
                this.version = file.ReadByte();

                this.width = file.ReadInt16();
                this.height = file.ReadInt16();
                this.graphId = file.ReadInt32();
                this.description = Helper.GetNullTerminatedASCIIString(file.ReadBytes(32));
            }
            #endregion

            #region Methods & Functions
            public bool Check()
            {
                return new string(this.id).Equals(MAP.HEADER_ID) &&
                       BitConverter.ToUInt32(this.signature) == BitConverter.ToUInt32(MAP.HEADER_SIGNATURE) &&
                       this.version == MAP.HEADER_VERSION;
            }

            public void Write(BinaryWriter file)
            {
                file.Write(Encoding.ASCII.GetBytes(MAP.HEADER_ID));
                file.Write(MAP.HEADER_SIGNATURE);
                file.Write(MAP.HEADER_VERSION);

                file.Write(this.width);
                file.Write(this.height);
                file.Write(this.graphId);
                file.Write(Encoding.ASCII.GetBytes(this.description), 0, 32);
            }

            public override string ToString()
            {
                return $"MAP Header:\n- Id: {new string(this.id)}\n- Signature: {BitConverter.ToString(this.signature)}\n- Version: {this.version}\n- Width: {this.width}\n- Height: {this.height}\n- Graph Id: {this.graphId}\n- Description: {this.description}\n";
            }
            #endregion
        }

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
        PAL.ColorPalette _palette;
        PAL.ColorRangeTable _colorRanges;
        ControlPointList _controlPoints;
        Bitmap _pixels;
        #endregion

        #region Properties
        public short Width => this._header.width;
        public short Height => this._header.height;
        public int GraphId
        {
            get
            { 
                return this._header.graphId; 
            }
            set
            {
                if (value < 1 || value > 999)
                {
                    throw new ArgumentOutOfRangeException("The GraphID must be a value between 1 and 999.");
                }

                this._header.graphId = value;
            }
        }
        public string Description 
        { 
            get
            {
                return this._header.description;
            }
            set
            {
                string description = value.Length > 32 ? value.Substring(0, 32) : value;
                this._header.description = value.PadRight(32);
            }
        }
        public PAL.ColorPalette Palette => this._palette;
        public PAL.ColorRangeTable Ranges => this._colorRanges;
        public ControlPointList ControlPoints => this._controlPoints;
        public Bitmap Pixels => this._pixels;
        #endregion

        #region Constructor
        public MAP()
        {
            this._controlPoints = new ControlPointList();
        }

        /// <summary>
        /// Import a MAP file.
        /// </summary>
        /// <param name="filename">MAP file.</param>
        /// <param name="verbose">Log MAP import data in console. By default is true.</param>
        public MAP(string filename, bool verbose = true)
        {
            using (var file = new BinaryReader(File.OpenRead(filename)))
            {
                Helper.Log($"Reading \"{filename}\"...\n", verbose);

                this._header = new Header(file);
                Helper.Log(this._header.ToString(), verbose);

                if (this._header.Check())
                {
                    this._palette = new PAL.ColorPalette(file);
                    Helper.Log(this._palette.ToString(), verbose);

                    this._colorRanges = new PAL.ColorRangeTable(file);
                    Helper.Log(this._colorRanges.ToString(), verbose);

                    this._controlPoints = new ControlPointList(file);
                    Helper.Log(this._controlPoints.ToString(), verbose);

                    this._pixels = new Bitmap(this._header.width, this._header.height, file);
                    Helper.Log($"Readed {this._pixels.Count} pixels in MAP.", verbose);
                }
                else
                {
                    throw new FormatException("Invalid MAP file!");
                }
            }
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Import an external PAL file to use in this MAP.
        /// </summary>
        /// <param name="filename">PAL file to import.</param>
        public void ImportPalette(string filename)
        {
            var pal = new PAL(filename, false);
            this._palette = pal.Palette;
            this._colorRanges = pal.Ranges;
        }

        /// <summary>
        /// Convert PNG to PCX format in memory and import it as MAP format.
        /// </summary>
        /// <param name="pcxfile">PNG file to convert to 256 color indexed PCX image.</param>
        /// <param name="palfile">PAL file uses to adapt PCX palette.</param>
        public void ImportPNG(string pcxfile, string palfile)
        {
            using (MagickImage png = new MagickImage(pcxfile))
            {
                png.ColorType = ColorType.Palette;
                png.Format = MagickFormat.Pcx;

                using (MagickImage pcx = new MagickImage(png.ToByteArray()))
                {
                    this._header.width = (short)pcx.Width;
                    this._header.height = (short)pcx.Height;
                    this._pixels = new Bitmap(this._header.width, this._header.height);

                    int writeIndex = 0;
                    byte[] pixels = pcx.GetPixels().ToArray(); // Returns a 4-color-component array. The indexed value is stored in the 3rd byte of each 4-component group.
                    for (int readIndex = 0; readIndex < pixels.Length; readIndex += 4)
                    {
                        this._pixels[writeIndex] = pixels[readIndex + 2];
                        writeIndex++;
                    }

                    this.ImportPalette(palfile); // Imports external PAL file and stored in MAP structure.
                    this._pixels = PAL.Convert(this._pixels, PAL.ColorPalette.ReadpaletteFromPCXFile(pcx.ToByteArray()), this._palette);
                }
            }
        }

        /// <summary>
        /// Write all data in a file.
        /// </summary>
        /// <param name="filename">MAP filename.</param>
        public void Write(string filename)
        {
            using (var file = new BinaryWriter(File.OpenWrite(filename)))
            {
                this._header.Write(file);
                this._palette.Write(file);
                this._colorRanges.Write(file);
                this._controlPoints.Write(file);
                this._pixels.Write(file);
            }
        }
        #endregion
    }
}
