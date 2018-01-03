using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Geomatics.Windows.Clipboard.Services.Images.Interfaces;
using Geomatics.Windows.Interop;
using PInvoke;

namespace Geomatics.Windows.Clipboard.Services.Images
{
    public class ImagePersistenceService : IImagePersistenceService
    {
        public static byte[] ConvertBitmapSourceToByteArray(BitmapSource bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            var metaInformation = new ImageMetaInformation
            {
                DpiX = bitmap.DpiX,
                DpiY = bitmap.DpiY,
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                PixelFormat = bitmap.Format
            };

            var imageData = ConvertImageDataToByteArray(bitmap);
            return DecorateSourceWithMetaInformation(
                imageData,
                metaInformation);
        }

        static byte[] ConvertImageDataToByteArray(BitmapSource bitmap)
        {
            var stride = bitmap.PixelWidth * ((bitmap.Format.BitsPerPixel + 7) / 8);

            var imageData = new byte[bitmap.PixelHeight * stride];
            bitmap.CopyPixels(imageData, stride, 0);
            return imageData;
        }

        public static byte[] DecorateSourceWithMetaInformation(
            byte[] source,
            ImageMetaInformation information)
        {
            var metaData = ConvertMetaInformationToByteArray(information);
            return metaData
                .Concat(source)
                .ToArray();
        }

        static IEnumerable<byte> ConvertMetaInformationToByteArray(
            ImageMetaInformation metaInformation)
        {
            return BinaryUtils
                .StructureToByteArray(metaInformation);
        }
    }
}