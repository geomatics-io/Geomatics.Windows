using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using PInvoke;
using PInvoke.Interfaces;

namespace Geomatics.Windows.Interop
{
    public class BitmapUtils
    {
        /*
         * References: https://github.com/Microsoft/Windows-classic-samples/blob/master/Samples/Win7Samples/multimedia/wia/getimage/BitmapUtil.cpp
         */

        /// <summary>
        /// The GetBitmapHeaderSize function returns the size of the DIB header.
        /// </summary>
        /// <param name="pDib">Pointer to the in-memory DIB that can be represented by BITMAPCOREHEADER, BITMAPINFOHEADER, BITMAPV4HEADER or BITMAPV5HEADER.</param>
        /// <returns>Returns the size of the DIB header in bytes or 0 if the header is not recognized.</returns>
        public static UInt32 GetBitmapHeaderSize(IntPtr pDib)
        {
            IntPtr ptr = Kernel32.GlobalLock(pDib);

            /*
             * The image header immediately follows the BITMAPFILEHEADER structure.
             * It comes in two distinct formats, defined by the BITMAPINFOHEADER and
             * BITMAPCOREHEADER structures. BITMAPCOREHEADER represents the OS/2 BMP
             * format and BITMAPINFOHEADER is the much more common Windows format.
             * Unfortunately, there is no version field in the BMP definitions. The only way to
             * determine the type of image structure used in a particular file is to examine the
             * structure's size field, which is the first 4 bytes of both structure types.
             */

//            UInt32 dibHdr = (UInt32)Marshal.PtrToStructure(pDib, typeof(UInt32));

            byte[] biSizeAr = new byte[4];
            Marshal.Copy(ptr, biSizeAr, 0, biSizeAr.Length);
            UInt32 nHeaderSize = BitConverter.ToUInt32(biSizeAr, 0);
            Kernel32.GlobalUnlock(pDib);

            if (
                nHeaderSize != Marshal.SizeOf(typeof(BITMAPCOREHEADER)) && // 12
                nHeaderSize != Marshal.SizeOf(typeof(BITMAPINFOHEADER)) && // 40
                nHeaderSize != Marshal.SizeOf(typeof(BITMAPV4HEADER)) && // 108
                nHeaderSize != Marshal.SizeOf(typeof(BITMAPV5HEADER))) // 124

                return 0;

            else return nHeaderSize;
        }

        /// <summary>
        /// The GetBitmapDimensions function returns the width and height of a DIB.
        /// </summary>
        /// <param name="nWidthInPixels">Width of a scan line in pixels.</param>
        /// <param name="nBitCount">Number of bits per pixel.</param>
        /// <returns>System.Int32.</returns>
        static int GetBitmapLineWidthInBytes(int nWidthInPixels, int nBitCount)
        {
            return (((nWidthInPixels * nBitCount) + 31) & ~31) >> 3;
        }

        /// <summary>
        /// Gets the bitmap dimensions.
        /// </summary>
        /// <param name="pDib">The p dib.</param>
        /// <param name="pWidth">Width of the p.</param>
        /// <param name="pHeight">Height of the p.</param>
        public static bool GetBitmapDimensions(IntPtr pDib, out int pWidth, out int pHeight)
        {
            uint nHeaderSize = GetBitmapHeaderSize(pDib);

            pWidth = 0;
            pHeight = 0;

            if (nHeaderSize == 0)
                return false;

            if (nHeaderSize == Marshal.SizeOf(typeof(BITMAPCOREHEADER)))
            {
                BITMAPCOREHEADER pbmch = (BITMAPCOREHEADER) Marshal.PtrToStructure(pDib, typeof(BITMAPCOREHEADER));

                pWidth = pbmch.bcWidth;
                pHeight = pbmch.bcHeight;
            }
            else
            {
                BITMAPINFOHEADER pbmih = (BITMAPINFOHEADER) Marshal.PtrToStructure(pDib, typeof(BITMAPINFOHEADER));
                pWidth = pbmih.biWidth;
                pHeight = pbmih.biHeight;
            }

            return true;
        }

