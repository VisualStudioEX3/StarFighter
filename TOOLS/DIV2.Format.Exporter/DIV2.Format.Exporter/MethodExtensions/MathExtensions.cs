using System;
using System.Collections.Generic;
using System.Text;

namespace DIV2.Format.Exporter.MethodExtensions
{
    public static class MathExtensions
    {
        /// <summary>
        /// Is this value in range?
        /// </summary>
        /// <param name="value"><see cref="int"/> value instance.</param>
        /// <param name="min">Min value of the range.</param>
        /// <param name="max">Max value of the range.</param>
        /// <returns></returns>
        public static bool IsClamped(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }
    }
}
