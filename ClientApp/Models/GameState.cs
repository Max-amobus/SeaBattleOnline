using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class GameState
    {
        public PlayerInfo Player1 { get; set; }
        public PlayerInfo Player2 { get; set; }

        // true — ходить Player1, false — Player2
        public bool IsPlayer1Turn { get; private set; } = true;

        public GameState(string player1Name, string player2Name)
        {
            Player1 = new PlayerInfo(player1Name);
            Player2 = new PlayerInfo(player2Name);
        }

        public PlayerInfo CurrentPlayer => IsPlayer1Turn ? Player1 : Player2;
        public PlayerInfo OpponentPlayer => IsPlayer1Turn ? Player2 : Player1;

        public bool MakeShot(int x, int y)
        {
            var opponent = OpponentPlayer;
            bool hit = opponent.HasShipAt(x, y);

            var shotResult = new ShotResult(x, y, hit);
            CurrentPlayer.AddShot(shotResult);

            if (hit)
            {
                opponent.ShipCells.Remove((x, y));
            }
            else
            {
                IsPlayer1Turn = !IsPlayer1Turn;
            }

            return hit;
        }

        public void Reset()
        {
            Player1.ShipCells.Clear();
            Player2.ShipCells.Clear();
            Player1.ShotsFired.Clear();
            Player2.ShotsFired.Clear();
            IsPlayer1Turn = true;
        }
    }
}
