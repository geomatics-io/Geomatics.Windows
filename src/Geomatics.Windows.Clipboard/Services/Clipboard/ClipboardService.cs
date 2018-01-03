using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Geomatics.Windows.Clipboard.Data;
using Geomatics.Windows.Clipboard.Services.Images;
using PInvoke;

namespace Geomatics.Windows.Clipboard.Services.Clipboard
{
    public class ClipboardService
    {
        /// <summary>
        /// Convert to a ClipboardData collection all data present in the clipboard
        /// </summary>
        /// <returns></returns>
        public static ClipboardDataPackage GetClipboard()
        {
//            ClipboardDataPackage clipboardDataPackage = new ClipboardDataPackage();
            // clipboardDataPackage.DataSource = DataSourceService.GetDataSource();

            List<KeyValuePair<uint, byte[]>> clipboardContent = CreateClipboardCopy();

            //Init a list of ClipboardData, which will contain each Clipboard Data
            List<ClipboardData> clipData = new List<ClipboardData>();

            foreach (KeyValuePair<uint, byte[]> keyValuePair in clipboardContent)
            {
                var formatName = GetClipboardFormatName(keyValuePair.Key);

                ClipboardData cd = new ClipboardData(keyValuePair.Key, formatName, keyValuePair.Value);
                clipData.Add(cd);

                Debug.WriteLine(string.Format("{0:X4} - {1} - {2:N4}", keyValuePair.Key, formatName, keyValuePair.Value.Length));
            }

//            clipboardDataPackage.Contents = clipData;

            return null;
        }

        private static List<KeyValuePair<uint, byte[]>> CreateClipboardCopy()
        {
            List<KeyValuePair<uint, byte[]>> clipboard = new List<KeyValuePair<uint, byte[]>>(5);

            //Open Clipboard to allow us to read from it
            if (!User32.OpenClipboard(IntPtr.Zero))
                return null;

            List<uint> formats = new List<uint>(ClipboardService.GetClipboardFormats());
            formats.Sort();

            foreach (uint format in formats)
            {
                if (!User32.IsClipboardFormatAvailable(format))
                    continue;

                byte[] buffer = new byte[] { };
                
                if (User32.IsClipboardFormatAvailable(format))
                {
                    if (format == (uint)User32.StandardClipboardFormat.CF_BITMAP)
                    {
                        IntPtr hbm = User32.GetClipboardData(format);
                        IntPtr hCopy = User32.CopyImage(hbm, User32.IMAGE_BITMAP, 0, 0, User32.LR_COPYRETURNORG);
                        buffer = GetBitmapData(hCopy);
                    }
                    else if (format == (uint) User32.StandardClipboardFormat.CF_DIB)
                    {
                        IntPtr hbm = User32.GetClipboardData(format);
                        buffer = ImageNativeConversionService.CF_DIBToByteArray(hbm);
                    }
                    else if (format == (uint) User32.StandardClipboardFormat.CF_DIBV5)
                    {

                    }
                    else
                        buffer = CopyFormat(format);

                    clipboard.Add(new KeyValuePair<uint, byte[]>(format, buffer));
                }
            }

            //Close the clipboard and realese unused resources
            User32.CloseClipboard();

            return clipboard;
        }

        /// <summary>
        /// Empty the Clipboard and Restore to system clipboard data contained in a collection of ClipboardData objects
        /// </summary>
        /// <param name="clipData">The collection of ClipboardData containing data stored from clipboard</param>
        /// <returns></returns>    
        public static bool SetClipboard(ReadOnlyCollection<ClipboardData> clipData)
        {
            //Open clipboard to allow its manipultaion
            if (!User32.OpenClipboard(IntPtr.Zero))
                return false;

            //Clear the clipboard
            User32.EmptyClipboard();

            //Get an Enumerator to iterate into each ClipboardData contained into the collection
            using (IEnumerator<ClipboardData> cData = clipData.GetEnumerator())
            {
                while (cData.MoveNext())
                {
                    ClipboardData cd = cData.Current;

                    //Get the pointer for inserting the buffer data into the clipboard
                    IntPtr alloc = Kernel32.GlobalAlloc(
                        (int)Kernel32.GlobalAllocFlags.GMEM_MOVEABLE | (int)Kernel32.GlobalAllocFlags.GMEM_DDESHARE,
                        cd.Size);
                    IntPtr gLock = Kernel32.GlobalLock(alloc);

                    //Clopy the buffer of the ClipboardData into the clipboard
                    if ((int)cd.Size > 0)
                    {
                        Marshal.Copy((byte[]) cd.Buffer, 0, gLock, cd.Buffer.GetLength(0));
                    }
                    else
                    {
                    }
                    //Release pointers 
                    Kernel32.GlobalUnlock(alloc);
                    User32.SetClipboardData(cd.Format, alloc);
                }
            }
            //Close the clipboard to realese unused resources
            User32.CloseClipboard();
            return true;
        }

        public static IReadOnlyCollection<uint> GetClipboardFormats()
        {
            var formats = new List<uint>();
            var lastRetrievedFormat = 0u;
            while (0 != (lastRetrievedFormat = User32.EnumClipboardFormats(lastRetrievedFormat)))
            {
                formats.Add(lastRetrievedFormat);
            }
            return formats;
        }

        public static string GetClipboardFormatName(uint format)
        {
            if (Enum.IsDefined(typeof(User32.StandardClipboardFormat), format))
                return ((User32.StandardClipboardFormat)format).ToString();

            var sb = new StringBuilder(512);
            User32.GetClipboardFormatName(format, sb, sb.Capacity);
            return sb.ToString();
        }
        
        private static byte[] GetBitmapData(IntPtr hBitmap)
        {

            // TODO: https://stackoverflow.com/questions/11084506/how-to-convert-wpf-bitmapsource-to-byte-in-c-sharp

            var source = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, null);
            // You may use Bmp, Jpeg or other encoder of your choice
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            var stream = new MemoryStream();
            encoder.Save(stream);
            return stream.ToArray();
        }

        private static BitmapSource GetBitmapSource(byte[] data)
        {
            return BitmapFrame.Create(new MemoryStream(data));
        }

        private static byte[] CopyFormat(uint format)
        {
            IntPtr hGlobal = IntPtr.Zero;
            try
            {
                Debug.WriteLine(GetClipboardFormatName(format));
                hGlobal = User32.GetClipboardData(format);
                var memoryPtr = Kernel32.GlobalLock(hGlobal);

                if (memoryPtr == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                var size = Kernel32.GlobalSize(hGlobal);
                var stream = new MemoryStream((int) size);
                stream.SetLength((int) size);
                // Fill the memory stream
                Marshal.Copy(memoryPtr, stream.GetBuffer(), 0, (int) size);

                return stream.GetBuffer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, GetClipboardFormatName(format), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                if (hGlobal != IntPtr.Zero)
                    Kernel32.GlobalUnlock(hGlobal);
            }
        }
    }
}