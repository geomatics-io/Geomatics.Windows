using System;

namespace PInvoke
{
    /// <summary>
    /// Specifies the color space of the DIB
    /// </summary>
    [Flags]
    public enum LogicalColorSpace : uint
    {
        /// <summary>
        /// This value implies that endpoints and gamma values are given in the appropriate fields
        /// </summary>
        LCS_CALIBRATED_RGB = 0,
        /// <summary>
        /// Specifies that the bitmap is in sRGB color space
        /// </summary>
        LCS_SRGB = 1,
        /// <summary>
        /// This value indicates that the bitmap is in the system default color space, sRGB
        /// </summary>
        LCS_WINDOWS_COLOR_SPACE = 2,
        /// <summary>
        /// This value indicates that bV5ProfileData points to the file name of the profile to
        /// use (gamma and endpoints values are ignored). 
        /// </summary>
        PROFILE_LINKED = 3,
        /// <summary>
        /// This value indicates that bV5ProfileData points to a memory buffer that 
        /// contains the profile to be used (gamma and endpoints values are ignored).
        /// </summary>
        PROFILE_EMBEDDED = 4
    }
}