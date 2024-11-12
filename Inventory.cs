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
        public readonly string Location;

        public Inventory(string location)
        {
            this.Location = new Location(location).Data;
        }

        public Inventory(Location location)
        {
            this.Location = location.Data;
        }

        public string GetName()
        {
            return new Item(this.Location).GetName();
        }

        public Inventory GetRootInventory()
        {
            return new Item(this.Location).GetInventory();
        }

        public bool Exists()
        {
            return Directory.Exists(this.Location);
        }

        public void Create()
        {
            if (Exists()) return;

            Directory.CreateDirectory(Location);
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
            if (!Exists())
                return Array.Empty<Location>();

            return Directory.GetFileSystemEntries(this.Location)
                .Select(x => new Location(x))
                .ToArray();
        }

        public Item[] ReadItems()
        {
            if (!Exists())
                return Array.Empty<Item>();

            return Directory.GetFiles(this.Location)
                .Select(x => new Item(x))
                .ToArray();
        }

        public Inventory[] ReadInventories()
        {
            if (!Exists())
                return Array.Empty<Inventory>();

            return Directory.GetDirectories(this.Location)
                .Select(x => new Inventory(x))
                .ToArray();
        }

        public void Zip(Item destination)
        {
            ZipFile.CreateFromDirectory(this.Location, destination.Location);
        }

        public void Clone(Inventory destination)
        {
            if (!Exists()) return;

            destination.Create();

            foreach (Item item in ReadItems())
            {
                item.InsertTo(destination);
            }

            foreach (Inventory inventory in ReadInventories())
            {
                inventory.InsertTo(destination);
            }
        }

        public void Move(Inventory destination)
        {
            Clone(destination);
            Delete();
        }

        public Inventory InsertTo(Inventory destination)
        {
            Inventory inventory = new Location(destination, GetName()).ParseInventory();

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
            if (!Exists()) return;

            Directory.Delete(this.Location, true);
        }
    }
}
