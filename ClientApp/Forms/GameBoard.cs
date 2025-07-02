using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp.Forms
{
    public partial class GameBoard : UserControl
    {
        private Button[,] cells = new Button[10, 10];

        public event Action<int, int>? OnCellClick;

        public GameBoard()
        {
            InitializeComponent();
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            // Додаємо стилі рядків і колонок
            tableLayoutPanel.ColumnStyles.Clear();
            tableLayoutPanel.RowStyles.Clear();
            for (int i = 0; i < 10; i++)
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            }

            tableLayoutPanel.Controls.Clear();

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Button btn = new Button
                    {
                        Dock = DockStyle.Fill,
                        Margin = new Padding(1),
                        BackColor = Color.LightBlue,
                        Tag = new Point(x, y),
                        Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                    };
                    btn.Click += Btn_Click;
                    cells[x, y] = btn;
                    tableLayoutPanel.Controls.Add(btn, x, y);
                }
            }
        }

        private void Btn_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is Point p)
            {
                OnCellClick?.Invoke(p.X, p.Y);
            }
        }

        /// <summary>
        /// Відзначити клітинку як промах
        /// </summary>
        public void MarkMiss(int x, int y)
        {
            SetCellState(x, y, Color.Gray, "•");
        }

        /// <summary>
        /// Відзначити клітинку як потрапляння
        /// </summary>
        public void MarkHit(int x, int y)
        {
            SetCellState(x, y, Color.Red, "X");
        }

        /// <summary>
        /// Очищення поля (наприклад, для нової гри)
        /// </summary>
        public void ResetBoard()
        {
            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                    SetCellState(x, y, Color.LightBlue, "");
        }

        private void SetCellState(int x, int y, Color backColor, string text)
        {
            var btn = cells[x, y];
            btn.BackColor = backColor;
            btn.Text = text;
            btn.Enabled = false; // щоб після пострілу клітинка не була натискабельна
        }

        /// <summary>
        /// Вмикає або вимикає усі клітинки (наприклад, щоб блокувати поле)
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            foreach (var btn in cells)
                btn.Enabled = enabled && string.IsNullOrEmpty(btn.Text);
        }
    }
}
