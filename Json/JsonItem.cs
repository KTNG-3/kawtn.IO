using System;
using System.Text.Json;

namespace kawtn.IO.Json
{
    public class JsonItem<T> : StringItem 
        where T : class
    {
        readonly T? defaultValue = default;

        public JsonItem(string location) 
            : base(location) { }

        public JsonItem(Location location)
            : base(location.Data) { }

        public JsonItem(string location, T defaultValue) : base(location)
        {
            this.defaultValue = defaultValue;
        }

        public JsonItem(Location location, T defaultValue)
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
