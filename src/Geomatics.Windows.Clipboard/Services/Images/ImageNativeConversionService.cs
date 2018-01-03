using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PInvoke;

namespace Geomatics.Windows.Clipboard.Services.Images
{
    public class ImageNativeConversionService
    {
        // Windows bitmaps are bottom up.  .NET bitmaps are top down.

        #region CF_DIB5

        public static Bitmap CF_DIBV5ToBitmap(IntPtr pPackedDib)
        {
            if (!User32.IsClipboardFormatAvailable((uint) User32.StandardClipboardFormat.CF_DIBV5))
                return null;

            IntPtr pDib = Kernel32.GlobalLock(pPackedDib);

            //                IntPtr pPackedDib = User32.GetClipboardData((uint)User32.StandardClipboardFormat.CF_DIBV5);
            BITMAPINFO bmi = (BITMAPINFO) Marshal.PtrToStructure(pPackedDib, typeof(BITMAPINFO));
            BITMAPV5HEADER bmh = (BITMAPV5HEADER) Marshal.PtrToStructure(pPackedDib, typeof(BITMAPV5HEADER));

            // Get the size of the information header and do validity check

            uint dwInfoSize = bmh.bV5Size;

            if (dwInfoSize != Marshal.SizeOf(typeof(BITMAPCOREHEADER)) &&
                dwInfoSize != Marshal.SizeOf(typeof(BITMAPINFOHEADER)) &&
                dwInfoSize != Marshal.SizeOf(typeof(BITMAPV4HEADER)) &&
                dwInfoSize != Marshal.SizeOf(typeof(BITMAPV5HEADER)))
            {
                return null;
            }

            // Get the possible size of the color masks

            uint dwMaskSize;

            if (dwInfoSize == Marshal.SizeOf(typeof(BITMAPINFOHEADER)) &&
                bmi.bmiHeader.biCompression == BitmapCompression.BI_BITFIELDS)
            {
                dwMaskSize = 3 * sizeof(uint);
            }
            else
            {
                dwMaskSize = 0;
            }

            // Get the size of the color table

            BitCount iBitCount;
            long dwColorSize;

            if (dwInfoSize == Marshal.SizeOf(typeof(BITMAPCOREHEADER)))
            {
                BITMAPCOREHEADER bmch = (BITMAPCOREHEADER) Marshal.PtrToStructure(pPackedDib, typeof(BITMAPCOREHEADER));

                iBitCount = bmch.bcBitCount;
                if (iBitCount <= BitCount.BitPerPixel8BPP)
                {
                    dwColorSize = (1 << (int) iBitCount) * Marshal.SizeOf(typeof(RGBTRIPLE));
                }
                else
                    dwColorSize = 0;
            }
            else // all non-OS/2 compatible DIBs
            {
                if (bmi.bmiHeader.biClrUsed > 0)
                {
                    dwColorSize = bmi.bmiHeader.biClrUsed * Marshal.SizeOf(typeof(RGBQUAD));
                }
                else if (bmi.bmiHeader.biBitCount <= 8)
                {
                    dwColorSize = (1 << bmi.bmiHeader.biBitCount) * Marshal.SizeOf(typeof(RGBQUAD));
                }
                else
                {
                    dwColorSize = 0;
                }
            }

            // Finally, get the pointer to the bits in the packed DIB
            IntPtr pBits = new IntPtr(pPackedDib.ToInt64() + dwInfoSize + dwMaskSize + dwColorSize);

            byte[] bBitmap = new byte[bmh.SizeImage()];

            Marshal.Copy(pBits, bBitmap, 0, bBitmap.Length);

            Bitmap bmp = new Bitmap(bmi.bmiHeader.biWidth, bmi.bmiHeader.biHeight, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, bmi.bmiHeader.biWidth, bmi.bmiHeader.biHeight);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(
                bBitmap,
                0,
                bmpData.Scan0,
                bBitmap.Length);

            bmp.UnlockBits(bmpData);

            Kernel32.GlobalUnlock(pPackedDib);

            return bmp;
        }

        public static byte[] CF_DIBV5ToByteArray(IntPtr pPackedDib)
        {
            byte[] result = null;
            IntPtr pDib = Kernel32.GlobalLock(pPackedDib);



            Kernel32.GlobalUnlock(pPackedDib);

            return result;
        }

        #endregion

        #region CF_BITMAP

        #endregion

        #region CF_DIB

        public static Bitmap CF_DIBToBitmap(IntPtr lpArray)
        {
            IntPtr pDib = Kernel32.GlobalLock(lpArray);

            BITMAPINFO pbmi = (BITMAPINFO) Marshal.PtrToStructure(pDib,
                typeof(BITMAPINFO)); // Get pointer to the BITMAPINFO structure

            if (pbmi.bmiHeader.biSizeImage == 0)
                pbmi.bmiHeader.biSizeImage = (uint) CalculateImageSize(pbmi);

            byte[] bBitmap = new byte[pbmi.bmiHeader.biSizeImage];

            Marshal.Copy(new IntPtr(pDib.ToInt32() + pbmi.bmiHeader.biSize), bBitmap, 0, bBitmap.Length);

            PixelFormat pixelFormat = pbmi.bmiHeader.GetPixelFormat();
           
            Bitmap bmp = new Bitmap(pbmi.bmiHeader.biWidth, pbmi.bmiHeader.biHeight, pixelFormat);

            Rectangle rect = new Rectangle(0, 0, pbmi.bmiHeader.biWidth, pbmi.bmiHeader.biHeight);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            Marshal.Copy(
                bBitmap,
                0,
                bmpData.Scan0,
                bBitmap.Length);

            bmp.UnlockBits(bmpData);

            Kernel32.GlobalUnlock(lpArray);

            return bmp;
        }

