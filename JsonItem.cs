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
        public JsonItem(string path) : base(path)
        {
            
        }

        public void Write(T data)
        {
            base.Write(JsonSerializer.Serialize(data));
        }

        public new T Read()
        {
            T? data = JsonSerializer.Deserialize<T>(base.Read());
            if (data == null) throw new NullReferenceException(this.path);

            return data;
        }

        public void Edit(Func<T, T> editor)
        {
            T read = Read();
            T data = editor.Invoke(read);

            Write(data);
        }
    }
}
