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
        /// <param name="intPtr">Pointer to the structor to return</param>
        /// <returns>struct</returns>
        public static T FromIntPtr<T>(IntPtr intPtr) where T : struct
        {
            object obj = Marshal.PtrToStructure(intPtr, typeof(T));
            return (T)obj;
        }
    }
}