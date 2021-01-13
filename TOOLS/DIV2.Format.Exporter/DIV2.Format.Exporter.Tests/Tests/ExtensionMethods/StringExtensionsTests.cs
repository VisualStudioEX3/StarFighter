using DIV2.Format.Exporter.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DIV2.Format.Exporter.Tests.ExtensionMethods
{
    [TestClass]
    public class StringExtensionsTests : AbstractTest
    {
        #region Constants
        const int TEST_CHAR_LENGTH = 16;
        const string TEST_STRING = "Hello, world!";
        static readonly byte[] TEST_ASCIIZ_STRING = { 72, 101, 108, 108, 111, 44, 32, 119, 111, 114, 108, 100, 33, 0, 0, 0 };
        #endregion

        #region Test Methods
        [TestMethod]
        public void ToASCIIString()
        {
            Assert.AreEqual(TEST_STRING, TEST_ASCIIZ_STRING.ToASCIIString());
        }
        
        [TestMethod]
        public void CharArrayToByteArray()
        {
            byte[] array = TEST_STRING.ToCharArray().ToByteArray();
            for (int i = 0; i < array.Length; i++)
                Assert.AreEqual(TEST_ASCIIZ_STRING[i], array[i]);
        }

        [TestMethod]
        public void StringToByteArray()
        {
            byte[] array = TEST_STRING.ToByteArray();
            for (int i = 0; i < array.Length; i++)
                Assert.AreEqual(TEST_ASCIIZ_STRING[i], array[i]);
        }

        [TestMethod]
        public void GetASCIIZString()
        {
            byte[] buffer = TEST_STRING.GetASCIIZString(TEST_CHAR_LENGTH);
            for (int i = 0; i < TEST_CHAR_LENGTH; i++)
                Assert.AreEqual(TEST_ASCIIZ_STRING[i], buffer[i]);
        }

        [TestMethod]
        public void GetSecureHashCode()
        {
            const int EXPECTED_HASH_CODE = 658283336;
            Assert.AreEqual(EXPECTED_HASH_CODE, TEST_STRING.GetSecureHashCode());
        }
        #endregion
    }
}
