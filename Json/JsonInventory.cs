using System;
using System.Text.Json;
using kawtn.IO.Konfig;
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
    }
}
