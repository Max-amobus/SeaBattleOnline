using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerApp
{
    public class Server
    {
        private readonly int _port;
        private TcpListener _listener;
        private readonly List<ClientHandler> _connectedClients = new();
        private readonly List<GameSession> _sessions = new();
        private readonly object _lock = new();

        public Server(int port)
        {
            _port = port;
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"[СЕРВЕР] Запущено на порту {_port}");

            while (true)
            {
                var client = _listener.AcceptTcpClient();
                var handler = new ClientHandler(client, this);

                lock (_lock)
                {
                    _connectedClients.Add(handler);
                    Console.WriteLine($"[ПІДКЛЮЧЕННЯ] Новий клієнт. Всього: {_connectedClients.Count}");
                }

                new Thread(handler.Handle).Start();
            }
        }

        public void PairPlayers(ClientHandler requester)
        {
            lock (_lock)
            {
                foreach (var client in _connectedClients)
                {
                    if (client != requester && !client.InGame && client.IsReady)
                    {
                        var session = new GameSession(requester, client);

                        // Прив’язуємо сесію до обох гравців
                        requester.Session = session;
                        client.Session = session;

                        _sessions.Add(session);

                        // Інформація у чат
                        requester.SendMessage($"{MessageTypes.CHAT}|Знайдено суперника: {client.PlayerName}");
                        client.SendMessage($"{MessageTypes.CHAT}|Знайдено суперника: {requester.PlayerName}");

                        Console.WriteLine($"[ПАРА] {requester.PlayerName} проти {client.PlayerName}");

                        return;
                    }
                }

                Console.WriteLine($"[INFO] Немає вільного супротивника для {requester.PlayerName}");
            }
        }

        public void Broadcast(string message, ClientHandler exclude = null)
        {
            lock (_lock)
            {
                foreach (var client in _connectedClients)
                {
                    if (client != exclude)
                        client.SendMessage(message);
                }
            }

            Console.WriteLine($"[ЧАТ] Відправлено: {message}");
        }


        public void RemoveClient(ClientHandler client)
        {
            lock (_lock)
            {
                _connectedClients.Remove(client);
            }
        }
    }
}
