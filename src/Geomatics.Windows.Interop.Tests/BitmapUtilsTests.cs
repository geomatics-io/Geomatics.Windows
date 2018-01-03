using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using PInvoke;
using Bitmap = System.Drawing.Bitmap;

namespace Geomatics.Windows.Interop.Tests
{
    [TestFixture]
    public class BitmapUtilsTests
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
        public void TestCombine()
        {
            Image image = System.Drawing.Image.FromFile(@"Z:\bitbucket\geomatics.io\Geomatics.Windows\src\Geomatics.Windows.Interop.Tests\Images\grinch.jpg"); //@TODO: embed image
            Clipboard.SetImage(image);


            //            MemoryStream dib = Clipboard.GetData(DataFormats.Dib) as MemoryStream;
            //            byte[] dibBytes = dib.ToArray();
            //            GCHandle hdl = GCHandle.Alloc(dibBytes, GCHandleType.Pinned);
            //            BITMAPINFOHEADER dibHdr = (BITMAPINFOHEADER) Marshal.PtrToStructure(hdl.AddrOfPinnedObject(), typeof(BITMAPINFOHEADER));
            //            hdl.Free();

            bool result = User32.OpenClipboard(IntPtr.Zero);
            Assert.True(result);

            result = User32.IsClipboardFormatAvailable((uint) User32.StandardClipboardFormat.CF_DIBV5);
            Assert.True(result);

            IntPtr pDib = User32.GetClipboardData((uint) User32.StandardClipboardFormat.CF_DIBV5);
            Assert.AreNotEqual(IntPtr.Zero, pDib);

            var filename = Path.Combine(Path.GetTempPath(), DateTime.Now.ToString("yyyyMMddHHmmssffffff") + ".bmp");

            result = BitmapUtils.SaveDibToFile(pDib, new FileInfo(filename));
            Assert.True(result);
            
            Bitmap bitmap = new Bitmap(filename);
            Assert.NotNull(bitmap);

            Bitmap bitmap2 = BitmapUtils.ConvertDibToBitmap(pDib);
            Assert.NotNull(bitmap2);

            var filename2 = Path.Combine(Path.GetTempPath(), DateTime.Now.ToString("yyyyMMddHHmmssffffff") + ".bmp");
            result = BitmapUtils.SaveBitmapToFile(bitmap2, new FileInfo(filename2));
            Assert.True(result);

            Assert.True(BitmapUtils.Equals(bitmap, bitmap2));

            User32.CloseClipboard();
        }
    }
}
