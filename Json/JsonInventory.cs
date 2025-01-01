using System;
using kawtn.IO.Serializable;

namespace kawtn.IO.Json
{
    public class JsonInventory<TValue> : JsonInventory<string, TValue>
    {
        public JsonInventory(string location)
            : base(location) { }

        public JsonInventory(Location location)
            : base(location) { }
    }

    public class JsonInventory<TKey, TValue> : SerializableInventory<TKey, TValue>
        where TKey : IConvertible
    {
        public JsonInventory(string location)
            : base(location, new JsonSerializer<TValue>()) { }

        public JsonInventory(Location location)
            : this(location.Data) { }

        public JsonItem<TValue> CreateJsonItem(string name)
        {
            Location location = new(this, $"{name}{this.ItemExtension}");

            return new JsonItem<TValue>(location);
        }

        public JsonInventory<TKey, TValue> CreateJsonInventory(string name)
        {
            Location location = new(this, name);

            return new JsonInventory<TKey, TValue>(location)
            {
                ItemExtension = this.ItemExtension
            };
        }
    }
}
