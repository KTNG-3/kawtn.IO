using System;
using System.IO;
using System.IO.Compression;

using kawtn.IO.External;

namespace kawtn.IO
{
    // File
    public class Item
    {
        public readonly Location Location;
        protected readonly Attributes Attributes;

        public Item(string location)
        {
            if (location.EndsWith(Path.DirectorySeparatorChar)
                || location.EndsWith(Path.AltDirectorySeparatorChar))
            {
                location = location.Substring(0, location.Length - 1);
            }

            this.Location = new Location(location);
            this.Attributes = new Attributes(this.Location);
        }

        public Item(Location location)
            : this(location.Data) { }

        public Item()
            : this(new Location(ApplicationInventory.Temporary, Path.GetRandomFileName())) { }

        public bool IsExists()
        {
            return File.Exists(this.Location.Data);
        }

        public FileInfo GetInfo()
        {
            return new FileInfo(this.Location.Data);
        }

        public string GetName()
        {
            return GetInfo().Name;
        }

        public Inventory? GetParent()
        {
            string? path = Path.GetDirectoryName(this.Location.Data);
            if (path == null) return null;

            return new Inventory(path);
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

        public Item InsertTo(Inventory destination)
        {
            Item item = destination.CreateItem(GetName());

            Clone(item);

            return item;
        }

        public Item TransferTo(Inventory destination)
        {
            Item item = InsertTo(destination);

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
