using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using PInvoke;

namespace Geomatics.Windows.Clipboard
{
    /// <summary>
    /// Hidden form to receive the WM_CLIPBOARDUPDATE message.
    /// </summary>
    [Obsolete("Not used anymore! Will be removed.", true)]
    [DefaultEvent("ClipboardChanged")]
    [global::System.ComponentModel.DesignerCategory("")]
    public class ClipboardWatcher : Form
    {
        // static instance of this form
        private static ClipboardWatcher mInstance;

        // needed to dispose this form
        private static IntPtr nextClipboardViewer;
       

        internal static IntPtr HWND => mInstance.Handle;

        /// <summary>
        /// Clipboard contents changed.
        /// </summary>
        public static event EventHandler ClipboardChanged;

        static ClipboardWatcher()
        {
           
        }
        
        // start listening
        public static void StartWatching()
        {
            // we can only have one instance if this class
            if (mInstance != null)
                return;

            var t = new Thread(new ParameterizedThreadStart(x => Application.Run(new ClipboardWatcher())));
            t.SetApartmentState(ApartmentState.STA); // give the [STAThread] attribute
            t.Start();
        }

        // stop listening (dispose form)
        public static void StopWatching()
        {
            if (mInstance == null)
                return;

            mInstance.Invoke(new MethodInvoker(() =>
            {
                
                User32.ChangeClipboardChain(mInstance.Handle, nextClipboardViewer);
            }));
            mInstance.Invoke(new MethodInvoker(mInstance.Close));

            mInstance.Dispose();

            mInstance = null;
        }

        public static void PauseWatching()
        {
            if (mInstance == null)
                return;

            mInstance.Invoke(new MethodInvoker(() =>
            {
                User32.ChangeClipboardChain(mInstance.Handle, nextClipboardViewer);
            }));
        }

        public static void ResumeWatching()
        {
            if (mInstance == null)
                return;

            mInstance.Invoke(new MethodInvoker(() =>
            {
                nextClipboardViewer = User32.SetClipboardViewer(mInstance.Handle);
            }));
        }

        // on load: (hide this window)
        protected override void SetVisibleCore(bool value)
        {
            CreateHandle();

            mInstance = this;

            nextClipboardViewer = User32.SetClipboardViewer(mInstance.Handle);

            base.SetVisibleCore(false);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int) User32.WindowMessage.WM_DRAWCLIPBOARD:
                    OnClipboardChanged();
                    User32.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case (int)User32.WindowMessage.WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        User32.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    User32.ChangeClipboardChain(this.Handle, nextClipboardViewer);
                }));
            }
            else User32.ChangeClipboardChain(this.Handle, nextClipboardViewer);
            
        }

        private void OnClipboardChanged()
        {
            ClipboardChanged?.Invoke(null, null);
        }
    }
}
