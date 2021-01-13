using DIV2.Format.Exporter.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DIV2.Format.Exporter.Tests.ExtensionMethods
{
    [TestClass]
    public class ByteExtensionsTests : AbstractTest
    {
        #region Test Methods
        [TestMethod]
        public void ByteClearBits()
        {
            const byte VALUE = 255;

            const byte MASK_A = 0xFE; // Clear bit 0.
            const byte MASK_B = 0xEF; // Clear bit 4.
            const byte MASK_C = 0x3F; // Clear bits 7 and 6.

            const byte EXPECTED_A = 254;
            const byte EXPECTED_B = 239;
            const byte EXPECTED_C = 63;

            Assert.AreEqual(EXPECTED_A, VALUE.ClearBits(MASK_A));
            Assert.AreEqual(EXPECTED_B, VALUE.ClearBits(MASK_B));
            Assert.AreEqual(EXPECTED_C, VALUE.ClearBits(MASK_C));
        }

        [TestMethod]
        public void CalculateChecksum()
        {
            const string EXPECTED_CHECKSUM = "42-65-56-F8-F0-FD-8A-F5-AA-83-57-D3-DB-F1-65-55";
            byte[] buffer = { 0, 32, 0, 0, 254, 233, 12, 0, 0, 56, 90, 255, 0 };
            Assert.AreEqual(EXPECTED_CHECKSUM, buffer.CalculateChecksum());
        } 
        #endregion
    }
}
