namespace ClientApp.Models;
    public class ShotResult
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsHit { get; set; }
        public bool IsPlayerShot { get; set; }

        public ShotResult() { }

        public ShotResult(int x, int y, bool isHit)
        {
            X = x;
            Y = y;
            IsHit = isHit;
        }

        public override string ToString()
        {
            return $"{X},{Y}:{(IsHit ? "HIT" : "MISS")}";
        }

        public static ShotResult Parse(string s)
        {
            var parts = s.Split(':');
            if (parts.Length != 2) throw new FormatException("Invalid ShotResult format");

            var coords = parts[0].Split(',');
            if (coords.Length != 2) throw new FormatException("Invalid coordinates format");

            int x = int.Parse(coords[0]);
            int y = int.Parse(coords[1]);
            bool isHit = parts[1].Equals("HIT", StringComparison.OrdinalIgnoreCase);

            return new ShotResult(x, y, isHit);
        }
    }
