using System;
using System.Runtime.InteropServices;

namespace PInvoke
{
    public static partial class Kernel32
    {
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, int size);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern UIntPtr GlobalSize(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern bool WriteFile(IntPtr hFile, IntPtr lpBuffer, int NumberOfBytesToWrite, out int lpNumberOfBytesWritten, IntPtr lpOverlapped);
        
    }
}
