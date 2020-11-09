using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace DIV2.Format.Exporter.Processors
{
    interface IImageProcessor
    {
        bool CheckFormat(byte[] buffer);

        Image Process(byte[] buffer, out IImageFormat mime);
    }
}
