using DIV2.Format.Exporter.ExtensionMethods;
using DIV2.Format.Exporter.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;

namespace DIV2.Format.Exporter.Processors.Images
{
    class MapImageProcessor : IImageProcessor
    {
        #region Properties
        public static MapImageProcessor Instance => new MapImageProcessor();
        #endregion

        #region Methods & Functions
        public bool Validate(byte[] buffer)
        {
            return MAP.ValidateFormat(buffer);
        }

        public Image Process(byte[] buffer, out IImageFormat mime)
        {
            var map = new MAP(buffer);

            var image = new Image<Rgb24>(map.Width, map.Height);
            image.ComposeBitmap(map.GetBitmapArray(), map.Palette.ToImageSharpColors());

            mime = new MapFormat();

            return image;
        }
        #endregion
    }

    internal sealed class MapFormat : IImageFormat
    {
        #region Properties
        /// <summary>
        /// Gets the current instance.
        /// </summary>
        public static MapFormat Instance { get; } = new MapFormat();
        public string Name => "MAP";
        public string DefaultMimeType => "image/div-map";
        public IEnumerable<string> MimeTypes => new[] { "image/map", "image/div-map" };
        public IEnumerable<string> FileExtensions => new[] { "map" };
        #endregion
    }
}
