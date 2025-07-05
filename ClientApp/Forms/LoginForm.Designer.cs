namespace ClientApp
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            txtPort = new TextBox();
            label3 = new Label();
            txtIP = new TextBox();
            label2 = new Label();
            btnConnect = new Button();
            txtName = new TextBox();
            label1 = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtPort);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtIP);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(btnConnect);
            groupBox1.Controls.Add(txtName);
            groupBox1.Controls.Add(label1);
            groupBox1.Font = new Font("Segoe UI", 16F);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(523, 361);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Login";
            // 
            // txtPort
            // 
            txtPort.Location = new Point(153, 210);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(350, 36);
            txtPort.TabIndex = 6;
            txtPort.Text = "8888";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(80, 210);
            label3.Name = "label3";
            label3.Size = new Size(64, 30);
            label3.TabIndex = 5;
            label3.Text = "Порт";
            // 
            // txtIP
            // 
            txtIP.Location = new Point(153, 127);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(350, 36);
            txtIP.TabIndex = 4;
            txtIP.Text = "localhost";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(23, 127);
            label2.Name = "label2";
            label2.Size = new Size(121, 30);
            label2.TabIndex = 3;
            label2.Text = "Ip сервера";
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(134, 281);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(240, 50);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "Підключення до гри";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // txtName
            // 
            txtName.Location = new Point(153, 44);
            txtName.Name = "txtName";
            txtName.Size = new Size(350, 36);
            txtName.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 44);
            label1.Name = "label1";
            label1.Size = new Size(124, 30);
            label1.TabIndex = 0;
            label1.Text = "Ім'я гравця";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(547, 382);
            Controls.Add(groupBox1);
            Name = "LoginForm";
            Text = "Login";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label label1;
        private TextBox txtPort;
        private Label label3;
        private TextBox txtIP;
        private Label label2;
        private Button btnConnect;
        private TextBox txtName;
    }
}
