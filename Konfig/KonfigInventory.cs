using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kawtn.IO.Json;

namespace kawtn.IO.Konfig
{
    public class KonfigInventory<T> : Inventory
    {
        public KonfigInventory(string location)
            : base(location) { }

        public KonfigInventory(Location location)
            : this(location.Data) { }

        public bool IsExists(string name)
        {
            Item item = base.CreateItem(name);

            return item.IsExists();
        }

        public new KonfigItem<T> CreateItem(string name)
        {
            Location location = new(this, name);

            return new KonfigItem<T>(location);
        }

        public void Write(string name, T data)
        {
            KonfigItem<T> item = CreateItem(name);

            item.Write(data);
        }

        public new T[] Read()
        {
            List<T> list = new();

            foreach (Item baseItem in ReadItems())
            {
                KonfigItem<T> item = new(baseItem.Location);

                T? data = item.Read();
                if (data != null)
                {
                    list.Add(data);
                }
            }

            return list.ToArray();
        }

        public T? Read(string name)
        {
            KonfigItem<T> item = CreateItem(name);

            return item.Read();
        }

        public void Edit(string name, Func<T, T> editor)
        {
            KonfigItem<T> item = CreateItem(name);

            item.Edit(editor);
        }

        public void Delete(string name)
        {
            Item item = base.CreateItem(name);

            item.Delete();
        }
    }
}
