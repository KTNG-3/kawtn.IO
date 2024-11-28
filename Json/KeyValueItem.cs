using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace kawtn.IO.Json
{
    public class KeyValueItem : JsonItem<Dictionary<string, string>>
    {
        public KeyValueItem(string location)
            : base(location, defaultValue: new()) { }

        public KeyValueItem(Location location)
            : this(location.Data) { }

        public bool IsExists(string key)
        {
            Dictionary<string, string>? data = Read();
            if (data == null) return false;

            return data.ContainsKey(key);
        }

        public void Write(string key, string value)
        {
            Edit(x =>
            {
                x[key] = value;

                return x;
            });
        }

        public string? Read(string key)
        {
            Dictionary<string, string>? data = Read();
            if (data == null) return null;
            if (!data.TryGetValue(key, out string? value)) return null;

            return value;
        }

        public void Delete(string key)
        {
            Edit(x =>
            {
                x.Remove(key);

                return x;
            });
        }
    }
}
