using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class ColorPaletteTests : AbstractTest
    {
        #region Intenral vars
        ColorPalette _palette = new ColorPalette();
        #endregion

        #region Initializer
        [TestInitialize]
        public void Initialize()
        {
            for (int i = 0; i < ColorPalette.LENGTH; i++)
                this._palette[i] = new Color(i, i, i).ToDAC();
        } 
        #endregion

        #region Tests methods
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
            Assert.AreEqual(this._palette.Serialize().Length, ColorPalette.SIZE);
        }

        [TestMethod]
        public void Write()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                this._palette.Write(stream);
            }
        }
        #endregion
    }
}
