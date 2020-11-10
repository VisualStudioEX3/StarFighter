using DIV2.Format.Importer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace DIV2.Format.Exporter.Processors.Images
{
    class ImageSharpImageProcessor : IImageProcessor
    {
        #region Properties
        public static ImageSharpImageProcessor Instance { get; } = new ImageSharpImageProcessor();
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
