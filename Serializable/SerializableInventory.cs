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
            Location location = new(this, $"{name}{this.ItemExtension}");

            return new SerializableItem<TValue>(location, this.Serializer);
        }

        public SerializableInventory<TKey, TValue> CreateSerializableInventory(TKey name)
        {
            Location location = new(this, name.ToString());

            return new SerializableInventory<TKey, TValue>(location, this.Serializer)
            {
                ItemExtension = this.ItemExtension
            };
        }

        public bool IsExists(TKey name)
        {
            Item item = this.CreateItem(name.ToString());

            return item.IsExists();
        }

        public void Write(TKey name, TValue data)
        {
            SerializableItem<TValue> item = this.CreateSerializableItem(name);

            item.Write(data);
        }

        public new IEnumerable<TValue> Read()
        {
            List<TValue> list = new();

            foreach (Item baseItem in this.ReadItems())
            {
                SerializableItem<TValue> item = new(baseItem.Location, this.Serializer);

                TValue? data = item.Read();

                if (data != null)
                {
                    list.Add(data);
                }
            }

            return list;
        }

        public IEnumerable<TValue> ReadOrThrow()
        {
            List<TValue> list = new();

            foreach (Item baseItem in this.ReadItems())
            {
                SerializableItem<TValue> item = new(baseItem.Location, this.Serializer);

                TValue data = item.ReadOrThrow();

                list.Add(data);
            }

            return list;
        }

        public TValue? Read(TKey name)
        {
            SerializableItem<TValue> item = this.CreateSerializableItem(name);

            return item.Read();
        }

        public TValue ReadOrThrow(TKey name)
        {
            SerializableItem<TValue> item = this.CreateSerializableItem(name);

            return item.ReadOrThrow();
        }

        public void Edit(TKey name, Func<TValue, TValue> editor)
        {
            SerializableItem<TValue> item = this.CreateSerializableItem(name);

            item.Edit(editor);
        }

        public void Delete(TKey name)
        {
            Item item = this.CreateItem(name.ToString());

            item.Delete();
        }
    }
}
