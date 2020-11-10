using DIV2.Format.Exporter.Converters;
using DIV2.Format.Exporter.MethodExtensions;
using DIV2.Format.Importer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DIV2.Format.Exporter
{
    #region Structures
    /// <summary>
    /// Image import definition data.
    /// </summary>
    /// <remarks>Supported formats are JPEG, PNG, BMP, GIF and TGA. Also supported 256 color PCX images and <see cref="MAP"/> files (only image data, discarding all metadata: graphId, description and control points).</remarks>
    [Serializable]
    public struct ImportDefinition
    {
        #region Properties
        internal bool BinaryLoad { get; private set; }
        /// <summary>
        /// Image data to import.
        /// </summary>
        public byte[] Buffer { get; private set; }
        /// <summary>
        /// Image filename to import.
        /// </summary>
        public string Filename { get; private set; }
        /// <summary>
        /// <see cref="MAP"/> graphic id. Must be a be a value between 1 and 999 and must be unique in the <see cref="FPG"/>.
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
        /// Creates an Image Import Definition for load buffer data.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array with the image data to import.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Optional <see cref="MAP>"/> description (32 characters maximum).</param>
        /// <param name="controlPoints">Optional <see cref="MAP"/> Control Point list.</param>
        public ImportDefinition(byte[] buffer, int graphId, string description = "", ControlPoint[] controlPoints = null) :
            this(true, buffer, string.Empty, graphId, description, controlPoints)
        {
        }

        /// <summary>
        /// Creates an Image Import Definition for load PNG file.
        /// </summary>
        /// <param name="filename">Image file to import.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id.</param>
        /// <param name="description">Optional <see cref="MAP"/> description (32 characters maximum).</param>
        /// <param name="controlPoints">Optional <see cref="MAP"/> Control Point list.</param>
        public ImportDefinition(string filename, int graphId, string description = "", ControlPoint[] controlPoints = null) :
            this(false, null, filename, graphId, description, controlPoints)
        {
        }

        /// <summary>
        /// Creates an Image Import Definition for load PNG file.
        /// </summary>
        /// <param name="filename">Image file to import.</param>
        /// <param name="metadata"><see cref="MAPHeader"/> instance with all metadata for the <see cref="MAP"/> creation.</param>
        public ImportDefinition(string filename, MAPHeader metadata) :
            this(false, null, filename, metadata.GraphId, metadata.Description, metadata.ControlPoints.ToArray())
        {
        }

        ImportDefinition(bool binaryLoad, byte[] buffer, string filename, int graphId, string description, ControlPoint[] controlPoints)
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
        public const int MAP_BASE_METADATA_LENGTH = 64;
        public const int CONTROLPOINT_LENGTH = 4;
        public const int MIN_GRAPHID = 1;
        public const int MAX_GRAPHID = 999;
        public const int DESCRIPTION_LENGTH = 32;
        public const int FILENAME_LENGTH = 12;

        readonly static string[] SUPPORTED_IMAGE_EXTENSIONS = { "jpg", "jpeg", "png", "bmp", "gif", "tga", "pcx", "map" };
        #endregion

        #region Structures
        struct Register
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
                return FPG.MAP_BASE_METADATA_LENGTH + (FPG.CONTROLPOINT_LENGTH * controlPoints.Length) + bitmap.Length;
            }
            #endregion
        }
        #endregion

        #region Internal vars
        List<ImportDefinition> _maps;
        #endregion

        #region Properties
        /// <summary>
        /// <see cref="PAL"/> instance used by this <see cref="FPG"/>.
        /// </summary>
        public PAL Palette { get; }

        /// <summary>
        /// All <see cref="ImportDefinition"/> setup for this <see cref="FPG"/>.
        /// </summary>
        public IReadOnlyList<ImportDefinition> Maps => this._maps;
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
            this._maps = new List<ImportDefinition>();
        }

        /// <summary>
        /// Create new <see cref="FPG"/> instace importing PNG files from a directory.
        /// </summary>
        /// <param name="palFilename"><see cref="PAL"/> file used to convert PNG colors to <see cref="MAP"/>.</param>
        /// <param name="importPath">Directory to import.</param>
        /// <remarks>Supported formats are JPEG, PNG, BMP, GIF and TGA. Also supported 256 color PCX images and <see cref="MAP"/> files. Each image file must be follow by their JSON file, a serialized <see cref="MAPHeader"/> object, with all <see cref="MAP"/> metadata.</remarks>
        public FPG(string palFilename, string importPath) : this(new PAL(palFilename), importPath)
        {
        }

        /// <summary>
        /// Create new <see cref="FPG"/> instance importing PNG files from a directory.
        /// </summary>
        /// <param name="palette">The <see cref="PAL"/> instance used to convert image colors to <see cref="MAP"/>.</param>
        /// <param name="importPath">Directory to import.</param>
        /// <remarks>Supported formats are JPEG, PNG, BMP, GIF and TGA. Also supported 256 color PCX images and <see cref="MAP"/> files. Each image file must be follow by their JSON file, a serialized <see cref="MAPHeader"/> object, with all <see cref="MAP"/> metadata.</remarks>
        public FPG(PAL palette, string importPath) : this()
        {
            // Get all files of the first extension detected:
            string[] images = null;
            
            foreach (var ext in FPG.SUPPORTED_IMAGE_EXTENSIONS)
            {
                if (images is null || images.Length == 0)
                {
                    images = Directory.GetFiles(importPath, $"?.{ext}");
                }
            }

            if (images == null || images.Length == 0)
            {
                throw new FileNotFoundException($"The directory \"{importPath}\" not contain any supported image file.");
            }

            // Key = image file, Value = JSON file:
            foreach (var file in images.ToDictionary(e => Path.ChangeExtension(e, ".json")))
            {
                if (File.Exists(file.Value))
                {
                    string imageFile = file.Key;
                    var metadata = MAPHeader.FromJSON(File.ReadAllText(file.Value));
                    this.AddMap(new ImportDefinition(imageFile, metadata));
                }
                else
                {
                    throw new FileNotFoundException($"The JSON file \"{file.Value}\" not exists. Each Image file must be follow by their JSON file, a serialized {nameof(MAPHeader)} object, with all {nameof(MAP)} metadata.");
                }
            }
        }
        #endregion

        #region Methods & Functions
        void WriteMapRegister(BinaryWriter file, Register register)
        {
            file.Write(register.graphId);
            file.Write(register.GetSize());
            file.Write(register.description.GetASCIIZString(FPG.DESCRIPTION_LENGTH));
            file.Write(register.filename.GetASCIIZString(FPG.FILENAME_LENGTH));
            file.Write(register.width);
            file.Write(register.height);
            register.controlPoints.Write<int>(file);
            file.Write(register.bitmap);
        }

        List<Register> ImportImages()
        {
            var mapRegisters = new List<Register>(this._maps.Count);

            foreach (var map in this._maps)
            {
                byte[] buffer = map.BinaryLoad ? map.Buffer : File.ReadAllBytes(map.Filename);

                BMP256Converter.Convert(buffer, out byte[] bitmap, out short width, out short height, this.Palette);

                mapRegisters.Add(new Register()
                {
                    graphId = map.GraphId,
                    description = map.Description,
                    filename = string.IsNullOrEmpty(map.Filename) ? string.Empty : Path.GetFileName(map.Filename),
                    width = width,
                    height = height,
                    controlPoints = map.ControlPoints,
                    bitmap = bitmap
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
        /// Adds an image definition to import in the <see cref="FPG"/>.
        /// </summary>
        /// <param name="buffer">Image data to import.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id. Must be a be a value between 1 and 999 and must be unique in the <see cref="FPG"/>.</param>
        /// <param name="description">Optional graphic description. 32 characters maximum.</param>
        /// <param name="controlPoints">Optional <see cref="MAP"/> Control Point list.</param>
        public void AddMap(byte[] buffer, int graphId, string description = "", ControlPoint[] controlPoints = null)
        {
            this.AddMap(new ImportDefinition(buffer, graphId, description, controlPoints ?? new ControlPoint[0]));
        }

        /// <summary>
        /// Adds an image definition to import in the <see cref="FPG"/>.
        /// </summary>
        /// <param name="filename">Image filename to import.</param>
        /// <param name="graphId"><see cref="MAP"/> graphic id. Must be a be a value between 1 and 999 and must be unique in the <see cref="FPG"/>.</param>
        /// <param name="description">Optional graphic description. 32 characters maximum.</param>
        /// <param name="controlPoints">Optional <see cref="MAP"/> Control Point list.</param>
        public void AddMap(string filename, int graphId, string description = "", ControlPoint[] controlPoints = null)
        {
            this.AddMap(new ImportDefinition(filename, graphId, description, controlPoints ?? new ControlPoint[0]));
        }

        /// <summary>
        /// Adds an <see cref="ImportDefinition"/> to import in the <see cref="FPG"/>.
        /// </summary>
        /// <param name="definition"><see cref="ImportDefinition"/> data to import.</param>
        public void AddMap(ImportDefinition definition)
        {
            if (!definition.GraphId.IsClamped(FPG.MIN_GRAPHID, FPG.MAX_GRAPHID))
            {
                throw new ArgumentOutOfRangeException(nameof(definition.GraphId), $"The GraphID must be a value between {FPG.MIN_GRAPHID} and {FPG.MAX_GRAPHID} (Current GraphId: {definition.GraphId})");
            }

            int mapIndex;
            if (!this.TryGetMapIndexByGraphId(definition.GraphId, out mapIndex))
            {
                this._maps.Add(definition);
            }
            else
            {
                throw new ArgumentException(nameof(definition.GraphId), $"The GraphID {definition.GraphId} is already in use by other map (Map index: {mapIndex})");
            }
        }

        /// <summary>
        /// Removes a <see cref="ImportDefinition"/> from the <see cref="FPG"/>.
        /// </summary>
        /// <param name="index">Index of the <see cref="ImportDefinition"/>.</param>
        public void RemoveMap(int index)
        {
            this._maps.RemoveAt(index);
        }

        /// <summary>
        /// Removes a <see cref="ImportDefinition"/> from the <see cref="FPG"/> using their graphic id.
        /// </summary>
        /// <param name="graphId">Graphic id of the <see cref="ImportDefinition"/>.</param>
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
        /// Removes all <see cref="ImportDefinition"/>s from the <see cref="FPG"/>.
        /// </summary>
        public void RemoveAllMaps()
        {
            this._maps.Clear();
        }

        /// <summary>
        /// Search a <see cref="ImportDefinition"/> by their graphic id.
        /// </summary>
        /// <param name="graphId">Graphic id of the <see cref="ImportDefinition"/>.</param>
        /// <returns>Returns the <see cref="ImportDefinition"/> data.</returns>
        public ImportDefinition FindByGraphId(short graphId)
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
        /// Imports all <see cref="ImportDefinition"/> and write all data to file.
        /// </summary>
        /// <param name="filename"><see cref="FPG"/> filename.</param>
        internal override void Write(BinaryWriter file)
        {
            if (this._maps.Count == 0)
            {
                throw new InvalidOperationException("The FPG not contain any MAP to import.");
            }

            List<Register> mapRegisters = this.ImportImages();

            base.Write(file);
            this.Palette.WriteEmbebed(file);

            foreach (var register in mapRegisters)
            {
                this.WriteMapRegister(file, register);
            }
        }
        #endregion
    }
    #endregion
}
