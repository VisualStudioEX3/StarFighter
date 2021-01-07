using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class ColorRangeTests :
        AbstractTest,
        IDefaultConstructorsTests,
        IEqualityTests,
        ISerializableAssetTests,
        IIterableReadTests,
        IIterableWriteTests,
        IEnumerableTests
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
            range.isFixed = true;
            range.blackColor = 31;

            return range;
        }

        byte[] SerializeDefaultRange()
        {
            return this.CreateDefaultRange(out _).Serialize();
        }

        void AssertAreEqualDefaults(ColorRange range)
        {
            this.AssertAreEqual(this.CreateDefaultRange(out _), range);
        }

        void AssertAreEqual(ColorRange a, ColorRange b)
        {
            Assert.AreEqual(a.colors, b.colors);
            Assert.AreEqual(a.type, b.type);
            Assert.AreEqual(a.isFixed, b.isFixed);
            Assert.AreEqual(a.blackColor, b.blackColor);
            for (int i = 0; i < ColorRange.LENGTH; i++)
                Assert.AreEqual(a[i], b[i]);
        }
        #endregion

        #region Test methods
        [TestMethod]
        public void CreateDefaultInstance()
        {
            var range = this.CreateDefaultRange(out byte index);
            Assert.AreEqual(ColorRange.LENGTH, index);
            this.AssertAreEqualDefaults(range);
        }

        [TestMethod]
        public void CreateInstanceFromBuffer()
        {
            var range = new ColorRange(this.SerializeDefaultRange());
            this.AssertAreEqualDefaults(range);
        }

        [TestMethod]
        public void AreEqual()
        {
            var a = this.CreateDefaultRange(out _);
            var b = this.CreateDefaultRange(out _);

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void AreNotEqual()
        {
            var a = this.CreateDefaultRange(out _);
            var b = this.CreateCustomRange(out _);

            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void ReadByIndex()
        {
            var range = this.CreateDefaultRange(out _);
            for (byte i = 0; i < ColorRange.LENGTH; i++)
                Assert.AreEqual(i, range[i]);
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() => _ = this.CreateDefaultRange(out _)[-1]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => _ = this.CreateDefaultRange(out _)[ColorRange.LENGTH + 1]);
        }

        [TestMethod]
        public void WriteByIndex()
        {
            var range = this.CreateDefaultRange(out _);

            for (byte i = 0, j = 64; i < ColorRange.LENGTH; i++, j++)
                range[i] = j;

            for (byte i = 0, j = 64; i < ColorRange.LENGTH; i++, j++)
                Assert.AreEqual(j, range[i]);
        }

        [TestMethod]
        public void FailWriteByIndex()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() => this.CreateDefaultRange(out _)[-1] = 0);
            Assert.ThrowsException<IndexOutOfRangeException>(() => this.CreateDefaultRange(out _)[ColorRange.LENGTH + 1] = 0);
        }

        [TestMethod]
        public void ReadByForEach()
        {
            int i = 0;
            var range = this.CreateDefaultRange(out _);
            foreach (var value in range)
                Assert.AreEqual(i++, value);
        }

        [TestMethod]
        public void Serialize()
        {
            byte[] data = this.SerializeDefaultRange();
            Assert.AreEqual(ColorRange.SIZE, data.Length);
        }

        [TestMethod]
        public void TestCustomValuesFromSerialization()
        {
            var a = this.CreateCustomRange(out _);
            var b = new ColorRange(a.Serialize());

            this.AssertAreEqual(a, b);
        }

        [TestMethod]
        public void Write()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                this.CreateDefaultRange(out _).Write(stream);
            }
        }
        #endregion
    }
}
