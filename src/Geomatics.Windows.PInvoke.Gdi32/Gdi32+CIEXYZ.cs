using System.Runtime.InteropServices;

namespace PInvoke
{
    /// <summary>
    /// The CIEXYZ structure contains the x, y, and z coordinates 
    /// of a specific color in a specified color space. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CIEXYZ
    {
        /// <summary>
        /// The x coordinate in fix point (2.30)
        /// </summary>
        public int ciexyzX;

        /// <summary>
        /// The y coordinate in fix point (2.30). 
        /// </summary>
        public int ciexyzY;

        /// <summary>
        /// The z coordinate in fix point (2.30). 
        /// </summary>
        public int ciexyzZ;
    }
}