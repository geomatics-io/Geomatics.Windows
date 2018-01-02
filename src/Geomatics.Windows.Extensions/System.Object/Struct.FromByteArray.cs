using System;
using System.Runtime.InteropServices;

namespace Geomatics.Windows.Extensions.System.Object
{
    public static partial class Extensions
    {
        /// <summary>
        /// Get a struct from a byte array
        /// </summary>
        /// <typeparam name="T">typeof struct</typeparam>
        /// <param name="bytes">byte[]</param>
        /// <returns>struct</returns>
        public static T FromByteArray<T>(byte[] bytes) where T : struct
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(typeof(T));
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(bytes, 0, ptr, size);
                return FromIntPtr<T>(ptr);
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }
    }
}