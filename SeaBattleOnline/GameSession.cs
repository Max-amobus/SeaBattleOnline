using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    public class GameSession
    {
        public ClientHandler Player1 { get; private set; }
        public ClientHandler Player2 { get; private set; }

        private ClientHandler currentTurn;

        public GameSession(ClientHandler player1, ClientHandler player2)
        {
            Player1 = player1;
            Player2 = player2;

            Player1.Opponent = Player2;
            Player2.Opponent = Player1;

            currentTurn = Player1; // перший хід Player1

            StartGame();
        }

        private void StartGame()
        {
            Player1.SendMessage($"{MessageTypes.START}|{Player2.PlayerName}");
            Player2.SendMessage($"{MessageTypes.START}|{Player1.PlayerName}");

            Player1.SendMessage($"{MessageTypes.TURN}|YOUR");
            Player2.SendMessage($"{MessageTypes.TURN}|WAIT");

            Console.WriteLine($"[GAME] Гра почалась між {Player1.PlayerName} та {Player2.PlayerName}");
        }

        public void HandleMove(ClientHandler fromPlayer, string message)
        {
            if (fromPlayer != currentTurn)
            {
                fromPlayer.SendMessage($"{MessageTypes.ERROR}|Не ваш хід!");
                return;
            }

            var opponent = fromPlayer == Player1 ? Player2 : Player1;
            opponent.SendMessage(message); // пересилаємо постріл

            // після пострілу — передача ходу іншому
            SwitchTurn();
        }

        private void SwitchTurn()
        {
            currentTurn = currentTurn == Player1 ? Player2 : Player1;

            currentTurn.SendMessage($"{MessageTypes.TURN}|YOUR");
            (currentTurn == Player1 ? Player2 : Player1).SendMessage($"{MessageTypes.TURN}|WAIT");
        }

        public void HandleRestartRequest(ClientHandler fromPlayer)
        {
            var opponent = fromPlayer.Opponent;
            opponent?.SendMessage($"{MessageTypes.RESTART_REQUEST}");
        }

        public void EndSession()
        {
            Player1.Opponent = null;
            Player2.Opponent = null;

            Player1.SendMessage($"{MessageTypes.DISCONNECTED}|Противник вийшов.");
            Player2.SendMessage($"{MessageTypes.DISCONNECTED}|Противник вийшов.");

            Console.WriteLine("[GAME] Сесію гри завершено");
        }
    }
}
