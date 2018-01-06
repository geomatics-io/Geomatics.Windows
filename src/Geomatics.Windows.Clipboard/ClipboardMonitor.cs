using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Interop;
using Geomatics.Windows.Clipboard.Data;
using PInvoke;

namespace Geomatics.Windows.Clipboard
{
    /// <summary>
    /// A monitor for clipboard changes
    /// <remarks>Original code and idea taken from https://raw.githubusercontent.com/dapplo/Dapplo.Windows/master/src/Dapplo.Windows.Clipboard/ClipboardMonitor.cs
    /// </remarks>
    /// </summary>
    public class ClipboardMonitor
    {
        /// <summary>
        ///     The singleton of the KeyboardHook
        /// </summary>
        private static readonly Lazy<ClipboardMonitor> Singleton = new Lazy<ClipboardMonitor>(() => new ClipboardMonitor());

        /// <summary>
        ///     Used to store the observable
        /// </summary>
        private readonly IObservable<ClipboardDataPackage> _clipboardObservable;

        // This maintains the sequence
        private uint _previousSequence = uint.MinValue;

        /// <summary>
        ///     Private constructor to create the observable
        /// </summary>
        private ClipboardMonitor()
        {
            _clipboardObservable = Observable.Create<ClipboardDataPackage>(observer =>
            {
                // This handles the message
                HwndSourceHook winProcHandler = (IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
                {
                    var windowsMessage = (User32.WindowMessage)msg;
                    if (windowsMessage != User32.WindowMessage.WM_CLIPBOARDUPDATE)
                    {
                        return IntPtr.Zero;
                    }
                    
                    var clipboardDataPackage = ClipboardDataPackage.Create(hwnd);

                    // Check if private clipboard contents should be handled or not
                    // http://www.clipboardextender.com/developing-clipboard-aware-programs-for-windows/ignoring-clipboard-updates-with-the-cf_clipboard_viewer_ignore-clipboard-format
                    if (!BypassClipboardIgnoreFlag && (clipboardDataPackage.Formats.Values.Contains("CF_CLIPBOARD_VIEWER_IGNORE") || clipboardDataPackage.Formats.Values.Contains("Clipboard Viewer Ignore")))
                        return IntPtr.Zero;

                    // Make sure we don't trigger multiple times, this happend while developing.
                    if (clipboardDataPackage.Id > _previousSequence)
                    {
                        _previousSequence = clipboardDataPackage.Id;
                        observer.OnNext(clipboardDataPackage);
                    }

                    return IntPtr.Zero;
                };

                var hookSubscription = WinProcHandler.Instance.Subscribe(winProcHandler);
                if (!User32.AddClipboardFormatListener(WinProcHandler.Instance.Handle))
                {
                    observer.OnError(new Win32Exception());
                }
                else
                {
                    // Make sure the current contents are always published
                    observer.OnNext(ClipboardDataPackage.Create());
                }

                return Disposable.Create(() =>
                {
                    User32.RemoveClipboardFormatListener(WinProcHandler.Instance.Handle);
                    hookSubscription.Dispose();
                });
            })
                // Make sure there is always a value produced when connecting
                .Publish()
                .RefCount();
        }

        /// <summary>
        /// Do not handle clipboard contents if it contains CF_CLIPBOARD_VIEWER_IGNORE or Clipboard Viewer Ignore format.
        /// </summary>
        /// <value><c>true</c> if [handle clipboard ignore]; otherwise, <c>false</c>.</value>
        public bool BypassClipboardIgnoreFlag { get; set; } = false;

        /// <summary>
        ///     This observable publishes the current clipboard contents after every paste action.
        ///     Best to use SubscribeOn with the UI SynchronizationContext.
        /// </summary>
        public static IObservable<ClipboardDataPackage> OnUpdate => Singleton.Value._clipboardObservable;
    }
}