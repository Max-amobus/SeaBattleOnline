using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    public class Server
    {
        private readonly int _port;
        private TcpListener _listener;
        private readonly List<ClientHandler> _connectedClients = new List<ClientHandler>();

        public Server(int port)
        {
            _port = port;
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"[SERVER] Сервер запущено на порту {_port}");

            while (true)
            {
                try
                {
                    TcpClient tcpClient = _listener.AcceptTcpClient();
                    Console.WriteLine("[SERVER] Клієнт підключився");

                    ClientHandler clientHandler = new ClientHandler(tcpClient, this);
                    lock (_connectedClients)
                    {
                        _connectedClients.Add(clientHandler);
                    }

                    Thread clientThread = new Thread(clientHandler.Handle);
                    clientThread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Помилка при прийомі підключення: {ex.Message}");
                }
            }
        }

        public void RemoveClient(ClientHandler client)
        {
            lock (_connectedClients)
            {
                _connectedClients.Remove(client);
            }
        }

        public ClientHandler FindOpponent(ClientHandler requester)
        {
            lock (_connectedClients)
            {
                foreach (var client in _connectedClients)
                {
                    if (client != requester && !client.InGame)
                    {
                        return client;
                    }
                }
            }

            return null;
        }
    }
}

