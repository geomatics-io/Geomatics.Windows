using System;
using Geomatics.Windows.Clipboard.Data.Interfaces;
using Geomatics.Windows.Extensions.System.ByteArray;

namespace Geomatics.Windows.Clipboard.Data
{
    /// <summary>
    /// Holds clipboard data of a single data format.
    /// </summary>
    [Serializable]
    public class ClipboardData : IClipboardData
    {
        private uint format;
        private string formatName;
        private byte[] buffer;
        private int size;

        /// <summary>
        /// Init a ClipboardDataPackage Data object, containing one clipboard data and its format
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatName"></param>
        /// <param name="buffer"></param>
        public ClipboardData(uint format, string formatName, byte[] buffer)
        {
            this.UUID = Guid.NewGuid();
            this.format = format;
            this.formatName = formatName;
            this.buffer = buffer;
            this.MD5 = buffer.GetMd5Hash();
            this.size = 0;
        }

        /// <summary>
        /// Init an empty ClipboardDataPackage Data object, used for serialize object
        /// </summary>
        public ClipboardData() { }

        public byte[] MD5 { get; set; }

        public Guid UUID { get; set; }

        /// <summary>
        /// Get or Set the format code of the data 
        /// </summary>
        public uint Format
        {
            get { return format; }
            set { format = value; }
        }

        /// <summary>
        /// Get or Set the format name of the data 
        /// </summary>
        public string FormatName
        {
            get { return formatName; }
            set { formatName = value; }
        }

        /// <summary>
        /// Get or Set the buffer data
        /// </summary>
        public byte[] Buffer
        {
            get { return buffer; }
            set
            {
                MD5 = value.GetMd5Hash();
                buffer = value;
            }
        }

        /// <summary>
        /// Get the data buffer lenght
        /// </summary>
        public UIntPtr Size
        {
            get
            {
                if (buffer != null)
                {
                    //Read the correct size from buffer, if it is not null
                    return new UIntPtr(Convert.ToUInt32(buffer.GetLength(0)));
                }
                else
                {
                    //else return the optional set size
                    return new UIntPtr(Convert.ToUInt32(size));
                }
            }
        }
    }
}