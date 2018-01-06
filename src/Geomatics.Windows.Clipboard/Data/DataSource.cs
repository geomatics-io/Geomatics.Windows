using System;
using Geomatics.Windows.Clipboard.Data.Interfaces;

namespace Geomatics.Windows.Clipboard.Data
{
    [Serializable]
    public class DataSource
    {
        public DataSource()
        {
        }

        public DataSource(string windowTitle, string applicationPath, string applicationName, string applicationDescription, string url, byte[] smallApplicationIcon, byte[] largeApplicationIcon, string userName, string machineName, string operatingSystem)
        {
            WindowTitle = windowTitle;
            ApplicationPath = applicationPath;
            ApplicationName = applicationName;
            ApplicationDescription = applicationDescription;
            Url = url;
            SmallApplicationIcon = smallApplicationIcon;
            LargeApplicationIcon = largeApplicationIcon;
            UserName = userName;
            MachineName = machineName;
            OperatingSystem = operatingSystem;
        }

        public string WindowTitle { get; set; }


        public string ApplicationPath { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationDescription { get; set; }
        
        
        public string Url { get; set; }


        public byte[] SmallApplicationIcon { get; set; }
        public byte[] LargeApplicationIcon { get; set; }
        


        public string UserName { get; set; }
        public string MachineName { get; set; }
        public string OperatingSystem { get; set; }
    }
}