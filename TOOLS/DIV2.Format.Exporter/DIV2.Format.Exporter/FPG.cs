using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        /// MAP graphic id. Must be a be a value between 1 and 999 and must be unique in the FPG.
        /// </summary>
        public int GraphId { get; private set; }
        /// <summary>
        /// Optional MAP graphic description (32 characters maximum).
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Optional MAP Control Point list.
        /// </summary>
        public ControlPoint[] ControlPoints { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a PNG Import Definition for load buffer data.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array with the PNG data to import.</param>
        /// <param name="graphId">MAP graphic id.</param>
        /// <param name="description">Optional MAP description (32 characters maximum).</param>
        /// <param name="controlPoints">Optional MAP Control Point list.</param>
        public PNGImportDefinition(byte[] buffer, int graphId, string description = "", ControlPoint[] controlPoints = null) :
            this(true, buffer, null, graphId, description, controlPoints)
        {
        }

        /// <summary>
        /// Creates a PNG Import Definition for load PNG file.
        /// </summary>
        /// <param name="filename">PNG file to import.</param>
        /// <param name="graphId">MAP graphic id.</param>
        /// <param name="description">Optional MAP description (32 characters maximum).</param>
        /// <param name="controlPoints">Optional MAP Control Point list.</param>
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

    #region Class
    /// <summary>
    /// FPG creator.
    /// </summary>
    public class FPG
    {
        #region Constants
        const string HEADER_ID = "fpg";
        const int HEADER_LENGTH = 64;
        const int MIN_GRAPHID = 1;
        const int MAX_GRAPHID = 999;
        const int DESCRIPTION_LENGTH = 32;
        const int FILENAME_LENGTH = 12;
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
        }
        #endregion

        #region Internal vars
        List<PNGImportDefinition> _maps;
        byte[] _palette;
        List<MapRegister> _registers;
        CancellationToken _asyncCancellationToken;
        #endregion

        #region Properties
        /// <summary>
        /// All <see cref="PNGImportDefinition"/> setup for this FPG.
        /// </summary>
        public IReadOnlyList<PNGImportDefinition> Maps => this._maps;
        #endregion

        #region Constructor
        public FPG()
        {
            this._maps = new List<PNGImportDefinition>();
        }
        #endregion

        #region Methods & Functions
        void WriteMapRegister(BinaryWriter file, MapRegister register)
        {
            file.Write(register.graphId);
            file.Write(register.bitmap.Length + FPG.HEADER_LENGTH);
            file.Write(register.description.GetASCIIZString(FPG.DESCRIPTION_LENGTH));
            file.Write(register.filename.GetASCIIZString(FPG.FILENAME_LENGTH));
            file.Write(register.width);
            file.Write(register.height);
            file.Write(register.controlPoints.Length);
            foreach (var point in register.controlPoints)
            {
                this.CheckForCancellationTokenRequest(file);

                file.Write(point.x);
                file.Write(point.y);
            }
            file.Write(register.bitmap);
        }

        void ImportPNGs()
        {
            bool isPaletteSet = false;
            this._registers = new List<MapRegister>();

            // FYI: Tried to paralleized using Parallel.ForEach (that reduced to half the time process) but sometimes the PCX constructor, the internal MagickImage instance, fails throwing a System.AccessViolationException exception in random points of the code.
            foreach (var map in this._maps)
            {
                this.CheckForCancellationTokenRequest(null);

                PCX pcx = map.BinaryLoad ? new PCX(map.Buffer, isPaletteSet, true) : new PCX(map.Filename, isPaletteSet, true);

                this.CheckForCancellationTokenRequest(null);

                if (!isPaletteSet)
                {
                    this._palette = pcx.Palette; // Using this palette for the FPG (assuming the all maps shared the same palette).
                    isPaletteSet = true;
                }

                this._registers.Add(new MapRegister()
                {
                    graphId = map.GraphId,
                    description = map.Description,
                    filename = string.IsNullOrEmpty(map.Filename) ? string.Empty : Path.GetFileName(map.Filename),
                    width = pcx.Width,
                    height = pcx.Height,
                    controlPoints = map.ControlPoints,
                    bitmap = pcx.Bitmap
                });
            }
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
        /// Adds a PNG definition to import in the FPG.
        /// </summary>
        /// <param name="buffer">PNG data to import.</param>
        /// <param name="graphId">MAP graphic id. Must be a be a value between 1 and 999 and must be unique in the FPG.</param>
        /// <param name="description">Optional graphic description. 32 characters maximum.</param>
        /// <param name="controlPoints">Optional MAP Control Point list.</param>
        public void AddMap(byte[] buffer, int graphId, string description = "", ControlPoint[] controlPoints = null)
        {
            this.AddMap(new PNGImportDefinition(buffer, graphId, description, controlPoints ?? new ControlPoint[0]));
        }

        /// <summary>
        /// Adds a PNG definition to import in the FPG.
        /// </summary>
        /// <param name="filename">PNG filename to import.</param>
        /// <param name="graphId">MAP graphic id. Must be a be a value between 1 and 999 and must be unique in the FPG.</param>
        /// <param name="description">Optional graphic description. 32 characters maximum.</param>
        /// <param name="controlPoints">Optional MAP Control Point list.</param>
        public void AddMap(string filename, int graphId, string description = "", ControlPoint[] controlPoints = null)
        {
            this.AddMap(new PNGImportDefinition(filename, graphId, description, controlPoints ?? new ControlPoint[0]));
        }

        /// <summary>
        /// Adds a <see cref="PNGImportDefinition"/> to import in the FPG.
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
        /// Removes a <see cref="PNGImportDefinition"/> from the FPG.
        /// </summary>
        /// <param name="index">Index of the <see cref="PNGImportDefinition"/>.</param>
        public void RemoveMap(int index)
        {
            this._maps.RemoveAt(index);
        }

        /// <summary>
        /// Removes a <see cref="PNGImportDefinition"/> from the FPG using their graphic id.
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
        /// Removes all <see cref="PNGImportDefinition"/>s from the FPG.
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
        /// <param name="filename">FPG filename.</param>
        public void Save(string filename)
        {
            if (this._maps.Count == 0)
            {
                throw new InvalidOperationException("The FPG not contain any MAP to import.");
            }

            this.ImportPNGs();

            using (var file = new BinaryWriter(File.OpenWrite(filename)))
            {
                DIVFormatCommonBase.WriteCommonHeader(file, FPG.HEADER_ID);
                DIVFormatCommonBase.WritePalette(file, this._palette);

                foreach (var register in this._registers)
                {
                    this.CheckForCancellationTokenRequest(file);
                    this.WriteMapRegister(file, register);
                }
            }
        }

        /// <summary>
        /// Imports all <see cref="PNGImportDefinition"/> and write, in separated thread, all data to file.
        /// </summary>
        /// <param name="filename">FPG filename.</param>
        /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> for this <see cref="Task"/>.</param>
        /// <remarks>If this <see cref="Task"/> is cancelled by <see cref="CancellationToken"/> token, this throw an <see cref="OperationCanceledException"/>.</remarks>
        public Task SaveAsync(string filename, CancellationToken cancellationToken = default)
        {
            this._asyncCancellationToken = cancellationToken;
            return Task.Factory.StartNew(() => this.Save(filename), cancellationToken);
        }

        void CheckForCancellationTokenRequest(BinaryWriter file)
        {
            if (this._asyncCancellationToken.IsCancellationRequested)
            {
                file?.DisposeAsync();
            }
            this._asyncCancellationToken.ThrowIfCancellationRequested();
        }
        #endregion
    }
    #endregion
}
