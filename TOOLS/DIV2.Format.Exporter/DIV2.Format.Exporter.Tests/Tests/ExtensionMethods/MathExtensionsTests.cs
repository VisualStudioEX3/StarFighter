using DIV2.Format.Exporter.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DIV2.Format.Exporter.Tests.ExtensionMethods
{
    [TestClass]
    public class MathExtensionsTests : AbstractTest
    {
        #region Test Methods
        [TestMethod]
        public void ByteIsClamped()
        {
            const byte min = 8;
            const byte max = 64;

            for (byte b = min; b <= max; b++)
                Assert.IsTrue(b.IsClamped(min, max));

            byte value = 4;
            Assert.IsFalse(value.IsClamped(min, max));

            value = 128;
            Assert.IsFalse(value.IsClamped(min, max));
        }

        [TestMethod]
        public void ShortIsClamped()
        {
            const short min = -1024;
            const short max = 320;

            for (short b = min; b <= max; b++)
                Assert.IsTrue(b.IsClamped(min, max));

            short value = -16000;
            Assert.IsFalse(value.IsClamped(min, max));

            value = 4096;
            Assert.IsFalse(value.IsClamped(min, max));
        }

        [TestMethod]
        public void UintIsClamped()
        {
            const uint min = 128;
            const uint max = 480;

            for (uint b = min; b <= max; b++)
                Assert.IsTrue(b.IsClamped(min, max));

            uint value = 4;
            Assert.IsFalse(value.IsClamped(min, max));

            value = 68000;
            Assert.IsFalse(value.IsClamped(min, max));
        }

        [TestMethod]
        public void IntIsClamped()
        {
            const int min = -100;
            const int max = 100;

            for (int b = min; b <= max; b++)
                Assert.IsTrue(b.IsClamped(min, max));

            int value = -1234567;
            Assert.IsFalse(value.IsClamped(min, max));

            value = 32000;
            Assert.IsFalse(value.IsClamped(min, max));
        }
        #endregion
    }
}
