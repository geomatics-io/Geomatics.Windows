using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Interop;
using Common.Logging;
using PInvoke;

namespace Geomatics.Windows
{
    /// <summary>
    ///     This can be used to handle WinProc messages, for instance when there is no running winproc
    /// </summary>
    public class WinProcHandler
    {
        private static readonly ILog Log = LogManager.GetLogger<WinProcHandler>();

        private static readonly object Lock = new object();
        private static HwndSource _hwndSource;

        /// <summary>
        ///     Hold the singeton
        /// </summary>
        private static readonly Lazy<WinProcHandler> Singleton = new Lazy<WinProcHandler>(() => new WinProcHandler());

        /// <summary>
        ///     Store hooks, so they can be removed
        /// </summary>
        private readonly IList<HwndSourceHook> _hooks = new List<HwndSourceHook>();

        /// <summary>
        ///     Special HwndSource which is only there for handling messages, is top-level (no parent) to be able to handle ALL windows messages
        /// </summary>
        [SuppressMessage("Sonar Code Smell", "S2696:Instance members should not write to static fields", Justification = "Instance member needs access to the _hooks, this is checked!")]
        private HwndSource MessageHandlerWindow
        {
            get
            {
                // Special code to make sure the _hwndSource is (re)created when it's not yet there or disposed
                // For example in xunit tests when WpfFact is used, the _hwndSource is disposed.
                if (_hwndSource != null && !_hwndSource.IsDisposed)
                {
                    return _hwndSource;
                }
                lock (Lock)
                {
                    if (_hwndSource != null && !_hwndSource.IsDisposed)
                    {
                        return _hwndSource;
                    }
                    // Create a new message window
                    _hwndSource = CreateMessageWindow();
                    // Hook automatic removing of all the hooks
                    _hwndSource.AddHook((IntPtr hwnd, int msg, IntPtr param, IntPtr lParam, ref bool handled) =>
                    {
                        var windowsMessage = (User32.WindowMessage)msg;
                        if (windowsMessage != User32.WindowMessage.WM_NCDESTROY)
                        {
                            return IntPtr.Zero;
                        }
                        Log.Info($@"Message window with handle {_hwndSource.Handle} is destroyed, removing all hooks.");
                        // The hooks are no longer valid, either there is no _hwndSource or it was disposed.
                        lock (Lock)
                        {
                            _hooks.Clear();
                        }
                        return IntPtr.Zero;
                    });
                }
                return _hwndSource;
            }
        }

        /// <summary>
        ///     The actual handle for the HwndSource
        /// </summary>
        public IntPtr Handle => MessageHandlerWindow.Handle;

        /// <summary>
        ///     Singleton instance of the WinProcHandler
        /// </summary>
        public static WinProcHandler Instance => Singleton.Value;

        /// <summary>
        ///     Subscribe a hook to handle messages
        /// </summary>
        /// <param name="hook">HwndSourceHook</param>
        /// <returns>IDisposable which unsubscribes the hook when Dispose is called</returns>
        public IDisposable Subscribe(HwndSourceHook hook)
        {
            lock (Lock)
            {
                if (_hooks.Contains(hook))
                {
                    Log.Info("Ignoring duplicate hook.");
                    return Disposable.Empty;
                }
                Log.Info("Adding a hook to handle messages.");

                MessageHandlerWindow.AddHook(hook);
                _hooks.Add(hook);
            }
            return Disposable.Create(() => { Unsubscribe(hook); });
        }

        /// <summary>
        ///     Unsubscribe a hook
        /// </summary>
        /// <param name="hook">HwndSourceHook</param>
        private void Unsubscribe(HwndSourceHook hook)
        {
            lock (Lock)
            {
                Log.Info("Removing a hook to handle messages.");
                MessageHandlerWindow.RemoveHook(hook);
                _hooks.Remove(hook);
            }
        }

        /// <summary>
        ///     Unsubscribe all current hooks
        /// </summary>
        public void UnsubscribeAllHooks()
        {
            foreach (var hwndSourceHook in _hooks.ToList())
            {
                Unsubscribe(hwndSourceHook);
            }
        }

        /// <summary>
        /// Creates a HwndSource to catch windows message
        /// </summary>
        /// <param name="parent">IntPtr for the parent, this should usually not be set</param>
        /// <param name="title">Title of the window, a default is already set</param>
        /// <returns>HwndSource</returns>
        public static HwndSource CreateMessageWindow(IntPtr parent = default(IntPtr), string title = "Dapplo.MessageHandlerWindow")
        {
            return new HwndSource(new HwndSourceParameters
            {
                ParentWindow = parent,
                Width = 0,
                Height = 0,
                PositionX = 0,
                PositionY = 0,
                AcquireHwndFocusInMenuMode = false,
                ExtendedWindowStyle = 0, // ExtendedWindowStyleFlags.WS_NONE
                WindowStyle = 0, // WindowStyleFlags.WS_OVERLAPPED
                WindowClassStyle = 0,
                WindowName = title
            });
        }
    }
}