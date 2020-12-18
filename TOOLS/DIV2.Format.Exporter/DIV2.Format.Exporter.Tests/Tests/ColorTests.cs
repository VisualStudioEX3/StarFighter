using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class ColorTests
    {
        #region Constants
        const byte RED = 64;
        const byte GREEN = 128;
        const byte BLUE = 255;

        readonly byte[] COLOR_VALUES = { RED, GREEN, BLUE };

        readonly Color RGB_COLOR = new Color(255, 255, 255);
        readonly Color DAC_COLOR = new Color(63, 63, 63);
        #endregion

        #region Test methods
        [TestMethod]
        public void ByteConstructor()
        {
            var color = new Color(COLOR_VALUES[0], COLOR_VALUES[1], COLOR_VALUES[2]);
            Assert.IsTrue(color.red == RED &&
                          color.green == GREEN &&
                          color.blue == BLUE);
        }

        [TestMethod]
        public void ByteArrayConstructor()
        {
            var color = new Color(COLOR_VALUES);
            Assert.IsTrue(COLOR_VALUES[0] == color.red &&
                          COLOR_VALUES[1] == color.green &&
                          COLOR_VALUES[2] == color.blue);
        }

        [TestMethod]
        public void FailByteArrayConstructor()
        {
            try
            {
                new Color(new byte[5]);
            }
            catch
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void AreEquals()
        {
            Assert.AreEqual(RGB_COLOR, RGB_COLOR);
        }

        [TestMethod]
        public void AreNotEquals()
        {
            Assert.AreNotEqual(RGB_COLOR, DAC_COLOR);
        }

        [TestMethod]
        public void RGB2DAC()
        {
            RGB_COLOR.ToDAC();
        }

        [TestMethod]
        public void DAC2RGB()
        {
            DAC_COLOR.ToRGB();
        }

        [TestMethod]
        public void ReadComponentsByIndex()
        {
            var color = new Color(RED, GREEN, BLUE);
            for (int i = 0; i < Color.LENGTH; i++)
                Assert.AreEqual(color[i], COLOR_VALUES[i]);
        }

        [TestMethod]
        public void WriteComponentsByIndex()
        {
            var color = new Color();
            for (int i = 0; i < COLOR_VALUES.Length; i++)
            {
                color[i] = COLOR_VALUES[i];
                Assert.AreEqual(color[i], COLOR_VALUES[i]);
            }
        }

        [TestMethod]
        public void Serialize()
        {
            Assert.AreEqual(RGB_COLOR.Serialize().Length, Color.SIZE);
        }

        [TestMethod]
        public void Write()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                RGB_COLOR.Write(stream);
            }
        }
        #endregion
    }
}
