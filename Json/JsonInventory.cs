using System;
using System.Collections.Generic;
using System.Text.Json;
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
        public JsonInventory
            (
                string location
            )

            : base
            (
                location,
                serializer: (TValue data) => JsonSerializer.Serialize<TValue>(data),
                deserializer: (string content) => JsonSerializer.Deserialize<TValue>(content)
            )
        { }

        public JsonInventory(Location location)
            : this(location.Data) { }
    }
}
