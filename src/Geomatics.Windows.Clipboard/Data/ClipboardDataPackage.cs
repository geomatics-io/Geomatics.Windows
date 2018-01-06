using System;
using System.Collections.Generic;
using System.Linq;
using Geomatics.Windows.Clipboard.Data.Interfaces;
using Geomatics.Windows.Clipboard.Services.Clipboard;
using Geomatics.Windows.Collections.Generic;
using Geomatics.Windows.Extensions.System.ByteArray;
using PInvoke;

namespace Geomatics.Windows.Clipboard.Data
{
    [Serializable]
    public class ClipboardDataPackage : IClipboardDataPackage
    {
        /// <summary>
        /// This class can only be instantiated when there is a clipboard lock, that is why the constructor is private.
        /// </summary>
        private ClipboardDataPackage()
        {
          
        }

        /// <summary>
        /// Sequence-number of the clipboard, starts at 0 when the Windows session starts
        /// </summary>
        public uint Id { get; } = User32.GetClipboardSequenceNumber();
        public Guid UUID { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The formats in this clipboard contents
        /// </summary>
        public OrderedDictionary<uint, string> Formats { get; } = ClipboardNative.AvailableFormats();

        /// <summary>
        /// The handle of the window which owns the clipboard content
        /// </summary>
        public IntPtr OwnerHandle { get; } = User32.GetClipboardOwner();

        public DataSource DataSource { get; set; } = DataSourceService.GetDataSource();

        /// <summary>
        /// Timestamp of the clipboard update event, this value will not be correct for the first event
        /// </summary>
        public DateTimeOffset Created { get; set; } = DateTime.Now;
        public List<ClipboardData> Contents { get; set; }
        
        public bool IsEncrypted { get; set; } = false;
        public string MD5
        {
            get
            {
                byte[][] arrays = Contents.Select(cd => cd.MD5).ToArray();
                return BitConverter.ToString(Geomatics.Windows.Extensions.System.ByteArray.Utils.Combine(arrays).GetMd5Hash()).Replace("-", string.Empty);
            }
        }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="hWnd">IntPtr, optional, with the hWnd for the clipboard lock</param>
        /// <returns>ClipboardUpdateInformation</returns>
        public static ClipboardDataPackage Create(IntPtr hWnd = default(IntPtr))
        {
            if (hWnd == IntPtr.Zero)
            {
                hWnd = WinProcHandler.Instance.Handle;
            }
            using (ClipboardNative.Lock(hWnd))
            {
                return new ClipboardDataPackage();
            }
        }
        
    }
}