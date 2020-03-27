using System.Text;

namespace DIV2.Format.Exporter.MethodExtensions
{
    /// <summary>
    /// Method extensions for ASCII/ASCIIZ strings.
    /// </summary>
    public static class ASCIIStringExtensions
    {
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
        /// If the input string is lower than the desired length, the string is filled with spaces.
        /// If the input string is higher than the length, getting a substring with the desired length.</returns>
        public static byte[] GetASCIIZString(this string text, int length)
        {
            char[] buffer = (text.Length > length ? text.Substring(0, length) : text.PadRight(length)).ToCharArray();
            buffer[length - 1] = '\0';
            return buffer.ToByteArray();
        }
    }
}
