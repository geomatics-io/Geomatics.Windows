using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geomatics.Windows.Clipboard.Tests
{
    public partial class TestForm : Form
    {
        private SynchronizationContext _synchronizationContext;
        private IDisposable _clipboardMonitor;
        public TestForm()
        {
            InitializeComponent();
            this.FormClosing += TestForm_FormClosing1;
        }

        private void TestForm_FormClosing1(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("1");
            _clipboardMonitor?.Dispose();
            Debug.WriteLine("2");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _synchronizationContext =
                WindowsFormsSynchronizationContext.Current;

            _clipboardMonitor = ClipboardMonitor.OnUpdate.SubscribeOn(_synchronizationContext)
                //                .Where(contents => contents.OwnerHandle != WinProcHandler.Instance.Handle)
                .Synchronize().Subscribe(clipboardContents =>
                {
                    textBox.Clear();
                    foreach (var format in clipboardContents.Formats)
                    {
                        textBox.AppendText(format + "\n");
                    }
                });
        }
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _clipboardMonitor?.Dispose();
            Application.Exit();
        }
    }
}
