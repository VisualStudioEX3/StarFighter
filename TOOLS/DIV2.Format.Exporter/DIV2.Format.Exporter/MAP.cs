using System;
using System.Collections.Generic;
using System.IO;
using DIV2.Format.Exporter.MethodExtensions;

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
    }
    #endregion

    #region Class
    /// <summary>
    /// MAP exporter.
    /// </summary>
    public class MAP : DIVFormatCommonBase
    {
        #region Internal vars
        short _width;
        short _height;
        int _graphId;
        string _description;
        List<ControlPoint> _controlPoints;
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
        public IReadOnlyList<ControlPoint> ControlPoints => this._controlPoints;
        #endregion

        #region Constructor
        internal MAP() : base("map")
        {
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
            PNG2BMP.Convert(pngFilename, out this._bitmap, out this._width, out this._height);

            this._graphId = graphId;
            this._description = description;
            this._controlPoints = new List<ControlPoint>(controlPoints);
        }
        #endregion

        #region Methods & Functions
        void WriteControlPoints(BinaryWriter file)
        {
            file.Write((short)this._controlPoints.Count);
            foreach (var point in this._controlPoints)
            {
                file.Write(point.x);
                file.Write(point.y);
            }
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
            this._controlPoints.Add(point);
        }

        /// <summary>
        /// Removes a <see cref="ControlPoint"/> data of this <see cref="MAP"/>.
        /// </summary>
        /// <param name="index"><see cref="ControlPoint"/> index.</param>
        public void RemoveControlPoint(int index)
        {
            this._controlPoints.RemoveAt(index);
        }

        /// <summary>
        /// Removes all <see cref="ControlPoint"/>s of this <see cref="MAP"/>.
        /// </summary>
        public void RemoveAllControlPoints()
        {
            this._controlPoints.Clear();
        }

        /// <summary>
        /// Write all data to file.
        /// </summary>
        /// <param name="filename"><see cref="MAP"/> filename.</param>
        public override void Write(BinaryWriter file)
        {
            base.Write(file);

            file.Write(this._width);
            file.Write(this._height);
            file.Write(this._graphId);
            file.Write(this._description.GetASCIIZString(32));

            this.Palette.Write(file);

            this.WriteControlPoints(file);
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
