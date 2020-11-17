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
            new PAL(SharedConstants.FILENAME_SPACE_PAL);
        }

        [TestMethod]
        public void LoadFromBuffer()
        {
            new PAL(File.ReadAllBytes(SharedConstants.FILENAME_SPACE_PAL));
        }

        [TestMethod]
        public void CreateFromImage()
        {
            foreach (string filename in SharedConstants.IMAGE_FILES)
            {
                this.Log($"Create palette from \"{filename}\"...");
                PAL.FromImage(filename);
                this.Log($"Ok");
            }
        }
    }
}
