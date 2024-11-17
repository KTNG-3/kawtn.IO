using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace kawtn.IO
{
    public class KeyValueItem : JsonItem<Dictionary<string, string>>
    {
        public KeyValueItem(string location) : base(location, defaultValue: new())
        {
            
        }

        public void Write(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value");

            base.Edit(x =>
            {
                x[key] = value;

                return x;
            });
        }

        public string Read(string key)
        {
            Dictionary<string, string> data = base.Read();
            if (!data.TryGetValue(key, out string? value)) throw new NullReferenceException(key);

            return value;
        }

        public void Delete(string key)
        {
            base.Edit(x =>
            {
                x.Remove(key);

                return x;
            });
        }
    }
}
