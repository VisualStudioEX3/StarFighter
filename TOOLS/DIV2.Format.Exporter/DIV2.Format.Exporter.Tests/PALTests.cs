using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class PALTests : AbstractTest
    {
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
        public PAL CreatePalette()
        {
            var pal = new PAL();

            for (int i = 0; i < PAL.LENGTH; i++)
                pal[i] = new Color(i, i, i);

            return pal;
        }

        [TestMethod]
        public void ReadColorsByIndex()
        {
            Color color;
            var pal = this.CreatePalette();
            for (int i = 0; i < PAL.LENGTH; i++)
                color = pal[i];
        }

        [TestMethod]
        public void ReadColorsByForEach()
        {
            Color color;
            var pal = this.CreatePalette();
            foreach (var value in pal)
                color = value;
        }

        [TestMethod]
        public void SaveFile()
        {
            var pal = this.CreatePalette();
            pal.Save(this.GetOutputPath("TEST.PAL"));
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
            string saveFilename = $"{Path.GetFileNameWithoutExtension(file)}.PAL";
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
    }
}
