using System.Text;

namespace Geomatics.Windows.Extensions.System.ByteArray
{
    public static partial class Extensions
    {

        public static string ToHex(this byte[] bytes, bool upperCase = true)
        {
            // OR: string hashPassword = BitConverter.ToString(byteHashedPassword).Replace("-","");

            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }
    }
}