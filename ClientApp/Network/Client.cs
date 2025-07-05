using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClientApp.Network
{
    public class Client
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public event Action<byte[]>? MessageReceived;

        private readonly string _host;
        private readonly int _port;

        public Client(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Connect()
        {
            _client = new TcpClient(_host, _port);
            _stream = _client.GetStream();
            Task.Run(ReceiveLoop);
        }

        private async Task ReceiveLoop()
        {
            byte[] buffer = new byte[4096];
            while (true)
            {
                int byteCount = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (byteCount == 0) break;
                byte[] data = new byte[byteCount];
                Array.Copy(buffer, data, byteCount);
                MessageReceived?.Invoke(data);
            }
        }

        public async Task SendAsync(byte[] data)
        {
            if (_stream != null)
                await _stream.WriteAsync(data, 0, data.Length);
        }

        public void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
        }
    }
}
