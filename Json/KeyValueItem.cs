using System;
using System.Collections.Generic;
using kawtn.IO.Serializable;

namespace kawtn.IO.Json
{
    public class KeyValueItem<TKey, TValue> : SerializableItem<Dictionary<TKey, TValue>>
    {
        public KeyValueItem(string location)
            : base(location, new KeyValueSerializer<TKey, TValue>()) { }

        public KeyValueItem(Location location)
            : this(location.Data) { }

        public bool IsExists(TKey key)
        {
            Dictionary<TKey, TValue>? data = base.Read();
            if (data == null)
            {
                return false;
            }

            return data.ContainsKey(key);
        }

        public void Write(TKey key, TValue value)
        {
            this.Edit(x =>
            {
                x[key] = value;

                return x;
            });
        }

        public TValue? Read(TKey key)
        {
            Dictionary<TKey, TValue>? data = base.Read();
            if (data == null)
            {
                return default;
            }

            if (!data.TryGetValue(key, out TValue? value))
            {
                return default;
            }

            return value;
        }

        public void Edit(TKey key, Func<TValue, TValue> editor)
        {
            TValue? read = this.Read(key);
            if (read == null) return;

            TValue value = editor.Invoke(read);
            this.Write(key, value);
        }

        public void Delete(TKey key)
        {
            this.Edit(x =>
            {
                x.Remove(key);

                return x;
            });
        }
    }
}
