using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace kawtn.IO
{
    public static class FileHelper
    {
        public static void Create(string path)
        {
            if (File.Exists(path)) return;

            DirectoryHelper.Create(DirectoryHelper.GetRootDirectory(path));

            File.WriteAllBytes(path, Array.Empty<byte>());
        }

        public static string Read(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            return File.ReadAllText(path);
        }

        public static byte[] ReadBytes(string path)
        {
            if (!File.Exists(path))
            {
                return Array.Empty<byte>();
            }

            return File.ReadAllBytes(path);
        }

        public static T? ReadJSON<T>(string path)
        {
            string content = Read(path);

            if (string.IsNullOrWhiteSpace(content))
            {
                // null
                return default;
            }

            return JsonSerializer.Deserialize<T>(content);
        }

        public static void Update(string path, string data)
        {
            Create(path);

            File.WriteAllText(path, data);
        }

        public static void UpdateBytes(string path, byte[] data)
        {
            Create(path);

            File.WriteAllBytes(path, data);
        }

        public static void UpdateJSON<T>(string path, T? data)
        {
            Create(path);

            Update(path, JsonSerializer.Serialize(data));
        }

        public static void Clone(string sourcePath, string destinationPath)
        {
            if (!File.Exists(sourcePath)) return;

            UpdateBytes(destinationPath, ReadBytes(sourcePath));
        }

        public static void Delete(string path)
        {
            if (!File.Exists(path)) return;

            File.Delete(path);
        }
    }
}
