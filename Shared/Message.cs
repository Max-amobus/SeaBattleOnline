using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Message
    {
        public MessageType Type { get; set; }
        public string Data { get; set; }

        public Message() { }

        public Message(MessageType type, string data)
        {
            Type = type;
            Data = data;
        }

        public override string ToString()
        {
            return $"{Type}|{Data}";
        }

        public static Message Parse(string rawMessage)
        {
            var parts = rawMessage.Split('|', 2);
            if (parts.Length != 2) return new Message(MessageType.Unknown, rawMessage);

            if (!Enum.TryParse(parts[0], out MessageType type))
                type = MessageType.Unknown;

            return new Message(type, parts[1]);
        }
    }
}
