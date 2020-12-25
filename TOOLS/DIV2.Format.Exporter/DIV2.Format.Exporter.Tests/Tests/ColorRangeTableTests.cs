﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class ColorRangeTableTests :
        AbstractTest,
        IDefaultConstructorsTests,
        IEqualityTests,
        ISerializableAssetTests,
        IIterableReadTests,
        IIterableWriteTests,
        IEnumerableTests
    {
        #region Helper functions
        byte CheckDefaultRange(ColorRange range, byte startIndex)
        {
            Assert.AreEqual(ColorRange.DEFAULT_RANGE_COLORS, range.colors);
            Assert.AreEqual(ColorRange.DEFAULT_TYPE, range.type);
            Assert.AreEqual(ColorRange.DEFAULT_FIXED_STATE, range.isfixed);
            Assert.AreEqual(ColorRange.DEFAULT_BLACK_COLOR, range.blackColor);
            for (byte i = 0; i < ColorRange.LENGTH; i++, startIndex++)
                Assert.AreEqual(startIndex, range[i]);

            return startIndex;
        }

        void CheckDefaultRangeTable(ColorRangeTable table)
        {
            byte startIndex = 0;
            for (int i = 0; i < ColorRangeTable.LENGTH; i++)
                startIndex = this.CheckDefaultRange(table[i], startIndex);
        }
        #endregion

        #region Tests methods
        [TestMethod]
        public void CreateDefaultInstance()
        {
            var ranges = new ColorRangeTable();
            this.CheckDefaultRangeTable(ranges);
        }

        [TestMethod]
        public void CreateInstanceFromBuffer()
        {
            var ranges = new ColorRangeTable();
            var newRanges = new ColorRangeTable(ranges.Serialize());
            this.CheckDefaultRangeTable(newRanges);
        }

        [TestMethod]
        public void AreEqual()
        {
            var a = new ColorRangeTable();
            var b = new ColorRangeTable();
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void AreNotEqual()
        {
            var a = new ColorRangeTable();
            
            var b = new ColorRangeTable();
            byte startIndex = 64;
            for (int i = 0; i < ColorRangeTable.LENGTH; i++)
                b[i] = new ColorRange(ref startIndex);

            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void ReadByIndex()
        {
            var a = new ColorRangeTable();
            var b = new ColorRangeTable();
            for (int i = 0; i < ColorRangeTable.LENGTH; i++)
                Assert.AreEqual(a[i], b[i]);
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            try
            {
                _ = new ColorRangeTable()[-1];
                Assert.Fail();
            }
            catch
            {
                try
                {
                    _ = new ColorRangeTable()[ColorRangeTable.LENGTH + 1];
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
            var a = new ColorRangeTable();
            byte startIndex = 0;

            for (int i = 0; i < ColorRangeTable.LENGTH; i++)
                a[i] = new ColorRange(ref startIndex);

            var b = new ColorRangeTable();
            for (int i = 0; i < ColorRangeTable.LENGTH; i++)
                Assert.AreEqual(a[i], b[i]);
        }

        [TestMethod]
        public void FailWriteByIndex()
        {
            byte startIndex = 0;
            try
            {
                new ColorRangeTable()[-1] = new ColorRange(ref startIndex);
                Assert.Fail();
            }
            catch
            {
                try
                {
                    new ColorRangeTable()[ColorPalette.LENGTH + 1] = new ColorRange(ref startIndex);
                    Assert.Fail();
                }
                catch
                {
                }
            }
        }

        [TestMethod]
        public void ReadByForEach()
        {
            var a = new ColorRangeTable();
            var b = new ColorRangeTable();
            int i = 0;

            foreach (var range in b)
                Assert.AreEqual(a[i++], range);
        }

        [TestMethod]
        public void Serialize()
        {
            byte[] data = new ColorRangeTable().Serialize();
            Assert.AreEqual(ColorRangeTable.SIZE, data.Length);
        }

        [TestMethod]
        public void Write()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                new ColorRangeTable().Write(stream);
            }
        }
        #endregion
    }
}
