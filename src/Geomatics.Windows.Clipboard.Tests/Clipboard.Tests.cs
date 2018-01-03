using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using Geomatics.Windows.Clipboard.Data;
using NUnit.Framework;

namespace Geomatics.Windows.Clipboard.Tests
{
    [TestFixture]
    public class ClipboardTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void TestClipboardMonitor()
        {
            using (TestForm form = new TestForm())
            {
                form.TopMost = true;
                form.ShowDialog();
            }
        }
    }
}
