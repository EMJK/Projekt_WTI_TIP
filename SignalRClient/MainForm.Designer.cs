namespace SignalRClient
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.chatCtrl1 = new SignalRClient.ChatCtrl();
            this.chatCtrl2 = new SignalRClient.ChatCtrl();
            this.SuspendLayout();
            // 
            // chatCtrl1
            // 
            this.chatCtrl1.Location = new System.Drawing.Point(12, 12);
            this.chatCtrl1.Name = "chatCtrl1";
            this.chatCtrl1.Size = new System.Drawing.Size(458, 573);
            this.chatCtrl1.TabIndex = 0;
            // 
            // chatCtrl2
            // 
            this.chatCtrl2.Location = new System.Drawing.Point(476, 12);
            this.chatCtrl2.Name = "chatCtrl2";
            this.chatCtrl2.Size = new System.Drawing.Size(458, 573);
            this.chatCtrl2.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 597);
            this.Controls.Add(this.chatCtrl2);
            this.Controls.Add(this.chatCtrl1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ChatCtrl chatCtrl1;
        private ChatCtrl chatCtrl2;
    }
}