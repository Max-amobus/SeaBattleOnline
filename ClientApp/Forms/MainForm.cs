using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ClientApp.Forms;
using Shared;

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

        private GameBoard myBoard;
        private GameBoard enemyBoard;

        public MainForm(string playerName, string serverIP, int serverPort)
        {
            InitializeComponent();

            myBoard = new GameBoard();
            enemyBoard = new GameBoard();

            myBoard.Location = new Point(10, 10);
            enemyBoard.Location = new Point(320, 10);

            enemyBoard.OnCellClick += (x, y) =>
            {
                // Відправити координати пострілу на сервер
                SendMessage($"SHOT|{x},{y}");
                enemyBoard.SetEnabled(false);
            };

            this.Controls.Add(myBoard);
            this.Controls.Add(enemyBoard);
        }

        private void InitBoards()
        {
            myBoard = new GameBoard(true);
            enemyBoard = new GameBoard(false);

            myBoard.Location = new System.Drawing.Point(20, 60);
            enemyBoard.Location = new System.Drawing.Point(400, 60);

            enemyBoard.OnCellClick += EnemyBoard_OnCellClick;

            this.Controls.Add(myBoard);
            this.Controls.Add(enemyBoard);
        }

        private void ConnectToServer()
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(_serverIP, _serverPort);
                _stream = _client.GetStream();

                SendMessage($"CONNECT|{_playerName}");

                _receiveThread = new Thread(ReceiveLoop);
                _receiveThread.IsBackground = true;
                _receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка підключення: {ex.Message}");
                this.Close();
            }
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
                    HandleMessage(message);
                }
            }
            catch
            {
                Invoke(() => MessageBox.Show("З'єднання з сервером втрачено."));
                Invoke(() => this.Close());
            }
        }

        private void HandleMessage(string rawMessage)
        {
            var msg = Shared.Message.Parse(rawMessage);

            switch (msg.Type)
            {
                case MessageType.Start:
                    Invoke(() => lblStatus.Text = $"Почалась гра проти {msg.Data}");
                    break;

                case MessageType.Turn:
                    Invoke(() =>
                    {
                        if (msg.Data == "YOUR")
                        {
                            lblStatus.Text = "Ваш хід";
                            enemyBoard.SetEnabled(true);
                        }
                        else
                        {
                            lblStatus.Text = "Хід супротивника";
                            enemyBoard.SetEnabled(false);
                        }
                    });
                    break;

                case MessageType.Result:
                    Invoke(() => enemyBoard.ApplyShotResult(msg.Data));
                    break;

                case MessageType.Shot:
                    Invoke(() => myBoard.HandleIncomingShot(msg.Data));
                    break;

                case MessageType.Chat:
                    Invoke(() => chatPanel.AddMessage($"Опонент: {msg.Data}"));
                    break;

                case MessageType.GameOver:
                    Invoke(() =>
                    {
                        string text = msg.Data == "WIN" ? "Ви перемогли!" : "Ви програли!";
                        MessageBox.Show(text);
                    });
                    break;

                case MessageType.Disconnected:
                    Invoke(() =>
                    {
                        MessageBox.Show("Супротивник вийшов.");
                        this.Close();
                    });
                    break;

                case MessageType.Error:
                    Invoke(() => MessageBox.Show("Сервер: " + msg.Data));
                    break;
            }
        }

        private void EnemyBoard_OnCellClick(int x, int y)
        {
            SendMessage($"SHOT|{x},{y}");
            enemyBoard.SetEnabled(false);
        }

        private void SendMessage(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            _stream.Write(data, 0, data.Length);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _client?.Close();
            _receiveThread?.Abort();
        }



    }
}
