using DIV2.Format.Exporter.MethodExtensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;

namespace DIV2.Format.Exporter.Processors.Images
{
    class MapImageProcessor : IImageProcessor
    {
        #region Constants
        const int MAP_HEADER_WIDTH_OFFSET = MAP.BASE_HEADER_LENGTH;
        const int MAP_HEADER_HEIGHT_OFFSET = MAP.BASE_HEADER_LENGTH + sizeof(short);
        const int MAP_HEADER_PALETTE_OFFSET = MAP.HEADER_LENGTH;
        #endregion

        #region Properties
        public static MapImageProcessor Instance { get; } = new MapImageProcessor(); 
        #endregion

        #region Methods & Functions
        public bool CheckFormat(byte[] buffer)
        {
            return new MAP().Validate(buffer);
        }

        public Image Process(byte[] buffer, out IImageFormat mime)
        {
            this.ImportMAP(buffer, out short width, out short height, out byte[] pixels, out PAL palette);

            var image = new Image<Rgb24>(width, height);
            image.ComposeBitmap(pixels, palette.ToColors());

            mime = new MapFormat();

            return image;
        }

        void ImportMAP(byte[] buffer, out short width, out short height, out byte[] pixels, out PAL palette)
        {
            using (var stream = new MemoryStream(buffer))
            {
                width = this.ReadInt16(stream, MapImageProcessor.MAP_HEADER_WIDTH_OFFSET);
                height = this.ReadInt16(stream, MapImageProcessor.MAP_HEADER_HEIGHT_OFFSET);
                palette = PAL.CreatePalette(this.ReadBytes(stream, MapImageProcessor.MAP_HEADER_PALETTE_OFFSET, PAL.COLOR_TABLE_LENGTH), false);

                int length = width * height;
                pixels = this.ReadBytes(stream, (int)(stream.Length - length), length);
            }
        }

        byte[] ReadBytes(Stream buffer, int offset, int length)
        {
            var data = new byte[length];
            if (buffer.Read(data, offset, length) == length)
            {
                return data;
            }
            else
            {
                throw new OutOfMemoryException("Error reading the requested bytes.");
            }
        }

        short ReadInt16(Stream buffer, int offset)
        {
            return BitConverter.ToInt16(this.ReadBytes(buffer, offset, sizeof(short)), 0);
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
