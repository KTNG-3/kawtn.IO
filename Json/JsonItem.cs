using kawtn.IO.Serializable;

namespace kawtn.IO.Json
{
    public class JsonItem<T> : SerializableItem<T>
    {
        public JsonItem(string location)
            : base(location, new JsonSerializer<T>()) { }

        public JsonItem(Location location)
            : this(location.Data) { }
    }
}
