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
        public readonly string Location;

        public Item(string location)
        {
            this.Location = new Location(location).Data;
        }

        public Item(Location location)
        {
            this.Location = location.Data;
        }

        public string GetName()
        {
            return Path.GetFileName(this.Location) ?? string.Empty;
        }

        public Inventory GetInventory()
        {
            return new(Path.GetDirectoryName(Location) ?? string.Empty);
        }

        public bool IsExists()
        {
            return File.Exists(this.Location);
        }

        public void Create()
        {
            if (IsExists()) return;

            GetInventory().Create();

            File.WriteAllBytes(Location, Array.Empty<byte>());
        }

        public void Write(byte[] data)
        {
            Create();

            File.WriteAllBytes(Location, data);
        }

        public byte[] Read()
        {
            if (IsExists())
            {
                return File.ReadAllBytes(this.Location);
            }
            else
            {
                return Array.Empty<byte>();
            }
        }

        public void Unzip(Inventory destination)
        {
            destination.Create();
            ZipFile.ExtractToDirectory(this.Location, destination.Location);
        }

        public void Clone(Item destination)
        {
            if (!IsExists()) return;

            destination.Write(Read());
        }

        public void Move(Item destination)
        {
            Clone(destination);
            Delete();
        }

        public Item InsertTo(Inventory destination)
        {
            Item item = new Location(destination, GetName()).ParseItem();

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

            File.Delete(this.Location);
        }
    }
}
