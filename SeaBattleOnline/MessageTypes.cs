using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    public class MessageTypes
    {
        public const string CONNECT = "CONNECT";             // Підключення клієнта (з іменем)
        public const string READY = "READY";
        public const string START = "START";                 // Старт гри
        public const string TURN = "TURN";                   // Хід гравця ("YOUR", "WAIT")
        public const string SHOT = "SHOT";                   // Постріл (наприклад, SHOT|3,4)
        public const string RESULT = "RESULT";               // Результат пострілу (наприклад, RESULT|HIT, MISS, KILL)
        public const string CHAT = "CHAT";                   // Чат повідомлення
        public const string RESTART_REQUEST = "RESTART";     // Запит на перезапуск гри
        public const string GAME_OVER = "GAMEOVER";          // Завершення гри (переможець)
        public const string DISCONNECTED = "DISCONNECTED";   // Відключення гравця
        public const string ERROR = "ERROR";                 // Повідомлення про помилку
        public const string WAITING = "WAITING";
        public const string SYSYTEM = "SYSTEM";               // Для системних повідомлень// Очікування іншого гравця
        public const string YOUR_TURN = "YOUR_TURN";
        public const string OPPONENT_TURN = "OPPONENT_TURN";
        public const string MISS = "MISS";
    }
}
