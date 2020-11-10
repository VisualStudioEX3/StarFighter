using DIV2.Format.Exporter.MethodExtensions;
using DIV2.Format.Importer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

namespace DIV2.Format.Exporter.Processors.Images
{
    class PcxImageProcessor : IImageProcessor
    {
        #region Properties
        public static PcxImageProcessor Instance { get; } = new PcxImageProcessor();
        #endregion

        #region Methods & Functions
        public bool CheckFormat(byte[] buffer)
        {
            return PCX.IsPCX256(buffer);
        }

        public Image Process(byte[] buffer, out IImageFormat mime)
        {
            PCX.Import(buffer, out short width, out short height, out byte[] pixels, out PAL palette);

            var image = new Image<Rgb24>(width, height);
            image.ComposeBitmap(pixels, palette.ToColors());

            mime = new PcxFormat();

            return image;
        } 
        #endregion
    }

    internal sealed class PcxFormat : IImageFormat
    {
        #region Properties
        /// <summary>
        /// Gets the current instance.
        /// </summary>
        public static PcxFormat Instance { get; } = new PcxFormat();

        public string Name => "PCX";
        public string DefaultMimeType => "image/x-pcx";
        public IEnumerable<string> MimeTypes => new[] { "image/pcx", "image/x-pcx" };
        public IEnumerable<string> FileExtensions => new[] { "pcx" };
        #endregion
    }
}
