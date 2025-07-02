using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientApp.Utils
{
    public static class Serializer
    {
        // Серіалізація об'єкта у байти (через JSON + UTF8)
        public static byte[] SerializeToBytes<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            string json = JsonSerializer.Serialize(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        // Десеріалізація об'єкта з байтів
        public static T DeserializeFromBytes<T>(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            string json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException("Deserialization returned null");
        }
    }
}
