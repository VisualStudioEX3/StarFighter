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
        #endregion

        #region Internal vars
        Header _header;
        PAL.ColorPallete _pallete;
        PAL.ColorRangeTable _colorRanges;
        ControlPointList _controlPoints;
        byte[] _pixels;
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
        public PAL.ColorPallete Pallete => this._pallete;
        public PAL.ColorRangeTable Ranges => this._colorRanges;
        public ControlPointList ControlPoints => this._controlPoints;
        public ReadOnlyMemory<byte> Pixels => this._pixels;
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
                    this._pallete = new PAL.ColorPallete(file);
                    Helper.Log(this._pallete.ToString(), verbose);

                    this._colorRanges = new PAL.ColorRangeTable(file);
                    Helper.Log(this._colorRanges.ToString(), verbose);

                    this._controlPoints = new ControlPointList(file);
                    Helper.Log(this._controlPoints.ToString(), verbose);

                    this._pixels = file.ReadBytes(this._header.width * this._header.height);
                    Helper.Log($"Readed {this._pixels.Length} pixels in MAP.", verbose);
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
        public void ImportPallete(string filename)
        {
            var pal = new PAL(filename, false);
            this._pallete = pal.Pallete;
            this._colorRanges = pal.Ranges;
        }

        /// <summary>
        /// Convert PNG to PCX format in memory and import it as MAP format.
        /// </summary>
        /// <param name="pcxfile">PNG file to convert to 256 color indexed PCX image.</param>
        /// <param name="palfile">PAL file uses to adapt PCX pallete.</param>
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
                    this._pixels = new byte[this._header.width * this._header.height];

                    int writeIndex = 0;
                    byte[] pixels = pcx.GetPixels().ToArray(); // Returns a 4-color-component array. The indexed value is stored in the 3rd byte of each 4-component group.
                    for (int readIndex = 0; readIndex < pixels.Length; readIndex += 4)
                    {
                        this._pixels[writeIndex] = pixels[readIndex + 2];
                        writeIndex++;
                    }

                    this.ImportPallete(palfile); // Imports external PAL file and stored in MAP structure.
                    this._pixels = PAL.Convert(this._pixels, PAL.ColorPallete.ReadPalleteFromPCXFile(pcx.ToByteArray()), this._pallete);

                    // TODO: Not need to change the PCX pallete, need to changes the pixel pallete references to DIV pallete, using equivalences between 2 palletes.
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
                this._pallete.Write(file);
                this._colorRanges.Write(file);
                this._controlPoints.Write(file);
                file.Write(this._pixels);
            }
        }
        #endregion
    }
}
