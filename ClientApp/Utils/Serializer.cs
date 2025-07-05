using System;
using System.Text;
using System.Text.Json;

namespace ClientApp.Utils
{
    public static class Serializer
    {
        private class Wrapper
        {
            public string Type { get; set; } = "";
            public string Payload { get; set; } = "";
        }

        public static byte[] SerializeToBytes(object obj)
        {
            var wrapper = new Wrapper
            {
                Type = obj.GetType().Name,
                Payload = JsonSerializer.Serialize(obj)
            };

            string json = JsonSerializer.Serialize(wrapper);
            return Encoding.UTF8.GetBytes(json);
        }

        public static string PeekType(byte[] data)
        {
            try
            {
                string json = Encoding.UTF8.GetString(data);
                var wrapper = JsonSerializer.Deserialize<Wrapper>(json);
                return wrapper?.Type ?? "";
            }
            catch
            {
                return "";
            }
        }

        public static T DeserializeFromBytes<T>(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            var wrapper = JsonSerializer.Deserialize<Wrapper>(json);
            return JsonSerializer.Deserialize<T>(wrapper?.Payload ?? "")!;
        }
    }
}
