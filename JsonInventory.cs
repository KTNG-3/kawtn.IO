using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace kawtn.IO
{
    public class JsonInventory<T> : Inventory
    {
        public JsonInventory(string location)
            : base(location) { }

        public JsonInventory(Location location)
            : this(location.Data) { }

        public bool IsExists(string name)
        {
            Item item = base.CreateItem(name);

            return item.IsExists();
        }

        public new JsonItem<T> CreateItem(string name)
        {
            Location location = new(this, name);

            return new JsonItem<T>(location);
        }

        public void Write(string name, T data)
        {
            JsonItem<T> item = this.CreateItem(name);

            item.Write(data);
        }

        public new T[] Read()
        {
            List<T> list = new();

            foreach (Item baseItem in base.ReadItems())
            {
                JsonItem<T> item = new(baseItem.Location);

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
            JsonItem<T> item = this.CreateItem(name);

            return item.Read();
        }

        public void Edit(string name, Func<T, T> editor)
        {
            JsonItem<T> item = this.CreateItem(name);

            item.Edit(editor);
        }

        public void Delete(string name)
        {
            Item item = base.CreateItem(name);

            item.Delete();
        }
    }
}
