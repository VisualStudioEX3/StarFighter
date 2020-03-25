using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DIV2Tools.MethodExtensions
{
    public static class ByteMethodExtensions
    {
        #region Methods & Functions
        /// <summary>
        /// Read all <see cref="char"/>s, using ASCII encoding, from a <see cref="byte"/> array until get <see cref="null"/> character termination.
        /// </summary>
        /// <param name="buffer">This <see cref="byte"/> array instance.</param>
        /// <returns>Returns the <see cref="string"/> with all chars readed using ASCII encoding.</returns>
        public static string GetNullTerminatedASCIIString(this byte[] buffer)
        {
            int len = 0;
            do { len++; } while (len < buffer.Length && buffer[len] != 0);
            return Encoding.ASCII.GetString(buffer, 0, len);
        }

        /// <summary>
        /// Print in a string all values from the <see cref="byte"/> array.
        /// </summary>
        /// <param name="array">This <see cref="byte"/> array instance.</param>
        /// <returns>Returns a <see cref="string"/> with format "{ 0, 1, 2, ... }".</returns>
        public static string Print(this byte[] array)
        {
            var sb = new StringBuilder();
            sb.Append("{ ");
            foreach (var item in array)
            {
                sb.Append($"{item} ");
            }
            sb.Append('}');

            return sb.ToString();
        }

        /// <summary>
        /// Performs a <see cref="byte"/> to <see cref="byte"/> comparison of this array with another.
        /// </summary>
        /// <param name="array">This <see cref="byte"/> array instance.</param>
        /// <param name="other">Other <see cref="byte"/> array to compare.</param>
        /// <returns>Returns <see cref="true"/> if two arrays has the same values and length.</returns>
        public static bool Compare(this byte[] array, byte[] other)
        {
            if (array.Length != other.Length) return false;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != other[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Converts this <see cref="byte"/> array to <see cref="uint"/> value.
        /// </summary>
        /// <param name="array">This <see cref="byte"/> array instance.</param>
        /// <returns>Returns a <see cref="uint"/> value.</returns>
        public static uint ToUInt32(this byte[] array)
        {
            return BitConverter.ToUInt32(array);
        }

        /// <summary>
        /// Converts this <see cref="byte"/> array to Hexadecimal <see cref="string"/> value.
        /// </summary>
        /// <param name="array">This <see cref="byte"/> array instance.</param>
        /// <returns>Returns a <see cref="string"/> value.</returns>
        public static string ToHexadecimalString(this byte[] array)
        {
            return BitConverter.ToString(array);
        } 

        /// <summary>
        /// Converts this <see cref="byte"/> in a binary representation.
        /// </summary>
        /// <param name="value">This <see cref="byte"/> value instance.</param>
        /// <returns>Returns a <see cref="string"/> with format 0000000#.</returns>
        public static string ToBinaryString(this byte value)
        {
            return Convert.ToString(value, 2).PadLeft(8, '0');
        }
        #endregion
    }
}
