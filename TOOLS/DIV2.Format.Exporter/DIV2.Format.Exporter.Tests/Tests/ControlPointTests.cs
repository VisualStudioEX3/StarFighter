using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class ControlPointTests :
        AbstractTest,
        IDefaultConstructorsTests,
        IEqualityTests,
        ISerializableAssetTests,
        IIterableReadTests,
        IIterableWriteTests
    {
        #region Constants
        const short TEST_X = 128;
        const short TEST_Y = 64;
        #endregion

        #region Test methods
        [TestMethod]
        public void CreateDefaultInstance()
        {
            new ControlPoint();
        }

        [TestMethod]
        public void CreateInstanceFromBuffer()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                stream.Write(TEST_X);
                stream.Write(TEST_Y);
                stream.BaseStream.Position = 0;

                var point = new ControlPoint((stream.BaseStream as MemoryStream).ToArray());
                Assert.AreEqual(TEST_X, point.x);
                Assert.AreEqual(TEST_Y, point.y);

                point = new ControlPoint(new BinaryReader(stream.BaseStream));
                Assert.AreEqual(TEST_X, point.x);
                Assert.AreEqual(TEST_Y, point.y);
            }
        }

        [TestMethod]
        public void AreEqual()
        {
            var a = new ControlPoint(TEST_X, TEST_Y);
            var b = new ControlPoint(TEST_X, TEST_Y);
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void AreNotEqual()
        {
            var a = new ControlPoint(TEST_X, TEST_Y);
            var b = new ControlPoint(32, 1024);
            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void ReadByIndex()
        {
            var point = new ControlPoint(TEST_X, TEST_Y);
            short x = point[0];
            short y = point[1];

            Assert.AreEqual(TEST_X, x);
            Assert.AreEqual(TEST_Y, y);
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() => _ = new ControlPoint()[-1]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => _ = new ControlPoint()[PAL.LENGTH + 1]);
        }

        [TestMethod]
        public void WriteByIndex()
        {
            var point = new ControlPoint();
            
            point[0] = TEST_X;
            point[1] = TEST_Y;

            Assert.AreEqual(TEST_X, point[0]);
            Assert.AreEqual(TEST_Y, point[1]);

            Assert.AreEqual(point.x, point[0]);
            Assert.AreEqual(point.y, point[1]);
        }

        [TestMethod]
        public void FailWriteByIndex()
        {
            var point = new ControlPoint();
            Assert.ThrowsException<IndexOutOfRangeException>(() => point[-1] = 0);
            Assert.ThrowsException<IndexOutOfRangeException>(() => point[ControlPoint.LENGTH + 1] = 0);
        }

        [TestMethod]
        public void Serialize()
        {
            byte[] buffer = new ControlPoint(TEST_X, TEST_Y).Serialize();
            short x = BitConverter.ToInt16(buffer, 0);
            short y = BitConverter.ToInt16(buffer, 2);
            
            Assert.AreEqual(TEST_X, x);
            Assert.AreEqual(TEST_Y, y);
        }

        [TestMethod]
        public void Write()
        {
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                new ControlPoint().Write(stream);
                int streamSize = (stream.BaseStream as MemoryStream).ToArray().Length;
                Assert.AreEqual(ControlPoint.SIZE, streamSize);
            }
        }
        #endregion
    }
}
