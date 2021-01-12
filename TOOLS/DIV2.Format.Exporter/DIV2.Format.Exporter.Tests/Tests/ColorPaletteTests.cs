using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class ColorPaletteTests :
        AbstractTest,
        IDefaultConstructorsTests,
        IEqualityTests,
        ISerializableAssetTests,
        IIterableReadTests,
        IIterableWriteTests,
        IEnumerableTests
    {
        #region Constants
        readonly static Color BLACK_COLOR = new Color();
        #endregion

        #region Helper functions
        ColorPalette CreateTestsPalette()
        {
            var palette = new ColorPalette();
            for (int i = 0; i < ColorPalette.LENGTH; i++)
                palette[i] = new Color(i, i, i).ToDAC();

            return palette;
        }
        #endregion

        #region Tests methods
        [TestMethod]
        public void CreateDefaultInstance()
        {
            var palette = new ColorPalette();
            for (int i = 0; i < ColorPalette.LENGTH; i++)
                Assert.AreEqual(BLACK_COLOR, palette[i]);
        }

        [TestMethod]
        public void CreateInstanceFromBuffer()
        {
            var buffer = new byte[ColorPalette.SIZE];

            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)this.Random.Next(0, Color.MAX_DAC_VALUE + 1);

            var palette = new ColorPalette(buffer);

            for (int i = 0, j = 0; i < ColorPalette.LENGTH; i++)
            {
                Color color = palette[i];
                Assert.AreEqual(buffer[j++], color.red);
                Assert.AreEqual(buffer[j++], color.green);
                Assert.AreEqual(buffer[j++], color.blue);
            }
        }

        [TestMethod]
        public void FailCreateInstanceFromBuffer()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                var buffer = new byte[ColorPalette.SIZE];

                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] = byte.MaxValue;

                new ColorPalette(buffer);
            });
        }

        [TestMethod]
        public void FailCreateInstanceFromColorArray()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                var colors = new Color[ColorPalette.LENGTH];
                for (int i = 0; i < ColorPalette.LENGTH; i++)
                    colors[i] = new Color(i, i, i);

                new ColorPalette(colors);
            });
        }

        [TestMethod]
        public void AreEqual()
        {
            var a = new ColorPalette();
            var b = new ColorPalette();

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void AreNotEqual()
        {
            var a = new ColorPalette();
            ColorPalette b = this.CreateTestsPalette();

            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void ReadByIndex()
        {
            ColorPalette palette = this.CreateTestsPalette();
            Color[] colors = palette.ToArray();
            for (int i = 0; i < ColorPalette.LENGTH; i++)
                Assert.AreEqual(colors[i], palette[i]);
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() => _ = new ColorPalette()[-1]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => _ = new ColorPalette()[ColorPalette.LENGTH + 1]);
        }

        [TestMethod]
        public void WriteByIndex()
        {
            ColorPalette a = this.CreateTestsPalette();
            var b = new ColorPalette();

            for (int i = 0; i < ColorPalette.LENGTH; i++)
                b[i] = a[i];

            for (int i = 0; i < ColorPalette.LENGTH; i++)
                Assert.AreEqual(a[i], b[i]);
        }

        [TestMethod]
        public void TryToSetFullRGBColor()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ColorPalette()[0] = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue));
        }

        [TestMethod]
        public void FailWriteByIndex()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() => new ColorPalette()[-1] = new Color());
            Assert.ThrowsException<IndexOutOfRangeException>(() => new ColorPalette()[ColorPalette.LENGTH + 1] = new Color());
        }

        [TestMethod]
        public void ReadByForEach()
        {
            int i = 0;
            ColorPalette palette = this.CreateTestsPalette();
            foreach (var value in palette)
                Assert.AreEqual(palette[i++], value);
        }

        [TestMethod]
        public void Serialize()
        {
            Assert.AreEqual(ColorPalette.SIZE, new ColorPalette().Serialize().Length);
        }

        [TestMethod]
        public void Write()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                new ColorPalette().Write(stream);
            }
        }

        [TestMethod]
        public void ToArray()
        {
            ColorPalette palette = this.CreateTestsPalette();
            Color[] colors = palette.ToArray();

            for (int i = 0; i < ColorPalette.LENGTH; i++)
                Assert.AreEqual(palette[i], colors[i]);
        }

        [TestMethod]
        public void ToRGB()
        {
            ColorPalette palette = this.CreateTestsPalette();
            Color[] colors = palette.ToRGB();

            for (int i = 0; i < ColorPalette.LENGTH; i++)
                Assert.AreEqual(palette[i].ToRGB(), colors[i]);
        }

        [TestMethod]
        public void Sort()
        {
            ColorPalette palette = this.CreateTestsPalette();
            palette.Sort();
        }
        #endregion
    }
}