        /// <summary>
        /// The GetBitmapSize function returns total size of the DIB. The size is the sum of the bitmap header, 
        /// the color palette (if present), the color profile data (if present) and the pixel data.
        /// </summary>
        /// <param name="pDib">Pointer to the in-memory DIB that can be represented by BITMAPCOREHEADER, BITMAPINFOHEADER, BITMAPV4HEADER or BITMAPV5HEADER.</param>
        /// <returns>Returns the size of the image in bytes or 0 if the header is not recognized.</returns>
        public static UInt32 GetBitmapSize(IntPtr pDib)
        {
            uint nHeaderSize = GetBitmapHeaderSize(pDib);

            if (nHeaderSize == 0)
            {
                return 0;
            }

            // Start the calculation with the header size
            UInt32 nDibSize = nHeaderSize;

            // is this an old style BITMAPCOREHEADER?
            if (nHeaderSize == Marshal.SizeOf(typeof(BITMAPCOREHEADER)))
            {
                BITMAPCOREHEADER pbmch = (BITMAPCOREHEADER) Marshal.PtrToStructure(pDib, typeof(BITMAPCOREHEADER));


                // Add the color table size
                if ((ushort) pbmch.bcBitCount <= 8)
                {
                    nDibSize += Convert.ToUInt32(Marshal.SizeOf(typeof(RGBTRIPLE)) * (1 << (int) pbmch.bcBitCount));
                }

                // Add the bitmap size
                int nWidth = GetBitmapLineWidthInBytes(pbmch.bcWidth, (int) pbmch.bcBitCount);

                nDibSize += Convert.ToUInt32(nWidth * pbmch.bcHeight);
            }
            else
            {
                // this is at least a BITMAPINFOHEADER
                BITMAPINFOHEADER pbmih = (BITMAPINFOHEADER) Marshal.PtrToStructure(pDib, typeof(BITMAPINFOHEADER));

                // Add the color table size
                if (pbmih.biClrUsed != 0)
                {
                    nDibSize += Convert.ToUInt32(Marshal.SizeOf(typeof(RGBQUAD)) * pbmih.biClrUsed);
                }
                else if (pbmih.biBitCount <= 8)
                {
                    nDibSize += Convert.ToUInt32(Marshal.SizeOf(typeof(RGBQUAD)) * (1 << (int) pbmih.biBitCount));
                }

                // Add the bitmap size

                if (pbmih.biSizeImage != 0)
                {
                    nDibSize += pbmih.biSizeImage;
                }
                else
                {
                    // biSizeImage must be specified for compressed bitmaps

                    if (pbmih.biCompression != BitmapCompression.BI_RGB &&
                        pbmih.biCompression != BitmapCompression.BI_BITFIELDS)
                    {
                        return 0;
                    }

                    int nWidth = GetBitmapLineWidthInBytes(pbmih.biWidth, pbmih.biBitCount);

                    nDibSize += Convert.ToUInt32(nWidth * Math.Abs(pbmih.biHeight));
                }

                // Consider special cases
                if (nHeaderSize == Marshal.SizeOf(typeof(BITMAPINFOHEADER)))
                {
                    // If this is a 16 or 32 bit bitmap and BI_BITFIELDS is used, 
                    // bmiColors member contains three DWORD color masks.
                    // For V4 or V5 headers, this info is included the header

                    if (pbmih.biCompression == BitmapCompression.BI_BITFIELDS)
                    {
                        nDibSize += 3 * sizeof(uint);
                    }
                }
                else if (nHeaderSize >= Marshal.SizeOf(typeof(BITMAPV5HEADER)))
                {
                    // If this is a V5 header and an ICM profile is specified,
                    // we need to consider the profile data size
                    BITMAPV5HEADER pbV5h = (BITMAPV5HEADER) Marshal.PtrToStructure(pDib, typeof(BITMAPV5HEADER));

                    // if there is some padding before the profile data, add it

                    if (pbV5h.bV5ProfileData > nDibSize)
                    {
                        nDibSize = pbV5h.bV5ProfileData;
                    }

                    // add the profile data size

                    nDibSize += pbV5h.bV5ProfileSize;
                }
            }

            return nDibSize;
        }

