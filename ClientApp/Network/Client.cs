using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Network
{
    public class Client
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;

        public event Action<string>? OnMessageReceived;
        public event Action? OnDisconnected;

        public bool IsConnected => _tcpClient?.Connected ?? false;

        public Client()
        {
            _tcpClient = new TcpClient();
            _cts = new CancellationTokenSource();
        }

        public async Task<bool> ConnectAsync(string ip, int port)
        {
            try
            {
                await _tcpClient.ConnectAsync(ip, port);
                _stream = _tcpClient.GetStream();
                StartListening(_cts.Token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task SendAsync(string message)
        {
            if (_stream == null || !_tcpClient.Connected) return;

            byte[] buffer = Encoding.UTF8.GetBytes(message + "\n");
            try
            {
                await _stream.WriteAsync(buffer, 0, buffer.Length);
                await _stream.FlushAsync();
            }
            catch
            {
                Disconnect();
            }
        }

        private async void StartListening(CancellationToken token)
        {
            byte[] buffer = new byte[1024];
            StringBuilder sb = new();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead == 0)
                    {
                        // Сервер закрив з’єднання
                        Disconnect();
                        break;
                    }

                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    // Обробка повних повідомлень, розділених '\n'
                    string content = sb.ToString();
                    int newlineIndex;
                    while ((newlineIndex = content.IndexOf('\n')) >= 0)
                    {
                        string message = content[..newlineIndex].Trim();
                        content = content[(newlineIndex + 1)..];
                        if (!string.IsNullOrEmpty(message))
                            OnMessageReceived?.Invoke(message);
                    }
                    sb.Clear();
                    sb.Append(content);
                }
            }
            catch
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (_tcpClient != null)
            {
                _cts.Cancel();

                try
                {
                    _stream?.Close();
                    _tcpClient?.Close();
                }
                catch { }

                OnDisconnected?.Invoke();
            }
        }

    }
}
