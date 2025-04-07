using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using kawtn.IO.External;

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
            return this.Read().Count() == 0;
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
            return this.GetInfo().Name;
        }

        public Inventory? GetParent()
        {
            DirectoryInfo? dir = this.GetInfo().Parent;
            if (dir == null)
            {
                return null;
            }

            return new Inventory(dir.FullName);
        }

        public void Create()
        {
            if (this.IsExists()) return;

            Directory.CreateDirectory(this.Location.Data);
        }

        public Item CreateItem(string name)
        {
            Location location = new(this, $"{name}{this.ItemExtension}");

            return new Item(location);
        }

        public Inventory CreateInventory(string name)
        {
            Location location = new(this, name);

            return new Inventory(location)
            {
                ItemExtension = this.ItemExtension
            };
        }

        public Item Insert(Item item)
        {
            this.Create();

            Item insertedItem = item.InsertTo(this);

            if (!string.IsNullOrWhiteSpace(this.ItemExtension))
                insertedItem.ChangeExtension(ItemExtension);

            return insertedItem;
        }

        public Inventory Insert(Inventory inventory)
        {
            this.Create();

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

                    return new Item(location).GetExtension() == this.ItemExtension;
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
            if (!this.IsExists()) return;

            if (!destination.IsEmpty())
            {
                throw new KawtnIOException("destination inventory is not empty");
            }

            destination.Create();

            foreach (Item item in this.ReadItems())
            {
                item.InsertTo(destination);
            }

            foreach (Inventory inventory in this.ReadInventories())
            {
                inventory.InsertTo(destination);
            }

            FileAttributes read = this.Attributes.Get();
            destination.Attributes.Set(read);
        }

        public void Move(Inventory destination)
        {
            this.Clone(destination);
            this.Delete();

            this.Location = destination.Location;
        }

        public Inventory InsertTo(Inventory destination)
        {
            Inventory inventory = destination.CreateInventory(this.GetName());

            this.Clone(inventory);

            return inventory;
        }

        public void TransferTo(Inventory destination)
        {
            Inventory inventory = this.InsertTo(destination);

            this.Delete();

            this.Location = inventory.Location;
        }

        public void ChangeName(string name)
        {
            if (this.GetName() == name) return;

            Inventory? parent = this.GetParent();
            if (parent == null) return;

            Inventory inventory = parent.CreateInventory(name);
            this.Move(inventory);
        }

        public void Delete()
        {
            if (!this.IsExists()) return;

            Directory.Delete(this.Location.Data, true);
        }
    }
}
