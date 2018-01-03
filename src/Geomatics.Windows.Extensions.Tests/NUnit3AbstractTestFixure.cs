using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace Geomatics.Windows.Extensions.Tests
{
    public abstract class NUnit3AbstractTestFixure
    {
        protected String LoadFile(string resourcePath)
        {
            var assembly = typeof(NUnit3AbstractTestFixure).Assembly;
            Assert.NotNull(assembly);

            string[] resourceNames = assembly.GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                Debug.WriteLine(resourceName);
            }

            string file = string.Format("{0}.{1}", assembly.GetName().Name, resourcePath);

            Assert.That(file, Is.Not.Null.And.Not.Empty);
            Stream fileStream = assembly.GetManifestResourceStream(file);
            Assert.IsNotNull(fileStream);

            var fileName = Path.GetTempFileName();
            SaveStreamToFile(fileName, fileStream);

            if (File.Exists(fileName))
            {
                return fileName;
            }
            return null;
        }

        private void SaveStreamToFile(string fileFullPath, Stream stream)
        {
            if (stream.Length == 0) return;

            // Create a FileStream object to write a stream to a file
            using (FileStream fileStream = global::System.IO.File.Create(fileFullPath, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }
    }
}