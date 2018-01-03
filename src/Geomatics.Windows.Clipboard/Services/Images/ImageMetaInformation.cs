using PixelFormat = System.Windows.Media.PixelFormat;

namespace Geomatics.Windows.Clipboard.Services.Images
{
    public struct ImageMetaInformation
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public double DpiX { get; set; }

        public double DpiY { get; set; }

        public PixelFormat PixelFormat { get; set; }
    }
}