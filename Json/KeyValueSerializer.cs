using System.Collections.Generic;

namespace kawtn.IO.Json
{
    class KeyValueSerializer<TKey, TValue> : JsonSerializer<Dictionary<TKey, TValue>>
    {
        public KeyValueSerializer()
        {
            this.DefaultValue = new Dictionary<TKey, TValue>();
        }
    }
}
