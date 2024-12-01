using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO.Konfig
{
    public class KonfigItem<T> : StringItem
    {
        public KonfigItem(string location) 
            : base(location) { }

        public KonfigItem(Location location)
            : base(location) { }

        public void Write(T data)
        {
            Write(KonfigSerializer.Serialize(data));
        }

        public new T? Read()
        {
            string read = this.ReadString();

            try
            {
                return KonfigSerializer.Deserialize<T>(read);
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
