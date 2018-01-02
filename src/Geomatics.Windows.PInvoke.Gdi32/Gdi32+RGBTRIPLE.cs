using System.Runtime.InteropServices;

namespace PInvoke
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBTRIPLE
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/dd162939(v=vs.85).aspx

        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
    }
}