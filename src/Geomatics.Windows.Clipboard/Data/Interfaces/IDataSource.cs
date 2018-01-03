namespace Geomatics.Windows.Clipboard.Data.Interfaces
{
    public interface IDataSource
    {
        byte[] Icon { get; }

        string Text { get; }

        string UserName { get; set; }
        string MachineName { get; set; }
        string OS { get; set; }
    }
}