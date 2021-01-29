using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Numerics;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class ColorTests :
        AbstractTest,
        IDefaultConstructorsTests,
        IEqualityTests,
        ISerializableAssetTests,
        IIterableReadTests,
        IIterableWriteTests
    {
        #region Constants
        const byte RED = 64;
        const byte GREEN = 128;
        const byte BLUE = 255;

        readonly byte[] COLOR_VALUES = { RED, GREEN, BLUE };

        readonly Color RGB_COLOR = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
        readonly Color DAC_COLOR = new Color(Color.MAX_DAC_VALUE, Color.MAX_DAC_VALUE, Color.MAX_DAC_VALUE);
        #endregion

        #region Test methods
        [TestMethod]
        public void CreateDefaultInstance()
        {
            var color = new Color(COLOR_VALUES[0], COLOR_VALUES[1], COLOR_VALUES[2]);
            Assert.AreEqual(RED, color.red);
            Assert.AreEqual(GREEN, color.green);
            Assert.AreEqual(BLUE, color.blue);
        }

        [TestMethod]
        public void CreateInstanceFromBuffer()
        {
            var color = new Color(COLOR_VALUES);
            Assert.AreEqual(COLOR_VALUES[0], color.red);
            Assert.AreEqual(COLOR_VALUES[1], color.green);
            Assert.AreEqual(COLOR_VALUES[2], color.blue);

            color = new Color(new BinaryReader(new MemoryStream(COLOR_VALUES)));
            Assert.AreEqual(COLOR_VALUES[0], color.red);
            Assert.AreEqual(COLOR_VALUES[1], color.green);
            Assert.AreEqual(COLOR_VALUES[2], color.blue);
        }

        [TestMethod]
        public void FailCreateInstanceFromBuffer()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Color(new byte[1]));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Color(new byte[5]));
        }

        [TestMethod]
        public void AreEqual()
        {
            Assert.AreEqual(RGB_COLOR, RGB_COLOR);
        }

        [TestMethod]
        public void AreNotEqual()
        {
            Assert.AreNotEqual(RGB_COLOR, DAC_COLOR);
        }

        [TestMethod]
        public void ReadByIndex()
        {
            var color = new Color(RED, GREEN, BLUE);
            for (int i = 0; i < Color.LENGTH; i++)
                Assert.AreEqual(COLOR_VALUES[i], color[i]);
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() => _ = RGB_COLOR[-1]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => _ = RGB_COLOR[Color.LENGTH + 1]);
        }

        [TestMethod]
        public void WriteByIndex()
        {
            var color = new Color();
            for (int i = 0; i < COLOR_VALUES.Length; i++)
            {
                color[i] = COLOR_VALUES[i];
                Assert.AreEqual(COLOR_VALUES[i], color[i]);
            }
        }

        [TestMethod]
        public void FailWriteByIndex()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var color = new Color();
                color[-1] = 0;
            });
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var color = new Color();
                color[Color.LENGTH + 1] = 0;
            });
        }

        [TestMethod]
        public void Serialize()
        {
            Assert.AreEqual(Color.SIZE, RGB_COLOR.Serialize().Length);
        }

        [TestMethod]
        public void Write()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                RGB_COLOR.Write(stream);
            }
        }

        [TestMethod]
        public void RGB2DAC()
        {
            Assert.AreEqual(DAC_COLOR, RGB_COLOR.ToDAC());
        }

        [TestMethod]
        public void DAC2RGB()
        {
            Assert.AreEqual(RGB_COLOR, DAC_COLOR.ToRGB());
        }

        [TestMethod]
        public void IsDAC()
        {
            Assert.IsTrue(DAC_COLOR.IsDAC());
        }

        [TestMethod]
        public void FailIsDAC()
        {
            Assert.IsFalse(RGB_COLOR.IsDAC());
        }

        [TestMethod]
        public void MinorThan()
        {
            var a = new Color(0, 128, 255); // 33023
            var b = new Color(32, 255, 64); // 2162496
            var c = a;

            Assert.IsTrue(a < b);
            Assert.IsFalse(b < a);

            Assert.IsTrue(a <= c);
            Assert.IsFalse(b <= a);
        }

        [TestMethod]
        public void MajorThan()
        {
            var a = new Color(32, 255, 64); // 2162496
            var b = new Color(0, 128, 255); // 33023
            var c = b;

            Assert.IsTrue(a > b);
            Assert.IsFalse(b > a);

            Assert.IsTrue(a >= c);
            Assert.IsFalse(b >= a);
        }

        [TestMethod]
        public void Normalize()
        {
            var factor = (float)ColorFormat.DAC;
            var color = new Color(8, 32, 63);
            var expected = new Vector3(color.red / factor, color.green / factor, color.blue / factor);

            Assert.AreEqual(expected, color.Normalize(ColorFormat.DAC));

            factor = (float)ColorFormat.RGB;
            color = new Color(16, 128, 255);
            expected = new Vector3(color.red / factor, color.green / factor, color.blue / factor);

            Assert.AreEqual(expected, color.Normalize(ColorFormat.RGB));
        }
        #endregion
    }
}
