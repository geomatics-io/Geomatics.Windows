﻿using System.Text;

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
        public static extern bool AddClipboardFormatListener(IntPtr hWnd);
        
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
        public static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

        /// <summary>
        /// Delegate for the EnumChildWindows method
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="parameter">Caller-defined variable; we use it for a pointer to our list</param>
        /// <returns>True to continue enumerating, false to bail.</returns>
        public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        /// <summary>
        /// Determines whether the clipboard contains data in the specified format.
        /// </summary>
        /// <param name="format">uint for the format</param>
        /// <returns>bool</returns>
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsClipboardFormatAvailable(uint format);

        /// <summary>
        /// Registers a new clipboard format. This format can then be used as a valid clipboard format.
        /// 
        /// If a registered format with the specified name already exists, a new format is not registered and the return value identifies the existing format. This enables more than one application to copy and paste data using the same registered clipboard format. Note that the format name comparison is case-insensitive.
        /// Registered clipboard formats are identified by values in the range 0xC000 through 0xFFFF.
        /// When registered clipboard formats are placed on or retrieved from the clipboard, they must be in the form of an HGLOBAL value.
        /// </summary>
        /// <param name="lpszFormat">The name of the new format.</param>
        /// <returns>
        /// If the function succeeds, the return value identifies the registered clipboard format.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
//        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
//        internal static extern uint RegisterClipboardFormat(string lpszFormat);

        /// <summary>
        /// The RegisterClipboardFormat function registers a new clipboard format. This format can then be used as a valid clipboard format
        /// </summary>
        /// <param name="lpszFormat">Pointer to a null-terminated string that names the new format.</param>
        /// <returns>If the function succeeds, the return value identifies the registered clipboard format. 
        /// If the function fails, the return value is zero </returns>
        public static uint RegisterClipboardFormat([In] string lpszFormat)
        {
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

        /// <summary>
        /// Empties the clipboard and frees handles to data in the clipboard. The function then assigns ownership of the clipboard to the window that currently has the clipboard open.
        /// </summary>
        /// <returns>bool</returns>
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EmptyClipboard();

        /// <summary>
        /// Places data on the clipboard in a specified clipboard format.
        /// The window must be the current clipboard owner, and the application must have called the OpenClipboard function.
        /// (When responding to the WM_RENDERFORMAT and WM_RENDERALLFORMATS messages, the clipboard owner must not call OpenClipboard before calling SetClipboardData.)
        /// </summary>
        /// <param name="format">uint</param>
        /// <param name="memory">IntPtr to the memory area</param>
        /// <returns></returns>
        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr SetClipboardData(uint format, IntPtr memory);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        public static extern IntPtr GetOpenClipboardWindow();

        /// <summary>
        /// Returns the hWnd of the owner of the clipboard content
        /// </summary>
        /// <returns>IntPtr with a hWnd</returns>
        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetClipboardOwner();

        /// <summary>
        /// Retrieves the sequence number of the clipboard
        /// </summary>
        /// <returns>sequence number or 0 if this cannot be retrieved</returns>
        [DllImport("user32.dll", EntryPoint = "GetClipboardSequenceNumber", SetLastError = true)]
        public static extern uint GetClipboardSequenceNumber();

        /// <summary>
        /// Retrieves the names of dropped files that result from a successful drag-and-drop operation.
        /// </summary>
        /// <param name="hDrop">Identifier of the structure that contains the file names of the dropped files.</param>
        /// <param name="iFile">Index of the file to query. If the value of this parameter is 0xFFFFFFFF, DragQueryFile returns a count of the files dropped. If the value of this parameter is between zero and the total number of files dropped, DragQueryFile copies the file name with the corresponding value to the buffer pointed to by the lpszFile parameter.</param>
        /// <param name="lpszFile">The address of a buffer that receives the file name of a dropped file when the function returns. This file name is a null-terminated string. If this parameter is NULL, DragQueryFile returns the required size, in characters, of this buffer.</param>
        /// <param name="cch">The size, in characters, of the lpszFile buffer.</param>
        /// <returns>
        /// A nonzero value indicates a successful call.
        /// When the function copies a file name to the buffer, the return value is a count of the characters copied, not including the terminating null character.
        /// If the index value is 0xFFFFFFFF, the return value is a count of the dropped files. Note that the index variable itself returns unchanged, and therefore remains 0xFFFFFFFF.
        /// If the index value is between zero and the total number of dropped files, and the lpszFile buffer address is NULL, the return value is the required size, in characters, of the buffer, not including the terminating null character.
        /// </returns>
        [DllImport("shell32")]
        public static extern uint DragQueryFile(IntPtr hDrop, uint iFile, [Out] StringBuilder lpszFile, int cch);
    
    /// <summary>
    /// Retrieves data from the clipboard in a specified format. The clipboard must have been opened previously.
    /// </summary>
    /// <param name="format">uint with the clipboard format.</param>
    /// <returns>IntPtr with a handle to the memory</returns>
    [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetClipboardData(uint format);

        /// <summary>
        ///     See
        ///     <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms649040(v=vs.85).aspx">GetClipboardFormatName function</a>
        ///     Retrieves from the clipboard the name of the specified registered format.
        ///     The function copies the name to the specified buffer.
        /// </summary>
        /// <param name="format">uint with the id of the format</param>
        /// <param name="lpszFormatName">Name of the format</param>
        /// <param name="cchMaxCount">Maximum size of the output</param>
        /// <returns></returns>
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetClipboardFormatName(uint format, [Out] StringBuilder lpszFormatName, int cchMaxCount);
        

        /// <summary>
        ///     See
        ///     <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms649038(v=vs.85).aspx">EnumClipboardFormats function</a>
        ///     Enumerates the data formats currently available on the clipboard.
        ///     Clipboard data formats are stored in an ordered list. To perform an enumeration of clipboard data formats, you make
        ///     a series of calls to the EnumClipboardFormats function. For each call, the format parameter specifies an available
        ///     clipboard format, and the function returns the next available clipboard format.
        /// </summary>
        /// <param name="format">
        ///     To start an enumeration of clipboard formats, set format to zero. When format is zero, the
        ///     function retrieves the first available clipboard format. For subsequent calls during an enumeration, set format to
        ///     the result of the previous EnumClipboardFormats call.
        /// </param>
        /// <returns>If the function succeeds, the return value is the clipboard format that follows the specified format, namely the next available clipboard format.
        ///     If the function fails, the return value is zero. To get extended error information, call GetLastError. If the clipboard is not open, the function fails.
        ///     If there are no more clipboard formats to enumerate, the return value is zero. In this case, the GetLastError function returns the value ERROR_SUCCESS.
        ///     This lets you distinguish between function failure and the end of enumeration.
        /// </returns>
        [DllImport("user32", SetLastError = true)]
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