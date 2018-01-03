using System;

namespace Geomatics.Windows.Clipboard.Data.Interfaces
{
    public interface IClipboardData
    {
        byte[] MD5 { get; set; }
        Guid UUID { get; set; }

        /// <summary>
        /// Get or Set the format code of the data 
        /// </summary>
        uint Format { get; set; }

        /// <summary>
        /// Get or Set the format name of the data 
        /// </summary>
        string FormatName { get; set; }

        /// <summary>
        /// Get or Set the buffer data
        /// </summary>
        byte[] Buffer { get; set; }

        /// <summary>
        /// Get the data buffer lenght
        /// </summary>
        UIntPtr Size { get; }
    }
}