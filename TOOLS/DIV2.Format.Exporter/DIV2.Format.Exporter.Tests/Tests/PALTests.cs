using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class PALTests : AbstractTest
    {
        #region Constants
        const string RESULT_FOLDER_NAME = "PAL";
        #endregion

        #region Intenral vars
        PAL _palette;
        #endregion

        [TestInitialize]
        public void Initialize()
        {
            this.InitializeResultFolder(RESULT_FOLDER_NAME);
            this.CreateNewPalette();
            for (int i = 0; i < PAL.LENGTH; i++)
                this._palette[i] = new Color(i, i, i).ToDAC();
        }

        #region Test methods
        [TestMethod]
        public void ValidateFile()
        {
            Assert.IsTrue(PAL.ValidateFormat(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV)));
        }

        [TestMethod]
        public void ValidateBuffer()
        {
            byte[] buffer = File.ReadAllBytes(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            Assert.IsTrue(PAL.ValidateFormat(buffer));
        }

        [TestMethod]
        public void FailValidateFile()
        {
            Assert.IsFalse(PAL.ValidateFormat(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_BMP)));
        }

        [TestMethod]
        public void FailValidateBuffer()
        {
            byte[] buffer = File.ReadAllBytes(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_BMP));
            Assert.IsFalse(PAL.ValidateFormat(buffer));
        }

        [TestMethod]
        public void CreateNewPalette()
        {
            this._palette = new PAL();
        }

        [TestMethod]
        public void ReadColorsByIndex()
        {
            Color color;
            for (int i = 0; i < PAL.LENGTH; i++)
                color = this._palette[i];
        }

        [TestMethod]
        public void ReadColorsByForEach()
        {
            Color color;
            foreach (var value in this._palette)
                color = value;
        }

        [TestMethod]
        public void Serialize()
        {
            Assert.AreEqual(this._palette.Serialize().Length, ColorPalette.SIZE + ColorRangeTable.SIZE);
        }

        [TestMethod]
        public void Write()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                this._palette.Write(stream);
            }
        }

        [TestMethod]
        public void SaveFile()
        {
            this._palette.Save(this.GetOutputPath("GRAYSCAL.PAL"));
        }

        [TestMethod]
        public void LoadFromFilename()
        {
            new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
        }

        [TestMethod]
        public void LoadFromBuffer()
        {
            byte[] buffer = File.ReadAllBytes(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            new PAL(buffer);
        }

        [DataTestMethod]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_PCX)]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_BMP)]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_PNG)]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_MAP)]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_FPG)]
        public void CreateFromImage(string file)
        {
            var pal = PAL.FromImage(this.GetAssetPath(file));
            string saveFilename = $"{Path.GetExtension(file)[1..4]}.PAL";
            pal.Save(this.GetOutputPath(saveFilename));
        }

        [TestMethod]
        public void AreEqual()
        {
            var a = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            var b = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void AreNotEqual()
        {
            var a = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            var b = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_SPACE));

            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void AreEqualByCompare()
        {
            var a = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            var b = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));

            Assert.IsTrue(a.Compare(b));
        }

        [TestMethod]
        public void AreNotEqualByCompare()
        {
            var a = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            var b = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_SPACE));

            Assert.IsFalse(a.Compare(b));
        }
        #endregion
    }
}
