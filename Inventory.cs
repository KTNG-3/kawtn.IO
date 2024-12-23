﻿using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

using kawtn.IO.External;

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

            this.Location = new Location(location);
            this.Attributes = new Attributes(this.Location);
        }

        public Inventory(Location location) 
            : this(location.Data) { }

        public Inventory()
            : this(new Location(ApplicationInventory.Temporary, Path.GetRandomFileName())) { }

        public bool IsExists()
        {
            return Directory.Exists(this.Location.Data);
        }

        public bool IsEmpty()
        {
            return Read().Length == 0;
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
            Location location = new Location(this, name);

            return new Item(location);
        }

        public Inventory CreateInventory(string name)
        {
            Location location = new Location(this, name);

            return new Inventory(location);
        }

        public Item Insert(Item item)
        {
            Create();

            return item.InsertTo(this);
        }

        public Inventory Insert(Inventory inventory)
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

        public Inventory InsertTo(Inventory destination)
        {
            Inventory inventory = destination.CreateInventory(GetName());

            Clone(inventory);

            return inventory;
        }

        public Inventory TransferTo(Inventory destination)
        {
            Inventory inventory = InsertTo(destination);

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
