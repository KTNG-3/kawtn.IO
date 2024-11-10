using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace kawtn.IO
{
    public class Inventory
    {
        public readonly string path;

        public Inventory(string path)
        {
            this.path = Path.GetFullPath(path);
        }

        public string GetName()
        {
            return new Item(this.path).GetName();
        }

        public Inventory GetRootInventory()
        {
            return new Item(this.path).GetInventory();
        }

        public bool Exists()
        {
            return Directory.Exists(this.path);
        }

        public void Create()
        {
            if (Exists()) return;

            Directory.CreateDirectory(path);
        }

        public void WriteItem(params Item[] items)
        {
            Create();

            foreach (Item item in items)
            {
                item.Clone(this);
            }
        }

        public void WriteInventory(params Inventory[] inventories)
        {
            Create();

            foreach (Inventory inventory in inventories)
            {
                inventory.Clone(this);
            }
        }

        public string[] Read()
        {
            if (Exists())
            {
                return Directory.GetFileSystemEntries(this.path);
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        public Item[] ReadItems()
        {
            if (Exists())
            {
                return Directory.GetFiles(this.path).Select(x => new Item(x)).ToArray();
            }
            else
            {
                return Array.Empty<Item>();
            }
        }

        public Inventory[] ReadInventories()
        {
            if (Exists())
            {
                return Directory.GetDirectories(this.path).Select(x => new Inventory(x)).ToArray();
            }
            else
            {
                return Array.Empty<Inventory>();
            }
        }

        public void Zip(Item destination)
        {
            ZipFile.CreateFromDirectory(this.path, destination.path);
        }

        public void Clone(Inventory destination)
        {
            CloneFiles(destination);
            CloneInventories(destination);
        }

        public void CloneFiles(Inventory destination)
        {
            if (!Exists()) return;

            destination.WriteItem(ReadItems());
        }

        public void CloneInventories(Inventory destination)
        {
            if (!Exists()) return;

            foreach (Inventory inventory in ReadInventories())
            {
                string path = Path.Join(destination.path, inventory.GetName());
                Inventory destInv = new(path);

                destInv.WriteItem(inventory.ReadItems());
                destInv.WriteInventory(inventory.ReadInventories());
            }
        }

        public void Delete()
        {
            Directory.Delete(this.path);
        }
    }
}
