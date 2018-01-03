﻿using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

namespace Geomatics.Windows.Clipboard
{
    /// <summary>
    /// This can be used to get a lock to the clipboard, and free it again.
    /// </summary>
    internal sealed class ClipboardSemaphore : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger<ClipboardSemaphore>();
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        // To detect redundant calls
        private bool _disposedValue;

        /// <summary>
        /// Get a lock to the clipboard
        /// </summary>
        /// <param name="hWnd">IntPtr with a hWnd for the potential new owner</param>
        /// <param name="retries">int with number of retries</param>
        /// <param name="retryInterval">TimeSpan for the time between retries</param>
        /// <param name="timeout">A timeout for waiting on the semaphore</param>
        /// <returns></returns>
        public IDisposable Lock(IntPtr hWnd = default(IntPtr), int retries = 5, TimeSpan? retryInterval = null, TimeSpan? timeout = null)
        {
            if (hWnd == IntPtr.Zero)
            {
                // Take the default
                Log.Trace($@"Taking windows handle { WinProcHandler.Instance.Handle} from the WinProcHandler");

                hWnd = WinProcHandler.Instance.Handle;
            }

            // If a timeout is passed, use this in the wait
            if (timeout.HasValue)
            {
                if (!_semaphoreSlim.Wait(timeout.Value))
                {
                    throw new TimeoutException("Clipboard lock timeout.");
                }
            }
            else
            {
                // This could block idenfinately if used incorrectly.
                _semaphoreSlim.Wait();
            }

            // Create the clipboard lock itself
            bool isLocked = false;
            do
            {
                if (OpenClipboard(hWnd))
                {
                    isLocked = true;
                    break;
                }
                retries--;
                Thread.Sleep(retryInterval ?? TimeSpan.FromMilliseconds(200));
            } while (retries >= 0);

            if (!isLocked)
            {
                throw new Win32Exception();
            }
            // Return a disposable which cleans up the current state.
            return Disposable.Create(() => {
                CloseClipboard();
                _semaphoreSlim.Release();
            });
        }

        /// <summary>
        /// Lock the clipboard, return a disposable which can free this again.
        /// </summary>
        /// <param name="hWnd">IntPtr with the hWnd of the potential new owner</param>
        /// <param name="retries">int with the number of retries</param>
        /// <param name="retryInterval">optional TimeSpan</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Task with disposable</returns>
        public async Task<IDisposable> LockAsync(IntPtr hWnd = default(IntPtr), int retries = 5, TimeSpan? retryInterval = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (hWnd == IntPtr.Zero)
            {
                // Take the default
                hWnd = WinProcHandler.Instance.Handle;
            }
            await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            bool isLocked = false;
            do
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                if (OpenClipboard(hWnd))
                {
                    isLocked = true;
                    break;
                }
                retries--;
                await Task.Delay(retryInterval ?? TimeSpan.FromMilliseconds(200), cancellationToken);
            } while (retries >= 0);

            if (!isLocked)
            {
                throw new Win32Exception();
            }
            return Disposable.Create(() =>
            {
                CloseClipboard();
                _semaphoreSlim.Release();
            });
        }


        #region Native

        /// <summary>
        ///     <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms649048(v=vs.85).aspx"></a>
        ///     Opens the clipboard for examination and prevents other applications from modifying the clipboard content.
        /// </summary>
        /// <param name="hWndNewOwner">IntPtr with the hWnd of the new owner. If this parameter is NULL, the open clipboard is associated with the current task.</param>
        /// <returns>true if the clipboard is opened</returns>
        [DllImport("user32", SetLastError = true)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        /// <summary>
        ///     <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms649048(v=vs.85).aspx"></a>
        ///     Opens the clipboard for examination and prevents other applications from modifying the clipboard content.
        /// </summary>
        /// <returns>true if the clipboard is closed</returns>
        [DllImport("user32", SetLastError = true)]
        private static extern bool CloseClipboard();

        #endregion

        #region IDisposable Support

        /// <summary>
        ///     Dispose the current async lock, and it's underlying SemaphoreSlim
        /// </summary>
        private void DisposeInternal()
        {
            if (_disposedValue)
            {
                return;
            }
            _semaphoreSlim.Dispose();

            _disposedValue = true;
        }

        /// <summary>
        ///     Finalizer, as it would be bad to leave a SemaphoreSlim hanging around
        /// </summary>
        ~ClipboardSemaphore()
        {
            DisposeInternal();
        }

        /// <summary>
        ///     Implementation of the IDisposable
        /// </summary>
        public void Dispose()
        {
            DisposeInternal();
            // Make sure the finalizer for this instance is not called, as we already did what we need to do
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}