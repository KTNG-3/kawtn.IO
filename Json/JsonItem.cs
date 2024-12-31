using System.Text.Json;
using kawtn.IO.Serializable;

namespace kawtn.IO.Json
{
    public class JsonItem<T> : SerializableItem<T>
    {
        public JsonItem
            (
                string location,
                T? defaultValue = default
            )

            : base
            (
                location,
                serializer: (T data) => JsonSerializer.Serialize<T>(data),
                deserializer: (string content) => JsonSerializer.Deserialize<T>(content),
                defaultValue
            )
        { }

        public JsonItem(Location location, T? defaultValue = default)
            : this(location.Data, defaultValue) { }
    }
}
