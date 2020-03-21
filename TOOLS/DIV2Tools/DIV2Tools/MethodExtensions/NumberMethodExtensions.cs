using System;
using System.Collections.Generic;
using System.Text;

namespace DIV2Tools.MethodExtensions
{
    public static class NumberMethodExtensions
    {
        /// <summary>
        /// Checks if a value is clamped into a range.
        /// </summary>
        /// <param name="value">This <see cref="byte"/> value.</param>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <returns>Returns <see cref="true"/> if the value is clamped between min and max values.</returns>
        public static bool IsClamped(this byte value, byte min, byte max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is clamped into a range.
        /// </summary>
        /// <param name="value">This <see cref="short"/> value.</param>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <returns>Returns <see cref="true"/> if the value is clamped between min and max values.</returns>
        public static bool IsClamped(this short value, short min, short max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is clamped into a range.
        /// </summary>
        /// <param name="value">This <see cref="ushort"/> value.</param>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <returns>Returns <see cref="true"/> if the value is clamped between min and max values.</returns>
        public static bool IsClamped(this ushort value, ushort min, ushort max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is clamped into a range.
        /// </summary>
        /// <param name="value">This <see cref="int"/> value.</param>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <returns>Returns <see cref="true"/> if the value is clamped between min and max values.</returns>
        public static bool IsClamped(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is clamped into a range.
        /// </summary>
        /// <param name="value">This <see cref="uint"/> value.</param>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <returns>Returns <see cref="true"/> if the value is clamped between min and max values.</returns>
        public static bool IsClamped(this uint value, uint min, uint max)
        {
            return value >= min && value <= max;
        }
    }
}
