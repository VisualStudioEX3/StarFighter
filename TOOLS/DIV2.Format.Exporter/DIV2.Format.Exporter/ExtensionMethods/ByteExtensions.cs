using DIV2.Format.Exporter.Cryptographic;

namespace DIV2.Format.Exporter.ExtensionMethods
{
    /// <summary>
    /// Extension methods for <see cref="byte"/> arrays.
    /// </summary>
    public static class ByteExtensions
    {
        #region Methods & Functions
        /// <summary>
        /// Clear bits of this <see cref="byte"/> value.
        /// </summary>
        /// <param name="value"><see cref="byte"/> value to clear bits.</param>
        /// <param name="mask"><see cref="byte"/> mask to apply.</param>
        /// <returns>Returns the <see cref="byte"/> value with the cleared bits.</returns>
        public static byte ClearBits(this byte value, byte mask)
        {
            int i = value; // .NET bit operations works in Int32 values. A conversion is needed to work with bytes.
            return (byte)(i & mask);
        }

        /// <summary>
        /// Calculates the checksum of this <see cref="string"/>.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array.</param>
        /// <returns>Returns the checksum string of this <see cref="byte"/> array.</returns>
        public static string CalculateChecksum(this byte[] buffer)
        {
            return HashGenerator.CalculateChecksum(buffer);
        } 
        #endregion
    }
}
