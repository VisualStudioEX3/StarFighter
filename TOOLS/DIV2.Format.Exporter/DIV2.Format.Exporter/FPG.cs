using DIV2.Format.Exporter.ExtensionMethods;
using DIV2.Format.Exporter.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DIV2.Format.Exporter
{
    struct Register : ISerializableAsset
    {
        #region Constants
        const int FILENAME_LENGTH = 12; // DOS 8:3 format.
        #endregion

        #region Public vars
        public string filename;
        public MAP map;
        #endregion

        #region Properties
        public int GraphId => this.map.GraphId;
        #endregion

        #region Operators
        public static bool operator ==(Register a, Register b)
        {
            return a.map == b.map &&
                   a.filename == b.filename;
        }

        public static bool operator !=(Register a, Register b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructor
        public Register(BinaryReader stream, PAL palette)
        {
            int graphId = stream.ReadInt32();
            stream.BaseStream.Position += sizeof(int); // Skip register size.
            string description = stream.ReadBytes(MAP.DESCRIPTION_LENGTH).ToASCIIString();
            this.filename = stream.ReadBytes(FILENAME_LENGTH).ToASCIIString();
            int width = stream.ReadInt32();
            int height = stream.ReadInt32();

            this.map = new MAP(palette, (short)width, (short)height, graphId, description);

            int count = stream.ReadInt32(); // Control Points counter.
            for (int i = 0; i < count; i++)
            {
                var point = new ControlPoint(stream);
                if (point.x < 0 || point.y < 0)
                {
                    // If the control point has values under zero (x:-1, y:-1) means that this point has not defined values in DIV Games Studio MAP editor.
                    // DIV Games Studio read this values but when used for drawing a MAP this values are the MAP center coordintates.
                    point.x = (short)(width / 2);
                    point.y = (short)(height / 2);
                }
                this.map.ControlPoints.Add(point);
            }

            this.map.SetBitmapArray(stream.ReadBytes(width * height));
        }
        #endregion

        #region Methods & Functions
        int GetSize()
        {
            return (sizeof(int) * 2) + // GraphId + register length value.
                   (sizeof(byte) * MAP.DESCRIPTION_LENGTH) + // Description.
                   (sizeof(byte) * FILENAME_LENGTH) + // Filename.
                   (sizeof(int) * 3) + // Width + Height + Control Points counter.
                   (ControlPoint.SIZE * this.map.ControlPoints.Count) + // Control Points list.
                   (sizeof(byte) * (this.map.Width * this.map.Height)); // Bitmap.
        }

        public byte[] Serialize()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                stream.Write(this.map.GraphId); // GraphId.

                int size = this.GetSize();
                stream.Write(size); // Register length.

                byte[] description = this.map.Description.GetASCIIZString(MAP.DESCRIPTION_LENGTH);
                stream.Write(description); // Description.

                byte[] filename = this.filename.GetASCIIZString(FILENAME_LENGTH);
                stream.Write(filename); // Filename.

                stream.Write((int)this.map.Width); // Width.
                stream.Write((int)this.map.Height); // Height.

                int count = Math.Min(this.map.ControlPoints.Count, MAP.MAX_CONTROL_POINTS);
                stream.Write(count); // Control Points counter.

                for (int i = 0; i < count; i++)
                    this.map.ControlPoints[i].Write(stream); // Each Control Point in the list.

                byte[] bitmap = this.map.GetBitmapArray();
                stream.Write(bitmap); // Bitmap array.

                return (stream.BaseStream as MemoryStream).ToArray();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        public static bool TryToRead(BinaryReader stream)
        {
            try
            {
                stream.ReadInt64(); // GraphId + register length value.
                stream.ReadBytes(MAP.DESCRIPTION_LENGTH); // Description.
                stream.ReadBytes(FILENAME_LENGTH); // Filename.
                int width = stream.ReadInt32();
                int height = stream.ReadInt32();

                int count = stream.ReadInt32(); // Control Points counter.
                for (int i = 0; i < count; i++)
                    stream.ReadInt32(); // Control Point coordinates.

                stream.ReadBytes(width * height); // Bitmap array.

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Register)) return false;

            return this == (Register)obj;
        }

        public override int GetHashCode()
        {
            return this.Serialize().CalculateChecksum().GetHashCode();
        }
        #endregion
    }

    class FPGEnumerator : IEnumerator<MAP>
    {
        #region Internal vars
        IList<Register> _registers;
        int _currentIndex;
        #endregion

        #region Properties
        public MAP Current { get; private set; }
        object IEnumerator.Current => this.Current;
        #endregion

        #region Constructor & Destructor
        public FPGEnumerator(IList<Register> registers)
        {
            this._registers = registers;
            this.Current = default(MAP);
            this.Reset();
        }

        void IDisposable.Dispose()
        {
        }
        #endregion

        #region Methods & Functions
        public bool MoveNext()
        {
            if (++this._currentIndex >= this._registers.Count)
                return false;
            else
                this.Current = this._registers[this._currentIndex].map;

            return true;
        }

        public void Reset()
        {
            this._currentIndex = -1;
        }
        #endregion
    }

    /// <summary>
    /// A representation of a DIV Games Studio FPG file.
    /// </summary>
    /// <remarks>Implements functions to import, export and create File Package Graphic files.</remarks>
    public sealed class FPG : IAssetFile, IEnumerable<MAP>
    {
        #region Constants
        readonly static DIVFileHeader FPG_FILE_HEADER = new DIVFileHeader('f', 'p', 'g');
        readonly static FPG VALIDATOR = new FPG();
        #endregion

        #region Internal vars
        List<Register> _registers;
        #endregion

        #region Properties
        /// <summary>
        /// Color palette used by this <see cref="FPG"/>.
        /// </summary>
        public PAL Palette { get; private set; }
        /// <summary>
        /// Gets the number of <see cref="MAP"/> instances contained in the <see cref="FPG"/>.
        /// </summary>
        public int Count => this._registers.Count;
        /// <summary>
        /// Get a <see cref="MAP"/> instance.
        /// </summary>
        /// <param name="index">Index of the <see cref="MAP"/> in the <see cref="FPG"/>.</param>
        /// <returns>Returns the <see cref="MAP"/> instance.</returns>
        public MAP this[int index]
        {
            get
            {
                if (!index.IsClamped(0, this.Count - 1))
                    throw new IndexOutOfRangeException($"The index value must be a value beteween 0 and {this.Count}. (Index: {index})");

                return this._registers[index].map;
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(FPG a, FPG b)
        {
            if (a.Palette == b.Palette)
            {
                if (a.Count != b.Count)
                    return false;

                for (int i = 0; i < a.Count; i++)
                    if (a._registers[i] != b._registers[i])
                        return false;

                return true;
            }
            else
                return false;
        }

        public static bool operator !=(FPG a, FPG b)
        {
            return !(a == b);
        }
        #endregion

        #region Constructors
        FPG()
        {
            this._registers = new List<Register>();
        }

        /// <summary>
        /// Creates a new <see cref="FPG"/> instance.
        /// </summary>
        /// <param name="palette">The <see cref="PAL"/> instance used in this <see cref="FPG"/> instance.</param>
        public FPG(PAL palette)
            : this()
        {
            this.Palette = palette;
        }

        /// <summary>
        /// Loads a <see cref="FPG"/> file.
        /// </summary>
        /// <param name="filename">Filename to load.</param>
        public FPG(string filename)
            : this(File.ReadAllBytes(filename))
        {
        }

        /// <summary>
        /// Loads a <see cref="FPG"/> file.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array that contain the <see cref="FPG"/> file data to load.</param>
        public FPG(byte[] buffer)
            : this()
        {
            try
            {
                using (var stream = new BinaryReader(new MemoryStream(buffer)))
                {
                    if (FPG_FILE_HEADER.Validate(stream.ReadBytes(DIVFileHeader.SIZE)))
                    {
                        this.Palette = new PAL(new ColorPalette(stream.ReadBytes(ColorPalette.SIZE)),
                                               new ColorRangeTable(stream.ReadBytes(ColorRangeTable.SIZE)));

                        while (stream.BaseStream.Position < stream.BaseStream.Length)
                            this._registers.Add(new Register(stream, this.Palette));

                        this._registers.Sort(this.OrderByAsc);
                    }
                    else
                        throw new DIVFormatHeaderException();
                }
            }
            catch (Exception ex)
            {
                throw new DIVFileFormatException<FPG>(ex);
            }
        }
        #endregion

        #region Methods & Functions
        // Sort MAP list by graphic identifiers in ascending order:
        int OrderByAsc(Register x, Register y)
        {
            if (x.GraphId > y.GraphId)
                return 1;
            else if (x.GraphId == y.GraphId)
                return 0;
            else
                return -1;
        }

        /// <summary>
        /// Adds a <see cref="MAP"/> file.
        /// </summary>
        /// <param name="filename">Filename to load.</param>
        public void Add(string filename)
        {
            this.Add(new MAP(File.ReadAllBytes(filename)));
        }

        /// <summary>
        /// Adds a <see cref="MAP"/> file.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array that contain the <see cref="MAP"/> file data to load.</param>
        /// <param name="filename">Optional filename value in DOS 8:3 format.</param>
        public void Add(byte[] buffer, string filename = "")
        {
            this.Add(new MAP(buffer), filename);
        }

        /// <summary>
        /// Adds a <see cref="MAP"/> file.
        /// </summary>
        /// <param name="map"><see cref="MAP"/> instance to add.</param>
        /// <param name="filename">Optional filename value in DOS 8:3 format.</param>
        public void Add(MAP map, string filename = "")
        {
            if (this.Contains(map))
                throw new ArgumentException($"This {nameof(MAP)} already exists.");

            if (this.Contains(map.GraphId))
                throw new ArgumentException($"This {nameof(MAP.GraphId)} already is in use for some {nameof(MAP)} instance.");

            MAP image = map;

            if (this.Palette != map.Palette)
                image = MAP.FromImage(map.SerializeFile(), this.Palette); // Converts source MAP colors to FPG colors.

            this._registers.Add(new Register() { map = image, filename = filename });
            this._registers.Sort(this.OrderByAsc);
        }

        /// <summary>
        /// Determines whether a <see cref="MAP"/> is in this instance.
        /// </summary>
        /// <param name="map">The <see cref="MAP"/> to locate in this instance.</param>
        /// <returns>Returns true if the <see cref="MAP"/> exists.</returns>
        public bool Contains(MAP map)
        {
            foreach (var item in this)
                if (item == map)
                    return true;

            return false;
        }

        /// <summary>
        /// Determines whether a <see cref="MAP"/> with a graphic identifier is in this instance.
        /// </summary>
        /// <param name="graphId"><see cref="MAP"/> graphic identifier to search.</param>
        /// <returns>Returns true if a <see cref="MAP"/> graphic identifier exists.</returns>
        public bool Contains(int graphId)
        {
            foreach (var map in this)
                if (map.GraphId == graphId)
                    return true;

            return false;
        }

        /// <summary>
        /// Removes a <see cref="MAP"/> from this instance.
        /// </summary>
        /// <param name="map"><see cref="MAP"/> instance to remove.</param>
        public void Remove(MAP map)
        {
            if (this._registers.Count == 0)
                throw new InvalidOperationException($"The {nameof(FPG)} is empty!");

            for (int i = 0; i < this.Count; i++)
                if (this[i] == map)
                {
                    this._registers.RemoveAt(i);
                    return;
                }

            throw new ArgumentException($"The {nameof(MAP)} instance not exists in this {nameof(FPG)}.");
        }

        /// <summary>
        /// Removes a <see cref="MAP"/> from this instance.
        /// </summary>
        /// <param name="graphId"><see cref="MAP.GraphId"/> to search.</param>
        public void Remove(int graphId)
        {
            if (this._registers.Count == 0)
                throw new InvalidOperationException($"The {nameof(FPG)} is empty!");

            for (int i = 0; i < this.Count; i++)
                if (this[i].GraphId == graphId)
                {
                    this.RemoveAt(i);
                    return;
                }

            throw new ArgumentException($"The {nameof(MAP)} with {nameof(MAP.GraphId)} {graphId} not exists in this {nameof(FPG)}.");
        }

        /// <summary>
        /// Removes a <see cref="MAP"/> from this instance.
        /// </summary>
        /// <param name="index"><see cref="MAP"/> index in this instance.</param>
        public void RemoveAt(int index)
        {
            if (this._registers.Count == 0)
                throw new InvalidOperationException($"The {nameof(FPG)} is empty!");

            if (!index.IsClamped(0, this._registers.Count - 1))
                throw new IndexOutOfRangeException($"The {nameof(MAP)} index {index} is out of range in this {nameof(FPG)}.");

            this._registers.RemoveAt(index);
        }

        /// <summary>
        /// Removes all <see cref="MAP"/> in this instance.
        /// </summary>
        public void Clear()
        {
            this._registers.Clear();
        }

        /// <summary>
        /// Gets the associate filename value for this <see cref="MAP"/> in the <see cref="FPG"/>.
        /// </summary>
        /// <param name="map"><see cref="MAP"/> instance to find.</param>
        /// <returns>Returns the <see cref="MAP"/> filename value.</returns>
        public string GetFilename(MAP map)
        {
            for (int i = 0; i < this._registers.Count; i++)
                if (this._registers[i].map == map)
                    return this.GetFilename(i);

            throw new ArgumentException(nameof(map), $"The {nameof(MAP)} instance not exists in this {nameof(FPG)}");
        }

        /// <summary>
        /// Gets the associate filename value for this <see cref="MAP"/> in the <see cref="FPG"/>.
        /// </summary>
        /// <param name="index">The index of the <see cref="MAP"/> instance to find. Not confuse with the <see cref="MAP.GraphId"/> value.</param>
        /// <returns>Returns the <see cref="MAP"/> filename value.</returns>
        public string GetFilename(int index)
        {
            return this._registers[index].filename;
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="FPG"/> file.
        /// </summary>
        /// <param name="filename">File to validate.</param>
        /// <returns>Returns true if the file is a valid <see cref="FPG"/>.</returns>
        public static bool ValidateFormat(string filename)
        {
            return VALIDATOR.Validate(filename);
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="FPG"/> file.
        /// </summary>
        /// <param name="buffer">Memory buffer that contain a <see cref="FPG"/> file data.</param>
        /// <returns>Returns true if the file is a valid <see cref="FPG"/>.</returns>
        public static bool ValidateFormat(byte[] buffer)
        {
            return VALIDATOR.Validate(buffer);
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="FPG"/> file.
        /// </summary>
        /// <param name="filename">File to validate.</param>
        /// <returns>Returns true if the file is a valid <see cref="FPG"/>.</returns>
        public bool Validate(string filename)
        {
            return this.Validate(File.ReadAllBytes(filename));
        }

        /// <summary>
        /// Validate if the file is a valid <see cref="FPG"/> file.
        /// </summary>
        /// <param name="buffer">Memory buffer that contain a <see cref="FPG"/> file data.</param>
        /// <returns>Returns true if the file is a valid <see cref="FPG"/>.</returns>
        public bool Validate(byte[] buffer)
        {
            return FPG_FILE_HEADER.Validate(buffer[0..DIVFileHeader.SIZE]) && this.TryToReadFile(buffer);
        }

        bool TryToReadFile(byte[] buffer)
        {
            try
            {
                using (var stream = new BinaryReader(new MemoryStream(buffer)))
                {
                    stream.ReadBytes(DIVFileHeader.SIZE); // DIV Header.

                    // Palette:
                    stream.ReadBytes(ColorPalette.SIZE);
                    stream.ReadBytes(ColorRangeTable.SIZE);

                    // Try to read all registers:
                    while (stream.BaseStream.Position < stream.BaseStream.Length)
                        if (!Register.TryToRead(stream))
                            return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Serialize the <see cref="FPG"/> instance in a <see cref="byte"/> array.
        /// </summary>
        /// <returns>Returns the <see cref="byte"/> array with the <see cref="FPG"/> serialized data.</returns>
        /// <remarks>This function not include the file header data.</remarks>
        public byte[] Serialize()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                this.Palette.Write(stream);
                foreach (var register in this._registers)
                    register.Write(stream);

                return (stream.BaseStream as MemoryStream).ToArray();
            }
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write(this.Serialize());
        }

        /// <summary>
        /// Save the instance in a <see cref="FPG"/> file.
        /// </summary>
        /// <param name="filename">Filename to write the data.</param>
        public void Save(string filename)
        {
            using (var stream = new BinaryWriter(File.OpenWrite(filename)))
            {
                FPG_FILE_HEADER.Write(stream);
                this.Write(stream);
            }
        }

        public IEnumerator<MAP> GetEnumerator()
        {
            return new FPGEnumerator(this._registers);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FPG)) return false;

            return this == (FPG)obj;
        }

        public override int GetHashCode()
        {
            return this.Serialize().CalculateChecksum().GetSecureHashCode();
        }

        public override string ToString()
        {
            int registersHash = 0;
            foreach (var register in this._registers)
                registersHash ^= register.GetHashCode();

            var sb = new StringBuilder();

            sb.Append($"{{ {nameof(FPG)}: ");
            sb.Append($"{{ Hash: {this.GetHashCode()}, ");
            sb.Append($"Palette Hash: {this.Palette.GetHashCode()}, ");
            sb.Append($"MAP Registers Hashs: {registersHash} }}");
            sb.Append(" }");

            return sb.ToString();
        }
        #endregion
    }
}
