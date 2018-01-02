using System;

namespace PInvoke
{
    /// <summary>
    /// Bitmap Compression values
    /// </summary>
    [Flags]
    public enum BitmapCompression : uint
    {
        /// <summary>
        /// An uncompressed format.
        /// </summary>
        BI_RGB = 0,
        /// <summary>
        /// A run-length encoded (RLE) format for bitmaps with 8 bpp. 
        /// The compression format is a 2-byte format consisting of a count 
        /// byte followed by a byte containing a color index.
        /// </summary>
        BI_RLE8 = 1,
        /// <summary>
        /// An RLE format for bitmaps with 4 bpp. The compression format is a 
        /// 2-byte format consisting of a count byte followed by two word-length
        /// color indexes.
        /// </summary>
        BI_RLE4 = 2,
        /// <summary>
        /// Specifies that the bitmap is not compressed and that the color table 
        /// consists of three DWORD color masks that specify the red, green, and
        /// blue components, respectively, of each pixel. This is valid when used 
        /// with 16- and 32-bpp bitmaps.
        /// </summary>
        BI_BITFIELDS = 3,
        /// <summary>
        /// Windows 98/Me, Windows 2000/XP: Indicates that the image is a JPEG image.
        /// </summary>
        BI_JPEG = 4,
        /// <summary>
        /// Windows 98/Me, Windows 2000/XP: Indicates that the image is a PNG image.
        /// </summary>
        BI_PNG = 5,
    }
}