using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class PlayerInfo
    {
        public string Name { get; set; }
        public List<(int X, int Y)> ShipCells { get; set; } = new List<(int X, int Y)>();
        public List<ShotResult> ShotsFired { get; set; } = new List<ShotResult>();

        public PlayerInfo(string name)
        {
            Name = name;
        }

        public bool HasShipAt(int x, int y)
        {
            return ShipCells.Contains((x, y));
        }

        public void AddShot(ShotResult shot)
        {
            ShotsFired.Add(shot);
            if (shot.IsHit)
            {
                ShipCells.Remove((shot.X, shot.Y));
            }
        }

        public bool AllShipsSunk()
        {
            return ShipCells.Count == 0;
        }


    }
}
