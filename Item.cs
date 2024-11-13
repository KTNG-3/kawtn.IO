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
        public readonly Location Location;

        public Item(Location location)
        {
            this.Location = location;

            Create();
        }

        public Item(string location) 
            : this(new Location(location)) { }

        public string GetName()
        {
            return Path.GetFileName(this.Location.Data) ?? string.Empty;
        }

        public Inventory GetInventory()
        {
            return new(Path.GetDirectoryName(this.Location.Data) ?? string.Empty);
        }

        public bool IsExists()
        {
            return File.Exists(this.Location.Data);
        }

        public void Create()
        {
            if (IsExists()) return;

            GetInventory().Create();

            File.WriteAllBytes(this.Location.Data, Array.Empty<byte>());
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

            File.Delete(this.Location.Data);
        }
    }
}
