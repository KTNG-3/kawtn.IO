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

        void Validate()
        {
            this.Data = Path.GetFullPath(this.Data);
        }

        public Location(params string[] str)
        {
            this.Data = Path.Join(str);

            Validate();
        }

        public Location(Location baseLocation, params string[] str)
        {
            this.Data = Path.Join(baseLocation.Data, Path.Join(str));

            Validate();
        }

        public Location(Item baseLocation, params string[] str)
        {
            this.Data = Path.Join(baseLocation.Location, Path.Join(str));

            Validate();
        }

        public Location(Inventory baseLocation, params string[] str)
        {
            this.Data = Path.Join(baseLocation.Location, Path.Join(str));

            Validate();
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
