using System.IO;
using System.Linq;

using kawtn.IO.Json;
using kawtn.IO.Konfig;

namespace kawtn.IO
{
    // Path
    public class Location
    {
        public string Data { get; private set; }

        public Location(params string[] str)
        {
            this.Data = Path.GetFullPath(Join(str));
        }

        public Location(Location baseLocation, params string[] str)
            : this(baseLocation.Data, Join(str)) { }

        public Location(Item baseLocation, params string[] str)
            : this(baseLocation.Location.Data, Join(str)) { }

        public Location(Inventory baseLocation, params string[] str)
            : this(baseLocation.Location.Data, Join(str)) { }

        public static string Join(params string[] str)
        {
            if (str.Length == 0) return string.Empty;

            string joinStr = str[0];

            for (int i = 1; i < str.Length; i++)
            {
                joinStr = Path.Join(joinStr, str[i]);
            }

            return joinStr;
        }

        public static string Join(params Location[] str)
        {
            return Join(str.Select(x => x.Data).ToArray());
        }

        public bool IsExists()
        {
            return IsItem() || IsInventory();
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
            else
            {
                return new Inventory(path);
            }
        }

        public Item ParseItem()
        {
            return new Item(this);
        }

        public JsonItem<T> ParseJsonItem<T>()
            where T : class
        {
            return new JsonItem<T>(this);
        }

        public KonfigItem<T> ParseKonfigItem<T>()
            where T : class
        {
            return new KonfigItem<T>(this);
        }

        public Inventory ParseInventory()
        {
            return new Inventory(this);
        }

        public JsonInventory<T> ParseJsonInventory<T>()
            where T : class
        {
            return new JsonInventory<T>(this);
        }

        public KonfigInventory<T> ParseKonfigInventory<T>()
            where T : class
        {
            return new KonfigInventory<T>(this);
        }
    }
}
