using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Geomatics.Windows.Clipboard.Data;
using Geomatics.Windows.Collections.Generic;
using PInvoke;

namespace Geomatics.Windows.Clipboard
{
    internal struct WINDOWINFO
    {
        public uint ownerpid;
        public uint childpid;
    }

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
        public static Geomatics.Windows.Collections.Generic.OrderedDictionary<uint, string> AvailableFormats()
        {
            var dict = new Geomatics.Windows.Collections.Generic.OrderedDictionary<uint, string>();

            var lastRetrievedFormat = 0u;

            do
            {
                lastRetrievedFormat = User32.EnumClipboardFormats(lastRetrievedFormat);

                if (lastRetrievedFormat == 0)
                {
                    // If GetLastWin32Error return SuccessError, this is the end
                    if (Marshal.GetLastWin32Error() == SuccessError)
                    {
                        return dict;
                    }
                    // GetLastWin32Error didn't return SuccessError, so throw exception
                    throw new Win32Exception();
                }

                string formatName;
                if (Id2Format.TryGetValue(lastRetrievedFormat, out formatName))
                {
                    dict.Add(lastRetrievedFormat, formatName);
                    continue;
                }
                
                if (GetClipboardFormatName(lastRetrievedFormat, out formatName))
                {
                    dict.Add(lastRetrievedFormat, formatName);
                    Id2Format[lastRetrievedFormat] = formatName;
                    Format2Id[formatName] = lastRetrievedFormat;
                }

            } while (0 != lastRetrievedFormat);

            return dict;
        }

        public static bool GetClipboardFormatName(uint format, out string formatName)
        {
            formatName = null;

            if (Enum.IsDefined(typeof(User32.StandardClipboardFormat), format))
            {
                formatName = ((User32.StandardClipboardFormat) format).ToString();
                return true;
            }

            var sb = new StringBuilder(512);
            if (User32.GetClipboardFormatName(format, sb, sb.Capacity) <= 0)
                return false;
            
            formatName = sb.ToString();

            return true;
        }

        /// <summary>
        /// Returns a list of child windows
        /// </summary>
        /// <param name="parent">Parent of the windows to return</param>
        /// <returns>List of child windows</returns>
        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                User32.EnumWindowProc childProc = new User32.EnumWindowProc(EnumWindow);
                User32.EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        /// <summary>
        /// Callback method to be used when enumerating windows.
        /// </summary>
        /// <param name="handle">Handle of the next window</param>
        /// <param name="pointer">Pointer to a GCHandle that holds a reference to the list to fill</param>
        /// <returns>True to continue the enumeration, false to bail</returns>
        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            //  You can modify this to check to see if you want to cancel the operation, then return a null here
            return true;
        }

        /// <summary>
        /// Gets the name of the process.
        /// </summary>
        /// <param name="hWnd">hWnd</param>
        /// <returns>Name of the process.</returns>
        public static string GetProcessName(IntPtr hWnd)
        {
            string processName = null;

            hWnd = User32.GetForegroundWindow();

            if (hWnd == IntPtr.Zero)
                return null;

            uint pID;
            User32.GetWindowThreadProcessId(hWnd, out pID);

            IntPtr proc;
            if ((proc = Kernel32.OpenProcess(Kernel32.PROCESS_QUERY_INFORMATION | Kernel32.PROCESS_VM_READ, false, (int)pID)) == IntPtr.Zero)
                return null;

            int capacity = 2000;
            StringBuilder sb = new StringBuilder(capacity);
            Kernel32.QueryFullProcessImageName(proc, 0, sb, ref capacity);

            processName = sb.ToString(0, capacity);

            // UWP apps are wrapped in another app called, if this has focus then try and find the child UWP process
            if (Path.GetFileName(processName).Equals("ApplicationFrameHost.exe"))
            {
                processName = UWP_AppName(hWnd, pID);
            }

            return processName;
        }

        #region Get UWP Application Name

        /// <summary>
        /// Find child process for uwp apps, edge, mail, etc.
        /// </summary>
        /// <param name="hWnd">hWnd</param>
        /// <param name="pID">pID</param>
        /// <returns>The application name of the UWP.</returns>
        private static string UWP_AppName(IntPtr hWnd, uint pID)
        {
            WINDOWINFO windowinfo = new WINDOWINFO();
            windowinfo.ownerpid = pID;
            windowinfo.childpid = windowinfo.ownerpid;

            IntPtr pWindowinfo = Marshal.AllocHGlobal(Marshal.SizeOf(windowinfo));

            Marshal.StructureToPtr(windowinfo, pWindowinfo, false);

            User32.EnumWindowProc lpEnumFunc = new User32.EnumWindowProc(EnumChildWindowsCallback);
            User32.EnumChildWindows(hWnd, lpEnumFunc, pWindowinfo);

            windowinfo = (WINDOWINFO)Marshal.PtrToStructure(pWindowinfo, typeof(WINDOWINFO));

            IntPtr proc;
            if ((proc = Kernel32.OpenProcess(Kernel32.PROCESS_QUERY_INFORMATION | Kernel32.PROCESS_VM_READ, false, (int)windowinfo.childpid)) == IntPtr.Zero)
                return null;

            int capacity = 2000;
            StringBuilder sb = new StringBuilder(capacity);
            Kernel32.QueryFullProcessImageName(proc, 0, sb, ref capacity);

            Marshal.FreeHGlobal(pWindowinfo);

            return sb.ToString(0, capacity);
        }

        /// <summary>
        /// Callback for enumerating the child windows.
        /// </summary>
        /// <param name="hWnd">hWnd</param>
        /// <param name="lParam">lParam</param>
        /// <returns>always <c>true</c>.</returns>
        private static bool EnumChildWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            WINDOWINFO info = (WINDOWINFO)Marshal.PtrToStructure(lParam, typeof(WINDOWINFO));

            uint pID;
            User32.GetWindowThreadProcessId(hWnd, out pID);

            if (pID != info.ownerpid)
                info.childpid = pID;

            Marshal.StructureToPtr(info, lParam, true);

            return true;
        }
        #endregion
        
    }
}