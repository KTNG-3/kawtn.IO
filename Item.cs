using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO
{
    public class Item
    {
        public readonly string path;

        public Item(string path)
        {
            this.path = Path.GetFullPath(path);
        }

        public string GetName()
        {
            return Path.GetFileName(this.path) ?? string.Empty;
        }

        public Inventory GetInventory()
        {
            return new(Path.GetDirectoryName(path) ?? string.Empty);
        }

        public bool Exists()
        {
            return File.Exists(this.path);
        }

        public void Create()
        {
            if (Exists()) return;

            GetInventory().Create();

            File.WriteAllBytes(path, Array.Empty<byte>());
        }

        public void Write(byte[] data)
        {
            Create();

            File.WriteAllBytes(path, data);
        }

        public byte[] Read()
        {
            if (Exists())
            {
                return File.ReadAllBytes(this.path);
            }
            else
            {
                return Array.Empty<byte>();
            }
        }

        public void Unzip(Inventory destination)
        {
            destination.Create();
            ZipFile.ExtractToDirectory(this.path, destination.path);
        }

        public void Clone(Inventory destination)
        {
            if (!Exists()) return;

            string path = Path.Join(destination.path, GetName());
            Item item = new(path);

            item.Write(Read());
        }

        public void Move(Inventory destination)
        {
            Clone(destination);
            Delete();
        }

        public void Delete()
        {
            if (!Exists()) return;

            File.Delete(this.path);
        }
    }
}
