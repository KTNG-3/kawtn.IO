using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO
{
    // File
    public class Item
    {
        public readonly Location Location;
        protected readonly Attributes Attributes;

        public Item(string location)
        {
            string path = Path.TrimEndingDirectorySeparator(location);

            this.Location = new(path);
            this.Attributes = new(this.Location);
        }

        public Item(Location location) 
            : this(location.Data) { }

        public bool IsExists()
        {
            return File.Exists(this.Location.Data);
        }

        public FileInfo GetInfo()
        {
            return new(this.Location.Data);
        }

        public string GetName()
        {
            return GetInfo().Name;
        }

        public Inventory? GetParent()
        {
            string? path = Path.GetDirectoryName(this.Location.Data);
            if (path == null) return null;

            return new(path);
        }

        public void Create()
        {
            if (IsExists()) return;

            Inventory? parent = GetParent();
            if (parent != null) parent.Create();

            File.WriteAllBytes(this.Location.Data, Array.Empty<byte>());
        }

        public void Hidden(bool hidden = true)
        {
            if (hidden)
            {
                this.Attributes.Add(FileAttributes.Hidden);
            }
            else
            {
                this.Attributes.Remove(FileAttributes.Hidden);
            }
        }

        public void Write(byte[] data)
        {
            Create();

            File.WriteAllBytes(this.Location.Data, data);
        }

        public byte[] Read()
        {
            if (IsExists())
            {
                return File.ReadAllBytes(this.Location.Data);
            }
            else
            {
                return Array.Empty<byte>();
            }
        }

        public void Unzip(Inventory destination)
        {
            destination.Create();
            ZipFile.ExtractToDirectory(this.Location.Data, destination.Location.Data);
        }

        public void Clone(Item destination)
        {
            if (!IsExists()) return;

            destination.Write(Read());

            FileAttributes read = this.Attributes.Get();
            destination.Attributes.Set(read);
        }

        public void Move(Item destination)
        {
            Clone(destination);
            Delete();
        }

        public Item? InsertTo(Inventory destination)
        {
            Item item = new Location(destination, GetName()).ParseItem();

            Clone(item);

            return item;
        }

        public Item? TransferTo(Inventory destination)
        {
            Item? item = InsertTo(destination);

            Delete();

            return item;
        }

        public void Delete()
        {
            if (!IsExists()) return;

            File.Delete(this.Location.Data);
        }
    }
}
