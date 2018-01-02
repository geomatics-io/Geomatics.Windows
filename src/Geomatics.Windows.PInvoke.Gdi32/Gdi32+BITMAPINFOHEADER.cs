using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PInvoke
{
    /// biSizeImage > The size, in bytes, of the image. 
    /// bfSize > The size, in bytes, of the bitmap file. 
    /// 
    /// What is the difference between image & bitmap file? biSizeImage is the whole image size, 
    /// bfSize is the same, but you have to add the size of the 2 header files.

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/dd183376(v=vs.85).aspx

        /// <summary>
        /// The number of bytes required by the structure. 
        /// What is 'the structure' exactly?  The structure is the struct BITMAPINFOHEADER. That is a fixed value.
        /// </summary>
        public uint biSize;
        /// <summary>
        /// The width of the bitmap, in pixels. 
        /// If biCompression is BI_JPEG or BI_PNG, the biWidth member specifies the width of the decompressed JPEG or PNG image file, respectively.
        /// </summary>
        public int biWidth;
        /// <summary>
        /// The height of the bitmap, in pixels. If biHeight is positive, the bitmap is a bottom-up DIB and its origin is the lower-left corner. If biHeight is negative, the bitmap is a top-down DIB and its origin is the upper-left corner.
        /// If biHeight is negative, indicating a top-down DIB, biCompression must be either BI_RGB or BI_BITFIELDS. Top-down DIBs cannot be compressed.
        /// If biCompression is BI_JPEG or BI_PNG, the biHeight member specifies the height of the decompressed JPEG or PNG image file, respectively.
        /// </summary>
        public int biHeight;
        /// <summary>
        /// The number of planes for the target device. This value must be set to 1.
        /// </summary>
        public ushort biPlanes;
        /// <summary>
        /// The number of bits-per-pixel. The biBitCount member of the BITMAPINFOHEADER structure determines the number of bits that define each pixel and the maximum number of colors in the bitmap. This member must be one of the following values.
        /// </summary>
        public ushort biBitCount;
        /// <summary>
        /// The type of compression for a compressed bottom-up bitmap (top-down DIBs cannot be compressed). This member can be one of the following values.
        /// </summary>
        public BitmapCompression biCompression;
        /// <summary>
        /// The size, in bytes, of the image. This may be set to zero for BI_RGB bitmaps. 
        /// If biCompression is BI_JPEG or BI_PNG, biSizeImage indicates the size of the JPEG or PNG image buffer, respectively.
        /// </summary>
        public uint biSizeImage;
        /// <summary>
        /// The horizontal resolution, in pixels-per-meter, of the target device for the bitmap. An application can use this value to select a bitmap from a resource group that best matches the characteristics of the current device.
        /// </summary>
        public int biXPelsPerMeter;
        /// <summary>
        /// The vertical resolution, in pixels-per-meter, of the target device for the bitmap.
        /// </summary>
        public int biYPelsPerMeter;
        /// <summary>
        /// The number of color indexes in the color table that are actually used by the bitmap. If this value is zero, the bitmap uses the maximum number of colors corresponding to the value of the biBitCount member for the compression mode specified by biCompression.
        /// </summary>
        public uint biClrUsed;
        /// <summary>
        /// The number of color indexes that are required for displaying the bitmap. If this value is zero, all colors are required.
        /// </summary>
        public uint biClrImportant;

        public bool IsDibV5
        {
            get
            {
                uint sizeOfBMI = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                return biSize >= sizeOfBMI;
            }
        }

        public PixelFormat GetPixelFormat()
        {
            switch (biBitCount)
            {
                case 32:
                {
                    return PixelFormat.Format32bppRgb;
                }
                case 24:
                {
                    return PixelFormat.Format24bppRgb;
                }
                case 16:
                {
                   return PixelFormat.Format16bppRgb565;
                }
                case 15:
                {
                    return PixelFormat.Format16bppRgb555;
                }
                default:
                {
                    return PixelFormat.DontCare;
                }
            }
        }
    }
}