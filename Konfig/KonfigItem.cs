using System;
using System.Text.Json;
using kawtn.IO.Serializable;

namespace kawtn.IO.Konfig
{
    public class KonfigItem<T> : SerializableItem<T>
    {
        public KonfigItem
            (
                string location
            )

            : base
            (
                location,
                serializer: (T data) => KonfigSerializer.Serialize<T>(data),
                deserializer: (string content) => KonfigSerializer.Deserialize<T>(content)
            )
        { }

        public KonfigItem(Location location)
            : this(location.Data) { }
    }
}
