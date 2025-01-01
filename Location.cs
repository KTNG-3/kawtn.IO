using System.IO;
using System.Linq;

namespace kawtn.IO
{
    // Path
    public class Location
    {
        public readonly string Data;

        public Location(params string[] path)
        {
            this.Data = Path.GetFullPath(Join(path));
        }

        public Location(Location baseLocation, params string[] path)
            : this(baseLocation.Data, Join(path)) { }

        public Location(Item baseLocation, params string[] path)
            : this(baseLocation.Location.Data, Join(path)) { }

        public Location(Inventory baseLocation, params string[] path)
            : this(baseLocation.Location.Data, Join(path)) { }

        public static string Join(params string[] path)
        {
            if (path.Length == 0)
            {
                return string.Empty;
            }

            string joinStr = path[0];

            for (int i = 1; i < path.Length; i++)
            {
                joinStr = Path.Join(joinStr, path[i]);
            }

            return joinStr;
        }

        public static string Join(params Location[] location)
        {
            return Location.Join(location.Select(x => x.Data).ToArray());
        }

        public bool IsExists()
        {
            return this.IsItem() || this.IsInventory();
        }

        public bool IsItem()
        {
            return File.Exists(this.Data);
        }

        public bool IsInventory()
        {
            return Directory.Exists(this.Data);
        }

        public Inventory? GetRoot()
        {
            string? path = Path.GetPathRoot(this.Data);
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            return new Inventory(path);
        }
    }
}
