namespace ClientApp.Forms
{
    partial class ChatPanel
    {
        private System.ComponentModel.IContainer components = null;

        private ListBox listBox1;
        private GroupBox groupBox1;
        private TextBox txtMessage;
        private Button btnSend;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            listBox1 = new ListBox();
            groupBox1 = new GroupBox();
            txtMessage = new TextBox();
            btnSend = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 17;
            listBox1.Location = new Point(6, 22);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(500, 276);
            listBox1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(listBox1);
            groupBox1.Controls.Add(txtMessage);
            groupBox1.Controls.Add(btnSend);
            groupBox1.Font = new Font("Segoe UI", 10F);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(526, 360);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Чат з супротивником";
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(6, 310);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(400, 25);
            txtMessage.TabIndex = 1;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(415, 310);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(91, 26);
            btnSend.TabIndex = 2;
            btnSend.Text = "Надіслати";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // ChatPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(562, 390);
            Controls.Add(groupBox1);
            Name = "ChatPanel";
            Text = "ChatPanel";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }
    }
}
