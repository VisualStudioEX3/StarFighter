using System;
using System.Collections.Generic;
using System.IO;
using DIV2.Format.Exporter.MethodExtensions;

namespace DIV2.Format.Exporter
{
    #region Structures
    /// <summary>
    /// PNG import definition data.
    /// </summary>
    [Serializable]
    public struct PNGImportDefinition
    {
        #region Properties
        internal bool BinaryLoad { get; private set; }
        /// <summary>
        /// PNG data to import.
        /// </summary>
        public byte[] Buffer { get; private set; }
        /// <summary>
        /// PNG filename to import.
        /// </summary>
        public string Filename { get; private set; }
        /// <summary>
        /// <see cref="MAP"/> graphic id. Must be a be a value between 1 and 999 and must be unique in the FPG.
        /// </summary>
        public int GraphId { get; private set; }
        /// <summary>
        /// Optional <see cref="MAP"/> graphic description (32 characters maximum).
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Optional <see cref="MAP"/> Control Point list.
        /// </summary>
        public ControlPoint[] ControlPoints { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a PNG Import Definition for load buffer data.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array with the PNG data to import.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Optional MAP description (32 characters maximum).</param>
        /// <param name="controlPoints">Optional <see cref="MAP"/> Control Point list.</param>
        public PNGImportDefinition(byte[] buffer, int graphId, string description = "", ControlPoint[] controlPoints = null) :
            this(true, buffer, String.Empty, graphId, description, controlPoints)
        {
        }

        /// <summary>
        /// Creates a PNG Import Definition for load PNG file.
        /// </summary>
        /// <param name="filename">PNG file to import.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Optional MAP description (32 characters maximum).</param>
        /// <param name="controlPoints">Optional <see cref="MAP"/> Control Point list.</param>
        public PNGImportDefinition(string filename, int graphId, string description = "", ControlPoint[] controlPoints = null) :
            this(false, null, filename, graphId, description, controlPoints)
        {
        }

        PNGImportDefinition(bool binaryLoad, byte[] buffer, string filename, int graphId, string description, ControlPoint[] controlPoints)
        {
            this.BinaryLoad = binaryLoad;
            this.Filename = filename;
            this.Buffer = buffer;
            this.GraphId = graphId;
            this.Description = description;
            this.ControlPoints = controlPoints;
        }
        #endregion
    }
    #endregion

    #region Classes
    /// <summary>
    /// FPG creator.
    /// </summary>
    public class FPG : DIVFormatCommonBase
    {
        #region Constants
        public const int HEADER_LENGTH = 64;
        public const int CONTROLPOINT_LENGTH = 4;
        public const int MIN_GRAPHID = 1;
        public const int MAX_GRAPHID = 999;
        public const int DESCRIPTION_LENGTH = 32;
        public const int FILENAME_LENGTH = 12;
        #endregion

        #region Structures
        struct MapRegister
        {
            #region Public vars
            public int graphId;
            public string description;
            public string filename;
            public int width;
            public int height;
            public ControlPoint[] controlPoints;
            public byte[] bitmap;
            #endregion

            #region Methods & Functions
            public int GetSize()
            {
                return FPG.HEADER_LENGTH + (FPG.CONTROLPOINT_LENGTH * controlPoints.Length) + bitmap.Length;
            } 
            #endregion
        }
        #endregion

        #region Internal vars
        List<PNGImportDefinition> _maps;
        #endregion

        #region Properties
        /// <summary>
        /// <see cref="PAL"/> instance used by this <see cref="FPG"/>.
        /// </summary>
        public PAL Palette { get; }
        /// <summary>
        /// All <see cref="PNGImportDefinition"/> setup for this <see cref="FPG"/>.
        /// </summary>
        public IReadOnlyList<PNGImportDefinition> Maps => this._maps;
        #endregion

        #region Constructor
        internal FPG() : base("fpg")
        {
        }

        /// <summary>
        /// Create new <see cref="FPG"/> instance.
        /// </summary>
        /// <param name="palFilename"><see cref="PAL"/> filename to use with this <see cref="FPG"/>.</param>
        public FPG(string palFilename) : this(new PAL(palFilename))
        {
        }

        /// <summary>
        /// Create new <see cref="FPG"/> instance.
        /// </summary>
        /// <param name="palette"><see cref="PAL"/> instance to use with this <see cref="FPG"/>.</param>
        public FPG(PAL palette) : this()
        {
            this.Palette = palette;
            this._maps = new List<PNGImportDefinition>();
        }
        #endregion

        #region Methods & Functions
        void WriteMapRegister(BinaryWriter file, MapRegister register)
        {
            file.Write(register.graphId);
            file.Write(register.GetSize());
            file.Write(register.description.GetASCIIZString(FPG.DESCRIPTION_LENGTH));
            file.Write(register.filename.GetASCIIZString(FPG.FILENAME_LENGTH));
            file.Write(register.width);
            file.Write(register.height);
            file.Write(register.controlPoints.Length);
            foreach (var point in register.controlPoints)
            {
                file.Write(point.x);
                file.Write(point.y);
            }
            file.Write(register.bitmap);
        }

        List<MapRegister> ImportPNGs()
        {
            var mapRegisters = new List<MapRegister>(this._maps.Count);

            PNG2BMP.SetupBMPEncoder(this.Palette);

            foreach (var map in this._maps)
            {
                byte[] pixels;
                short width, height;

                if (map.BinaryLoad)
                {
                    PNG2BMP.Convert(map.Buffer, out pixels, out width, out height);
                }
                else
                {
                    PNG2BMP.Convert(map.Filename, out pixels, out width, out height);
                }

                mapRegisters.Add(new MapRegister()
                {
                    graphId = map.GraphId,
                    description = map.Description,
                    filename = string.IsNullOrEmpty(map.Filename) ? string.Empty : Path.GetFileName(map.Filename),
                    width = width,
                    height = height,
                    controlPoints = map.ControlPoints,
                    bitmap = pixels
                });
            }

            return mapRegisters;
        }

