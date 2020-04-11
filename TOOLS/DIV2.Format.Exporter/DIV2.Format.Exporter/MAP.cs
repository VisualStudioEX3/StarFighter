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
    }
    #endregion

    #region Class
    /// <summary>
    /// MAP exporter.
    /// </summary>
    public class MAP
    {
        #region Constants
        const string HEADER_ID = "map"; 
        #endregion

        #region Internal vars
        short _width;
        short _height;
        int _graphId;
        string _description;
        byte[] _palette;
        List<ControlPoint> _controlPoints;
        byte[] _bitmap;
        #endregion

        #region Properties
        /// <summary>
        /// <see cref="ControlPoint"/> list of this MAP.
        /// </summary>
        public IReadOnlyList<ControlPoint> ControlPoints => this._controlPoints; 
        #endregion

        #region Constructor
        /// <summary>
        /// Imports a PNG file and convert to MAP format.
        /// </summary>
        /// <param name="pngFilename">PNG filename to export.</param>
        /// <param name="graphId">MAP graphic id.</param>
        public MAP(string pngFilename, int graphId) : this(pngFilename, graphId, string.Empty)
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to MAP format.
        /// </summary>
        /// <param name="pngFilename">PNG filename to export.</param>
        /// <param name="graphId">MAP graphic id.</param>
        /// <param name="description">Description (32 characters maximum).</param>
        public MAP(string pngFilename, int graphId, string description) : this(pngFilename, graphId, description, new ControlPoint[0])
        {
        }

        /// <summary>
        /// Imports a PNG file and convert to MAP format.
        /// </summary>
        /// <param name="pngFilename">PNG filename to export.</param>
        /// <param name="graphId">MAP graphic id.</param>
        /// <param name="description">Description (32 characters maximum).</param>
        /// <param name="controlPoints">MAP Control Point list.</param>
        public MAP(string pngFilename, int graphId, string description, ControlPoint[] controlPoints)
        {
            var pcx = new PCX(pngFilename, false, true);

            this._width = pcx.Width;
            this._height = pcx.Height;
            this._graphId = graphId;
            this._description = description;
            this._palette = pcx.Palette;
            this._controlPoints = new List<ControlPoint>(controlPoints);
            this._bitmap = pcx.Bitmap;
        }
        #endregion

        #region Methods & Functions
        void WriteHeader(BinaryWriter file)
        {
            DIVFormatCommonBase.WriteCommonHeader(file, MAP.HEADER_ID);
            file.Write(this._width);
            file.Write(this._height);
            file.Write(this._graphId);
            file.Write(this._description.GetASCIIZString(32));
        }
        
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
        /// Adds a <see cref="ControlPoint"/> to this MAP.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public void AddControlPoint(short x, short y)
        {
            this.AddControlPoint(new ControlPoint() { x = x, y = y });
        }

        /// <summary>
        /// Adds a <see cref="ControlPoint"/> to this MAP.
        /// </summary>
        /// <param name="point"><see cref="ControlPoint"/> data.</param>
        public void AddControlPoint(ControlPoint point)
        {
            this._controlPoints.Add(point);
        }

        /// <summary>
        /// Removes a <see cref="ControlPoint"/> data of this MAP.
        /// </summary>
        /// <param name="index"><see cref="ControlPoint"/> index.</param>
        public void RemoveControlPoint(int index)
        {
            this._controlPoints.RemoveAt(index);
        }

        /// <summary>
        /// Removes all <see cref="ControlPoint"/>s of this MAP.
        /// </summary>
        public void RemoveAllControlPoints()
        {
            this._controlPoints.Clear();
        }

        /// <summary>
        /// Write all data to file.
        /// </summary>
        /// <param name="filename">MAP filename.</param>
        public void Save(string filename)
        {
            using (var file = new BinaryWriter(File.OpenWrite(filename)))
            {
                this.WriteHeader(file);
                DIVFormatCommonBase.WritePalette(file, this._palette);
                this.WriteControlPoints(file);
                file.Write(this._bitmap);
            }
        } 
        #endregion
    } 
    #endregion
}