        /// <summary>
        /// The GetBitmapOffsetBits function returns the offset, in bytes, from the beginning of the DIB data block to the bitmap bits.
        /// </summary>
        /// <param name="pDib">Pointer to the in-memory DIB that can be represented by BITMAPCOREHEADER, BITMAPINFOHEADER, BITMAPV4HEADER or BITMAPV5HEADER.</param>
        /// <returns> Returns the offset from the beginning of the DIB data block to the bitmap pixels in bytes or 0 if the header is not recognized.</returns>
        public static UInt32 GetBitmapOffsetBits(IntPtr pDib)
        {
            uint nHeaderSize = GetBitmapHeaderSize(pDib);

            if (nHeaderSize == 0)
            {
                return 0;
            }

            // Start the calculation with the header size
            UInt32 nOffsetBits = nHeaderSize;

            // is this an old style BITMAPCOREHEADER?
            if (nHeaderSize == Marshal.SizeOf(typeof(BITMAPCOREHEADER)))
            {
                BITMAPCOREHEADER pbmch = (BITMAPCOREHEADER)Marshal.PtrToStructure(pDib, typeof(BITMAPCOREHEADER));


                // Add the color table size
                if ((ushort)pbmch.bcBitCount <= 8)
                {
                    nOffsetBits += Convert.ToUInt32(Marshal.SizeOf(typeof(RGBTRIPLE)) * (1 << (int)pbmch.bcBitCount));
                }
            }
            else
            {
                // this is at least a BITMAPINFOHEADER
                BITMAPINFOHEADER pbmih = (BITMAPINFOHEADER)Marshal.PtrToStructure(pDib, typeof(BITMAPINFOHEADER));

                // Add the color table size
                if (pbmih.biClrUsed != 0)
                {
                    nOffsetBits += Convert.ToUInt32(Marshal.SizeOf(typeof(RGBQUAD)) * pbmih.biClrUsed);
                }
                else if (pbmih.biBitCount <= 8)
                {
                    nOffsetBits += Convert.ToUInt32(Marshal.SizeOf(typeof(RGBQUAD)) * (1 << (int)pbmih.biBitCount));
                }
                

                // Consider special cases
                if (nHeaderSize == Marshal.SizeOf(typeof(BITMAPINFOHEADER)))
                {
                    // If this is a 16 or 32 bit bitmap and BI_BITFIELDS is used, 
                    // bmiColors member contains three DWORD color masks.
                    // For V4 or V5 headers, this info is included the header

                    if (pbmih.biCompression == BitmapCompression.BI_BITFIELDS)
                    {
                        nOffsetBits += 3 * sizeof(uint);
                    }
                }
                else if (nHeaderSize >= Marshal.SizeOf(typeof(BITMAPV5HEADER)))
                {
                    // If this is a V5 header and an ICM profile is specified,
                    // we need to consider the profile data size
                    BITMAPV5HEADER pbV5h = (BITMAPV5HEADER)Marshal.PtrToStructure(pDib, typeof(BITMAPV5HEADER));

                    // if the profile data comes before the pixel data, add it
                    if (pbV5h.bV5ProfileData <= nOffsetBits)
                    {
                        nOffsetBits += pbV5h.bV5ProfileData;
                    }
                }
            }

            return nOffsetBits;
        }

