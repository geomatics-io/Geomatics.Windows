using System.Linq;

namespace Geomatics.Windows.Extensions.System.ByteArray
{
    public static partial class Utils
    {

        /// <summary>
        /// Combines the specified arrays.
        /// </summary>
        /// <param name="arrays">The arrays.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                global::System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}