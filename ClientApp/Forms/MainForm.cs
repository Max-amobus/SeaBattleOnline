using System.Net.Sockets;
using System.Text;
using Shared;
using ClientApp.Models;

namespace ClientApp
{
    public partial class MainForm : Form
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _receiveThread;

        private readonly string _playerName;
        private readonly string _serverIP;
        private readonly int _serverPort;

        private Button[,] _myCells = new Button[10, 10];
        private Button[,] _enemyCells = new Button[10, 10];
        private bool _myTurn = false;
        private bool _gameStarted = false;
        private int _shipsToPlace = 10;

        public MainForm(string playerName, string serverIP, int serverPort)
        {
            _playerName = playerName;
            _serverIP = serverIP;
            _serverPort = serverPort;

            InitializeComponent();
            InitializeGameFields();
            SetupShipPlacementMode();
            ConnectToServer();
        }


        private void InitializeGameFields()
        {
            // Ваше поле (лівий TableLayoutPanel)
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    _myCells[col, row] = new Button
                    {
                        Dock = DockStyle.Fill,
                        Tag = new Point(col, row),
                        BackColor = Color.LightBlue
                    };
                    _myCells[col, row].Click += MyCell_Click;
                    tableLayoutPanel1.Controls.Add(_myCells[col, row], col, row);
                }
            }

            // Поле противника (правий TableLayoutPanel)
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    _enemyCells[col, row] = new Button
                    {
                        Dock = DockStyle.Fill,
                        Tag = new Point(col, row),
                        BackColor = Color.LightBlue,
                        Enabled = false
                    };
                    _enemyCells[col, row].Click += EnemyCell_Click;
                    tableLayoutPanel2.Controls.Add(_enemyCells[col, row], col, row);
                }
            }

            // Видалити цей рядок, якщо він є - він перезаписує події кнопок
            // tableLayoutPanel2.Click += tableLayoutPanel2_Click; 
        }

        private void SetupShipPlacementMode()
        {
            lblStatus.Text = "Розмістіть ваші кораблі (залишилось: " + _shipsToPlace + ")";
            btnPlaceShips.Visible = true;
            foreach (Control ctrl in tableLayoutPanel1.Controls)
            {
                if (ctrl is Button btn) btn.Enabled = true;
            }
        }

        private void ConnectToServer()
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(_serverIP, _serverPort);
                _stream = _client.GetStream();

                SendMessage($"CONNECT|{_playerName}\n");

                _receiveThread = new Thread(ReceiveLoop);
                _receiveThread.IsBackground = true;
                _receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка підключення: {ex.Message}\n");
                this.Close();
            }
        }

        private void MyCell_Click(object sender, EventArgs e)
        {
            if (!_gameStarted && btnPlaceShips.Visible)
            {
                var button = (Button)sender;
                var point = (Point)button.Tag;

                if (button.BackColor == Color.LightBlue && _shipsToPlace > 0)
                {
                    button.BackColor = Color.DarkBlue;
                    _shipsToPlace--;
                    lblStatus.Text = "Розмістіть ваші кораблі (залишилось: " + _shipsToPlace + ")";
                }
                else if (button.BackColor == Color.DarkBlue)
                {
                    button.BackColor = Color.LightBlue;
                    _shipsToPlace++;
                    lblStatus.Text = "Розмістіть ваші кораблі (залишилось: " + _shipsToPlace + ")";
                }
            }
        }

        private void EnemyCell_Click(object sender, EventArgs e)
        {
            if (!_gameStarted || !_myTurn) return;

            var button = (Button)sender;
            var point = (Point)button.Tag;

            Console.WriteLine($"Постріл по: {point.X},{point.Y}");

            SendMessage($"SHOT|{point.X},{point.Y}\n");

            // Тимчасово вимикаємо кнопку
            button.Enabled = false;
            //lblStatus.Text = "Очікування результату...";
        }

        private void ReceiveLoop()
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) continue;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    foreach (var line in message.Split('\n'))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            ProcessServerMessage(line);
                            var parsed = Shared.Message.Parse(line);
                            if (parsed.Type == MessageType.GameOver)
                            {
                                // Очищаємо стан гри
                                _gameStarted = false;
                                _myTurn = false;
                                ResetEnemyBoard();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Втрачено зв'язок з сервером: {ex.Message}");
                    this.Close();
                });
            }
        }

        private void ProcessServerMessage(string rawMessage)
        {
            var message = Shared.Message.Parse(rawMessage);
            Console.WriteLine($"[DEBUG] Прибуло: {rawMessage}");

            this.Invoke((MethodInvoker)delegate
            {
                switch (message.Type)
                {
                    case MessageType.Start:
                        _gameStarted = true;
                        lblStatus.Text = $"Гра проти {message.Data}";
                        SetEnemyBoardEnabled(false);
                        break;

                    case MessageType.Turn:
                        _myTurn = message.Data.Trim().ToUpper() == "YOUR";
                        lblStatus.Text = _myTurn ? "Ваш хід" : "Хід противника";
                        SetEnemyBoardEnabled(_myTurn);
                        break;

                    case MessageType.Result:
                        var shotResult = Models.ShotResult.Parse(message.Data);
                        var button = _enemyCells[shotResult.X, shotResult.Y];
                        button.BackColor = shotResult.IsHit ? Color.Red : Color.Gray;
                        button.Text = shotResult.IsHit ? "X" : "•";
                        break;

                    case MessageType.Shot:
                        var coords = message.Data.Split(',');
                        if (coords.Length == 2 && int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int y))
                        {
                            bool hit = _myCells[x, y].BackColor == Color.DarkBlue;
                            _myCells[x, y].BackColor = hit ? Color.Red : Color.Gray;
                            _myCells[x, y].Text = hit ? "X" : "•";
                            SendMessage($"RESULT|{x},{y}:{(hit ? "HIT" : "MISS")}\n");
                        }
                        break;

                    case MessageType.Chat:
                        listBoxChat.Items.Add($"{message.Data}");
                        listBoxChat.TopIndex = listBoxChat.Items.Count - 1;
                        break;

                    case MessageType.GameOver:
                        bool isWinner = message.Data == "WIN";
                        MessageBox.Show(isWinner ? "Ви перемогли!" : "Ви програли!");
                        SetEnemyBoardEnabled(false);
                        _gameStarted = false;

                        foreach (var cell in _enemyCells)
                        {
                            cell.BackColor = Color.LightBlue;
                            cell.Text = "";
                        }
                        break;

                    case MessageType.Disconnected:
                        MessageBox.Show("Суперник відключився");
                        this.Close();
                        break;

                    case MessageType.Error:
                        MessageBox.Show($"Помилка: {message.Data}");
                        break;
                }
            });
        }

        private void SetEnemyBoardEnabled(bool enabled)
        {

            foreach (Control ctrl in tableLayoutPanel2.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Enabled = enabled && (btn.Text != "X" && btn.Text != "•");
                }
            }
        }
        private void UpdateEnemyCell(int x, int y, bool isHit)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (x >= 0 && x < 10 && y >= 0 && y < 10)
                {
                    Button cell = _enemyCells[x, y];
                    cell.BackColor = isHit ? Color.Red : Color.Gray;
                    cell.Text = isHit ? "X" : "•";
                    cell.Enabled = false;
                }
            });
        }
        private void ResetEnemyBoard()
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    Button cell = _enemyCells[x, y];
                    cell.BackColor = Color.LightBlue;
                    cell.Text = "";
                    cell.Enabled = false;
                }
            }
        }


        private void SendMessage(string message)
        {
            try
            {
                message += "\n";
                byte[] data = Encoding.UTF8.GetBytes(message);
                _stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відправки: {ex.Message}");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                SendMessage("DISCONNECT\n");
                _stream?.Close();
                _client?.Close();
                _receiveThread?.Abort();
            }
            catch { }
        }

        private void btnPlaceShips_Click_1(object sender, EventArgs e)
        {
            if (_shipsToPlace == 0)
            {
                btnPlaceShips.Visible = false;
                _gameStarted = true;
                var shipCoords = new List<string>();
                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        if (_myCells[x, y].BackColor == Color.DarkBlue)
                        {
                            shipCoords.Add($"{x},{y}");
                        }
                    }
                }
                SendMessage($"SHIPS|{string.Join(";", shipCoords)}\n");
                SendMessage("READY\n");
                lblStatus.Text = "Очікування початку гри...";

                foreach (Control ctrl in tableLayoutPanel1.Controls)
                {
                    if (ctrl is Button btn) btn.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show($"Розмістіть усі {_shipsToPlace} кораблі перед початком гри");
            }
        }

        private void btnSend_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtChatMessage.Text))
            {
                SendMessage($"CHAT|{_playerName}: {txtChatMessage.Text}\n");
                listBoxChat.Items.Add($"Ви: {txtChatMessage.Text}");
                txtChatMessage.Clear();
            }
        }

    }
}
