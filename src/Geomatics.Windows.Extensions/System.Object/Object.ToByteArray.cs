using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Geomatics.Windows.Extensions.System.Object
{
    public static partial class Extensions
    {

        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}