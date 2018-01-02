using System;
using System.Runtime.InteropServices;

namespace PInvoke
{
    public static partial class Shell32
    {
        /// <summary>
        /// This structure will contain information about the file
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            // Handle to the icon representing the file
            public IntPtr hIcon;
            // Index of the icon within the image list
            public IntPtr iIcon;
            // Various attributes of the file
            public uint dwAttributes;
            // Path to the file
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            // File type
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
    }
}