using System.Text;

namespace PInvoke
{
    using System;
    using System.Runtime.InteropServices;

#pragma warning disable SA1300 // Elements must begin with an uppercase letter

    /// <summary>
    /// Exported functions from the User32.dll Windows library.
    /// </summary>
    public static partial class User32
    {
        public static IntPtr ICON_BIG => new IntPtr(1);
        public const int GCL_HICON = -14;
        public static IntPtr IDI_APPLICATION => new IntPtr(0x7F00);

        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms648045(v=vs.85).aspx
        public static uint IMAGE_BITMAP = 0;

        public static uint LR_COPYRETURNORG = 0x00000004;


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLong64(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLong32(IntPtr hWnd, int nIndex);

        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return new IntPtr(GetClassLong32(hWnd, nIndex));
            }
            return GetClassLong64(hWnd, nIndex);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        #region Clipboard
        /// <summary>
        ///     Add a window as a clipboard format listener
        ///     See
        ///     <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms649033(v=vs.85).aspx">
        ///         AddClipboardFormatListener
        ///         function
        ///     </a>
        /// </summary>
        /// <param name="hWnd">IntPtr for the window to handle the messages</param>
        /// <returns>true if it worked, false if not; call GetLastError to see what was the problem</returns>
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr hWnd);
        
        /// <summary>
        ///     Remove a window as a clipboard format listener
        ///     See
        ///     <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms649050(v=vs.85).aspx">
        ///         RemoveClipboardFormatListener
        ///         function
        ///     </a>
        /// </summary>
        /// <param name="hWnd">IntPtr for the window to handle the messages</param>
        /// <returns>true if it worked, false if not; call GetLastError to see what was the problem</returns>
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

        /// <summary>
        /// Delegate for the EnumChildWindows method
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="parameter">Caller-defined variable; we use it for a pointer to our list</param>
        /// <returns>True to continue enumerating, false to bail.</returns>
        public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsClipboardFormatAvailable(uint format);

        /// <summary>
        /// The RegisterClipboardFormat function registers a new clipboard format. This format can then be used as a valid clipboard format
        /// </summary>
        /// <param name="lpszFormat">Pointer to a null-terminated string that names the new format.</param>
        /// <returns>If the function succeeds, the return value identifies the registered clipboard format. 
        /// If the function fails, the return value is zero </returns>
        public static uint RegisterClipboardFormat([In] string lpszFormat)
        {
            // http://www.clipboardextender.com/developing-clipboard-aware-programs-for-windows/ignoring-clipboard-updates-with-the-cf_clipboard_viewer_ignore-clipboard-format
            return Environment.OSVersion.Platform >= PlatformID.Win32NT ? RegisterClipboardFormatW(lpszFormat) : RegisterClipboardFormatA(lpszFormat);
        }

        [DllImport("user32.dll", EntryPoint = "RegisterClipboardFormatA", SetLastError = true)]
        private static extern uint RegisterClipboardFormatA([In] [MarshalAs(UnmanagedType.LPStr)] string lpszFormat);

        [DllImport("user32.dll", EntryPoint = "RegisterClipboardFormatW", SetLastError = true)]
        private static extern uint RegisterClipboardFormatW([In] [MarshalAs(UnmanagedType.LPWStr)] string lpszFormat);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int IsClipboardFormatAvailable(int wFormat);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetClipboardData(int wFormat);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EmptyClipboard();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool CloseClipboard();

        [DllImport("user32.dll")]
        static public extern IntPtr GetOpenClipboardWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardOwner();

        [DllImport("user32.dll", EntryPoint = "GetClipboardSequenceNumber", SetLastError = true)]
        public static extern uint GetClipboardSequenceNumber();

        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(uint uFormat);


        [DllImport("user32.dll")]
        public static extern int GetClipboardFormatName(
            uint format,
            [Out] StringBuilder lpszFormatName,
            int cchMaxCount);

        [DllImport("user32.dll")]
        public static extern uint EnumClipboardFormats(uint format);
        #endregion

        #region Windows
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        #endregion

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CopyImage(IntPtr hImage, uint uType, int cxDesired, int cyDesired, uint fuFlags);

        public static string GetWindowTitle(IntPtr windowHandle)
        {
            const int numberOfCharacters = 512;
            var buffer = new StringBuilder(numberOfCharacters);

            if (GetWindowText(windowHandle, buffer, numberOfCharacters) > 0)
            {
                return buffer.ToString();
            }
            return null;
        }

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hwnd);
    }
}