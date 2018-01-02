using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Geomatics.Windows.Extensions.System.ByteArray
{
    public static partial class Extensions
    {

        public static object ToObject(this byte[] array)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(array, 0, array.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            global::System.Object obj = (global::System.Object)binForm.Deserialize(memStream);
            return obj;
        }
    }
}