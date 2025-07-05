using Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServerApp.ClientHandler;

namespace ServerApp
{
    public class GameSession
    {
        public ClientHandler Player1 { get; private set; }
        public ClientHandler Player2 { get; private set; }
        private Dictionary<ClientHandler, HashSet<Point>> _playerShips = new();

        private ClientHandler currentTurn;

        public GameSession(ClientHandler player1, ClientHandler player2)
        {
            Player1 = player1;
            Player2 = player2;

            _playerShips[player1] = new HashSet<Point>();
            _playerShips[player2] = new HashSet<Point>();

            StartGame();
        }

        public void RegisterShips(ClientHandler player, string shipsData)
        {
            try
            {
                Console.WriteLine($"[SHIPS REGISTER] Початков? дан?: {shipsData}");

                // Очищаємо попередн? значення
                _playerShips[player] = new HashSet<Point>();

                var shipCoords = shipsData.Split(';')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Split(','))
                    .Where(parts => parts.Length == 2)
                    .Select(parts => new Point(int.Parse(parts[0]), int.Parse(parts[1])))
                    .ToList();

                foreach (var coord in shipCoords)
                {
                    _playerShips[player].Add(coord);
                    Console.WriteLine($"[SHIP ADDED] Додано корабель: {coord.X},{coord.Y}");
                }

                Console.WriteLine($"[SHIPS REGISTERED] Для {player.PlayerName} зареєстровано {_playerShips[player].Count} корабл?в");

                // Виводимо вс? корабл? для перев?рки
                Console.WriteLine("[SHIPS LIST] Список корабл?в:");
                foreach (var ship in _playerShips[player])
                {
                    Console.WriteLine($"- {ship.X},{ship.Y} (Hash: {ship.GetHashCode()})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REGISTER ERROR] Помилка: {ex.Message}");
            }
        }

        private void StartGame()
        {
            Player1.HitShots.Clear();
            Player2.HitShots.Clear();

            Player1.SendMessage($"{MessageTypes.START}|{Player2.PlayerName}");
            Player2.SendMessage($"{MessageTypes.START}|{Player1.PlayerName}");

            currentTurn = new Random().Next(0, 2) == 0 ? Player1 : Player2;

            Console.WriteLine($"[GAME START] Перший хід: {currentTurn.PlayerName}");
            currentTurn.SendMessage($"{MessageTypes.TURN}|YOUR");
            GetOpponent(currentTurn).SendMessage($"{MessageTypes.TURN}|WAIT");
        }

        private ClientHandler GetOpponent(ClientHandler player)
        {
            return player == Player1 ? Player2 : Player1;
        }

        public void HandleMove(ClientHandler fromPlayer, string message)
        {

            if (fromPlayer != currentTurn)
            {
                fromPlayer.SendMessage($"{MessageTypes.ERROR}|Не ваш хід!");
                return;
            }

            var opponent = GetOpponent(fromPlayer);
            opponent.SendMessage($"{MessageTypes.SHOT}|{message.Substring(5)}");
        }

        public void SwitchTurn()
        {
            currentTurn = (currentTurn == Player1) ? Player2 : Player1;

            currentTurn.SendMessage($"{MessageTypes.TURN}|YOUR");     
            GetOpponent(currentTurn).SendMessage($"{MessageTypes.TURN}|OPPONENT");

            Console.WriteLine($"[TURN] Хід переключено на {currentTurn.PlayerName}");
        }

        public void HandleShotResult(ClientHandler fromPlayer, string message)
        {
            ClientHandler attackingPlayer = currentTurn;
            ClientHandler targetPlayer = GetOpponent(attackingPlayer);

            // Перев?рка ?н?ц?ал?зац??
            if (!_playerShips.ContainsKey(targetPlayer) || _playerShips[targetPlayer] == null)
            {
                Console.WriteLine($"[ERROR] Немає даних про корабл? {targetPlayer.PlayerName}");
                return;
            }

            var parts = message.Split(':');
            if (parts.Length != 2) return;

            var coords = parts[0].Split(',');
            if (coords.Length != 2 || !int.TryParse(coords[0], out int x) || !int.TryParse(coords[1], out int y))
                return;

            var result = parts[1];
            var hitPoint = new Point(x, y);

            Console.WriteLine($"[SHOT] {attackingPlayer.PlayerName} -> {targetPlayer.PlayerName} ({x},{y})");

            if (result == "HIT")
            {
                // Альтернативний спос?б перев?рки
                var shipFound = _playerShips[targetPlayer].FirstOrDefault(p => p.X == x && p.Y == y);
                if (shipFound != default)
                {
                    _playerShips[targetPlayer].Remove(shipFound);
                    Console.WriteLine($"[HIT SUCCESS] Видалено корабель на ({x},{y})");

                    if (_playerShips[targetPlayer].Count == 0)
                    {
                        Console.WriteLine($"[GAME OVER] Вс? корабл? знищен?!");
                        attackingPlayer.SendMessage($"{MessageTypes.GAME_OVER}|WIN");
                        targetPlayer.SendMessage($"{MessageTypes.GAME_OVER}|LOSE");
                        EndSession();
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"[HIT FAILED] Координати ({x},{y}) не знайдено серед корабл?в");
                    Console.WriteLine("[CURRENT SHIPS] Список корабл?в ц?л?:");
                    foreach (var ship in _playerShips[targetPlayer])
                    {
                        Console.WriteLine($"- {ship.X},{ship.Y}");
                    }
                }

                attackingPlayer.SendMessage($"{MessageTypes.RESULT}|{message}");
            }
            else if (result == "MISS")
            {
                attackingPlayer.SendMessage($"{MessageTypes.RESULT}|{message}");
                SwitchTurn();
            }
        }

        public bool CheckWinCondition(ClientHandler attackingPlayer)
        {
            var targetPlayer = GetOpponent(attackingPlayer);

            if (!_playerShips.ContainsKey(targetPlayer))
            {
                Console.WriteLine($"[ERROR] Немає даних про кораблі {targetPlayer.PlayerName}");
                return false;
            }

            bool allDestroyed = _playerShips[targetPlayer].Count == 0;

            if (allDestroyed)
            {
                Console.WriteLine($"[WIN] {attackingPlayer.PlayerName} знищив усі кораблі {targetPlayer.PlayerName}");
            }
            else
            {
                Console.WriteLine($"[DEBUG] У {targetPlayer.PlayerName} ще є {_playerShips[targetPlayer].Count} кораблів");
            }

            return allDestroyed;
        }




        public void HandleRestartRequest(ClientHandler fromPlayer)
        {
            var opponent = fromPlayer.Opponent;
            opponent?.SendMessage($"{MessageTypes.RESTART_REQUEST}");
        }

        public void EndSession()
        {
            Player1.HitShots.Clear();
            Player2.HitShots.Clear();

            Player1.Opponent = null;
            Player2.Opponent = null;

            Player1.SendMessage($"{MessageTypes.DISCONNECTED}|Противник вийшов.");
            Player2.SendMessage($"{MessageTypes.DISCONNECTED}|Противник вийшов.");

            Console.WriteLine("[GAME] Сесію гри завершено");
        }
    }
}

public struct GamePoint
{
    public int X { get; }
    public int Y { get; }

    public GamePoint(int x, int y) { X = x; Y = y; }

    public override bool Equals(object obj) => obj is GamePoint p && p.X == X && p.Y == Y;
    public override int GetHashCode() => HashCode.Combine(X, Y);
}