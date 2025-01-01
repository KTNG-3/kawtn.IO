using System.Text.Json;
using kawtn.IO.Serializable;

namespace kawtn.IO.Json
{
    class JsonSerializer<T> : Serializer<T>
    {
        public override string Serialize(T data)
        {
            return JsonSerializer.Serialize<T>(data);
        }

        public override T? Deserialize(string content)
        {
            return JsonSerializer.Deserialize<T>(content);
        }
    }
}
