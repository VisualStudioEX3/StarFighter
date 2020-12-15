using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class ColorPaletteTests : AbstractTest
    {
        #region Intenral vars
        ColorPalette _palette;
        #endregion

        #region Helper Functions
        public ColorPalette GenerateTestPalette()
        {
            if (this._palette is null)
            {
                this._palette = new ColorPalette();

                for (int i = 0; i < ColorPalette.LENGTH; i++)
                    this._palette[i] = new Color(i / 4, i / 4, i / 4);
            }

            return this._palette;
        }
        #endregion

        #region Tests methods
        [TestMethod]
        public void ReadColorsByIndex()
        {
            Color color;
            var pal = this.GenerateTestPalette();
            for (int i = 0; i < PAL.LENGTH; i++)
                color = pal[i];
        }

        [TestMethod]
        public void ReadColorsByForEach()
        {
            Color color;
            var pal = this.GenerateTestPalette();
            foreach (var value in pal)
                color = value;
        } 
        #endregion
    }
}
