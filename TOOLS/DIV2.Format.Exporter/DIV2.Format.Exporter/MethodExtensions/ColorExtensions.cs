using System;

namespace DIV2.Format.Exporter.MethodExtensions
{
    public static class ColorExtensions
    {
        static ArgumentOutOfRangeException CreateException(int length)
        {
            return new ArgumentOutOfRangeException($"The array value must be a {length} length array.");
        }

        public static Color[] ToDAC(this Color[] colors)
        {
            if (colors.Length != ColorPalette.LENGTH)
                throw CreateException(ColorPalette.LENGTH);

            var dac = new Color[ColorPalette.LENGTH];

            for (int i = 0; i < colors.Length; i++)
                dac[i] = colors[i].ToDAC();

            return dac;
        }

        public static Color[] ToRGB(this Color[] colors)
        {
            if (colors.Length != ColorPalette.LENGTH)
                throw CreateException(ColorPalette.LENGTH);

            var rgb = new Color[ColorPalette.LENGTH];

            for (int i = 0; i < colors.Length; i++)
                rgb[i] = colors[i].ToRGB();

            return rgb;
        }

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
    }
}
