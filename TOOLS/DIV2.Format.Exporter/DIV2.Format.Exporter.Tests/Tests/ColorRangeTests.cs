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

        ColorRange CreateCustomRange(out byte index)
        {
            index = 32;

            var range = new ColorRange(ref index);

            range.colors = ColorRange.RangeColors._32;
            range.type = ColorRange.RangeTypes.Edit4;
            range.isfixed = true;
            range.blackColor = 31;

            return range;
        }

        byte[] SerializeDefaultRange()
        {
            return this.CreateDefaultRange(out _).Serialize();
        }

        void AssertAreEqualDefaults(ColorRange range)
        {
            this.AssertAreEqual(range, this.CreateDefaultRange(out _));
        }

        void AssertAreEqual(ColorRange a, ColorRange b)
        {
            Assert.AreEqual(a.colors, b.colors);
            Assert.AreEqual(a.type, b.type);
            Assert.AreEqual(a.isfixed, b.isfixed);
            Assert.AreEqual(a.blackColor, b.blackColor);
            for (int i = 0; i < ColorRange.LENGTH; i++)
                Assert.AreEqual(a[i], b[i]);
        }
        #endregion

        #region Test methods
        [TestMethod]
        public void CreateNewDefaultRange()
        {
            var range = this.CreateDefaultRange(out byte index);
            Assert.AreEqual(index, 32);
            this.AssertAreEqualDefaults(range);
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
            this.AssertAreEqualDefaults(range);
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
            var a = this.CreateCustomRange(out _);
            var b = new ColorRange(a.Serialize());

            this.AssertAreEqual(a, b);
        }

        [TestMethod]
        public void AreEquals()
        {
            var a = this.CreateDefaultRange(out _);
            var b = this.CreateDefaultRange(out _);

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void AreNotEquals()
        {
            var a = this.CreateDefaultRange(out _);
            var b = this.CreateCustomRange(out _);

            Assert.AreNotEqual(a, b);
        }
        #endregion
    }
}
