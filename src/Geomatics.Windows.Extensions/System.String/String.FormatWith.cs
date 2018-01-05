using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Geomatics.Windows.Extensions.System.String
{
    public static partial class Extensions
    {

        /// <summary>
        /// FormatWith is an extension method that wraps String.Format
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentNullException">format</exception>
        public static string FormatWith(this string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(format, args);
        }

        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(provider, format, args);
        }
    }
}