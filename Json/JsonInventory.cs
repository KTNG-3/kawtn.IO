using System;
using System.Collections.Generic;

namespace kawtn.IO.Json
{
    public class JsonInventory<T> : Inventory
        where T : class
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

        public void Write(string name, T data)
        {
            JsonItem<T> item = CreateJsonItem<T>(name);

            item.Write(data);
        }

        public new T[] Read()
        {
            List<T> list = new List<T>();

            foreach (Item baseItem in ReadItems())
            {
                JsonItem<T> item = new JsonItem<T>(baseItem.Location);

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
            JsonItem<T> item = CreateJsonItem<T>(name);

            return item.Read();
        }

        public void Edit(string name, Func<T, T> editor)
        {
            JsonItem<T> item = CreateJsonItem<T>(name);

            item.Edit(editor);
        }

        public void Delete(string name)
        {
            Item item = base.CreateItem(name);

            item.Delete();
        }
    }
}
