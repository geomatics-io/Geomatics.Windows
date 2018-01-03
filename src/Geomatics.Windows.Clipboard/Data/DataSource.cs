using System;
using Geomatics.Windows.Clipboard.Data.Interfaces;

namespace Geomatics.Windows.Clipboard.Data
{
    [Serializable]
    public class DataSource : IDataSource
    {
        public DataSource(byte[] icon, string text, string username, string machinename, string os)
        {
            Icon = icon;
            Text = text;
            UserName = username;
            MachineName = machinename;
            OS = os;
        }

        public byte[] Icon { get; }

        public string Text { get; }

//        public string ApplicationName { get; set; }
//        public string ApplicationDescription { get; set; }
//        public string WindowTitle { get; set; }

        public string UserName { get; set; }
        public string MachineName { get; set; }
        public string OS { get; set; }
    }
}