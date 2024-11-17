using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace kawtn.IO
{
    // Directory
    public class Inventory
    {
        public readonly Location Location;
        protected readonly Attributes Attributes;

        public Inventory(string location)
        {
            if (!location.EndsWith(Path.DirectorySeparatorChar)
                && !location.EndsWith(Path.AltDirectorySeparatorChar))
            {
                location += Path.AltDirectorySeparatorChar;
            }

            this.Location = new(location);
            this.Attributes = new(this.Location);
        }

        public Inventory(Location location) 
            : this(location.Data) { }

        public bool IsExists()
        {
            return Directory.Exists(this.Location.Data);
        }

        public bool IsEmpty()
        {
            return Read().Length == 0;
        }

        public DirectoryInfo GetInfo()
        {
            return new(this.Location.Data);
        }

        public string GetName()
        {
            return GetInfo().Name;
        }

        public Inventory? GetParent()
        {
            DirectoryInfo? dir = GetInfo().Parent;
            if (dir == null) return null;

            return new(dir.FullName);
        }

        public void Create()
        {
            if (IsExists()) return;

            Directory.CreateDirectory(this.Location.Data);
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

        public Item? Insert(Item item)
        {
            Create();

            return item.InsertTo(this);
        }

        public Inventory? Insert(Inventory inventory)
        {
            Create();

            return inventory.InsertTo(this);
        }

        public Location[] Read()
        {
            if (!IsExists())
                return Array.Empty<Location>();

            return Directory.GetFileSystemEntries(this.Location.Data)
                .Select(x => new Location(x))
                .ToArray();
        }

        public Item[] ReadItems()
        {
            return Directory.GetFiles(this.Location.Data)
                .Select(x => new Item(x))
                .ToArray();
        }

        public Inventory[] ReadInventories()
        {
            return Directory.GetDirectories(this.Location.Data)
                .Select(x => new Inventory(x))
                .ToArray();
        }

        public void Zip(Item destination)
        {
            ZipFile.CreateFromDirectory(this.Location.Data, destination.Location.Data);
        }

        public void Clone(Inventory destination)
        {
            if (!IsExists()) return;

            if (!destination.IsEmpty())
            {
                throw new IOException("inventory not empty");
            }

            destination.Create();

            foreach (Item item in ReadItems())
            {
                item.InsertTo(destination);
            }

            foreach (Inventory inventory in ReadInventories())
            {
                inventory.InsertTo(destination);
            }

            FileAttributes read = this.Attributes.Get();
            destination.Attributes.Set(read);
        }

        public void Move(Inventory destination)
        {
            Clone(destination);
            Delete();
        }

        public Inventory? InsertTo(Inventory destination)
        {
            Inventory? inventory = new Location(destination, GetName()).ParseInventory();

            Clone(inventory);

            return inventory;
        }

        public Inventory? TransferTo(Inventory destination)
        {
            Inventory? inventory = InsertTo(destination);

            Delete();

            return inventory;
        }

        public void Delete()
        {
            if (!IsExists()) return;

            Directory.Delete(this.Location.Data, true);
        }
    }
}
