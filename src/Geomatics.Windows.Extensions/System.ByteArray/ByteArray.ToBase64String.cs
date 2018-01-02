using System.Security.Cryptography;

namespace Geomatics.Windows.Extensions.System.ByteArray
{
    public static partial class Extensions
    {

        public static byte[] GetMd5Hash(this byte[] bytes)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashValue = md5.ComputeHash(bytes);
            return hashValue;
        }
    }
}