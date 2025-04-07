using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using kawtn.IO.External;

namespace kawtn.IO
{
    // File
    public class Item
    {
        public Location Location { get; private set; }
        protected Attributes Attributes
        {
            get
            {
                return new Attributes(this.Location);
            }
        }

        public Item(string location)
        {
            if (location.EndsWith(Path.DirectorySeparatorChar)
                || location.EndsWith(Path.AltDirectorySeparatorChar))
            {
                location = location.Substring(0, location.Length - 1);
            }

            this.Location = new Location(location);
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

        public string GetSystemName()
        {
            return GetInfo().Name;
        }

        public string GetName()
        {
            string sysName = this.GetSystemName();

            int dotIndex = sysName.IndexOf('.');

            int notFound = -1;
            if (dotIndex == notFound)
            {
                return sysName;
            }

            return sysName.Substring(0, dotIndex);
        }

        public string GetExtension()
        {
            string sysName = this.GetSystemName();

            int dotIndex = sysName.IndexOf('.');

            int notFound = -1;
            if (dotIndex == notFound)
            {
                return string.Empty;
            }

            return sysName.Substring(dotIndex);
        }

        public Inventory? GetParent()
        {
            string? path = Path.GetDirectoryName(this.Location.Data);
            if (path == null)
            {
                return null;
            }

            return new Inventory(path);
        }

        public void Create()
        {
            if (this.IsExists()) return;

            Inventory? parent = GetParent();
            if (parent != null)
            {
                parent.Create();
            }

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

        public void Write(IEnumerable<byte> data)
        {
            this.Create();

            File.WriteAllBytes(this.Location.Data, data.ToArray());
        }

        public void WriteByte(IEnumerable<byte> data)
        {
            this.Write(data);
        }

        public IEnumerable<byte> Read()
        {
            if (IsExists())
            {
                return File.ReadAllBytes(this.Location.Data);
            }

            return Array.Empty<byte>();
        }

        public IEnumerable<byte> ReadByte()
        {
            return this.Read();
        }

        public void Unzip(Inventory destination)
        {
            destination.Create();
            ZipFile.ExtractToDirectory(this.Location.Data, destination.Location.Data);
        }

        public void Clone(Item destination)
        {
            if (!this.IsExists()) return;

            destination.Write(this.Read());

            FileAttributes read = this.Attributes.Get();
            destination.Attributes.Set(read);
        }

        public void Move(Item destination)
        {
            this.Clone(destination);
            this.Delete();

            this.Location = destination.Location;
        }

        public Item InsertTo(Inventory destination)
        {
            Item item = destination.CreateItem(this.GetName());

            this.Clone(item);

            return item;
        }

        public void TransferTo(Inventory destination)
        {
            Item item = this.InsertTo(destination);

            this.Delete();

            this.Location = item.Location;
        }

        public void ChangeSystemName(string name)
        {
            if (GetSystemName() == name) return;

            Inventory? parent = GetParent();
            if (parent == null) return;

            Item item = parent.CreateItem(name);
            Move(item);
        }

        public void ChangeName(string name)
        {
            if (GetName() == name) return;

            if (name.Contains('.'))
            {
                throw new KawtnIOException("extension should not be included in the name", new ArgumentException(name));
            }

            ChangeSystemName($"{name}{GetExtension()}");
        }

        public void ChangeExtension(string name)
        {
            if (GetExtension() == name) return;

            if (string.IsNullOrWhiteSpace(name))
            {
                ChangeSystemName(GetName());
                return;
            }

            if (!name.StartsWith('.'))
            {
                name = $".{name}";
            }

            ChangeSystemName($"{GetName()}{name}");
        }

        public void Delete()
        {
            if (!IsExists()) return;

            File.Delete(this.Location.Data);
        }
    }
}
