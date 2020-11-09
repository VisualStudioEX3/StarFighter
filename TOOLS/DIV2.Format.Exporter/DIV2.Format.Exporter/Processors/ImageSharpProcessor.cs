using DIV2.Format.Importer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace DIV2.Format.Exporter.Processors
{
    class ImageSharpProcessor : IImageProcessor
    {
        #region Properties
        public static ImageSharpProcessor Instance { get; } = new ImageSharpProcessor();
        #endregion

        #region Methods & Functions
        public bool CheckFormat(byte[] buffer)
        {
            return !(PCX.IsPCX(buffer) && new MAP().Validate(buffer));
        }

        public Image Process(byte[] buffer, out IImageFormat mime)
        {
            return Image.Load(buffer, out mime);
        } 
        #endregion
    }
}
