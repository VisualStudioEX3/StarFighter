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
            bool isPCX = PCX.IsPCX(buffer);
            bool isMAP = MAP.Instance.CheckHeader(buffer);

            return !(isPCX || isMAP);
        }

        public Image Process(byte[] buffer, out IImageFormat mime)
        {
            return Image.Load(buffer, out mime);
        } 
        #endregion
    }
}
