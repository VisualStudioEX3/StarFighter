using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace DIV2.Format.Exporter.Processors.Images
{
    interface IImageProcessor : IFormatValidable
    {
        #region Methods & Functions
        Image Process(byte[] buffer, out IImageFormat mime); 
        #endregion
    }
}
