using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Geomatics.Windows.Clipboard.Data;
using PInvoke;

namespace Geomatics.Windows.Clipboard.Tests
{
    public partial class TestForm : Form
    {
        private IList<DataSource> clips = new List<DataSource>();
        private BindingSource bs;

        private SynchronizationContext _synchronizationContext;
        private IDisposable _clipboardMonitor;
        public TestForm()
        {
            InitializeComponent();
            this.FormClosing += TestForm_FormClosing1;

            BindingList<DataSource> bl = new BindingList<DataSource>(clips);

            bs = new BindingSource {DataSource = bl};

            this.dgvClips.DataSource = bs;
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
                    tbDebug.Clear();

                    tbProcess.Text = ClipboardNative.GetProcessName(clipboardContents.OwnerHandle);

                    clips.Add(clipboardContents.DataSource);
                    bs.ResetBindings(false);

                    foreach (var key in clipboardContents.Formats.Keys)
                    {
                        tbDebug.AppendText(string.Format($@"{key:X4} - {clipboardContents.Formats[key]}") + "\n");
                    }
                });
        }

        
        

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _clipboardMonitor?.Dispose();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
