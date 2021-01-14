using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class FPGTests
        : AbstractTest,
        IDefaultConstructorsTests,
        IEqualityTests,
        ISerializableAssetTests,
        IIterableReadTests,
        IEnumerableTests,
        IFormatValidableTests,
        IAssetFileTests
    {
        #region Constants
        const string RESULT_FOLDER_NAME = "FPG";
        const int FPG_TEST_REGISTERS_COUNT = 5;
        const int FPG_REGISTER_BASE_SIZE = 64;
        #endregion

        #region Structs
        struct Register
        {
            #region Public vars
            public int graphId;
            public int width;
            public int height;
            public string description;
            public string filename;
            public ControlPoint[] controlPoints;
            #endregion

            #region Properties
            public int BitmapLength => this.width * this.height;
            public int Size => (sizeof(int) * 5) + // GraphId + Register size + Width + Height + Control Point counter
                               MAP.DESCRIPTION_LENGTH + 12 + // Description + Filename
                               (ControlPoint.SIZE * this.controlPoints.Length) + // Control points
                               this.BitmapLength; // Bitmap data
            #endregion
        }
        #endregion

        #region Internal vars
        PAL _palette;
        List<Register> _testFPGRegisters;

        static bool _isFirstRun = true;
        #endregion

        #region Helper functions
        void AssertAreEqualDefaultFPG(FPG fpg)
        {
            Assert.AreEqual(this._palette, fpg.Palette);
            Assert.AreEqual(FPG_TEST_REGISTERS_COUNT, fpg.Count);

            for (int i = 0; i < FPG_TEST_REGISTERS_COUNT; i++)
                AssertAreEqualDefaultRegisters(fpg, i);
        }

        void AssertAreEqualDefaultRegisters(Register reg, MAP map, string filename)
        {
            Assert.AreEqual(reg.graphId, map.GraphId);
            Assert.AreEqual(reg.width, map.Width);
            Assert.AreEqual(reg.height, map.Height);
            Assert.AreEqual(reg.description, map.Description);
            Assert.AreEqual(reg.filename, filename);
            Assert.AreEqual(reg.controlPoints.Length, map.ControlPoints.Count);
            for (int i = 0; i < reg.controlPoints.Length; i++)
                Assert.AreEqual(reg.controlPoints[i], map.ControlPoints[i]);
            Assert.AreEqual(reg.BitmapLength, map.Count);
        }

        void AssertAreEqualDefaultRegisters(FPG fpg, int index)
        {
            this.AssertAreEqualDefaultRegisters(this._testFPGRegisters[index], fpg[index], fpg.GetFilename(index));
        }

        int GetDefaultFPGSize()
        {
            int size = ColorPalette.SIZE + ColorRangeTable.SIZE;
            foreach (var reg in this._testFPGRegisters)
                size += FPG_REGISTER_BASE_SIZE + (ControlPoint.SIZE * reg.controlPoints.Length) + reg.BitmapLength;
            return size;
        }
        #endregion

        #region Initializer
        [TestInitialize]
        public void Initialize()
        {
            this.InitializeResultFolder(RESULT_FOLDER_NAME);

            if (_isFirstRun)
            {
                this.CleanUp();
                _isFirstRun = false;
            }

            this._palette = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            this._testFPGRegisters = new List<Register>() // This is the content of the TEST.FPG asset.
            {
                new Register()
                {
                    graphId = 24,
                    width = 128,
                    height = 128,
                    description = "Asteroid #24",
                    filename = "ASTER24.BMP",
                    controlPoints = new ControlPoint[0]
                },
                new Register()
                {
                    graphId = 100,
                    width = 256,
                    height = 256,
                    description = "TEST PLAYER",
                    filename = "PLAYER.BMP",
                    controlPoints = new ControlPoint[7]
                        {
                            new ControlPoint(128, 128), // Undefined control point (x:-1, y:-1). Must be received a default control point values (MAP center).
                            new ControlPoint(127, 159),
                            new ControlPoint(169, 164),
                            new ControlPoint(19, 154),
                            new ControlPoint(234, 154),
                            new ControlPoint(96, 103),
                            new ControlPoint(157, 103),
                        }
                },
                new Register()
                {
                    graphId = 320,
                    width = 256,
                    height = 256,
                    description = "Enemy sprite 13",
                    filename = "Enemy.bmp",
                    controlPoints = new ControlPoint[4]
                        {
                            new ControlPoint(157, 138),
                            new ControlPoint(94, 116),
                            new ControlPoint(64, 64),
                            new ControlPoint(138, 57),
                        }
                },
                new Register()
                {
                    graphId = 600,
                    width = 800,
                    height = 480,
                    description = "ASTEROID FIELD",
                    filename = "ASTEROID.PCX",
                    controlPoints = new ControlPoint[0]
                },
                new Register()
                {
                    graphId = 601,
                    width = 800,
                    height = 480,
                    description = "SPACE BACKGROUND",
                    filename = "SPACE.PCX",
                    controlPoints = new ControlPoint[0]
                }
            };
        }
        #endregion

        #region Test Methods
        [TestMethod]
        public void CreateDefaultInstance()
        {
            var fpg = new FPG(this._palette);
            Assert.AreEqual(this._palette, fpg.Palette);
            Assert.AreEqual(0, fpg.Count);
        }

        [TestMethod]
        public void CreateInstanceFromBuffer()
        {
            byte[] buffer = File.ReadAllBytes(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            var fpg = new FPG(buffer);
            this.AssertAreEqualDefaultFPG(fpg);
        }

        [TestMethod]
        public void CreateInstanceFromFile()
        {
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            this.AssertAreEqualDefaultFPG(fpg);
        }

        [TestMethod]
        public void AreEqual()
        {
            string assetPath = this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST);
            var a = new FPG(assetPath);
            var b = new FPG(assetPath);

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void AreNotEqual()
        {
            var a = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            var b = new FPG(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_FPG));

            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void ReadByIndex()
        {
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            for (int i = 0; i < FPG_TEST_REGISTERS_COUNT; i++)
                AssertAreEqualDefaultRegisters(fpg, i);
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            Assert.ThrowsException<IndexOutOfRangeException>(() => _ = fpg[-1]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => _ = fpg[fpg.Count]);
        }

        [TestMethod]
        public void ReadByForEach()
        {
            int i = 0;
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            foreach (var map in fpg)
            {
                AssertAreEqualDefaultRegisters(this._testFPGRegisters[i], map, fpg.GetFilename(i));
                i++;
            }
        }

        [TestMethod]
        public void AddMapWithTheSamePalette()
        {
            const string PLAYER_MAP_FILENAME_FIELD = "PLAYER.MAP";

            var pal = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_SPACE));
            var map = new MAP(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));
            var fpg = new FPG(pal);

            fpg.Add(map, PLAYER_MAP_FILENAME_FIELD);

            Assert.AreEqual(pal, fpg.Palette);
            Assert.AreEqual(1, fpg.Count);
            Assert.AreEqual(map, fpg[0]);
            Assert.AreEqual(PLAYER_MAP_FILENAME_FIELD, fpg.GetFilename(0));
        }

        [TestMethod]
        public void AddMapWithDifferentPalette()
        {
            const string PLAYER_MAP_FILENAME_FIELD = "PLAYER.MAP";
            string playerMapPath = this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP);

            var pal = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            var map = new MAP(playerMapPath);
            var fpg = new FPG(pal);

            fpg.Add(map, PLAYER_MAP_FILENAME_FIELD);

            Assert.AreEqual(pal, fpg.Palette);
            Assert.AreEqual(1, fpg.Count);
            Assert.AreEqual(PLAYER_MAP_FILENAME_FIELD, fpg.GetFilename(0));

            map = MAP.FromImage(playerMapPath, pal);
            Assert.AreEqual(map, fpg[0]);
        }

        [TestMethod]
        [DataRow(24)]
        [DataRow(100)]
        [DataRow(320)]
        [DataRow(600)]
        [DataRow(601)]
        public void ContainsGraphId(int graphId)
        {
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            Assert.IsTrue(fpg.Contains(graphId));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(120)]
        [DataRow(255)]
        public void FailContainsGraphId(int graphId)
        {
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            Assert.IsFalse(fpg.Contains(graphId));
        }

        [TestMethod]
        public void ContainsMap()
        {
            var pal = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_SPACE));
            var map = new MAP(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));
            var fpg = new FPG(pal);

            fpg.Add(map);

            Assert.IsTrue(fpg.Contains(map));
        }

        [TestMethod]
        public void FailContainsMap()
        {
            var pal = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            var map = new MAP(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));
            var fpg = new FPG(pal);

            fpg.Add(map); // Force palette conversion.

            Assert.IsFalse(fpg.Contains(map));
        }

        [TestMethod]
        public void RemoveMapByGraphId()
        {
            const int GRAPH_ID = 100; // PLAYER.MAP, index 1.
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));

            fpg.Remove(GRAPH_ID);

            Assert.IsFalse(fpg.Contains(GRAPH_ID));
        }

        [TestMethod]
        public void FailRemoveMapByGraphId()
        {
            const int GRAPH_ID = 333;
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            Assert.ThrowsException<ArgumentException>(() => fpg.Remove(GRAPH_ID));
        }

        [TestMethod]
        public void FailRemoveMapByGraphIdWhenFPGIsEmpty()
        {
            const int GRAPH_ID = 333;
            var pal = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_SPACE));
            var fpg = new FPG(pal);
            Assert.ThrowsException<InvalidOperationException>(() => fpg.Remove(GRAPH_ID));
        }

        [TestMethod]
        public void RemoveMapByInstance()
        {
            var pal = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_SPACE));
            var map = new MAP(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));
            var fpg = new FPG(pal);

            fpg.Add(map);
            Assert.IsTrue(fpg.Contains(map));

            fpg.Remove(map);
            Assert.IsFalse(fpg.Contains(map));
        }

        [TestMethod]
        public void FailRemoveMapByInstance()
        {
            var pal = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_SPACE));
            var fpg = new FPG(pal);

            var a = new MAP(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));
            var b = MAP.FromImage(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_BMP));

            fpg.Add(b);

            Assert.ThrowsException<ArgumentException>(() => fpg.Remove(a));
        }

        [TestMethod]
        public void FailRemoveMapByInstanceWhenFPGIsEmpty()
        {
            var pal = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_SPACE));
            var map = new MAP(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));
            var fpg = new FPG(pal);

            Assert.ThrowsException<InvalidOperationException>(() => fpg.Remove(map));
        }

        [TestMethod]
        public void RemoveMapByIndex()
        {
            const int GRAPH_ID = 100; // PLAYER.MAP, index 1.
            const int INDEX = 1;
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));

            fpg.RemoveAt(INDEX);

            Assert.IsFalse(fpg.Contains(GRAPH_ID));
        }

        [TestMethod]
        public void FailRemoveMapByIndex()
        {
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            Assert.ThrowsException<IndexOutOfRangeException>(() => fpg.RemoveAt(-1));
            Assert.ThrowsException<IndexOutOfRangeException>(() => fpg.RemoveAt(fpg.Count + 1));
        }

        [TestMethod]
        public void FailRemoveMapByIndexWhenFPGIsEmpty()
        {
            var pal = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_SPACE));
            var fpg = new FPG(pal);
            Assert.ThrowsException<InvalidOperationException>(() => fpg.Remove(0));
        }

        [TestMethod]
        public void ClearFPG()
        {
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            Assert.AreEqual(FPG_TEST_REGISTERS_COUNT, fpg.Count);

            fpg.Clear();
            Assert.AreEqual(0, fpg.Count);
        }

        [TestMethod]
        public void Serialize()
        {
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            byte[] serialized = fpg.Serialize();
            int expectedSize = this.GetDefaultFPGSize();

            Assert.AreEqual(expectedSize, serialized.Length);
        }

        [TestMethod]
        public void Write()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
                fpg.Write(stream);

                int expectedSize = this.GetDefaultFPGSize();
                Assert.AreEqual(expectedSize, stream.BaseStream.Length);
            }
        }

        [TestMethod]
        public void Validate()
        {
            string filename = this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST);
            Assert.IsTrue(FPG.ValidateFormat(filename));
        }

        [TestMethod]
        public void FailValidate()
        {
            string filename = this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_BMP);
            Assert.IsFalse(FPG.ValidateFormat(filename));
        }

        [TestMethod]
        public void Save()
        {
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            string filename = this.GetOutputPath(SharedConstants.FILENAME_FPG_TEST);

            fpg.Save(filename);

            fpg = new FPG(filename);
            this.AssertAreEqualDefaultFPG(fpg);
        }
        #endregion
    }
}
