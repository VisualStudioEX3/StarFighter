using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DIV2.Format.Exporter.MethodExtensions;
using Newtonsoft.Json;

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
            palette.Write(file);
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
        #region Internal vars
        MAPHeader _header;
        byte[] _bitmap;
        #endregion

        #region Properties
        /// <summary>
        /// <see cref="PAL"/> instance used by this <see cref="MAP"/>.
        /// </summary>
        public PAL Palette { get; }
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
        /// Imports a PNG file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="pngFilename">PNG filename to export.</param>
        /// <param name="palFilename"><see cref="PAL"/> filename to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        public MAP(string pngFilename, string palFilename, int graphId) : this(pngFilename, palFilename, graphId, string.Empty)
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="pngFilename">PNG filename to export.</param>
        /// <param name="palette"><see cref="PAL"/> instance to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        public MAP(string pngFilename, PAL palette, int graphId) : this(pngFilename, palette, graphId, string.Empty)
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="pngFilename">PNG filename to export.</param>
        /// <param name="palFilename"><see cref="PAL"/> filename to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Description (32 characters maximum).</param>
        public MAP(string pngFilename, string palFilename, int graphId, string description) : this(pngFilename, palFilename, graphId, description, new ControlPoint[0])
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="pngFilename">PNG filename to export.</param>
        /// <param name="palette"><see cref="PAL"/> instance to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Description (32 characters maximum).</param>
        public MAP(string pngFilename, PAL palette, int graphId, string description) : this(pngFilename, palette, graphId, description, new ControlPoint[0])
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="pngFilename">PNG filename to export.</param>
        /// <param name="palFilename"><see cref="PAL"/> filename to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Description (32 characters maximum).</param>
        /// <param name="controlPoints"><see cref="MAP"/> Control Point list.</param>
        public MAP(string pngFilename, string palFilename, int graphId, string description, ControlPoint[] controlPoints) : this(pngFilename, new PAL(palFilename), graphId, description, controlPoints)
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to <see cref="MAP"/> format.
        /// </summary>
        /// <param name="pngFilename">PNG filename to export.</param>
        /// <param name="palette"><see cref="PAL"/> instance to use with this <see cref="MAP"/>.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Description (32 characters maximum).</param>
        /// <param name="controlPoints"><see cref="MAP"/> Control Point list.</param>
        public MAP(string pngFilename, PAL palette, int graphId, string description, ControlPoint[] controlPoints) : this()
        {
            this.Palette = palette;
            PNG2BMP.SetupBMPEncoder(this.Palette);

            short width, height;
            PNG2BMP.Convert(pngFilename, out this._bitmap, out width, out height);

            this._header.Width = width;
            this._header.Height = height;
            this._header.GraphId = graphId;
            this._header.Description = description;
            this._header.ControlPoints = new List<ControlPoint>(controlPoints);
        }
        #endregion

        #region Methods & Functions
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

        /// <summary>
        /// Validates if the file is a valid <see cref="MAP"/> file.
        /// </summary>
        /// <param name="filename"><see cref="MAP"/> filename.</param>
        /// <returns>Returns true if the file contains a valid <see cref="MAP"/> header format.</returns>
        public static bool Validate(string filename)
        {
            using (var file = new BinaryReader(File.OpenRead(filename)))
            {
                return new MAP().Validate(file);
            }
        }
        #endregion
    }
    #endregion
}
