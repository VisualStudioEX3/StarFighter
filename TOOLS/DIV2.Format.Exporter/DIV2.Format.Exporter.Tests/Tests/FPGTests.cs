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
        IIterableWriteTests,
        IEnumerableTests,
        IFormatValidableTests,
        IAssetFileTests
    {
        #region Constants
        const string RESULT_FOLDER_NAME = "FPG";
        #endregion

        #region Structs
        struct Register
        {
            public int graphId;
            public short width;
            public short height;
            public string description;
            public string filename;
            public int points;
            public ControlPoint[] pointList;
        }
        #endregion

        #region Internal vars
        PAL _palette;
        List<Register> _testFPGRegisters;
        #endregion

        #region Helper functions
        void AssertAreEqualDefault(FPG fpg)
        {
            Assert.AreEqual(this._palette, fpg.Palette);
            Assert.AreEqual(5, fpg.Count);

            for (int i = 0; i < 5; i++)
            {
                Register reg = this._testFPGRegisters[i];
                MAP map = fpg[i];

                Assert.AreEqual(reg.graphId, map.GraphId);
                Assert.AreEqual(reg.width, map.Width);
                Assert.AreEqual(reg.height, map.Height);
                Assert.AreEqual(reg.description, map.Description);
                Assert.AreEqual(reg.filename, fpg.GetFilename(i));
                Assert.AreEqual(reg.points, map.ControlPoints.Count);
                for (int j = 0; j < reg.points; j++)
                    Assert.AreEqual(reg.pointList[i], map.ControlPoints[i]);
            }
        } 
        #endregion

        #region Initializer
        [TestInitialize]
        public void Initialize()
        {
            this.InitializeResultFolder(RESULT_FOLDER_NAME);
            this._palette = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            this._testFPGRegisters = new List<Register>()
            {
                new Register()
                {
                    graphId = 24,
                    width = 128,
                    height = 128,
                    description = "ASTEROID #24",
                    filename = "ASTER24.BMP",
                    points = 1,
                    pointList = new ControlPoint[1] 
                        { 
                            new ControlPoint(64, 64) 
                        }
                },
                new Register()
                {
                    graphId = 100,
                    width = 256,
                    height = 256,
                    description = "TEST PLAYER",
                    filename = "PLAYER.BMP",
                    points = 7,
                    pointList = new ControlPoint[7]
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
                    points = 4,
                    pointList = new ControlPoint[4]
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
                    points = 1,
                    pointList = new ControlPoint[1]
                        {
                            new ControlPoint(400, 240)
                        }
                },
                new Register()
                {
                    graphId = 601,
                    width = 800,
                    height = 480,
                    description = "SPACE BACKGROUND",
                    filename = "SPACE.PCX",
                    points = 1,
                    pointList = new ControlPoint[1]
                        {
                            new ControlPoint(400, 240)
                        }
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
            byte[] buffer = File.ReadAllBytes(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_FPG));
            var fpg = new FPG(buffer);
            this.AssertAreEqualDefault(fpg);
        }

        [TestMethod]
        public void CreateInstanceFromFile()
        {
            var fpg = new FPG(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_FPG));
            this.AssertAreEqualDefault(fpg);
        }

        [TestMethod]
        public void AreEqual()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void AreNotEqual()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void ReadByIndex()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void WriteByIndex()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void FailWriteByIndex()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void ReadByForEach()
        {
            throw new System.NotImplementedException();
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
