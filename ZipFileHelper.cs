using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO
{
    public static class ZipFileHelper
    {
        public static void Unzip(string sourceFile, string destinationDir)
        {
            DirectoryHelper.Create(destinationDir);
            ZipFile.ExtractToDirectory(sourceFile, destinationDir);
        }

        public static void UnzipBytes(byte[] data, string destinationDir)
        {
            string filePath = Path.Combine(ApplicationDirectoryHelper.Temporary, $"zip-{DateTime.Now.ToFileTimeUtc()}");
            FileHelper.UpdateBytes(filePath, data);

            Unzip(filePath, destinationDir);
        }
    }
}
