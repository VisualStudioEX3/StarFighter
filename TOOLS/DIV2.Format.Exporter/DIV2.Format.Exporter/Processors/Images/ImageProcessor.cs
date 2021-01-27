using DIV2.Format.Exporter.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System;

namespace DIV2.Format.Exporter.Processors.Images
{
    class ImageProcessor
    {
        #region Constants
        static readonly IImageProcessor[] IMAGE_PROCESSORS = {
            ImageSharpImageProcessor.Instance,
            PcxImageProcessor.Instance,
            MapImageProcessor.Instance
        };
        #endregion

        #region Methods & Functions
        public static Image ProcessImage(byte[] buffer, out IImageFormat mime)
        {
            foreach (var processor in IMAGE_PROCESSORS)
                if (processor.Validate(buffer))
                    return processor.Process(buffer, out mime);

            throw new FormatException("Invalid image format.");
        }
        #endregion
    }
}
