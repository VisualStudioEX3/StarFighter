using System;
using System.Linq;

namespace DIV2.Format.Exporter.ExtensionMethods
{
    /// <summary>
    /// Extension methods for <see cref="Color"/> values.
    /// </summary>
    public static class ColorExtensions
    {
        #region Methods & Functions
        static ArgumentOutOfRangeException CreateException(int length)
        {
            return new ArgumentOutOfRangeException($"The array value must be a {length} length array.");
        }

        /// <summary>
        /// Creates a new <see cref="Color"/> array in DAC format [0..63].
        /// </summary>
        /// <param name="colors"><see cref="Color"/> array in RGB format [0.255].</param>
        /// <returns>Returns a new <see cref="Color"/> array in DAC format [0..63].</returns>
        public static Color[] ToDAC(this Color[] colors)
        {
            if (colors.Length != ColorPalette.LENGTH)
                throw CreateException(ColorPalette.LENGTH);

            return colors.Select(e => e.ToDAC()).ToArray();
        }

        /// <summary>
        /// Creates a new <see cref="Color"/> array in RGB format [0.255].
        /// </summary>
        /// <param name="colors"><see cref="Color"/> array in DAC format [0..63].</param>
        /// <returns>Returns a new <see cref="Color"/> array in RGB format [0.255].</returns>
        public static Color[] ToRGB(this Color[] colors)
        {
            if (colors.Length != ColorPalette.LENGTH)
                throw CreateException(ColorPalette.LENGTH);

            return colors.Select(e => e.ToRGB()).ToArray();
        }

        /// <summary>
        /// Converts a <see cref="byte"/> array to <see cref="Color"/> array.
        /// </summary>
        /// <param name="buffer">A 768 <see cref="byte"/> array length.</param>
        /// <returns>Returns a <see cref="Color"/> array.</returns>
        public static Color[] ToColorArray(this byte[] buffer)
        {
            if (buffer.Length != ColorPalette.SIZE)
                throw CreateException(ColorPalette.LENGTH);

            var colors = new Color[ColorPalette.LENGTH];

            int index = 0;
            for (int i = 0; i < colors.Length; i++)
                colors[i] = new Color(buffer[index++], buffer[index++], buffer[index++]);

            return colors;
        }

        /// <summary>
        /// Converts a <see cref="Color"/> array to <see cref="byte"/> array.
        /// </summary>
        /// <param name="colors">A 256 <see cref="Color"/> array length.</param>
        /// <returns>Returns a <see cref="byte"/> array.</returns>
        public static byte[] ToByteArray(this Color[] colors)
        {
            if (colors.Length != ColorPalette.LENGTH)
                throw CreateException(ColorPalette.LENGTH);

            var buffer = new byte[ColorPalette.SIZE];

            int i = 0;
            foreach (var color in colors)
            {
                buffer[i++] = color.red;
                buffer[i++] = color.green;
                buffer[i++] = color.blue;
            }

            return buffer;
        }
        #endregion
    }
}
