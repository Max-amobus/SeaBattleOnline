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

            //Player1.Opponent = Player2;
            //Player2.Opponent = Player1;
            _playerShips[player1] = new HashSet<Point>();
            _playerShips[player2] = new HashSet<Point>();

            StartGame();
        }

        public void RegisterShips(ClientHandler player, string shipsData)
        {
            try
            {
                var ships = shipsData.Split(';')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Split(','))
                    .Where(parts => parts.Length == 2)
                    .Select(parts => new Point(int.Parse(parts[0]), int.Parse(parts[1])))
                    .ToList();

                _playerShips[player] = new HashSet<Point>(ships);
                Console.WriteLine($"[SHIPS REGISTERED] {player.PlayerName} має {ships.Count} кораблів:");
                foreach (var ship in ships.Take(5))
                    Console.WriteLine($"- {ship.X},{ship.Y}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Помилка реєстрації кораблів: {ex.Message}");
            }
        }

        private void StartGame()
        {
            // Очищаємо попередні попадання
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

            // Відправляємо ЧІТКІ повідомлення:
            currentTurn.SendMessage($"{MessageTypes.TURN}|YOUR");      // Активний гравець
            GetOpponent(currentTurn).SendMessage($"{MessageTypes.TURN}|OPPONENT"); // Чекаючий

            Console.WriteLine($"[TURN] Хід переключено на {currentTurn.PlayerName}");
        }

        public void HandleShotResult(ClientHandler fromPlayer, string message)
        {
            ClientHandler attackingPlayer = currentTurn;
            ClientHandler targetPlayer = GetOpponent(attackingPlayer);

            var parts = message.Split(':');
            if (parts.Length != 2) return;

            var coords = parts[0].Split(',');
            if (coords.Length != 2 || !int.TryParse(coords[0], out int x) || !int.TryParse(coords[1], out int y))
                return;

            var result = parts[1];
            var hitPoint = new Point(x, y);

            if (result == "HIT")
            {
                // Додаємо логування
                Console.WriteLine($"[SHIP HIT] {attackingPlayer.PlayerName} влучив у {targetPlayer.PlayerName} на ({x},{y})");

                if (_playerShips[targetPlayer].Remove(hitPoint))
                {
                    Console.WriteLine($"[SHIP REMOVED] Координата ({x},{y}) видалена");
                    Console.WriteLine($"[SHIPS LEFT] У {targetPlayer.PlayerName} залишилось: {_playerShips[targetPlayer].Count} кораблів");

                    // Перевіряємо перемогу через окремий метод
                    if (CheckWinCondition(attackingPlayer))
                    {
                        Console.WriteLine($"[GAME OVER] {attackingPlayer.PlayerName} переміг!");
                        attackingPlayer.SendMessage($"{MessageTypes.GAME_OVER}|WIN");
                        targetPlayer.SendMessage($"{MessageTypes.GAME_OVER}|LOSE");
                        EndSession();
                        return;
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