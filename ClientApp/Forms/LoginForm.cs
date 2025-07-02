namespace ClientApp
{
    public partial class LoginForm : Form
    {
        public string PlayerName => txtName.Text.Trim();
        public string ServerIP => txtIP.Text.Trim();
        public int ServerPort => int.TryParse(txtPort.Text.Trim(), out var port) ? port : 8888;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PlayerName))
            {
                MessageBox.Show("Введіть ім’я гравця.");
                return;
            }

            try
            {
                var mainForm = new MainForm(PlayerName, ServerIP, ServerPort);
                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка підключення: {ex.Message}");
            }
        }
    }
}
