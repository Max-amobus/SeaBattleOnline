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
    public partial class ChatPanel : UserControl
    {
        public event Action<string>? OnSendMessage;

        public ChatPanel()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                OnSendMessage?.Invoke(message);
                AddMessage("Ви: " + message);
                txtMessage.Clear();
            }
        }

        public void AddMessage(string text)
        {
            listBox1.Items.Add(text);
            listBox1.TopIndex = listBox1.Items.Count - 1;
        }
    }
}
