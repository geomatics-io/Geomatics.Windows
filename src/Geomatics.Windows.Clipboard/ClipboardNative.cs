using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using PInvoke;

namespace Geomatics.Windows.Clipboard
{
    public class ClipboardNative
    {
        private const int SuccessError = 0;

        // "Global" clipboard lock
        private static readonly ClipboardSemaphore ClipboardLock = new ClipboardSemaphore();

        // Cache for all the known clipboard format names
        private static readonly IDictionary<uint, string> Id2Format = new Dictionary<uint, string>();
        private static readonly IDictionary<string, uint> Format2Id = new Dictionary<string, uint>();

        /// <summary>
        /// Initialize the static data of the class
        /// </summary>
        static ClipboardNative()
        {
            // Create an entry for every enum element which has a Display attribute
            foreach (var enumValue in typeof(User32.StandardClipboardFormat).GetEnumValues())
            {
                uint id = (uint)enumValue;
                var displayAttribute = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault()?.GetCustomAttributes<DisplayAttribute>()?.FirstOrDefault();
                var formatName = displayAttribute != null ? displayAttribute.Name : enumValue.ToString();
                Format2Id[formatName] = id;
                Id2Format[id] = formatName;
            }
        }

        /// <summary>
        /// Get a global lock to the clipboard
        /// </summary>
        /// <param name="hWnd">IntPtr with the windows handle</param>
        /// <param name="retries">int with the amount of lock attempts are made</param>
        /// <param name="retryInterval">Timespan between retries, default 200ms</param>
        /// <param name="timeout">Timeout for getting the lock</param>
        /// <returns>IDisposable, which will unlock when Dispose is called</returns>
        public static IDisposable Lock(IntPtr hWnd = default(IntPtr), int retries = 5, TimeSpan? retryInterval = null, TimeSpan? timeout = null)
        {
            return ClipboardLock.Lock(hWnd, retries, retryInterval, timeout);
        }

        /// <summary>
        ///     Enumerate through all formats on the clipboard, assumes the clipboard was already locked.
        /// </summary>
        /// <returns>IEnumerable with strings defining the format</returns>
        public static IEnumerable<string> AvailableFormats()
        {
            uint clipboardFormatId = 0;
            var clipboardFormatName = new StringBuilder(256);
            while (true)
            {
                clipboardFormatId = User32.EnumClipboardFormats(clipboardFormatId);
                if (clipboardFormatId == 0)
                {
                    // If GetLastWin32Error return SuccessError, this is the end
                    if (Marshal.GetLastWin32Error() == SuccessError)
                    {
                        yield break;
                    }
                    // GetLastWin32Error didn't return SuccessError, so throw exception
                    throw new Win32Exception();
                }
                string formatName;
                clipboardFormatName.Length = 0;
                if (Id2Format.TryGetValue(clipboardFormatId, out formatName))
                {
                    yield return formatName;
                    continue;
                }
                if (User32.GetClipboardFormatName(clipboardFormatId, clipboardFormatName, clipboardFormatName.Capacity) <= 0)
                {
                    // No name
                    continue;
                }
                formatName = clipboardFormatName.ToString();
                Id2Format[clipboardFormatId] = formatName;
                Format2Id[formatName] = clipboardFormatId;
                yield return clipboardFormatName.ToString();
            }
        }
    }
}