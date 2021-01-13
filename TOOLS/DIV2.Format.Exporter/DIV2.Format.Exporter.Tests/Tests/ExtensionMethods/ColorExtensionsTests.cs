using DIV2.Format.Exporter.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DIV2.Format.Exporter.Tests.ExtensionMethods
{
    [TestClass]
    public class ColorExtensionsTests
    {
        #region Test Methods
        [TestMethod]
        public void ToDAC()
        {
            Color[] rgb = Enumerable.Repeat(new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue), ColorPalette.LENGTH).ToArray();

            foreach (var color in rgb)
                Assert.IsFalse(color.IsDAC());

            rgb = rgb.ToDAC();

            foreach (var color in rgb)
                Assert.IsTrue(color.IsDAC());
        }

        [TestMethod]
        public void FailToDAC()
        {
            var colors = new Color[3];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => colors.ToDAC());

            colors = new Color[1024];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => colors.ToDAC());
        }

        [TestMethod]
        public void ToRGB()
        {
            Color[] dac = Enumerable.Repeat(new Color(Color.MAX_DAC_VALUE, Color.MAX_DAC_VALUE, Color.MAX_DAC_VALUE), ColorPalette.LENGTH).ToArray();

            foreach (var color in dac)
                Assert.IsTrue(color.IsDAC());

            dac = dac.ToRGB();

            foreach (var color in dac)
                Assert.IsFalse(color.IsDAC());
        }

        [TestMethod]
        public void FailToRGB()
        {
            var colors = new Color[3];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => colors.ToRGB());

            colors = new Color[1024];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => colors.ToRGB());
        }

        [TestMethod]
        public void ToColorArray()
        {
            var buffer = new byte[ColorPalette.SIZE];

            for (int i = 0, j = 0; i < ColorPalette.LENGTH; i++)
                for (int k = 0; k < 3; k++)
                    buffer[j++] = (byte)i;

            Color[] colors = buffer.ToColorArray();

            for (int i = 0, j = 0; i < colors.Length; i++)
                for (int k = 0; k < 3; k++)
                    Assert.AreEqual(buffer[j++], colors[i][k]);
        }

        [TestMethod]
        public void FailToColorArray()
        {
            var buffer = new byte[3];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => buffer.ToColorArray());

            buffer = new byte[1024];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => buffer.ToColorArray());
        }

        [TestMethod]
        public void ToByteArray()
        {
            var colors = new Color[ColorPalette.LENGTH];

            for (int i = 0; i < colors.Length; i++)
                colors[i] = new Color(i, i, i);

            byte[] buffer = colors.ToByteArray();

            for (int i = 0, j = 0; i < colors.Length; i++)
                for (int k = 0; k < 3; k++)
                    Assert.AreEqual(colors[i][k], buffer[j++]);
        }

        [TestMethod]
        public void FailToByteArray()
        {
            var colors = new Color[3];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => colors.ToByteArray());

            colors = new Color[1024];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => colors.ToByteArray());
        }
        #endregion
    }
}
