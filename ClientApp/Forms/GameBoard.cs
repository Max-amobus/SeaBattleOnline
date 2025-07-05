using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClientApp.Forms
{
    public partial class GameBoard : UserControl
    {
        private const int GridSize = 10;
        private Button[,] cells = new Button[GridSize, GridSize];

        public event Action<int, int>? CellClicked;

        public GameBoard()
        {
            InitializeComponent();
            SetupGrid();
            this.Resize += GameBoard_Resize;
        }

        private void GameBoard_Resize(object? sender, EventArgs e)
        {
            ResizeGrid();
        }

        private void SetupGrid()
        {
            this.Controls.Clear();

            for (int y = 0; y < GridSize; y++)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    var btn = new Button();
                    btn.Tag = (x, y);
                    btn.BackColor = Color.LightBlue;
                    btn.Margin = Padding.Empty;
                    btn.Padding = Padding.Empty;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.Black;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.Click += Cell_Click;

                    this.Controls.Add(btn);
                    cells[x, y] = btn;
                }
            }

            ResizeGrid();
        }

        private void ResizeGrid()
        {
            if (cells == null) return;
            int cellSize = Math.Min(this.Width / GridSize, this.Height / GridSize);

            for (int y = 0; y < GridSize; y++)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    var btn = cells[x, y];
                    btn.Size = new Size(cellSize, cellSize);
                    btn.Location = new Point(x * cellSize, y * cellSize);
                }
            }
        }

        private void Cell_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<int, int> coords)
            {
                int x = coords.Item1;
                int y = coords.Item2;
                CellClicked?.Invoke(x, y);
            }
        }

        public void MarkHit(int x, int y)
        {
            if (IsValidCoord(x, y))
            {
                cells[x, y].BackColor = Color.Red;
                cells[x, y].Enabled = false;
            }
        }

        public void MarkMiss(int x, int y)
        {
            if (IsValidCoord(x, y))
            {
                cells[x, y].BackColor = Color.White;
                cells[x, y].Enabled = false;
            }
        }

        private bool IsValidCoord(int x, int y)
        {
            return x >= 0 && x < GridSize && y >= 0 && y < GridSize;
        }

        public void HandleIncomingShot(int x, int y, bool isHit)
        {
            if (isHit)
                MarkHit(x, y);
            else
                MarkMiss(x, y);
        }

        public void ClearBoard()
        {
            foreach (var btn in cells)
            {
                btn.BackColor = Color.LightBlue;
                btn.Enabled = true;
            }
        }
    }
}