        bool TryGetMapIndexByGraphId(int graphId, out int index)
        {
            for (int i = 0; i < this._maps.Count; i++)
            {
                if (this._maps[i].GraphId == graphId)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        /// <summary>
        /// Adds a PNG definition to import in the <see cref="FPG"/>.
        /// </summary>
        /// <param name="buffer">PNG data to import.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id. Must be a be a value between 1 and 999 and must be unique in the <see cref="FPG"/>.</param>
        /// <param name="description">Optional graphic description. 32 characters maximum.</param>
        /// <param name="controlPoints">Optional <see cref="MAP"/> Control Point list.</param>
        public void AddMap(byte[] buffer, int graphId, string description = "", ControlPoint[] controlPoints = null)
        {
            this.AddMap(new PNGImportDefinition(buffer, graphId, description, controlPoints ?? new ControlPoint[0]));
        }

        /// <summary>
        /// Adds a PNG definition to import in the <see cref="FPG"/>.
        /// </summary>
        /// <param name="filename">PNG filename to import.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id. Must be a be a value between 1 and 999 and must be unique in the <see cref="FPG"/>.</param>
        /// <param name="description">Optional graphic description. 32 characters maximum.</param>
        /// <param name="controlPoints">Optional <see cref="MAP"/> Control Point list.</param>
        public void AddMap(string filename, int graphId, string description = "", ControlPoint[] controlPoints = null)
        {
            this.AddMap(new PNGImportDefinition(filename, graphId, description, controlPoints ?? new ControlPoint[0]));
        }

        /// <summary>
        /// Adds a <see cref="PNGImportDefinition"/> to import in the <see cref="FPG"/>.
        /// </summary>
        /// <param name="pngFile"><see cref="PNGImportDefinition"/> data to import.</param>
        public void AddMap(PNGImportDefinition pngFile)
        {
            if (!pngFile.GraphId.IsClamped(FPG.MIN_GRAPHID, FPG.MAX_GRAPHID))
            {
                throw new ArgumentOutOfRangeException(nameof(pngFile.GraphId), $"The GraphID must be a value between {FPG.MIN_GRAPHID} and {FPG.MAX_GRAPHID} (Current GraphId: {pngFile.GraphId})");
            }

            int mapIndex;
            if (!this.TryGetMapIndexByGraphId(pngFile.GraphId, out mapIndex))
            {
                this._maps.Add(pngFile);
            }
            else
            {
                throw new ArgumentException(nameof(pngFile.GraphId), $"The GraphID {pngFile.GraphId} is already in use by other map (Map index: {mapIndex})");
            }
        }

        /// <summary>
        /// Removes a <see cref="PNGImportDefinition"/> from the <see cref="FPG"/>.
        /// </summary>
        /// <param name="index">Index of the <see cref="PNGImportDefinition"/>.</param>
        public void RemoveMap(int index)
        {
            this._maps.RemoveAt(index);
        }

        /// <summary>
        /// Removes a <see cref="PNGImportDefinition"/> from the <see cref="FPG"/> using their graphic id.
        /// </summary>
        /// <param name="graphId">Graphic id of the <see cref="PNGImportDefinition"/>.</param>
        public void RemoveMapByGraphId(short graphId)
        {
            int mapIndex;
            if (this.TryGetMapIndexByGraphId(graphId, out mapIndex))
            {
                this.RemoveMap(mapIndex);
            }
            else
            {
                throw new ArgumentException(nameof(graphId), $"Map with GraphId {graphId} not found.");
            }
        }

        /// <summary>
        /// Removes all <see cref="PNGImportDefinition"/>s from the <see cref="FPG"/>.
        /// </summary>
        public void RemoveAllMaps()
        {
            this._maps.Clear();
        }

        /// <summary>
        /// Search a <see cref="PNGImportDefinition"/> by their graphic id.
        /// </summary>
        /// <param name="graphId">Graphic id of the <see cref="PNGImportDefinition"/>.</param>
        /// <returns>Returns the <see cref="PNGImportDefinition"/> data.</returns>
        public PNGImportDefinition FindByGraphId(short graphId)
        {
            int mapIndex;
            if (this.TryGetMapIndexByGraphId(graphId, out mapIndex))
            {
                return this._maps[mapIndex];
            }
            else
            {
                throw new ArgumentException(nameof(graphId), $"Map with GraphId {graphId} not found.");
            }
        }

        /// <summary>
        /// Imports all <see cref="PNGImportDefinition"/> and write all data to file.
        /// </summary>
        /// <param name="filename"><see cref="FPG"/> filename.</param>
        public override void Write(BinaryWriter file)
        {
            if (this._maps.Count == 0)
            {
                throw new InvalidOperationException("The FPG not contain any MAP to import.");
            }

            List<MapRegister> mapRegisters = this.ImportPNGs();

            base.Write(file);
            this.Palette.Write(file);

            foreach (var register in mapRegisters)
            {
                this.WriteMapRegister(file, register);
            }
        }

        /// <summary>
        /// Validates if the file is a valid <see cref="FPG"/> file.
        /// </summary>
        /// <param name="filename"><see cref="FPG"/> filename.</param>
        /// <returns>Returns true if the file contains a valid <see cref="FPG"/> header format.</returns>
        public static bool Validate(string filename)
        {
            using (var file = new BinaryReader(File.OpenRead(filename)))
            {
                return new FPG().Validate(file);
            }
        }
        #endregion
    }
    #endregion
}
