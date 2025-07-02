namespace Shared
{
    public static class GameConstant
    {
        public const int BoardSize = 10; 

        public static readonly int[] ShipSizes = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 }; // Стандартний набір кораблів

        public const int MaxPlayersPerSession = 2;

        public const string DefaultServerIP = "127.0.0.1";
        public const int DefaultServerPort = 8888;
    }
}
