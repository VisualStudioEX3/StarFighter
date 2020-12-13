using System;

namespace DIV2.Format.Exporter.Processors.Palettes
{
    class PaletteProcessor
    {
        #region Constants
        static readonly IPaletteProcessor[] PALETTE_PROCESSORS = {
            ImageSharpPaletteProcessor.Instance,
            PcxPaletteProcessor.Instance,
            DIVPaletteProcessor.Instance
        };
        #endregion

        #region Methods & Functions
        public static PAL ProcessPalette(byte[] buffer)
        {
            foreach (var processor in PALETTE_PROCESSORS)
                if (processor.CheckFormat(buffer))
                    return processor.Process(buffer);

            throw new FormatException("Invalid image format.");
        }
        #endregion
    }
}
