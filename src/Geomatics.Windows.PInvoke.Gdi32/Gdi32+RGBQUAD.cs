using System;
using System.Runtime.InteropServices;

namespace PInvoke
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBQUAD
    {
        // https://msdn.microsoft.com/cs-cz/library/windows/desktop/dd162938(v=vs.85).aspx
        
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;
    }
}