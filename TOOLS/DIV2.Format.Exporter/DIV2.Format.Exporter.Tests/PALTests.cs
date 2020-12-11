using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class PALTests : AbstractTest
    {


        [TestMethod]
        public void LoadFromFilename()
        {
            new OldPAL(this.GetAssetPath(SharedConstants.FILENAME_SPACE_PAL));
        }

        [TestMethod]
        public void LoadFromBuffer()
        {
            byte[] buffer = File.ReadAllBytes(this.GetAssetPath(SharedConstants.FILENAME_SPACE_PAL));
            new OldPAL(buffer);
        }

        [DataTestMethod]
        [DataRow(SharedConstants.FILENAME_PLAYER_PCX)]
        [DataRow(SharedConstants.FILENAME_PLAYER_BMP)]
        [DataRow(SharedConstants.FILENAME_PLAYER_PNG)]
        [DataRow(SharedConstants.FILENAME_PLAYER_MAP)]
        [DataRow(SharedConstants.FILENAME_PLAYER_FPG)]
        public void CreateFromImage(string file)
        {
            var pal = OldPAL.FromImage(this.GetAssetPath(file));
            pal.Save(this.GetOutputPath(Path.GetFileNameWithoutExtension(file)));
        }
    }
}
