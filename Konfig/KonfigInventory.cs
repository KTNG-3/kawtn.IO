using System;
using kawtn.IO.Serializable;

namespace kawtn.IO.Konfig
{
    public class KonfigInventory<TValue> : KonfigInventory<string, TValue>
    {
        public KonfigInventory(string location)
            : base(location) { }

        public KonfigInventory(Location location)
            : base(location) { }
    }

    public class KonfigInventory<TKey, TValue> : SerializableInventory<TKey, TValue>
        where TKey : IConvertible
    {
        public KonfigInventory(string location)
            : base(location, new KonfigSerializer<TValue>()) { }

        public KonfigInventory(Location location)
            : this(location.Data) { }

        public KonfigItem<TValue> CreateKonfigItem(string name)
        {
            Location location = new(this, $"{name}{this.ItemExtension}");

            return new KonfigItem<TValue>(location);
        }

        public KonfigInventory<TKey, TValue> CreateKonfigInventory(string name)
        {
            Location location = new(this, name);

            return new KonfigInventory<TKey, TValue>(location)
            {
                ItemExtension = this.ItemExtension
            };
        }
    }
}
