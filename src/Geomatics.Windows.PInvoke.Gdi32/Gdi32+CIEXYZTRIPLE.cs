using System.Runtime.InteropServices;

namespace PInvoke
{
    /// <summary>
    /// The CIEXYZTRIPLE structure contains the x, y, and z coordinates
    /// of the three colors that correspond to the red, green, and blue
    /// endpoints for a specified logical color space
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CIEXYZTRIPLE
    {
        /// <summary>
        /// The xyz coordinates of red endpoint. 
        /// </summary>
        public CIEXYZ ciexyzRed;

        /// <summary>
        /// The xyz coordinates of green endpoint. 
        /// </summary>
        public CIEXYZ ciexyzGreen;

        /// <summary>
        /// The xyz coordinates of blue endpoint. 
        /// </summary>
        public CIEXYZ ciexyzBlue;
    }
}