using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace kawtn.IO
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
            base.Write(JsonSerializer.Serialize(data));
        }

        public string ReadString()
        {
            return base.Read();
        }

        public new T Read()
        {
            string read = ReadString();

            if (string.IsNullOrWhiteSpace(read) && this.defaultValue != null)
            {
                Write(defaultValue);

                return Read();
            }

            T? data = JsonSerializer.Deserialize<T>(read);

            if (data != null)
            {
                return data;
            }

            throw new NullReferenceException(this.Location.Data);
        }

        public void Edit(Func<T, T> editor)
        {
            T read = Read();
            T data = editor.Invoke(read);

            Write(data);
        }
    }
}
