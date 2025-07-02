using ClientApp.Forms;

namespace ClientApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label lblStatus;
        private ChatPanel chatPanel;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {


            this.lblStatus = new Label();
            this.chatPanel = new ChatPanel();

            // lblStatus
            this.lblStatus.Text = "Очікування гравця...";
            this.lblStatus.Location = new System.Drawing.Point(20, 20);
            this.lblStatus.Width = 400;

            // chatPanel
            this.chatPanel.Location = new System.Drawing.Point(750, 20);
            this.chatPanel.Size = new System.Drawing.Size(250, 400);
            this.chatPanel.OnSendMessage += ChatPanel_OnSendMessage;

            // MainForm
            this.ClientSize = new System.Drawing.Size(1024, 600);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.chatPanel);
            this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Морський бій";




            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "MainForm";
        }

        #endregion
    }
}