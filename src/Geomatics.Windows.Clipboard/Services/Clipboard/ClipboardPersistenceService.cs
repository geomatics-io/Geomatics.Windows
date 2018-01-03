using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using Geomatics.Windows.Clipboard.Data;

namespace Geomatics.Windows.Clipboard.Services.Clipboard
{
    public class ClipboardPersistenceService
    {
        /// <summary>
        /// Save a collection of ClipboardData to HardDisk
        /// </summary>
        /// <param name="clipData">The collection of ClipboardData to save</param>
        /// <param name="fileName">The name of the file</param>
        public static void SaveToFile(ClipboardDataPackage clipboardDataPackage, string path)
        {
            //Get the enumeration of the clipboard data
            using (IEnumerator<ClipboardData> cData = clipboardDataPackage.Contents.GetEnumerator())
            {

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //Open the directory on which save the files
                DirectoryInfo di = Directory.CreateDirectory(path);

                while (cData.MoveNext())
                {
                    //Init the serializer
                    XmlSerializer xml = new XmlSerializer(typeof(ClipboardData));

                    ClipboardData c = cData.Current;
                    var file = string.Format("{0}_{1}.attachment", clipboardDataPackage.UUID.ToString("N").ToUpper(), c.UUID.ToString("N").ToUpper());

                    var filename = Path.Combine(path, file);

                    // To write to a file, create a StreamWriter object.
                    using (StreamWriter sw = new StreamWriter(filename, false))
                    {
                        //Serialize the clipboard data
                        xml.Serialize(sw, cData.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Open the file and deserialize the collection of ClipDatas
        /// </summary>
        /// <param name="fileName">The path of the file to open</param>
        /// <returns></returns>
        public static ReadOnlyCollection<ClipboardData> ReadFromFile(string clipName)
        {
            //Init the List to return as result
            List<ClipboardData> clips = new List<ClipboardData>();
            //Check if the clipboardDataPackage exists on hd
            if (Directory.Exists(clipName))
            {
                DirectoryInfo di = new DirectoryInfo(clipName);

                //Loop for each clipboard data
                for (int x = 0; x < di.GetFiles().GetLength(0); x++)
                {
                    //Init the serializer
                    XmlSerializer xml = new XmlSerializer(typeof(ClipboardData));
                    //Set the file to read
                    FileInfo fi = new FileInfo(di.FullName + "\\" + x.ToString() + ".cli");
                    //Init the stream to deserialize
                    using (FileStream fs = fi.Open(FileMode.Open))
                    {
                        //deserialize and add to the List
                        clips.Add((ClipboardData)xml.Deserialize(fs));
                    }
                }
            }
            return new ReadOnlyCollection<ClipboardData>(clips);
        }


        /// <summary>
        /// Get data from clipboard and save it to Hard Disk
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        public static void Serialize(string clipName)
        {
            //Get data from clipboard
            ClipboardDataPackage clipboardDataPackage = ClipboardService.GetClipboard();
            //Save data to hard disk
            SaveToFile(clipboardDataPackage, clipName);
        }

        /// <summary>
        /// Get data from hard disk and put them into the clipboard
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool Deserialize(string clipName)
        {
            //Get data from hard disk
            ReadOnlyCollection<ClipboardData> clipData = ReadFromFile(clipName);
            //Set red data into clipboard
            return ClipboardService.SetClipboard(clipData);
        }
    }
}