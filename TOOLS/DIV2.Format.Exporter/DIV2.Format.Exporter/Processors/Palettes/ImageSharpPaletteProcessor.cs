using DIV2.Format.Exporter.Converters;
using DIV2.Format.Exporter.ExtensionMethods;
using DIV2.Format.Exporter.Interfaces;
using DIV2.Format.Importer;

namespace DIV2.Format.Exporter.Processors.Palettes
{
    class ImageSharpPaletteProcessor : IPaletteProcessor
    {
        #region Properties
        public static ImageSharpPaletteProcessor Instance => new ImageSharpPaletteProcessor();
        #endregion

        #region Methods & Functions
        public bool Validate(byte[] buffer)
        {
            bool isPCX = PCX.Instance.Validate(buffer);
            bool isMAP = MAP.ValidateFormat(buffer);
            bool isFPG = FPG.ValidateFormat(buffer);

            return !(isPCX || isMAP || isFPG);
        }

        public PAL Process(byte[] buffer)
        {
            BMP256Converter.Convert(buffer, out byte[] palette, out _, out _, out _);
            return new PAL(palette.ToColorArray().ToDAC());
        }
        #endregion
    }
}
