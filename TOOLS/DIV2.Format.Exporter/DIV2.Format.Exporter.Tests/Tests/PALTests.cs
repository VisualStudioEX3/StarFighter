using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using DIV2.Format.Exporter.MethodExtensions;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class PALTests : 
        AbstractTest,
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
        const string RESULT_FOLDER_NAME = "PAL";
        #endregion

        #region Intenral vars
        Color[] _colors = new Color[PAL.LENGTH];
        #endregion

        #region Initializer
        [TestInitialize]
        public void Initialize()
        {
            this.InitializeResultFolder(RESULT_FOLDER_NAME);
            for (int i = 0; i < PAL.LENGTH; i++)
                this._colors[i] = new Color(i, i, i).ToDAC();
        }
        #endregion

        #region Test methods
        [TestMethod]
        public void CreateDefaultInstance()
        {
            var palette = new PAL();
            for (int i = 0; i < PAL.LENGTH; i++)
                Assert.AreEqual(default(Color), palette[i]);
        }

        [TestMethod]
        public void TestConstructors()
        {
            PAL palette = null;

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0: palette = new PAL(this._colors.ToDAC()); break;
                    case 1: palette = new PAL(new ColorPalette(this._colors.ToDAC())); break;
                    case 2: palette = new PAL(new ColorPalette(this._colors.ToDAC()), new ColorRangeTable()); break;
                }

                for (int j = 0; j < PAL.LENGTH; j++)
                    Assert.AreEqual(this._colors[j].ToDAC(), palette[j]);
            }
        }

        [TestMethod]
        public void CreateInstanceFromBuffer()
        {
            var palette = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
        }

        [TestMethod]
        public void CreateInstanceFromFile()
        {
            new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
        }

        [DataTestMethod]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_PCX)]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_BMP)]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_PNG)]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_MAP)]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_FPG)]
        public void CreateFromImage(string file)
        {
            var palette = PAL.FromImage(this.GetAssetPath(file));
            string saveFilename = $"{Path.GetExtension(file)[1..4]}.PAL";
            palette.Save(this.GetOutputPath(saveFilename));
        }

        [TestMethod]
        public void FailCreateByRGBColors()
        {
            try
            {
                new PAL(this._colors);
                Assert.Fail();
            }
            catch
            {
            }
        }

        [TestMethod]
        public void AreEqual()
        {
            var a = new PAL(this._colors.ToDAC());
            var b = new PAL(this._colors.ToDAC());

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void AreNotEqual()
        {
            var a = new PAL();
            var b = new PAL(this._colors.ToDAC());

            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void ReadByIndex()
        {
            var palette = new PAL(this._colors.ToDAC());
            for (int i = 0; i < PAL.LENGTH; i++)
                Assert.AreEqual(this._colors[i].ToDAC(), palette[i]);
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            try
            {
                _ = new PAL()[-1];
                Assert.Fail();
            }
            catch
            {
                try
                {
                    _ = new PAL()[PAL.LENGTH + 1];
                    Assert.Fail();
                }
                catch
                {
                }
            }
        }

        [TestMethod]
        public void WriteByIndex()
        {
            var palette = new PAL();
            for (int i = 0; i < PAL.LENGTH; i++)
                palette[i] = new Color(i, i, i).ToDAC();

            for (int i = 0; i < PAL.LENGTH; i++)
                Assert.AreEqual(new Color(i, i, i).ToDAC(), palette[i]);
        }

        [TestMethod]
        public void FailWriteByIndex()
        {
            try
            {
                new PAL()[-1] = new Color();
                Assert.Fail();
            }
            catch
            {
                try
                {
                    new PAL()[PAL.LENGTH + 1] = new Color();
                    Assert.Fail();
                }
                catch
                {
                }
            }
        }

        [TestMethod]
        public void TryToSetFullRGBColor()
        {
            try
            {
                new PAL()[0] = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Assert.Fail();
            }
            catch
            {
            }
        }

        [TestMethod]
        public void ReadByForEach()
        {
            int i = 0;
            var palette = new PAL(this._colors.ToDAC());
            foreach (var value in palette)
                Assert.AreEqual(palette[i++], value);
        }

        [TestMethod]
        public void Serialize()
        {
            byte[] serialized = new PAL().Serialize();
            int expectedSize = ColorPalette.SIZE + ColorRangeTable.SIZE;
            Assert.AreEqual(expectedSize, serialized.Length);
        }

        [TestMethod]
        public void Write()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                new PAL().Write(stream);
            }
        }

        [TestMethod]
        public void Validate()
        {
            Assert.IsTrue(PAL.ValidateFormat(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV)));

            byte[] buffer = File.ReadAllBytes(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            Assert.IsTrue(PAL.ValidateFormat(buffer));
        }

        [TestMethod]
        public void FailValidate()
        {
            Assert.IsFalse(PAL.ValidateFormat(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_BMP)));

            byte[] buffer = File.ReadAllBytes(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_BMP));
            Assert.IsFalse(PAL.ValidateFormat(buffer));
        }

        [TestMethod]
        public void Save()
        {
            new PAL(this._colors.ToDAC()).Save(this.GetOutputPath("GRAYSCAL.PAL"));
        }

        [TestMethod]
        public void ToRGB()
        {
            var palette = new PAL(this._colors.ToDAC());
            palette.ToRGB();
        }
        #endregion
    }
}