        public static byte[] CF_DIBToByteArray(IntPtr lpArray)
        {
            IntPtr pDib = Kernel32.GlobalLock(lpArray);

            BITMAPINFO pbmi = (BITMAPINFO) Marshal.PtrToStructure(pDib,
                typeof(BITMAPINFO)); // Get pointer to the BITMAPINFO structure

            if (pbmi.bmiHeader.biSizeImage == 0)
                pbmi.bmiHeader.biSizeImage = (uint) CalculateImageSize(pbmi);

            byte[] bBitmap = new byte[pbmi.bmiHeader.biSizeImage];

            Marshal.Copy(new IntPtr(pDib.ToInt64() + pbmi.bmiHeader.biSize), bBitmap, 0, bBitmap.Length);

            Kernel32.GlobalUnlock(lpArray);

            return Geomatics.Windows.Extensions.System.ByteArray.Utils.Combine(new byte[][]
            {
                BITMAPINFOToByteArray(pbmi),
                bBitmap
            });
        }

        public static Bitmap CF_DIBByteArrayToBitmap(byte[] array)
        {
            int size = Marshal.SizeOf(typeof(BITMAPINFO));
            byte[] bitmapinfoArray = new byte[size];

            Array.Copy(array, 0, bitmapinfoArray, 0, bitmapinfoArray.Length);
            BITMAPINFO pbmi = ByteArrayToBITMAPINFO(bitmapinfoArray);

            byte[] bBitmap = new byte[array.Length - bitmapinfoArray.Length];
            Array.Copy(array, bitmapinfoArray.Length, bBitmap, 0, bBitmap.Length);

            PixelFormat pixelFormat;
            int stride;

            switch (pbmi.bmiHeader.biBitCount)
            {
                case 32:
                {
                    pixelFormat = PixelFormat.Format32bppRgb;
                    stride = pbmi.bmiHeader.biWidth * 4;
                    break;
                }
                case 24:
                {
                    pixelFormat = PixelFormat.Format24bppRgb;
                    stride = pbmi.bmiHeader.biWidth * 3;
                    break;
                }
                case 16:
                {
                    pixelFormat = PixelFormat.Format16bppRgb565;
                    stride = pbmi.bmiHeader.biWidth * 2;
                    break;
                }
                case 15:
                {
                    pixelFormat = PixelFormat.Format16bppRgb555;
                    stride = pbmi.bmiHeader.biWidth * 2;
                    break;
                }
                default:
                {
                    pixelFormat = PixelFormat.DontCare;
                    stride = pbmi.bmiHeader.biWidth * 1;
                    break;
                }
            }

            Bitmap bmp = new Bitmap(pbmi.bmiHeader.biWidth, pbmi.bmiHeader.biHeight, pixelFormat);

            Rectangle rect = new Rectangle(0, 0, pbmi.bmiHeader.biWidth, pbmi.bmiHeader.biHeight);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            Marshal.Copy(
                bBitmap,
                0,
                bmpData.Scan0,
                bBitmap.Length);

            bmp.UnlockBits(bmpData);

            return bmp;
        }

        private static int CalculateImageSize(BITMAPINFO bi)
        {
            int size = CalculateSurfaceStride(bi) * bi.bmiHeader.biHeight;
            return size;

            // http://www.vbforums.com/showthread.php?631031-RESOLVED-BMP-structure-calculate-the-biSizeImage

            // All scanlines in a DIB are padded to a DWORD boundary

            /* Bitmap images are stored in "scan lines" top to bottom. Most bitmaps are stored upside down,
             * but still each row of pixels is commonly referred to as a scan line.
             *
             * The scan line must be word-aligned.This means in multiples of 4.
             * So if you have a 24bpp bitmap at 3 pixels wide, it contains 9 pixels for each scan line.
             * 9 is not evenly divisible by 4.
             * When this happens buffer byte(s) as needed must be added to each scan line.
             */

            /*
             * Public Function ByteAlignOnWord(ByVal BitDepth As Byte, ByVal Width As Long) As Long
             * ' function to align any bit depth on dWord boundaries
             * &H1F& = 31 DEC
             * &H8& = 8 DEC
             *
             * ByteAlignOnWord = (((Width * BitDepth) + &H1F&) And Not &H1F&) \ &H8&
             * End Function
             *
             * .biSizeImage = Abs(.biHeight) * ByteAlignOnWord(.biBitCount, .biWidth)
             */
        }

        /// <summary>
        /// Calculates the surface stride.
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd318229(v=vs.85).aspx
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/9bf9dea5-e21e-4361-a0a6-be331efde835/how-do-you-calculate-the-image-stride-for-a-bitmap?forum=csharpgeneral
        /// </summary>
        /// <param name="bi">The bi.</param>
        /// <returns>System.UInt32.</returns>
        private static int CalculateSurfaceStride(BITMAPINFO bi)
        {
            int stride = ((((bi.bmiHeader.biWidth * bi.bmiHeader.biBitCount) + 31) & ~31) >> 3);
            return stride;
        }

        #endregion

        #region Helper

        static byte[] BITMAPINFOToByteArray(BITMAPINFO bmi)
        {
            int size = Marshal.SizeOf(bmi);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(bmi, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        static BITMAPINFO ByteArrayToBITMAPINFO(byte[] arr)
        {
            BITMAPINFO bmi = new BITMAPINFO();

            int size = Marshal.SizeOf(bmi);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            bmi = (BITMAPINFO) Marshal.PtrToStructure(ptr, (Type) bmi.GetType());
            Marshal.FreeHGlobal(ptr);

            return bmi;
        }

        #endregion
    }
}