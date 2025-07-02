using System;

namespace ServerApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int port = 8888; 

            try
            {
                Server server = new Server(port);
                server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FATAL] Неможливо запустити сервер: {ex.Message}");
            }
        }
    }
}
