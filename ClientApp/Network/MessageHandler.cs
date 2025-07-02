using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Network
{
    public class MessageHandler
    {
        private readonly Action<string>? _updateChat;
        private readonly Action<int, int>? _handleShot;
        private readonly Action<int, int, bool>? _handleShotResult;

        public MessageHandler(
            Action<string>? updateChat,
            Action<int, int>? handleShot,
            Action<int, int, bool>? handleShotResult)
        {
            _updateChat = updateChat;
            _handleShot = handleShot;
            _handleShotResult = handleShotResult;
        }

        public void Process(string message)
        {
            var parts = message.Split('|', 2);
            if (parts.Length < 2) return;

            string command = parts[0];
            string param = parts[1];

            switch (command)
            {
                case "CHAT":
                    _updateChat?.Invoke(param);
                    break;

                case "SHOT":
                    var coords = param.Split(',');
                    if (coords.Length == 2 &&
                        int.TryParse(coords[0], out int x) &&
                        int.TryParse(coords[1], out int y))
                    {
                        _handleShot?.Invoke(x, y);
                    }
                    break;

                case "RESULT":
                    // param: "x,y:HIT" або "x,y:MISS"
                    var resParts = param.Split(':');
                    if (resParts.Length == 2)
                    {
                        var shotCoords = resParts[0].Split(',');
                        if (shotCoords.Length == 2 &&
                            int.TryParse(shotCoords[0], out int rx) &&
                            int.TryParse(shotCoords[1], out int ry))
                        {
                            bool isHit = resParts[1].Equals("HIT", StringComparison.OrdinalIgnoreCase);
                            _handleShotResult?.Invoke(rx, ry, isHit);
                        }
                    }
                    break;

                default:
                    break;
            }
        }


    }
}
