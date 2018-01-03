using System;
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

        public static IDataSource GetDataSource()
        {
            string username = null, machinename = null, os = null;

            try
            {
                os = Environment.OSVersion.ToString();
                machinename = Environment.MachineName;
                username = Environment.UserName;
            }
            catch
            {
                // ignored
            }

            var activeWindowHandle = User32.GetForegroundWindow();
        
            var windowTitle = User32.GetWindowTitle(activeWindowHandle);
            var windowIcon = GetWindowIcon(activeWindowHandle);
        
            var iconBytes = ImagePersistenceService.ConvertBitmapSourceToByteArray(windowIcon);

            return new DataSource(iconBytes, windowTitle, username, machinename, os);
        }
    }
}