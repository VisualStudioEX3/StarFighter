using System;
using System.Security.Cryptography;
using System.Text;

namespace DIV2.Format.Exporter.Cryptographic
{
    static class HashGenerator
    {
        #region Properties
        static MD5 MD5HashAlgorithm => MD5.Create();
        static SHA256CryptoServiceProvider SHA256HashAlgorithm => new SHA256CryptoServiceProvider();
        #endregion

        #region Methods & Functions
        public static string CalculateChecksum(byte[] buffer)
        {
            byte[] hash = MD5HashAlgorithm.ComputeHash(buffer);
            return BitConverter.ToString(hash);
        }

        // Source: https://www.codeproject.com/Articles/34309/Convert-String-to-64bit-Integer
        public static int CalculateHashCode(string text)
        {
            int hashCode = 0;

            if (!string.IsNullOrEmpty(text))
            {
                byte[] byteContents = Encoding.Unicode.GetBytes(text);
                byte[] hashText = SHA256HashAlgorithm.ComputeHash(byteContents);

                int hashCodeStart =     BitConverter.ToInt32(hashText, 0);
                int hashCodeMedium =    BitConverter.ToInt32(hashText, 8);
                int hashCodeEnd =       BitConverter.ToInt32(hashText, 24);

                hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
            }

            return hashCode;
        } 
        #endregion
    }
}
