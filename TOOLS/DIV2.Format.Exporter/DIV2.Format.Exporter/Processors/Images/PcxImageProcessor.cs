using DIV2.Format.Exporter.ExtensionMethods;
using DIV2.Format.Exporter.Interfaces;
using DIV2.Format.Importer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;

namespace DIV2.Format.Exporter.Processors.Images
{
    class PcxImageProcessor : IImageProcessor
    {
        #region Properties
        public static PcxImageProcessor Instance => new PcxImageProcessor();
        #endregion

        #region Methods & Functions
        public bool Validate(byte[] buffer)
        {
            return PCX.Instance.Validate(buffer);
        }

        public Image Process(byte[] buffer, out IImageFormat mime)
        {
            var pcx = new PCX(buffer);

            var image = new Image<Rgb24>(pcx.Width, pcx.Height);
            image.ComposeBitmap(pcx.Bitmap, pcx.Colors.ToColorArray().ToImageSharpColors());

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
