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
        const ushort TEST_X = 128;
        const ushort TEST_Y = 64;
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
            ushort x = point[0];
            ushort y = point[1];

            Assert.AreEqual(TEST_X, x);
            Assert.AreEqual(TEST_Y, y);
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            try
            {
                _ = new ControlPoint()[-1];
                Assert.Fail();
            }
            catch
            {
                try
                {
                    _ = new ControlPoint()[PAL.LENGTH + 1];
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
            try
            {
                point[-1] = 0;
                Assert.Fail();
            }
            catch
            {
                try
                {
                    point[ControlPoint.LENGTH + 1] = 0;
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
            byte[] buffer = new ControlPoint(TEST_X, TEST_Y).Serialize();
            ushort x = BitConverter.ToUInt16(buffer, 0);
            ushort y = BitConverter.ToUInt16(buffer, 2);
            
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
