using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace kawtn.IO.Json
{
    public class JsonItem<T> : StringItem
    {
        readonly T? defaultValue = default;

        public JsonItem(string location, T? defaultValue = default) : base(location)
        {
            this.defaultValue = defaultValue;
        }

        public JsonItem(Location location, T? defaultValue = default)
            : this(location.Data, defaultValue) { }

        public void Write(T data)
        {
            Write(JsonSerializer.Serialize(data));
        }

        public new T? Read()
        {
            string read = ReadString();

            if (string.IsNullOrWhiteSpace(read) && defaultValue != null)
            {
                Write(defaultValue);

                return Read();
            }

            try
            {
                return JsonSerializer.Deserialize<T>(read);
            }
            catch { }

            return default;
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
