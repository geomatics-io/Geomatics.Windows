using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace PInvoke
{

    /// <summary>
    /// The BITMAPFILEHEADER structure contains information about the type, size, and layout of a file that contains a DIB.
    /// See <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd183374(v=vs.85).aspx">BITMAPFILEHEADER structure</a>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct BITMAPFILEHEADER
    {
        /// <summary>
        /// The file type; must be BM.
        /// </summary>
        public UInt16 bfType;

        /// <summary>
        /// The size, in bytes, of the bitmap file.
        /// </summary>
        public UInt32 bfSize;

        /// <summary>
        /// Reserved; must be zero.
        /// </summary>
        public UInt16 bfReserved1;

        /// <summary>
        ///     Reserved; must be zero.
        /// </summary>
        public UInt16 bfReserved2;

        /// <summary>
        /// The offset, in bytes, from the beginning of the BITMAPFILEHEADER structure to the bitmap bits.
        /// </summary>
        public UInt32 bfOffBits;
    }
}