using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class ColorRangeTests
    {
        #region Helper functions
        ColorRange CreateDefaultRange(out byte index)
        {
            index = 0;
            return new ColorRange(ref index);
        }

        byte[] SerializeDefaultRange()
        {
            return this.CreateDefaultRange(out _).Serialize();
        }
        #endregion

        #region Test methods
        [TestMethod]
        public void CreateNewDefaultRange()
        {
            var range = this.CreateDefaultRange(out byte index);

            Assert.AreEqual(index, 32);

            Assert.AreEqual(range.colors, ColorRange.RangeColors._8);
            Assert.AreEqual(range.type, ColorRange.RangeTypes.Direct);
            Assert.AreEqual(range.isfixed, false);
            Assert.AreEqual(range.blackColor, 0);
            for (int i = 0; i < ColorRange.LENGTH; i++)
                Assert.AreEqual(range[i], i);
        }

        [TestMethod]
        public void Serialize()
        {
            byte[] data = this.SerializeDefaultRange();
            Assert.IsTrue(data.Length == ColorRange.SIZE);
        }

        [TestMethod]
        public void Write()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                this.CreateDefaultRange(out _).Write(stream);
            }
        }

        [TestMethod]
        public void CreateNewRangeFromMemory()
        {
            var range = new ColorRange(this.SerializeDefaultRange());

            Assert.AreEqual(range.colors, ColorRange.RangeColors._8);
            Assert.AreEqual(range.type, ColorRange.RangeTypes.Direct);
            Assert.AreEqual(range.isfixed, false);
            Assert.AreEqual(range.blackColor, 0);
            for (int i = 0; i < ColorRange.LENGTH; i++)
                Assert.AreEqual(range[i], i);
        } 

        [TestMethod]
        public void IterateForEach()
        {
            byte i = 0;
            foreach (var color in this.CreateDefaultRange(out _))
                Assert.AreEqual(color, i++);
        }

        [TestMethod]
        public void WriteRangeValues()
        {
            var range = this.CreateDefaultRange(out _);

            for (byte i = 0, j = 64; i < ColorRange.LENGTH; i++, j++)
                range[i] = j;

            for (byte i = 0, j = 64; i < ColorRange.LENGTH; i++, j++)
                Assert.AreEqual(range[i], j);
        }

        [TestMethod]
        public void TestCustomValuesFromSerialization()
        {
            byte startIndex = 32;

            var a = new ColorRange(ref startIndex);

            a.colors = ColorRange.RangeColors._32;
            a.type = ColorRange.RangeTypes.Edit4;
            a.isfixed = true;
            a.blackColor = 31;

            var b = new ColorRange(a.Serialize());

            Assert.AreEqual(a.colors, b.colors);
            Assert.AreEqual(a.type, b.type);
            Assert.AreEqual(a.isfixed, b.isfixed);
            Assert.AreEqual(a.blackColor, b.blackColor);
            for (int i = 0; i < ColorRange.LENGTH; i++)
                Assert.AreEqual(a[i], b[i]);
        }
        #endregion
    }
}
