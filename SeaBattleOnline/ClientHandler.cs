using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using ServerApp.Models;

namespace ServerApp
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly Server _server;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        public CellState[,] GameBoard = new CellState[10, 10]; 
        public HashSet<Point> HitShots = new();

        public string PlayerName { get; private set; } = "Unknown";
        public PlayerInfo Info { get; private set; }
        public ClientHandler Opponent { get; set; }
        public GameSession Session { get; set; }
        public bool IsReady { get; private set; }
        public bool InGame => Opponent != null;

        public ClientHandler(TcpClient client, Server server)
        {
            _client = client;
            _server = server;
            string ip = ((System.Net.IPEndPoint)_client.Client.RemoteEndPoint).Address.ToString();
            Info = new PlayerInfo("Unknown", ip);
        }
        public enum CellState
        {
            Empty,      // Порожня
            Ship,       // Корабель
            Hit,        // Вражена
            Miss        // Промах
        }

        public void Handle()
        {
            try
            {
                _stream = _client.GetStream();
                _reader = new StreamReader(_stream, Encoding.UTF8);
                _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };

                string firstLine = _reader.ReadLine();
                if (firstLine?.StartsWith("CONNECT|") == true)
                {
                    PlayerName = firstLine.Substring(8);
                    Info.Name = PlayerName;
                    Console.WriteLine($"[SERVER] Гравець підключився: {PlayerName}");
                }

                while (_client.Connected)
                {
                    string message = _reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(message)) continue;

                    Console.WriteLine($"[ПОВІДОМЛЕННЯ ВІД {PlayerName}] {message}");

                    if (message == "READY")
                    {
                        IsReady = true;
                        _server.PairPlayers(this);
                    }
                    else if (message.StartsWith("CHAT|"))
                    {
                        _server.Broadcast($"{MessageTypes.CHAT}|{message.Substring(5)}", this);
                    }
                    else if (message.StartsWith("SHOT|"))
                    {
                        Session?.HandleMove(this, message);
                    }
                    else if (message.StartsWith("RESULT|"))
                    {
                        var data = message.Substring(7);
                        Session?.HandleShotResult(this, data);
                    }
                    else if (message.StartsWith("RESTART"))
                    {
                        Session?.HandleRestartRequest(this);
                    }
                    else if (message.StartsWith("SHIPS|"))
                    {
                        Session?.RegisterShips(this, message.Substring(6));
                    }
                    else if (message == "DISCONNECT")
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {PlayerName}: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"[SERVER] {PlayerName} відключився");
                Session?.EndSession();
                _server.RemoveClient(this);
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
                // Ігнор помилок надсилання
            }
        }
    }
}
