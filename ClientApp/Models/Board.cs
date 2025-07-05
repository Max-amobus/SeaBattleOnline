using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class Board
    {
        private readonly bool[,] _ships = new bool[GameConstant.BoardSize, GameConstant.BoardSize];

        public bool IsValidCoordinate(int x, int y) =>
            x >= 0 && x < GameConstant.BoardSize && y >= 0 && y < GameConstant.BoardSize;

        public ShotResult RegisterShot(int x, int y)
        {
            if (!IsValidCoordinate(x, y))
                throw new ArgumentOutOfRangeException();

            bool hit = _ships[x, y];
            _ships[x, y] = false;
            return new ShotResult(x, y, hit);
        }

    }
}
