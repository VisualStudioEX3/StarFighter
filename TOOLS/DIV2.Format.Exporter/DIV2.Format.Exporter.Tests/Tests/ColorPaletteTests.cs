using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        public void CreateFromMemory()
        {
            var a = this._palette;
            var b = new ColorPalette(a.Serialize());

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void ReadColorsByIndex()
        {
            for (int i = 0; i < PAL.LENGTH; i++)
                _ = this._palette[i];
        }

        [TestMethod]
        public void ReadColorsByForEach()
        {
            Color color;
            foreach (var value in this._palette)
                color = value;
        }

        [TestMethod]
        public void TryToSetFullRGBColor()
        {
            var colors = new ColorPalette();
            try
            {
                colors[0] = new Color(255, 255, 255);
                Assert.Fail();
            }
            catch
            {
            }
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

        [TestMethod]
        public void AreEqual()
        {
            Assert.AreEqual(this._palette, this._palette);
        }

        [TestMethod]
        public void AreNotEqual()
        {
            var a = this._palette;

            var colors = new byte[ColorPalette.SIZE];
            new Random().NextBytes(colors);
            var b = new ColorPalette(colors);

            Assert.AreNotEqual(a, b);
        }
        #endregion
    }
}
