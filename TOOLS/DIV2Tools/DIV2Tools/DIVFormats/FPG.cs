using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DIV2Tools.Helpers;
using DIV2Tools.MethodExtensions;

namespace DIV2Tools.DIVFormats
{
    /// <summary>
    /// FPG file.
    /// </summary>
    public class FPG : DIVFormatBase
    {
        #region Classes
        /// <summary>
        /// Header description.
        /// </summary>
        class FPGHeader : DIVFormatBaseHeader
        {
            #region Constants
            const string HEADER_ID = "fpg";
            #endregion

            #region Constructor
            public FPGHeader() : base(FPGHeader.HEADER_ID)
            {
            }

            public FPGHeader(BinaryReader file) : base(FPGHeader.HEADER_ID, file)
            {
            }
            #endregion

            #region Methods & Functions
            public override string ToString()
            {
                return $"{base.ToString()}\n";
            }
            #endregion
        }

        /// <summary>
        /// <see cref="MAP"/> data stored in <see cref="FPG"/> file.
        /// </summary>
        public class MAPRegister
        {
            #region Constants
            /// <summary>
            /// <see cref="FPG.MAPRegister"/> header length. This value must be added to <see cref="MAP"/> bitmap length to get the <see cref="MAPRegister"/> length.
            /// </summary>
            public const int HEADER_LENGTH = 64; 
            #endregion

            #region Internal vars
            MAP.BaseInfo _info;
            MAP _map;
            #endregion

            #region Properties
            public int GraphId { get { return this._info.GraphId; } set { this._info.GraphId = value; } }
            public int Length => this._info.Length;
            public string Description { get { return this._info.Description; } set { this._info.Description = value; } }
            public string Filename { get { return this._info.Filename; } set { this._info.Filename = value; } }
            public short Width => (short)this._info.Width;
            public short Height => (short)this._info.Height;
            public MAP.ControlPointList ControlPoints => this._map.ControlPoints;
            #endregion

            #region Constructors
            /// <summary>
            /// Imports a <see cref="MAPRegister"/> from a <see cref="FPG"/> instance.
            /// </summary>
            /// <param name="file"><see cref="BinaryReader"/> instance.</param>
            public MAPRegister(BinaryReader file)
            {
                this._info = new MAP.BaseInfo(file, true);
                this._map = MAP.ExtractFromFPG(file, Width, this.Height);
            }

            /// <summary>
            /// Imports a PNG file.
            /// </summary>
            /// <param name="filename">PNG filename.</param>
            /// <param name="graphId">GraphId for the new <see cref="MAP"/>.</param>
            /// <param name="description">Description for the new <see cref="MAP"/>.</param>
            /// <param name="storedFilename">Stored filename in 8.3 format. This field will be an empty string.</param>
            public MAPRegister(string filename, int graphId, string description, string storedFilename) : 
                this(filename, graphId, description, storedFilename, new MAP.ControlPoint[0], out PAL pal)
            {
            }

            /// <summary>
            /// Imports a PNG file.
            /// </summary>
            /// <param name="filename">PNG filename.</param>
            /// <param name="graphId">GraphId for the new <see cref="MAP"/>.</param>
            /// <param name="description">Description for the new <see cref="MAP"/>.</param>
            /// <param name="storedFilename">Stored filename in 8.3 format. This field will be an empty string.</param>
            /// <param name="palette">Returns the <see cref="PAL"/> embebed into the <see cref="MAP"/> file.</param>
            public MAPRegister(string filename, int graphId, string description, string storedFilename, out PAL palette) : 
                this(filename, graphId, description, storedFilename, new MAP.ControlPoint[0], out palette)
            {
            }

            /// <summary>
            /// Imports a PNG file.
            /// </summary>
            /// <param name="filename">PNG filename.</param>
            /// <param name="graphId">GraphId for the new <see cref="MAP"/>.</param>
            /// <param name="description">Description for the new <see cref="MAP"/>.</param>
            /// <param name="storedFilename">Stored filename in 8.3 format. This field will be an empty string.</param>
            /// <param name="controlPoints"><see cref="MAP.ControlPoint"/> array for the new <see cref="MAP"/>.</param>
            public MAPRegister(string filename, int graphId, string description, string storedFilename, MAP.ControlPoint[] controlPoints) : 
                this(filename, graphId, description, storedFilename, new MAP.ControlPoint[0], out PAL pal)
            {

            }

