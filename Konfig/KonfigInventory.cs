using System;
using System.Collections.Generic;
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
        public KonfigInventory
            (
                string location
            )

            : base
            (
                location,
                serializer: (TValue data) => KonfigSerializer.Serialize<TValue>(data),
                deserializer: (string content) => KonfigSerializer.Deserialize<TValue>(content)
            )
        { }

        public KonfigInventory(Location location)
            : this(location.Data) { }
    }
}
