using System;

namespace kawtn.IO.Serializable
{
    public class SerializableItem<T> : StringItem
    {
        protected readonly Serializer<T> Serializer;

        public SerializableItem(string location, Serializer<T> serializer)
            : base(location)
        {
            this.Serializer = serializer;
        }

        public SerializableItem(Location location, Serializer<T> serializer)
            : this(location.Data, serializer) { }

        public void Write(T data)
        {
            if (!Serializer.Validate(data)) return;

            Write(Serializer.Serialize(data));
        }

        public new T? Read()
        {
            if (!IsExists())
            {
                return default;
            }


            string read = ReadString();

            if (string.IsNullOrWhiteSpace(read) && Serializer.DefaultValue != null)
            {
                Write(Serializer.DefaultValue);

                return Read();
            }

            return Serializer.Deserialize(read);
        }

        public void Edit(Func<T, T> editor)
        {
            T? read = Read();
            if (read == null) return;

            T data = editor.Invoke(read);
            Write(data);
        }
    }
}