            /// <summary>
            /// Imports a PNG file.
            /// </summary>
            /// <param name="filename">PNG filename.</param>
            /// <param name="graphId">GraphId for the new <see cref="MAP"/>.</param>
            /// <param name="description">Description for the new <see cref="MAP"/>.</param>
            /// <param name="storedFilename">Stored filename in 8.3 format. This field will be an empty string.</param>
            /// <param name="controlPoints"><see cref="MAP.ControlPoint"/> array for the new <see cref="MAP"/>.</param>
            /// <param name="palette">Returns the <see cref="PAL"/> embebed into the <see cref="MAP"/> file.</param>
            public MAPRegister(string filename, int graphId, string description, string storedFilename, MAP.ControlPoint[] controlPoints, out PAL palette)
            {
                using (var file = new BinaryReader(File.OpenRead(filename)))
                {
                    this._map = new MAP();
                    {
                        this._map.ImportPNG(filename);

                        this._info = new MAP.BaseInfo(this._map.Width, this._map.Height, graphId, description, storedFilename, this._map.Pixels.Count + MAPRegister.HEADER_LENGTH);
                        {
                            foreach (var point in controlPoints)
                            {
                                this.ControlPoints.Add(point);
                            }
                        }

                        this._map.RemoveHeader();

                        palette = this._map.Palette;
                        this._map.RemovePalette();
                    }
                }
            } 
            #endregion

            #region Methods & Functions
            public void Write(BinaryWriter file)
            {
                this._info.Write(file);
                this._map.Write(file);
            }

            public override string ToString()
            {
                return $"- Graphic Identifier: {this.GraphId}\n" +
                       $"- Total MAPRegister size: {this.Length}\n" +
                       $"- Description: \"{this.Description}\"\n" +
                       $"- (Stored) Filename: \"{this.Filename}\"\n" +
                       $"- Width: {this.Width}\n" +
                       $"- Height: {this.Height}\n" +
                       $"- Control Points: {this._map.ControlPoints.ToString()}\n" +
                       $"- Image size: {this._map.Pixels.Count}\n";
            }
            #endregion
        }

        /// <summary>
        /// Collection of <see cref="MAPRegister"/> instances.
        /// </summary>
        public class MAPRegisterCollection
        {
            #region Internal vars
            List<MAPRegister> _maps;
            #endregion

            #region Properties
            public MAPRegister this[int index] => this._maps[index];
            public int Count => this._maps.Count;
            #endregion

            #region Constructors
            public MAPRegisterCollection()
            {
                this._maps = new List<MAPRegister>();
            }

            /// <summary>
            /// Imports all <see cref="MAPRegister"/> instances from a <see cref="FPG"/> instanace.
            /// </summary>
            /// <param name="file"><see cref="BinaryReader"/> instance.</param>
            public MAPRegisterCollection(BinaryReader file) : this()
            {
                while (!file.EOF()) { this._maps.Add(new MAPRegister(file)); }
            }
            #endregion

            #region Methods & Functions
            bool isGraphIdClamped(int value)
            {
                return value.IsClamped(MAP.BaseInfo.GRAPHID_MIN, MAP.BaseInfo.GRAPHID_MAX);
            }

