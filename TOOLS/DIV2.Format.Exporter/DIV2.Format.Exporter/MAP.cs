using DIV2.Format.Exporter.Converters;
using DIV2.Format.Exporter.MethodExtensions;
using DIV2.Format.Exporter.Processors;
using DIV2.Format.Importer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DIV2.Format.Exporter
{
    #region Structures
    /// <summary>
    /// Control Point data.
    /// </summary>
    [Serializable]
    public struct ControlPoint
    {
        #region Public vars
        public short x, y;
        #endregion

        #region Constructors
        public ControlPoint(short x, short y)
        {
            this.x = x;
            this.y = y;
        }

        public ControlPoint(int x, int y)
        {
            this.x = (short)x;
            this.y = (short)y;
        }

        public ControlPoint(float x, float y)
        {
            this.x = (short)x;
            this.y = (short)y;
        }

        public ControlPoint(double x, double y)
        {
            this.x = (short)x;
            this.y = (short)y;
        }
        #endregion

        #region Methods & Functions
        public void Write(BinaryWriter file)
        {
            file.Write(this.x);
            file.Write(this.y);
        }
        #endregion
    }

    public static class ControlPointExtensions
    {
        /// <summary>
        /// Serialize the <see cref="ControlPoint"/> list as DIV formats expected.
        /// </summary>
        /// <typeparam name="T">Type of the counter value must be writed. Only <see cref="short"/> and <see cref="int"/> types are valid.</typeparam>
        /// <param name="points"><see cref="ControlPoint"/> list to serialize.</param>
        /// <param name="file"><see cref="BinaryWriter"/> instance.</param>
        internal static void Write<T>(this IEnumerable<ControlPoint> points, BinaryWriter file) where T : struct
        {
            if (typeof(T) != typeof(short) ||
                typeof(T) != typeof(int))
            {
                throw new ArgumentException($"Invalid type for counter. Only short and int types are valid.");
            }

            file.Write(typeof(T) != typeof(short) ? (short)points.Count() : points.Count());
            foreach (var point in points)
            {
                point.Write(file);
            }
        }
    }

    /// <summary>
    /// MAP metadata.
    /// </summary>
    [Serializable]
    public class MAPHeader
    {
        #region Constants
        const string OUT_OF_RANGE_EXCEPTION_MESSAGE = "The \"{0}\" property must be a value between {1} and {2}.";
        const short MIN_PIXEL_SIZE = 1;

        public const int SIZE = 40;
        public const int MIN_GRAPHID = 1;
        public const int MAX_GRAPHID = 999;
        public const int DESCRIPTION_LENGTH = 32;
        #endregion

        #region Internal vars
        short _width;
        short _height;
        int _graphId;
        #endregion

        #region Properties
        ArgumentOutOfRangeException FormatException(string parameter, int min, int max)
        {
            return new ArgumentOutOfRangeException(string.Format(MAPHeader.OUT_OF_RANGE_EXCEPTION_MESSAGE, parameter, min, max));
        }

        /// <summary>
        /// <see cref="MAP"/> width.
        /// </summary>
        public short Width
        {
            get => this._width;
            set
            {
                if (!value.IsClamped(MAPHeader.MIN_PIXEL_SIZE, short.MaxValue))
                {
                    throw this.FormatException(nameof(Width), MAPHeader.MIN_PIXEL_SIZE, short.MaxValue);
                }
                this._width = value;
            }
        }

        /// <summary>
        /// <see cref="MAP"/> height.
        /// </summary>
        public short Height
        {
            get => this._height;
            set
            {
                if (!value.IsClamped(MAPHeader.MIN_PIXEL_SIZE, short.MaxValue))
                {
                    throw this.FormatException(nameof(Height), MAPHeader.MIN_PIXEL_SIZE, short.MaxValue);
                }
                this._height = value;
            }
        }

        /// <summary>
        /// <see cref="MAP"/> Graph Id, a value between 1 and 999, used only on FPGs.
        /// </summary>
        public int GraphId
        {
            get => this._graphId;
            set
            {
                if (!value.IsClamped(MAPHeader.MIN_GRAPHID, MAPHeader.MAX_GRAPHID))
                {
                    throw this.FormatException(nameof(GraphId), MAPHeader.MIN_GRAPHID, MAPHeader.MAX_GRAPHID);
                }
                this._graphId = value;
            }
        }


        /// <summary>
        /// Optional <see cref="MAP"/> description. Maximum 32 characteres.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optional list with <see cref="MAP"/> <see cref="ControlPoint"/>s.
        /// </summary>
        public List<ControlPoint> ControlPoints { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create new instance with default values.
        /// </summary>
        public MAPHeader()
        {
            this.Width = this.Height = MAPHeader.MIN_PIXEL_SIZE;
            this.GraphId = MAPHeader.MIN_GRAPHID;
            this.ControlPoints = new List<ControlPoint>();
        }

        /// <summary>
        /// Create new instance.
        /// </summary>
        /// <param name="width"><see cref="MAP"/> width.</param>
        /// <param name="height"><see cref="MAP"/> height.</param>
        /// <param name="graphId"><see cref="MAP"/> Graph ID, a value between 1 and 999, used only on FPGs.</param>
        /// <param name="description">Optional <see cref="MAP"/> description. Maximum 32 characteres.</param>
        /// <param name="controlPoints">Optional list with <see cref="MAP"/> <see cref="ControlPoint"/>s.</param>
        public MAPHeader(short width, short height, int graphId, string description = "", ControlPoint[] controlPoints = null)
        {
            this.Width = width;
            this.Height = height;
            this.GraphId = graphId;
            this.Description = description;
            if (controlPoints != null)
            {
                this.ControlPoints = new List<ControlPoint>(controlPoints);
            }
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Create new instance from <see cref="MAP"/> file.
        /// </summary>
        /// <param name="filename"><see cref="MAP"/> file to import.</param>
        /// <returns>Returns the new instance of <see cref="MAPHeader"/> from <see cref="MAP"/> data.</returns>
        public static MAPHeader FromMAP(string filename)
        {
            try
            {
                return MAPHeader.FromMAP(File.ReadAllBytes(filename));
            }
            catch (Exception)
            {
                throw new FormatException($"The file \"{filename}\" not contain a valid DIV MAP image.");
            }
        }

        /// <summary>
        /// Create new instance from <see cref="MAP"/> file.
        /// </summary>
        /// <param name="buffer"><see cref="MAP"/> file data to import.</param>
        /// <returns>Returns the new instance of <see cref="MAPHeader"/> from <see cref="MAP"/> data.</returns>
        public static MAPHeader FromMAP(byte[] buffer)
        {
            if (new MAP().Validate(buffer))
            {
                using (var reader = new BinaryReader(new MemoryStream(buffer)))
                {
                    var header = new MAPHeader();
                    
                    reader.BaseStream.Position = DIVFormatCommonBase.BASE_HEADER_LENGTH;
                    
                    header.Width = reader.ReadInt16();
                    header.Height = reader.ReadInt16();
                    header.GraphId = reader.ReadInt32();
                    header.Description = reader.ReadBytes(MAPHeader.DESCRIPTION_LENGTH).ToASCIIString();

                    reader.BaseStream.Position += PAL.COLOR_TABLE_LENGTH + PAL.RANGE_TABLE_LENGHT;

                    header.ControlPoints = new List<ControlPoint>(reader.ReadInt32());
                    for (int i = 0; i < header.ControlPoints.Capacity; i++)
                    {
                        header.ControlPoints.Add(new ControlPoint(reader.ReadInt16(), reader.ReadInt16()));
                    }

                    return header;
                }
            }

            throw new FormatException("The buffer data not contain a valid DIV MAP image.");
        }

        /// <summary>
        /// Create new instance from JSON data.
        /// </summary>
        /// <param name="data">JSON data to import.</param>
        /// <returns>Returns the new instance of <see cref="MAPHeader"/> from JSON data.</returns>
        public static MAPHeader FromJSON(string data)
        {
            return JsonConvert.DeserializeObject<MAPHeader>(data);
        }

        /// <summary>
        /// Serialized this instance to JSON format.
        /// </summary>
        /// <returns>Returns the <see cref="MAPHeader"/> instance serialized as JSON data.</returns>
        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        internal void Write(BinaryWriter file, PAL palette)
        {
            file.Write(this.Width);
            file.Write(this.Height);
            file.Write(this.GraphId);
            file.Write(this.Description.GetASCIIZString(MAPHeader.DESCRIPTION_LENGTH));
            palette.WriteEmbebed(file);
            this.ControlPoints.Write<short>(file);
        }
        #endregion
    }
    #endregion

    #region Class
    /// <summary>
    /// MAP exporter.
    /// </summary>
    public class MAP : DIVFormatCommonBase
    {
        #region Constants
        public const int HEADER_LENGTH = DIVFormatCommonBase.BASE_HEADER_LENGTH + MAPHeader.SIZE;
        #endregion

        #region Internal vars
        MAPHeader _header;
        byte[] _bitmap;
        #endregion

        #region Properties
        /// <summary>
        /// <see cref="PAL"/> instance used by this <see cref="MAP"/>.
        /// </summary>
        public PAL Palette { get; private set; }

        /// <summary>
        /// <see cref="ControlPoint"/> list of this <see cref="MAP"/>.
        /// </summary>
        public IReadOnlyList<ControlPoint> ControlPoints => this._header.ControlPoints;
        #endregion

        #region Constructor
        internal MAP() : base("map")
        {
            this._header = new MAPHeader();
        }

        /// <summary>
        /// Imports a image file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="filename">Image filename to export.</param>
        /// <param name="palFilename"><see cref="PAL"/> filename to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <remarks>Supported formats are JPEG, PNG, BMP, GIF and TGA, and also supported 256 color PCX images.</remarks>
        public MAP(string filename, string palFilename, int graphId) : this(filename, palFilename, graphId, string.Empty)
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="filename">PNG filename to export.</param>
        /// <param name="palette"><see cref="PAL"/> instance to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <remarks>Supported formats are JPEG, PNG, BMP, GIF and TGA, and also supported 256 color PCX images.</remarks>
        public MAP(string filename, PAL palette, int graphId) : this(filename, palette, graphId, string.Empty)
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="filename">PNG filename to export.</param>
        /// <param name="palFilename"><see cref="PAL"/> filename to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Description (32 characters maximum).</param>
        /// <remarks>Supported formats are JPEG, PNG, BMP, GIF and TGA, and also supported 256 color PCX images.</remarks>
        public MAP(string filename, string palFilename, int graphId, string description) : this(filename, palFilename, graphId, description, new ControlPoint[0])
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="filename">PNG filename to export.</param>
        /// <param name="palette"><see cref="PAL"/> instance to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Description (32 characters maximum).</param>
        /// <remarks>Supported formats are JPEG, PNG, BMP, GIF and TGA, and also supported 256 color PCX images.</remarks>
        public MAP(string filename, PAL palette, int graphId, string description) : this(filename, palette, graphId, description, new ControlPoint[0])
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="filename">PNG filename to export.</param>
        /// <param name="palFilename"><see cref="PAL"/> filename to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Description (32 characters maximum).</param>
        /// <param name="controlPoints"><see cref="MAP"/> Control Point list.</param>
        /// <remarks>Supported formats are JPEG, PNG, BMP, GIF and TGA, and also supported 256 color PCX images.</remarks>
        public MAP(string filename, string palFilename, int graphId, string description, ControlPoint[] controlPoints) : this(filename, new PAL(palFilename), graphId, description, controlPoints)
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="filename">PNG filename to export.</param>
        /// <param name="palette"><see cref="PAL"/> instance to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Description (32 characters maximum).</param>
        /// <param name="controlPoints"><see cref="MAP"/> Control Point list.</param>
        /// <remarks>Supported formats are JPEG, PNG, BMP, GIF and TGA, and also supported 256 color PCX images.</remarks>
        public MAP(string filename, PAL palette, int graphId, string description, ControlPoint[] controlPoints) : this()
        {
            byte[] buffer = File.ReadAllBytes(filename);

            this.Palette = palette;

            BMP256Converter.Convert(buffer, out this._bitmap, out short width, out short height, palette);

            this._header = new MAPHeader(width, height, graphId, description, controlPoints);
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Creates a new <see cref="MAP"/> instance using the bitmap and palette data from a PCX image.
        /// </summary>
        /// <param name="filename">The PCX image to import.</param>
        /// <returns>Returns new <see cref="MAP"/> instance from PCX data.</returns>
        /// <remarks>Only supported 256 color PCX images.</remarks>
        public static MAP ImportPCX(string filename)
        {
            try
            {
                return MAP.ImportPCX(File.ReadAllBytes(filename));
            }
            catch (Exception)
            {
                throw new FormatException($"The file \"{filename}\" not contain a valid 256 color PCX image.");
            }
        }

        /// <summary>
        /// Creates a new <see cref="MAP"/> instance using the bitmap and palette data from a PCX image.
        /// </summary>
        /// <param name="buffer">The PCX image data to import.</param>
        /// <returns>Returns new <see cref="MAP"/> instance from PCX data.</returns>
        /// <remarks>Only supported 256 color PCX images.</remarks>
        public static MAP ImportPCX(byte[] buffer)
        {
            if (PCX.IsPCX256(buffer))
            {
                PCX.Import(buffer, out short width, out short height, out byte[] bitmap, out PAL palette);

                return new MAP()
                {
                    _header = new MAPHeader(width, height, MAPHeader.MIN_GRAPHID),
                    Palette = palette,
                    _bitmap = bitmap
                };
            }

            throw new FormatException("The buffer data not contain a valid 256 color PCX image.");
        }

        /// <summary>
        /// Creates a new <see cref="MAP"/> instance using another <see cref="MAP"/> image converting to a new <see cref="PAL"/> colors.
        /// </summary>
        /// <param name="mapFilename"><see cref="MAP"/> file to convert.</param>
        /// <param name="palFilename"><see cref="PAL"/> file used to convert colors.</param>
        /// <returns>Returns a new <see cref="MAP"/> instance with the converted old <see cref="MAP"/> image.</returns>
        public static MAP Convert(string mapFilename, string palFilename)
        {
            try
            {
                return MAP.Convert(File.ReadAllBytes(mapFilename), new PAL(palFilename));
            }
            catch (Exception)
            {
                throw new FormatException($"The file \"{mapFilename}\" not contain a valid DIV MAP image.");
            }
        }

        /// <summary>
        /// Creates a new <see cref="MAP"/> instance using another <see cref="MAP"/> image converting to a new <see cref="PAL"/> colors.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array <see cref="MAP"/> data to convert.</param>
        /// <param name="palette"><see cref="PAL"/> palette used to convert colors.</param>
        /// <returns>Returns a new <see cref="MAP"/> instance with the converted old <see cref="MAP"/> image.</returns>
        public static MAP Convert(byte[] buffer, PAL palette)
        {
            BMP256Converter.Convert(buffer, out byte[] bitmap, out short width, out short height, palette);
            return new MAP()
            {
                _header = MAPHeader.FromMAP(buffer),
                Palette = palette,
                _bitmap = bitmap,
            };

            throw new FormatException($"The buffer data not contain a valid DIV MAP image.");
        }

        /// <summary>
        /// Adds a <see cref="ControlPoint"/> to this <see cref="MAP"/>.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public void AddControlPoint(short x, short y)
        {
            this.AddControlPoint(new ControlPoint() { x = x, y = y });
        }

        /// <summary>
        /// Adds a <see cref="ControlPoint"/> to this <see cref="MAP"/>.
        /// </summary>
        /// <param name="point"><see cref="ControlPoint"/> data.</param>
        public void AddControlPoint(ControlPoint point)
        {
            this._header.ControlPoints.Add(point);
        }

        /// <summary>
        /// Removes a <see cref="ControlPoint"/> data of this <see cref="MAP"/>.
        /// </summary>
        /// <param name="index"><see cref="ControlPoint"/> index.</param>
        public void RemoveControlPoint(int index)
        {
            this._header.ControlPoints.RemoveAt(index);
        }

        /// <summary>
        /// Removes all <see cref="ControlPoint"/>s of this <see cref="MAP"/>.
        /// </summary>
        public void RemoveAllControlPoints()
        {
            this._header.ControlPoints.Clear();
        }

        /// <summary>
        /// Write all data to file.
        /// </summary>
        /// <param name="filename"><see cref="MAP"/> filename.</param>
        internal override void Write(BinaryWriter file)
        {
            base.Write(file);
            this._header.Write(file, this.Palette);
            file.Write(this._bitmap);
        }
        #endregion
    }
    #endregion
}
