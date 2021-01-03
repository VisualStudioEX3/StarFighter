using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using System.Collections.Generic;

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
        const int TEST_FPG_REGISTERS_COUNT = 5;
        #endregion

        #region Structs
        struct Register
        {
            public int graphId;
            public short width;
            public short height;
            public string description;
            public string filename;
            public ControlPoint[] controlPoints;
        }
        #endregion

        #region Internal vars
        PAL _palette;
        List<Register> _testFPGRegisters;
        #endregion

        #region Helper functions
        void AssertAreEqualDefaultFPG(FPG fpg)
        {
            Assert.AreEqual(this._palette, fpg.Palette);
            Assert.AreEqual(TEST_FPG_REGISTERS_COUNT, fpg.Count);

            for (int i = 0; i < TEST_FPG_REGISTERS_COUNT; i++)
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
            Assert.AreEqual(reg.width * reg.height, map.Count);
        }

        void AssertAreEqualDefaultRegisters(FPG fpg, int index)
        {
            this.AssertAreEqualDefaultRegisters(this._testFPGRegisters[index], fpg[index], fpg.GetFilename(index));
        }
        #endregion

        #region Initializer
        [TestInitialize]
        public void Initialize()
        {
            this.InitializeResultFolder(RESULT_FOLDER_NAME);
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
                            new ControlPoint(128, 128),
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
            for (int i = 0; i < this._testFPGRegisters.Count; i++)
                AssertAreEqualDefaultRegisters(fpg, i);
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            try
            {
                _ = fpg[-1];
                Assert.Fail();
            }
            catch
            {
                try
                {
                    _ = fpg[fpg.Count];
                    Assert.Fail();
                }
                catch
                {
                }
            }
        }

        [TestMethod]
        public void ReadByForEach()
        {
            int i = 0;
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_FPG_TEST));
            foreach (var map in fpg)
                AssertAreEqualDefaultRegisters(this._testFPGRegisters[i++], map, fpg.GetFilename(i++));
        }

        [TestMethod]
        public void AddMap()
        {
            const string PLAYER_MAP_FILENAME = "PLAYER.MAP";

            var pal = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_SPACE));
            var map = new MAP(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));
            var fpg = new FPG(pal);

            fpg.Add(map, PLAYER_MAP_FILENAME);

            Assert.AreEqual(pal, fpg.Palette);
            Assert.AreEqual(1, fpg.Count);
            Assert.AreEqual(map, fpg[0]);
            Assert.AreEqual(PLAYER_MAP_FILENAME, fpg.GetFilename(0));
        }

        [TestMethod]
        public void Serialize()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void Write()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void Validate()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void FailValidate()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void Save()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
