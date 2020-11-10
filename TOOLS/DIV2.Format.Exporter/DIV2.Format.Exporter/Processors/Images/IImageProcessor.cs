using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace DIV2.Format.Exporter.Processors.Images
{
    interface IImageProcessor
    {
        #region Methods & Functions
        bool CheckFormat(byte[] buffer);
        Image Process(byte[] buffer, out IImageFormat mime); 
        #endregion
    }
}
