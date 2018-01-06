using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Geomatics.Windows.Clipboard.Data;
using Geomatics.Windows.Clipboard.Data.Interfaces;
using Geomatics.Windows.Clipboard.Services.Clipboard.Interfaces;
using Geomatics.Windows.Clipboard.Services.Images;
using PInvoke;

namespace Geomatics.Windows.Clipboard.Services.Clipboard
{
    public class DataSourceService : IDataSourceService
    {
        static BitmapSource GetWindowIcon(IntPtr windowHandle)
        {
            IntPtr hIcon = User32.SendMessage(
                windowHandle,
                (int)User32.WindowMessage.WM_GETICON,
                User32.ICON_BIG,
                IntPtr.Zero);

            if (hIcon == IntPtr.Zero)
            {
                hIcon = User32.GetClassLongPtr(windowHandle, User32.GCL_HICON);
            }

            if (hIcon == IntPtr.Zero)
            {
                hIcon = User32.LoadIcon(IntPtr.Zero, User32.IDI_APPLICATION);
            }

            if (hIcon != IntPtr.Zero)
            {
                return Imaging.CreateBitmapSourceFromHIcon(
                    hIcon,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            throw new InvalidOperationException("Could not load window icon.");
        }

        public static DataSource GetDataSource()
        {
            DataSource ds = new DataSource();
            
            try
            {
                ds.OperatingSystem = Environment.OSVersion.ToString();
                ds.MachineName = Environment.MachineName;
                ds.UserName = Environment.UserName;
            }
            catch
            {
                // ignored
            }

            var activeWindowHandle = User32.GetForegroundWindow();
            var process = ClipboardNative.GetProcessName(activeWindowHandle);

            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(process);
            ds.ApplicationName = fvi.ProductName;
            ds.ApplicationDescription = fvi.FileDescription;

            ds.SmallApplicationIcon = IconToBytes(GetSmallWindowIcon(process));
            ds.LargeApplicationIcon = IconToBytes(GetLargeWindowIcon(process));

            ds.ApplicationPath = process;

            ds.WindowTitle = User32.GetWindowTitle(activeWindowHandle);
            
            return ds;
        }

        private static Icon GetSmallWindowIcon(string fileName)
        {
            Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
            Shell32.SHGetFileInfo(fileName, 0, ref shfi, (uint)Marshal.SizeOf(shfi), Shell32.SHGFI_ICON | Shell32.SHGFI_SMALLICON);

            Icon icon = System.Drawing.Icon.FromHandle(shfi.hIcon);
            User32.DestroyIcon( shfi.hIcon );
            return icon;
        }
        private static Icon GetLargeWindowIcon(string fileName)
        {
            Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
            Shell32.SHGetFileInfo(fileName, 0, ref shfi, (uint)Marshal.SizeOf(shfi), Shell32.SHGFI_ICON | Shell32.SHGFI_LARGEICON);

            Icon icon = System.Drawing.Icon.FromHandle(shfi.hIcon);
            User32.DestroyIcon( shfi.hIcon );
            return icon;
        }

        public static byte[] IconToBytes(Icon icon)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                icon.Save(ms);
                return ms.ToArray();
            }
        }

        public static Icon BytesToIcon(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return new Icon(ms);
            }
        }
    }
}