using DIV2.Format.Exporter.Cryptographic;
using System.Text;

namespace DIV2.Format.Exporter.ExtensionMethods
{
    /// <summary>
    /// Extension methods for ASCIIZ (ASCII null terminated string or C string) <see cref="string"/> instances.
    /// </summary>
    public static class StringExtensions
    {
        #region Methods & Functions
        /// <summary>
        /// Get <see cref="string"/> representation, without null chars and other possible garbage data.
        /// </summary>
        /// <param name="buffer"><see cref="byte"/> array with the ASCIIZ string data (ASCII null terminated string or C string).</param>
        /// <returns></returns>
        public static string ToASCIIString(this byte[] buffer)
        {
            int i;

            for (i = 0; i < buffer.Length; i++)
                if (buffer[i] == 0)
                    break;

            return Encoding.ASCII.GetString(buffer, 0, i);
        }

        /// <summary>
        /// Returns a <see cref="byte"/> array representation of this string encoding as ASCII.
        /// </summary>
        /// <param name="text">This <see cref="char"/> array instance.</param>
        /// <returns>Returns a <see cref="byte"/> array representation of this <see cref="string"/> encoding as ASCII.</returns>
        public static byte[] ToByteArray(this char[] text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        /// <summary>
        /// Returns a <see cref="byte"/> array representation of this string encoding as ASCII.
        /// </summary>
        /// <param name="text">This <see cref="string"/> instance.</param>
        /// <returns>Returns a <see cref="byte"/> array representation of this <see cref="string"/> encoding as ASCII.</returns>
        public static byte[] ToByteArray(this string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        /// <summary>
        /// Get a binary representation of this <see cref="string"/> in ASCIIZ format (ASCII null terminated string or C string).
        /// </summary>
        /// <param name="text">This <see cref="string"/> instance.</param>
        /// <param name="length">Characters length.</param>
        /// <returns>Returns a <see cref="byte"/> array with the content of the <see cref="string"/> in ASCIIZ format.
        /// If the input string is shorter than the desired length, the string is filled with null chars.
        /// If the input string is longer than the length, getting a substring with the desired length.</returns>
        public static byte[] GetASCIIZString(this string text, int length)
        {
            char[] buffer = text.Length > length ? 
                            text.Substring(0, length).ToCharArray() : 
                            text.PadRight(length, '\0').ToCharArray();

            return buffer.ToByteArray();
        }

        /// <summary>
        /// Returns a secure hash code for this string.
        /// </summary>
        /// <param name="text"><see cref="string"/> instance.</param>
        /// <returns>Returns a <see cref="int"/> hash value.</returns>
        /// <remarks>Use this function instead of default <see cref="string.GetHashCode"/> to unsure that gets a unmutable hash code.</remarks>
        public static int GetSecureHashCode(this string text)
        {
            return HashGenerator.CalculateHashCode(text);
        } 
        #endregion
    }
}
