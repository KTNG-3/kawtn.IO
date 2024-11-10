using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace kawtn.IO
{
    public class KeyValueFileManager : JsonFileManager<Dictionary<string, string>>
    {
        public KeyValueFileManager(string path) : base(path)
        {
            
        }

        public void Write(string key, string value)
        {
            base.Edit(x =>
            {
                x.Add(key, value);
                return x;
            });
        }

        public string Read(string key)
        {
            Dictionary<string, string> data = base.Read();
            if (!data.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value)) throw new NullReferenceException(key);

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
