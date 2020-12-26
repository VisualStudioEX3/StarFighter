using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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
            try
            {
                new Color(new byte[1]);
                Assert.Fail();
            }
            catch
            {
                try
                {
                    new Color(new byte[5]);
                    Assert.Fail();
                }
                catch
                {
                }
            }
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
            try
            {
                _ = RGB_COLOR[-1];
                Assert.Fail();
            }
            catch
            {
                try
                {
                    _ = RGB_COLOR[Color.LENGTH + 1];
                    Assert.Fail();
                }
                catch
                {
                }
            }
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
            try
            {
                var color = new Color();
                color[-1] = 0;
                Assert.Fail();
            }
            catch
            {
                try
                {
                    var color = new Color();
                    color[Color.LENGTH + 1] = 0;
                    Assert.Fail();
                }
                catch
                {
                }
            }
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
        #endregion
    }
}
