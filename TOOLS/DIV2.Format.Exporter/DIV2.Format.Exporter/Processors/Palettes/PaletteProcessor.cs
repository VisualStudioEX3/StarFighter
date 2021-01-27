using DIV2.Format.Exporter.Interfaces;
using System;

namespace DIV2.Format.Exporter.Processors.Palettes
{
    class PaletteProcessor
    {
        #region Constants
        static readonly IPaletteProcessor[] PALETTE_PROCESSORS = {
            ImageSharpPaletteProcessor.Instance,
            PcxPaletteProcessor.Instance,
            MapPaletteProcessor.Instance,
            FpgPaletteProcessor.Instance
        };
        #endregion

        #region Methods & Functions
        public static PAL ProcessPalette(byte[] buffer)
        {
            foreach (var processor in PALETTE_PROCESSORS)
                if (processor.Validate(buffer))
                    return processor.Process(buffer);

            throw new FormatException("Invalid image format.");
        }
        #endregion
    }
}
