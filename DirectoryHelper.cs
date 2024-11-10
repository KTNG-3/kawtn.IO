using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO
{
    public static class DirectoryHelper
    {
        public static string GetRootDirectory(string path)
        {
            return Path.GetDirectoryName(path) ?? string.Empty;
        }

        public static string GetDirectoryName(string path)
        {
            return Path.GetFileName(path) ?? string.Empty;
        }

        public static void Create(string path)
        {
            if (Directory.Exists(path)) return;
            if (string.IsNullOrWhiteSpace(path)) return;

            Directory.CreateDirectory(path);
        }

        public static void Clone(string sourcePath, string destinationPath)
        {
            CloneFiles(sourcePath, destinationPath);
            CloneDirectories(sourcePath, destinationPath);
        }

        public static void CloneFiles(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(sourcePath)) return;

            Create(destinationPath);

            foreach (string fileName in Directory.GetFiles(sourcePath))
            {
                string destFileName = Path.Join(destinationPath, Path.GetFileName(fileName));

                FileHelper.Clone(fileName, destFileName);
            }
        }

        public static void CloneDirectories(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(sourcePath)) return;

            Create(destinationPath);

            foreach (string directory in Directory.GetDirectories(sourcePath))
            {
                string directoryName = GetDirectoryName(directory);

                Clone(
                        Path.Join(sourcePath, directoryName),
                        Path.Join(destinationPath, directoryName)
                    );
            }
        }

        public static void Delete(string path)
        {
            if (!Directory.Exists(path)) return;

            Directory.Delete(path, true);
        }
    }
}