        /// <summary>
        /// The FillBitmapFileHeader function fills in a BITMAPFILEHEADER structure according to the values specified in the DIB.
        /// </summary>
        /// <param name="pDib">Pointer to the in-memory DIB that can be represented by BITMAPCOREHEADER, BITMAPINFOHEADER, BITMAPV4HEADER or BITMAPV5HEADER.</param>
        /// <param name="pbmfh">Receives the BITMAPFILEHEADER structure filled with the values specified in the DIB</param>
        /// <returns>Returns TRUE if the header is recognized, FALSE otherwise.</returns>
        public static bool FillBitmapFileHeader(IntPtr pDib, out BITMAPFILEHEADER pbmfh)
        {
            uint nSize = GetBitmapHeaderSize(pDib);

            if (nSize == 0)
            {
                pbmfh = new BITMAPFILEHEADER();
                return false;
            }

            uint nOffset = GetBitmapOffsetBits(pDib);

            if (nOffset == 0)
            {
                pbmfh = new BITMAPFILEHEADER();
                return false;
            }
            
            pbmfh.bfType = 0x4d42; // Fill with "BM"
            pbmfh.bfSize = Convert.ToUInt32(Marshal.SizeOf(typeof(BITMAPFILEHEADER)) + nSize);
            pbmfh.bfReserved1 = 0;
            pbmfh.bfReserved2 = 0;
            pbmfh.bfOffBits = Convert.ToUInt32(Marshal.SizeOf(typeof(BITMAPFILEHEADER)) + nOffset);

            return true;
        }

        /// <summary>
        /// Saves the DIB to a Bitmap file.
        /// </summary>
        /// <param name="pDib">Pointer to the in-memory DIB that can be represented by BITMAPCOREHEADER, BITMAPINFOHEADER, BITMAPV4HEADER or BITMAPV5HEADER.</param>
        /// <param name="file">The file to which the BMP should be written.</param>
        /// <returns><c>true</c> if the file was successfully created, <c>false</c> otherwise.</returns>
        public static bool SaveDibToFile(IntPtr pDib, FileInfo file)
        {
            if (pDib == IntPtr.Zero)
                return false;

            if (file == null)
                return false;

            uint nHeaderSize = GetBitmapHeaderSize(pDib);

            if (nHeaderSize == 0)
                return false;

            using (MemoryStream ms = DibToMemoryStream(pDib))
            {
                using (Stream stream = new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    ms.CopyTo(stream);
                }
            }

            return true;
        }

        public static bool SaveBitmapToFile(Bitmap bitmap, FileInfo file)
        {
            Bitmap bm = new Bitmap(bitmap);

            using (Stream fs = new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite))
            {
                bm.Save(fs, ImageFormat.Bmp);
            }
                

//            MemoryStream ms = new MemoryStream();
//
//            Bitmap bit = new Bitmap(bitmap);
//
//            using (MemoryStream memory = new MemoryStream())
//            {
//                using (FileStream fs = new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite))
//                {
//                    bitmap.Save(memory, ImageFormat.Jpeg);
//                    byte[] bytes = memory.ToArray();
//                    fs.Write(bytes, 0, bytes.Length);
//                }
//            }

            return true;
        }

        public static Bitmap ConvertDibToBitmap(IntPtr pDib)
        {
            Bitmap bm = null;
            using (MemoryStream ms = DibToMemoryStream(pDib))
            {
                bm = new Bitmap(ms);
            }

            return bm;
        }

        internal static MemoryStream DibToMemoryStream(IntPtr pDib)
        {
            if (pDib == IntPtr.Zero)
                return null;
            
            uint nHeaderSize = GetBitmapHeaderSize(pDib);

            if (nHeaderSize == 0)
                return null;

            BITMAPFILEHEADER bmfh;
            if (!FillBitmapFileHeader(pDib, out bmfh))
                return null;

            byte[] fileHeaderBytes = BinaryUtils.StructureToByteArray<BITMAPFILEHEADER>(bmfh);
            int fileHeaderSize = Marshal.SizeOf(typeof(BITMAPFILEHEADER));

            uint dibSize = GetBitmapSize(pDib);
            byte[] dibArray = new byte[dibSize];
            Marshal.Copy(pDib, dibArray, 0, dibArray.Length);

            MemoryStream ms = new MemoryStream();
            ms.Write(fileHeaderBytes, 0, fileHeaderSize);
            ms.Write(dibArray, 0, dibArray.Length);
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        public static bool Equals(Bitmap bmp1, Bitmap bmp2)
        {
            if (!bmp1.Size.Equals(bmp2.Size))
            {
                return false;
            }
            for (int x = 0; x < bmp1.Width; ++x)
            {
                for (int y = 0; y < bmp1.Height; ++y)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}