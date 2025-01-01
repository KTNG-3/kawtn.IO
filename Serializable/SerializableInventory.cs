using System;
using System.Collections.Generic;

namespace kawtn.IO.Serializable
{
    public class SerializableInventory<TValue> : SerializableInventory<string, TValue>
    {
        public SerializableInventory(string location, Serializer<TValue> serializer)
            : base(location, serializer) { }

        public SerializableInventory(Location location, Serializer<TValue> serializer)
            : base(location, serializer) { }
    }

    public class SerializableInventory<TKey, TValue> : Inventory
        where TKey : IConvertible
    {
        protected readonly Serializer<TValue> Serializer;

        public SerializableInventory(string location, Serializer<TValue> serializer)
            : base(location)
        {
            this.Serializer = serializer;
        }

        public SerializableInventory(Location location, Serializer<TValue> serializer)
            : this(location.Data, serializer) { }

        public SerializableItem<TValue> CreateSerializableItem(TKey name)
        {
            Location location = new Location(this, $"{name.ToString()}{this.ItemExtension}");

            return new SerializableItem<TValue>(location, Serializer);
        }

        public SerializableInventory<TKey, TValue> CreateSerializableInventory(TKey name)
        {
            Location location = new Location(this, name.ToString());

            SerializableInventory<TKey, TValue> inventory =
                new SerializableInventory<TKey, TValue>(location, Serializer);

            inventory.ItemExtension = this.ItemExtension;

            return inventory;
        }

        public bool IsExists(TKey name)
        {
            Item item = CreateItem(name.ToString());

            return item.IsExists();
        }

        public void Write(TKey name, TValue data)
        {
            SerializableItem<TValue> item = CreateSerializableItem(name);

            item.Write(data);
        }

        public new IEnumerable<TValue> Read()
        {
            List<TValue> list = new List<TValue>();

            foreach (Item baseItem in ReadItems())
            {
                SerializableItem<TValue> item
                    = new SerializableItem<TValue>(baseItem.Location, this.Serializer);

                TValue? data = item.Read();
                if (data != null)
                {
                    list.Add(data);
                }
            }

            return list;
        }

        public TValue? Read(TKey name)
        {
            SerializableItem<TValue> item = CreateSerializableItem(name);

            return item.Read();
        }

        public void Edit(TKey name, Func<TValue, TValue> editor)
        {
            SerializableItem<TValue> item = CreateSerializableItem(name);

            item.Edit(editor);
        }

        public void Delete(TKey name)
        {
            Item item = CreateItem(name.ToString());

            item.Delete();
        }
    }
}
