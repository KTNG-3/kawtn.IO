using System;
using System.Collections.Generic;
using System.Text;

namespace kawtn.IO.Serializable
{
    public class SerializableItem<T> : StringItem
    {
        protected readonly Func<T, string> Serialize;
        protected readonly Func<string, T?> Deserialize;

        protected readonly T? DefaultValue;

        public SerializableItem
            (
                string location,
                Func<T, string> serializer,
                Func<string, T?> deserializer,
                T? defaultValue = default
            )

            : base
            (
                location
            )
        {
            this.Serialize = serializer;
            this.Deserialize = deserializer;

            this.DefaultValue = defaultValue;
        }

        public SerializableItem
            (
                Location location,
                Func<T, string> serializer,
                Func<string, T?> deserializer,
                T? defaultValue = default
            )

            : this
            (
                location.Data,
                serializer,
                deserializer,
                defaultValue
            ) 
        { }

        public void Write(T data)
        {
            Write(Serialize.Invoke(data));
        }

        public new T? Read()
        {
            if (!IsExists()) 
                return default;

            string read = ReadString();

            if (string.IsNullOrWhiteSpace(read) && DefaultValue != null)
            {
                Write(DefaultValue);

                return Read();
            }

            return Deserialize.Invoke(read);
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
