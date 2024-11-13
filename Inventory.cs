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

        public Inventory(Location location)
        {
            this.Location = location;

            Create();
        }

        public Inventory(string location) 
            : this(new Location(location)) { }

        public string GetName()
        {
            return new Item(this.Location.Data).GetName();
        }

        public Inventory GetRootInventory()
        {
            return new Item(this.Location.Data).GetInventory();
        }

        public bool IsExists()
        {
            return Directory.Exists(this.Location.Data);
        }

        public void Create()
        {
            if (IsExists()) return;

            Directory.CreateDirectory(this.Location.Data);
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
            if (!IsExists())
                return Array.Empty<Item>();

            return Directory.GetFiles(this.Location.Data)
                .Select(x => new Item(x))
                .ToArray();
        }

        public Inventory[] ReadInventories()
        {
            if (!IsExists())
                return Array.Empty<Inventory>();

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
            if (!IsExists()) return;

            Directory.Delete(this.Location.Data, true);
        }
    }
}