            /// <summary>
            /// Add a PNG file as <see cref="MAP"/>.
            /// </summary>
            /// <param name="filename">PNG filename.</param>
            /// <param name="graphId">GraphId for the new <see cref="MAP"/>.</param>
            /// <param name="description">Description for the new <see cref="MAP"/>.</param>
            /// <param name="storedFilename">Stored filename in 8.3 format. This field will be an empty string.</param>
            /// <param name="controlPoints"><see cref="MAP.ControlPoint"/> array for the new <see cref="MAP"/>.</param>
            /// <param name="palette">Returns the <see cref="PAL"/> embebed into the <see cref="MAP"/> file.</param>
            public void Add(string filename, int graphId, string description, string storedFilename, MAP.ControlPoint[] controlPoints, out PAL palette)
            {
                if (this.isGraphIdClamped(graphId))
                {
                    int index = this.FindByGraphId(graphId);
                    if (index == -1)
                    {
                        if (controlPoints.Length < MAP.ControlPointList.MAX_CAPACITY)
                        {
                            this._maps.Add(new MAPRegister(filename, graphId, description, storedFilename, controlPoints, out palette));
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(nameof(controlPoints), $"The control points array length must be less than {MAP.ControlPointList.MAX_CAPACITY}.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException(nameof(graphId), $"The graphic identifier {graphId} is in use by other MAP (MAP index: {index}).");
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(graphId), MAP.BaseInfo.graphIdOutOfRangeExceptionMessage);
                }
            }

            /// <summary>
            /// Add a PNG file as <see cref="MAP"/>.
            /// </summary>
            /// <param name="filename">PNG filename.</param>
            /// <param name="graphId">GraphId for the new <see cref="MAP"/>.</param>
            /// <param name="description">Description for the new <see cref="MAP"/>.</param>
            /// <param name="storedFilename">Stored filename in 8.3 format. This field will be an empty string.</param>
            /// <param name="controlPoints"><see cref="MAP.ControlPoint"/> array for the new <see cref="MAP"/>.</param>
            public void Add(string filename, int graphId, string description, string storedFilename, MAP.ControlPoint[] controlPoints)
            {
                this.Add(filename, graphId, description, storedFilename, new MAP.ControlPoint[0], out PAL palette);
            }

            /// <summary>
            /// Add a PNG file as <see cref="MAP"/>.
            /// </summary>
            /// <param name="filename">PNG filename.</param>
            /// <param name="graphId">GraphId for the new <see cref="MAP"/>.</param>
            /// <param name="description">Description for the new <see cref="MAP"/>.</param>
            /// <param name="storedFilename">Stored filename in 8.3 format. This field will be an empty string.</param>
            public void Add(string filename, int graphId, string description, string storedFilename)
            {
                this.Add(filename, graphId, description, storedFilename, new MAP.ControlPoint[0], out PAL palette);
            }

            /// <summary>
            /// Add a PNG file as <see cref="MAP"/>.
            /// </summary>
            /// <param name="filename">PNG filename.</param>
            /// <param name="graphId">GraphId for the new <see cref="MAP"/>.</param>
            /// <param name="description">Description for the new <see cref="MAP"/>.</param>
            /// <param name="storedFilename">Stored filename in 8.3 format. This field will be an empty string.</param>
            /// <param name="palette">Returns the <see cref="PAL"/> embebed into the <see cref="MAP"/> file.</param>
            public void Add(string filename, int graphId, string description, string storedFilename, out PAL palette)
            {
                this.Add(filename, graphId, description, storedFilename, new MAP.ControlPoint[0], out palette);
            }

            /// <summary>
            /// Remove selected <see cref="MAPRegister"/> by index.
            /// </summary>
            /// <param name="index"></param>
            public void Remove(int index)
            {
                this._maps.Remove(this[index]);
            }

            /// <summary>
            /// Find <see cref="MAPRegister"/> index searching by their <see cref="MAP.BaseInfo.GraphId"/> value.
            /// </summary>
            /// <param name="graphId">A value beteween <see cref="MAP.BaseInfo.GRAPHID_MIN"/> and <see cref="MAP.BaseInfo.GRAPHID_MAX"/> (usually 0 - 999).</param>
            /// <returns>Returns the index value of the match <see cref="MAPRegister"/> or -1 if not found any match.</returns>
            public int FindByGraphId(int graphId)
            {
                if (this.isGraphIdClamped(graphId))
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this[i].GraphId == graphId) return i;
                    }

                    return -1;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(graphId), MAP.BaseInfo.graphIdOutOfRangeExceptionMessage);
                }
            }

            public void Write(BinaryWriter file)
            {
                foreach (var map in this._maps)
                {
                    map.Write(file);
                }
            }

            public override string ToString()
            {
                var sb = new StringBuilder();

                sb.AppendLine($"{this.Count} {(this.Count < 2 ? "MAP": "MAPs")} readed:");

                foreach (var map in this._maps)
                {
                    sb.AppendLine(map.ToString());
                }

                return sb.ToString();
            }
            #endregion
        }
        #endregion

        #region Properties
        FPGHeader Header => (FPGHeader)this.header;

        public PAL Palette { get; private set; } 
        public MAPRegisterCollection Maps { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="FPG"/> empty instance.
        /// </summary>
        public FPG()
        {
            this.header = new FPGHeader();
            this.Maps = new MAPRegisterCollection();
        }

        /// <summary>
        /// Imports a <see cref="FPG"/> file.
        /// </summary>
        /// <param name="filename"><see cref="FPG"/> filename.</param>
        /// <param name="verbose">Log <see cref="FPG"/> import data to console. By default is <see cref="true"/>.</param>
        public FPG(string filename, bool verbose = true)
        {
            using (var file = new BinaryReader(File.OpenRead(filename)))
            {
                this.header = new FPGHeader(file);
                Helper.Log(this.Header.ToString(), verbose);

                if (this.Header.Check())
                {
                    this.Palette = PAL.ExtractFromFile(file);
                    Helper.Log(this.Palette.ToString(), verbose);

                    this.Maps = new MAPRegisterCollection(file);
                    Helper.Log(this.Maps.ToString(), verbose);

                    Helper.Log("FPG loaded!", verbose);
                }
                else
                {
                    throw new FormatException("Invalid FPG file!");
                } 
            }
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Import an external <see cref="PAL"/> file to use in this <see cref="FPG"/>.
        /// </summary>
        /// <param name="filename"><see cref="PAL"/> file to import.</param>
        public void ImportPalette(string filename)
        {
            this.ImportPalette(new PAL(filename, false));
        }

        /// <summary>
        /// Import an external <see cref="PAL"/> instance to use in this <see cref="FPG"/>.
        /// </summary>
        /// <param name="palette"><see cref="PAL"/> instance to import.</param>
        public void ImportPalette(PAL palette)
        {
            this.Palette = palette;
            this.Palette.RemoveHeader();
        }

        public void Write(string filename)
        {
            using (var file = new BinaryWriter(File.OpenWrite(filename)))
            {
                this.header.Write(file);
                this.Palette.Write(file);
                this.Maps.Write(file);
            }
        }
        #endregion
    }
}
