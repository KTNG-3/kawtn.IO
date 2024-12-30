using System;
using System.Collections.Generic;

namespace kawtn.IO.Konfig
{
    public class KonfigInventory<T> : Inventory
        where T : class
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

        public void Write(string name, T data)
        {
            KonfigItem<T> item = CreateKonfigItem<T>(name);

            item.Write(data);
        }

        public new T[] Read()
        {
            List<T> list = new List<T>();

            foreach (Item baseItem in ReadItems())
            {
                KonfigItem<T> item = new KonfigItem<T>(baseItem.Location);

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
            KonfigItem<T> item = CreateKonfigItem<T>(name);

            return item.Read();
        }

        public void Edit(string name, Func<T, T> editor)
        {
            KonfigItem<T> item = CreateKonfigItem<T>(name);

            item.Edit(editor);
        }

        public void Delete(string name)
        {
            Item item = base.CreateItem(name);

            item.Delete();
        }
    }
}
