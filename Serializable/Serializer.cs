using System;
using System.Collections.Generic;
using System.Text;

namespace kawtn.IO.Serializable
{
    public abstract class Serializer<T>
    {
        public T? DefaultValue { get; protected set; } = default;

        public abstract string Serialize(T data);
        public abstract T? Deserialize(string content);

        public bool Validate(T data)
        {
            return true;
        }
    }
}
