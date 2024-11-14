using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO
{
    // Path
    public class Location
    {
        public string Data { get; private set; }

        public Location(params string[] str)
        {
            this.Data = Path.GetFullPath(Path.Join(str));
        }

        public Location(Location baseLocation, params string[] str)
            : this(baseLocation.Data, Path.Join(str)) { }

        public Location(Item baseLocation, params string[] str)
            : this(baseLocation.Location.Data, Path.Join(str)) { }

        public Location(Inventory baseLocation, params string[] str)
            : this(baseLocation.Location.Data, Path.Join(str)) { }

        public bool IsExists()
        {
            return Path.Exists(this.Data);
        }

        public bool IsItem()
        {
            if (!IsExists()) return false;

            return !IsDirectory();
        }

        public bool IsDirectory()
        {
            if (!IsExists()) return false;

            return File.GetAttributes(this.Data).HasFlag(FileAttributes.Directory);
        }

        public Inventory? GetRoot()
        {
            string? path = Path.GetPathRoot(this.Data);

            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            else
            {
                return new(path);
            }
        }

        public Item ParseItem()
        {
            return new(this);
        }

        public Inventory ParseInventory()
        {
            return new(this);
        }
    }
}
