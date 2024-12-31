using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using kawtn.IO.Json;

namespace kawtn.IO.Serializable
{
    public class SerializableInventory<TValue> : SerializableInventory<string, TValue>
    {
        public SerializableInventory
            (
                string location,
                Func<TValue, string> serializer,
                Func<string, TValue?> deserializer,
                TValue? defaultValue = default
            )

            : base
            (
                location,
                serializer,
                deserializer,
                defaultValue
            )
        { }

        public SerializableInventory
            (
                Location location,
                Func<TValue, string> serializer,
                Func<string, TValue?> deserializer,
                TValue? defaultValue = default
            )

            : base
            (
                location,
                serializer,
                deserializer,
                defaultValue
            )
        { }
    }

    public class SerializableInventory<TKey, TValue> : Inventory
        where TKey : IConvertible
    {
        protected readonly Func<TValue, string> Serialize;
        protected readonly Func<string, TValue?> Deserialize;

        protected readonly TValue? DefaultValue;

        public SerializableInventory
            (
                string location,
                Func<TValue, string> serializer,
                Func<string, TValue?> deserializer,
                TValue? defaultValue = default
            )

            : base
            (
                location
            )
        {
            this.Serialize = serializer;
            this.Deserialize = deserializer;

            this.DefaultValue = defaultValue;
        }

        public SerializableInventory
            (
                Location location,
                Func<TValue, string> serializer,
                Func<string, TValue?> deserializer,
                TValue? defaultValue = default
            )

            : this
            (
                location.Data,
                serializer,
                deserializer,
                defaultValue
            )
        { }

        public SerializableItem<TValue> CreateSerializableItem(TKey name)
        {
            Location location = new Location(this, $"{name.ToString()}{ItemExtension}");

            return new SerializableItem<TValue>(location, this.Serialize, this.Deserialize, this.DefaultValue);
        }

        public SerializableInventory<TKey, TValue> CreateSerializableInventory(TKey name)
        {
            Location location = new Location(this, name.ToString());

            SerializableInventory<TKey, TValue> inventory =
                new SerializableInventory<TKey, TValue>(location, this.Serialize, this.Deserialize, this.DefaultValue);

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

        public IEnumerable<TValue> Read()
        {
            List<TValue> list = new List<TValue>();

            foreach (Item baseItem in ReadItems())
            {
                SerializableItem<TValue> item 
                    = new SerializableItem<TValue>(baseItem.Location, this.Serialize, this.Deserialize, this.DefaultValue);

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
