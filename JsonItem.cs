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

        public JsonItem(string path, T? defaultValue = default) : base(path)
        {
            this.defaultValue = defaultValue;
        }

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
            T? data = JsonSerializer.Deserialize<T>(ReadString());

            if (data != null)
            {
                return data;
            }

            if (this.defaultValue != null)
            {
                Write(defaultValue);

                return Read();
            }

            throw new NullReferenceException(this.path);
        }

        public void Edit(Func<T, T> editor)
        {
            T read = Read();
            T data = editor.Invoke(read);

            Write(data);
        }
    }
}
