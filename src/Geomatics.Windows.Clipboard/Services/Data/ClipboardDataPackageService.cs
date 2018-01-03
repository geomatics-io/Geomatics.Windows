using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Geomatics.Windows.Clipboard.Data;

namespace Geomatics.Windows.Clipboard.Services.Data
{
    public class ClipboardDataPackageService
    {
        const int VERSION = 1;

        public static void SaveToFile(ClipboardDataPackage clipboardDataPackage, string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var name = $"{clipboardDataPackage.UUID.ToString("N").ToUpper()}.cdp";
            var filename = Path.Combine(directory, name);

            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                IFormatter formatter = new BinaryFormatter();

                formatter.Serialize(stream, VERSION);
                formatter.Serialize(stream, clipboardDataPackage.IsEncrypted);
                formatter.Serialize(stream, clipboardDataPackage);

                
            }
        }

        public ClipboardDataPackage LoadFromFile(string filename)
        {
            ClipboardDataPackage clipboardDataPackage = null;

            if (!File.Exists(filename))
                return null;

            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                IFormatter formatter = new BinaryFormatter();
                int version = (int) formatter.Deserialize(stream);
                bool isEncrypted = (bool)formatter.Deserialize(stream);
                Debug.Assert(version == VERSION);
                clipboardDataPackage = (ClipboardDataPackage) formatter.Deserialize(stream);
            }

            return clipboardDataPackage;
        }
    }
}