using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace ServerApp
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly Server _server;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        public Models.PlayerInfo Info { get; private set; }

        public string PlayerName { get; private set; } = "Unknown";
        public ClientHandler Opponent { get; set; }
        public bool InGame => Opponent != null;

        public ClientHandler(TcpClient client, Server server)
        {
            _client = client;
            _server = server;

            string ip = ((System.Net.IPEndPoint)_client.Client.RemoteEndPoint).Address.ToString();
            Info = new Models.PlayerInfo("Unknown", ip);
        }

        public void Handle()
        {
            try
            {
                _stream = _client.GetStream();
                _reader = new StreamReader(_stream, Encoding.UTF8);
                _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };

                // Отримати ім’я гравця
                PlayerName = _reader.ReadLine();
                Info.Name = PlayerName;
                Console.WriteLine($"[SERVER] Гравець підключився: {PlayerName}");

                // Шукати суперника
                Opponent = _server.FindOpponent(this);

                if (Opponent != null)
                {
                    // Пов'язуємо гравців
                    Opponent.Opponent = this;

                    Console.WriteLine($"[GAME] Старт гри між {PlayerName} та {Opponent.PlayerName}");

                    Opponent.SendMessage($"START|{PlayerName}");
                    SendMessage($"START|{Opponent.PlayerName}");

                    // Гравець ініціатор починає першим
                    Opponent.SendMessage("TURN|WAIT");
                    SendMessage("TURN|YOUR");
                }
                else
                {
                    SendMessage("WAITING|Очікуємо іншого гравця...");
                }

                // Основний цикл обробки
                while (_client.Connected)
                {
                    string? message = _reader.ReadLine();
                    if (message == null) break;

                    Console.WriteLine($"[MSG] {PlayerName}: {message}");

                    // Передаємо повідомлення супернику
                    if (InGame && Opponent != null)
                    {
                        Opponent.SendMessage(message);
                    }

                    // Обробка запиту на нову гру
                    if (message.StartsWith("RESTART"))
                    {
                        Opponent?.SendMessage("RESTART");
                    }

                    // Чат повідомлення або хід, тощо
                    // Тут можна додати додаткову обробку
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {PlayerName}: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"[SERVER] {PlayerName} відключився");
                _server.RemoveClient(this);
                Opponent?.SendMessage("DISCONNECTED");
                Opponent = null;
                _client.Close();
            }
        }

        public void SendMessage(string msg)
        {
            try
            {
                _writer.WriteLine(msg);
            }
            catch
            {
                // Ігнорувати, якщо вже закрито
            }
        }
    }
}
