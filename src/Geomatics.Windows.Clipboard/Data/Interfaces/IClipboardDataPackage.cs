using System;
using System.Collections.Generic;
using Geomatics.Windows.Collections.Generic;

namespace Geomatics.Windows.Clipboard.Data.Interfaces
{
    public interface IClipboardDataPackage
    {
        /// <summary>
        /// Sequence-number of the clipboard, starts at 0 when the Windows session starts
        /// </summary>
        uint Id { get; }

        Guid UUID { get; set; }

        /// <summary>
        /// The formats in this clipboard contents
        /// </summary>
        OrderedDictionary<uint, string> Formats { get; }

        /// <summary>
        /// The handle of the window which owns the clipboard content
        /// </summary>
        IntPtr OwnerHandle { get; }

        /// <summary>
        /// Timestamp of the clipboard update event, this value will not be correct for the first event
        /// </summary>
        DateTimeOffset Created { get; set; }

        List<ClipboardData> Contents { get; set; }
        bool IsEncrypted { get; set; }
        string MD5 { get; }
    }
}