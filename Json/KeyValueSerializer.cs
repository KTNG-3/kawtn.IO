using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
