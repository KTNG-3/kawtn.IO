using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using kawtn.IO.External;
using kawtn.IO.Json;
using kawtn.IO.Konfig;

namespace kawtn.IO
{
    // Directory
    public class Inventory
    {
        public Location Location { get; private set; }
        protected Attributes Attributes
        {
            get
            {
                return new Attributes(this.Location);
            }
        }

        public string ItemExtension;

        public Inventory(string location, string? itemExtension = null)
        {
            if (!location.EndsWith(Path.DirectorySeparatorChar)
                && !location.EndsWith(Path.AltDirectorySeparatorChar))
            {
                location += Path.AltDirectorySeparatorChar;
            }

            this.Location = new Location(location);

            this.ItemExtension = itemExtension ?? string.Empty;
        }

        public Inventory(Location location, string? itemExtension = null)
            : this(location.Data, itemExtension) { }

        public Inventory()
            : this(new Location(ApplicationInventory.Temporary, Path.GetRandomFileName())) { }

        public bool IsExists()
        {
            return Directory.Exists(this.Location.Data);
        }

        public bool IsEmpty()
        {
            return Read().Count() == 0;
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

        public DirectoryInfo GetInfo()
        {
            return new DirectoryInfo(this.Location.Data);
        }

        public string GetName()
        {
            return GetInfo().Name;
        }

        public Inventory? GetParent()
        {
            DirectoryInfo? dir = GetInfo().Parent;
            if (dir == null) return null;

            return new Inventory(dir.FullName);
        }

        public void Create()
        {
            if (IsExists()) return;

            Directory.CreateDirectory(this.Location.Data);
        }

        public Item CreateItem(string name)
        {
            return new Location(this, $"{name}{ItemExtension}").ParseItem();
        }

        public JsonItem<T> CreateJsonItem<T>(string name)
        {
            return new Location(this, $"{name}{ItemExtension}").ParseJsonItem<T>();
        }

        public KonfigItem<T> CreateKonfigItem<T>(string name)
        {
            return new Location(this, $"{name}{ItemExtension}").ParseKonfigItem<T>();
        }

        public Inventory CreateInventory(string name)
        {
            Inventory inventory = new Location(this, name).ParseInventory();

            inventory.ItemExtension = this.ItemExtension;

            return inventory;
        }

        public JsonInventory<T> CreateJsonInventory<T>(string name)
        {
            JsonInventory<T> inventory = new Location(this, name).ParseJsonInventory<T>();

            inventory.ItemExtension = this.ItemExtension;

            return inventory;
        }

        public KonfigInventory<T> CreateKonfigInventory<T>(string name)
        {
            KonfigInventory<T> inventory = new Location(this, name).ParseKonfigInventory<T>();

            inventory.ItemExtension = this.ItemExtension;

            return inventory;
        }

        public Item Insert(Item item)
        {
            Create();

            Item insertedItem = item.InsertTo(this);

            if (!string.IsNullOrWhiteSpace(this.ItemExtension))
                insertedItem.ChangeExtension(ItemExtension);

            return insertedItem;
        }

        public Inventory Insert(Inventory inventory)
        {
            Create();

            return inventory.InsertTo(this);
        }

        public IEnumerable<Location> Read()
        {
            if (!IsExists())
                return Array.Empty<Location>();

            IEnumerable<Location> read = Directory.GetFileSystemEntries(this.Location.Data).Select(x => new Location(x));

            if (string.IsNullOrWhiteSpace(this.ItemExtension))
            {
                return read;
            }
            else
            {
                return read.Where(location =>
                {
                    if (location.IsInventory())
                        return true;

                    return location.ParseItem().GetExtension() == this.ItemExtension;
                });
            }
        }

        public IEnumerable<Item> ReadItems()
        {
            if (!IsExists())
                return Array.Empty<Item>();

            IEnumerable<Item> read = Directory.GetFiles(this.Location.Data).Select(x => new Item(x));

            if (string.IsNullOrWhiteSpace(this.ItemExtension))
            {
                return read;
            }
            else
            {
                return read.Where(item => item.GetExtension() == this.ItemExtension);
            }
        }

        public IEnumerable<Inventory> ReadInventories()
        {
            if (!IsExists())
                return Array.Empty<Inventory>();

            return Directory.GetDirectories(this.Location.Data).Select(x => new Inventory(x));
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

            this.Location = destination.Location;
        }

        public Inventory InsertTo(Inventory destination)
        {
            Inventory inventory = destination.CreateInventory(GetName());

            Clone(inventory);

            return inventory;
        }

        public void TransferTo(Inventory destination)
        {
            Inventory inventory = InsertTo(destination);

            Delete();

            this.Location = inventory.Location;
        }

        public void ChangeName(string name)
        {
            if (GetName() == name) return;

            Inventory? parent = GetParent();
            if (parent == null) return;

            Inventory inventory = parent.CreateInventory(name);
            Move(inventory);
        }

        public void Delete()
        {
            if (!IsExists()) return;

            Directory.Delete(this.Location.Data, true);
        }
    }
}
