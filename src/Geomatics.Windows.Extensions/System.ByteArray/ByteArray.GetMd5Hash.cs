using System;

namespace Geomatics.Windows.Extensions.System.ByteArray
{
    public static partial class Extensions
    {

        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }
}