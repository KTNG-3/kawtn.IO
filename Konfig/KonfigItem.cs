using kawtn.IO.Serializable;

namespace kawtn.IO.Konfig
{
    public class KonfigItem<T> : SerializableItem<T>
    {
        public KonfigItem(string location)
            : base (location, new KonfigSerializer<T>()) { }

        public KonfigItem(Location location)
            : this(location.Data) { }
    }
}
