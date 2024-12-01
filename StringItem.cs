using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO
{
    public class StringItem : Item
    {
        public StringItem(string location)
            : base(location) { }

        public StringItem(Location location)
            : this(location.Data) { }

        public void Write(string data)
        {
            Create();

            File.WriteAllText(Location.Data, data);
        }

        public new string Read()
        {
            if (File.Exists(Location.Data))
            {
                return File.ReadAllText(Location.Data);

            }
            else
            {
                return string.Empty;
            }
        }

        public string ReadString()
        {
            return Read();
        }
    }
}
