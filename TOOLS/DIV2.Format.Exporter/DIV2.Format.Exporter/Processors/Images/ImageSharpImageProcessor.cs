using DIV2.Format.Exporter.Interfaces;
using DIV2.Format.Importer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace DIV2.Format.Exporter.Processors.Images
{
    class ImageSharpImageProcessor : IImageProcessor
    {
        #region Properties
        public static ImageSharpImageProcessor Instance => new ImageSharpImageProcessor();
        #endregion

        #region Methods & Functions
        public bool Validate(byte[] buffer)
        {
            bool isPCX = PCX.Instance.Validate(buffer);
            bool isMAP = MAP.ValidateFormat(buffer);
            bool isFPG = FPG.ValidateFormat(buffer);

            return !(isPCX || isMAP || isFPG);
        }

        public Image Process(byte[] buffer, out IImageFormat mime)
        {
            return Image.Load(buffer, out mime);
        } 
        #endregion
    }
}
