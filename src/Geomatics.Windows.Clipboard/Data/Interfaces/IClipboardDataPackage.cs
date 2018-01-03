using System;
using System.Collections.Generic;

namespace Geomatics.Windows.Clipboard.Data.Interfaces
{
    public interface IClipboardDataPackage
    {
        IDataSource DataSource { get; set; }
        DateTimeOffset Created { get; set; }
        List<ClipboardData> Contents { get; set; }
        Guid UUID { get; set; }
        string MD5 { get; }
    }
}