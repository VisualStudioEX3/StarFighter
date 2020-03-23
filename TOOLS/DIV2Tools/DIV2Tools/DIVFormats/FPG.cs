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
    public class FPG
    {
        #region Classes
        /// <summary>
        /// Header description.
        /// </summary>
        class Header : DIVFormatBaseHeader
        {
            #region Constants
            const string HEADER_ID = "fpg";
            #endregion

            #region Constructor
            public Header() : base(Header.HEADER_ID)
            {
            }

            public Header(BinaryReader file) : base(Header.HEADER_ID, file)
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
            #region Internal vars
            MAP.BaseInfo _info;
            MAP _map;
            #endregion

            #region Properties
            public int GraphId { get { return this._info.GraphId; } set { this._info.GraphId = value; } }
            public int Length => this._info.Length;
            public string Description { get { return this._info.Description; } set { this._info.Description = value; } }
            public string Filename { get { return this._info.Filename; } set { this._info.Filename = value; } }
            public short Width => this._map.Width;
            public short Height => this._map.Height;
            public MAP.ControlPointList ControlPoints => this._map.ControlPoints;
            #endregion

            public MAPRegister(BinaryReader file)
            {
                this._info = new MAP.BaseInfo(file, true);
                this._map = new MAP(file, true, false); // Read MAP without header and palette.
            }

            #region Methods & Functions
            public void Write(BinaryWriter file)
            {
                file.Write(this.GraphId);
                file.Write(this.Length);
                file.Write(this.Description);
                file.Write(this.Filename);
                file.Write(this.Width);
                file.Write(this.Height);
                this._map.Write(file); // This write Control Point list and Pixels (ommit header and palette).
            } 
            #endregion
        }
        #endregion

        #region Internal vars
        Header _header;
        #endregion

        #region Properties
        public PAL Palette { get; private set; } 
        public List<MAPRegister> Maps { get; private set; }
        #endregion

        #region Constructors
        public FPG()
        {
            this._header = new Header();
        }

        public FPG(BinaryReader file, bool verbose = true)
        {
            this._header = new Header(file);
            Helper.Log(this._header.ToString(), verbose);

            if (this._header.Check())
            {
                this.Palette = new PAL(file, true, verbose);
                Helper.Log(this.Palette.ToString(), verbose);

                while (!file.EOF()) // TODO: Maybe need to use a try/catch to get the EOF exception.
                {
                    this.Maps.Add(new MAPRegister(file));
                }

                Helper.Log("FPG loaded!", verbose);
            }
            else
            {
                throw new FormatException("Invalid FPG file!");
            }
        }

        public FPG(string filename) : this(new BinaryReader(File.OpenRead(filename)))
        {
        }
        #endregion

        #region Methods & Functions
        public void ImportPalette(string filename)
        {
            this.Palette = new PAL(filename, true, false);
        } 
        #endregion
    }
}
