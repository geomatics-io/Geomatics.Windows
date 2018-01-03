using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Geomatics.Windows.Clipboard.Services.Images
{
    public class ImageService
    {
        /// <summary>
        /// Gets the raw bytes from an image.
        /// </summary>
        /// <param name="sourceImage">The image to get the bytes from.</param>
        /// <param name="stride">Stride of the retrieved image data.</param>
        /// <returns>The raw bytes of the image</returns>
        public static Byte[] GetImageData(Bitmap sourceImage, out Int32 stride)
        {
            // https://stackoverflow.com/a/43706643/100863

            BitmapData sourceData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            stride = sourceData.Stride;
            Byte[] data = new Byte[stride * sourceImage.Height];
            Marshal.Copy(sourceData.Scan0, data, 0, data.Length);
            sourceImage.UnlockBits(sourceData);
            return data;
        }

        /// <summary>
        /// Creates a bitmap based on data, width, height, stride and pixel format.
        /// </summary>
        /// <param name="sourceData">Byte array of raw source data</param>
        /// <param name="width">Width of the image</param>
        /// <param name="height">Height of the image</param>
        /// <param name="stride">Scanline length inside the data</param>
        /// <param name="pixelFormat">Pixel format</param>
        /// <param name="palette">Color palette</param>
        /// <param name="defaultColor">Default color to fill in on the palette if the given colors don't fully fill it.</param>
        /// <returns>The new image</returns>
        public static Bitmap BuildImage(Byte[] sourceData, Int32 width, Int32 height, Int32 stride, PixelFormat pixelFormat, Color[] palette, Color? defaultColor)
        {
            // https://stackoverflow.com/a/43967594/100863

            Bitmap newImage = new Bitmap(width, height, pixelFormat);
            BitmapData targetData = newImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, newImage.PixelFormat);
            CopyToMemory(targetData.Scan0, sourceData, 0, sourceData.Length, stride, targetData.Stride);
            newImage.UnlockBits(targetData);
            // For indexed images, set the palette.
            if ((pixelFormat & PixelFormat.Indexed) != 0 && palette != null)
            {
                ColorPalette pal = newImage.Palette;
                for (Int32 i = 0; i < pal.Entries.Length; i++)
                {
                    if (i < palette.Length)
                        pal.Entries[i] = palette[i];
                    else if (defaultColor.HasValue)
                        pal.Entries[i] = defaultColor.Value;
                    else
                        break;
                }
                newImage.Palette = pal;
            }
            return newImage;
        }

        public static void CopyToMemory(IntPtr target, Byte[] bytes, Int32 startPos, Int32 length, Int32 origStride, Int32 targetStride)
        {
            // https://stackoverflow.com/a/43967594/100863
            Int32 sourcePos = startPos;
            IntPtr destPos = target;
            Int32 minStride = Math.Min(origStride, targetStride);
            while (length >= targetStride)
            {
                Marshal.Copy(bytes, sourcePos, destPos, minStride);
                length -= origStride;
                sourcePos += origStride;
                destPos = new IntPtr(destPos.ToInt64() + targetStride);
            }
            if (length > 0)
            {
                Marshal.Copy(bytes, sourcePos, destPos, length);
            }
        }

        public static void GetStride(int width, PixelFormat format, ref int stride, ref int bytesPerPixel)
        {
            // https://stackoverflow.com/a/6697224/100863

            //int bitsPerPixel = ((int)format & 0xff00) >> 8;
            int bitsPerPixel = global::System.Drawing.Image.GetPixelFormatSize(format);
            bytesPerPixel = (bitsPerPixel + 7) / 8;
            stride = 4 * ((width * bytesPerPixel + 3) / 4);
        }

        public static Bitmap CloneImage(Bitmap bm)
        {
            Int32 stride;
            byte[] imageData = GetImageData(bm, out stride);

            Bitmap result = BuildImage(imageData, bm.Width, bm.Height, stride, bm.PixelFormat, bm.Palette.Entries, null);

            return result;

        }
    }
}