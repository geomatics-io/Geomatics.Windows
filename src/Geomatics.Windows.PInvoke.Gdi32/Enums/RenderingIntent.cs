using System;

namespace PInvoke
{
    /// <summary>
    /// Rendering intent for bitmap
    /// </summary>
    [Flags]
    public enum RenderingIntent
    {
        /// <summary>
        /// Maintains the white point. Matches the colors to their 
        /// nearest color in the destination gamut.
        /// </summary>
        LCS_GM_ABS_COLORIMETRIC = 8,
        /// <summary>
        /// Maintains saturation. Used for business charts and other
        /// situations in which undithered colors are required.
        /// </summary>
        LCS_GM_BUSINESS = 1,
        /// <summary>
        /// Maintains colorimetric match. Used for graphic designs and named colors.
        /// </summary>
        LCS_GM_GRAPHICS = 2,
        /// <summary>
        /// Maintains contrast. Used for photographs and natural images
        /// </summary>
        LCS_GM_IMAGES = 4
    }
}