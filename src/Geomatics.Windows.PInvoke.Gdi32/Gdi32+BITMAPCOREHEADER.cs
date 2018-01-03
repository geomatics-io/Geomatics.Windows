using System.Runtime.InteropServices;
using PInvoke.Interfaces;

namespace PInvoke
{
    /// <summary>
    /// The BITMAPCOREHEADER structure contains information about the dimensions and color format of a DIB.
    /// <remarks>The BITMAPCOREINFO structure combines the BITMAPCOREHEADER structure and a color table to provide 
    /// a complete definition of the dimensions and colors of a DIB. 
    /// For more information about specifying a DIB, see BITMAPCOREINFO.</remarks>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPCOREHEADER : IBitmapHeader
    {
        /// <summary>
        /// The number of bytes required by the structure.
        /// </summary>
        public uint bcSize;

        /// <summary>
        /// The width of the bitmap, in pixels.
        /// </summary>
        public int bcWidth;

        /// <summary>
        /// The height of the bitmap, in pixels.
        /// </summary>
        public int bcHeight;

        /// <summary>
        /// The number of planes for the target device. This value must be 1.
        /// </summary>
        public ushort bcPlanes;

        /// <summary>
        /// The number of bits-per-pixel. This value must be 1, 4, 8, or 24.
        /// </summary>
        public BitCount bcBitCount;
    }
}