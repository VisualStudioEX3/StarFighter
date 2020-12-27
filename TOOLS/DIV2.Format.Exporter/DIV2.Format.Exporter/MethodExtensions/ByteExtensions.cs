using System;
using System.Security.Cryptography;

namespace DIV2.Format.Exporter.MethodExtensions
{
    public static class ByteExtensions
    {
        public static byte ClearBits(this byte value, byte mask)
        {
            int i = value; // .NET bit operations works in Int32 values. A conversion is needed to work with bytes.
            return (byte)(i & mask);
        }

        public static string CalculateMD5Checksum(this byte[] buffer)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(buffer));
            }
        }
    }
}
