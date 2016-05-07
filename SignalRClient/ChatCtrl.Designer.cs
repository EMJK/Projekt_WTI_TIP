namespace SignalRClient
{
    partial class ChatCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLogin = new System.Windows.Forms.Button();
            this.tbSessionID = new System.Windows.Forms.TextBox();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbUserID = new System.Windows.Forms.TextBox();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(6, 97);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 0;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // tbSessionID
            // 
            this.tbSessionID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSessionID.Location = new System.Drawing.Point(3, 16);
            this.tbSessionID.Name = "tbSessionID";
            this.tbSessionID.ReadOnly = true;
            this.tbSessionID.Size = new System.Drawing.Size(500, 20);
            this.tbSessionID.TabIndex = 1;
            // 
            // tbInput
            // 
            this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInput.Location = new System.Drawing.Point(0, 482);
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(425, 20);
            this.tbInput.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tbOutput);
            this.groupBox1.Location = new System.Drawing.Point(6, 126);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(503, 328);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Messages";
            // 
            // tbOutput
            // 
            this.tbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbOutput.Location = new System.Drawing.Point(3, 16);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.Size = new System.Drawing.Size(497, 309);
            this.tbOutput.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.tbSessionID);
            this.groupBox2.Location = new System.Drawing.Point(3, 50);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(506, 41);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SessionID";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.tbUserID);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(506, 41);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "UserID";
            // 
            // tbUserID
            // 
            this.tbUserID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbUserID.Location = new System.Drawing.Point(3, 16);
            this.tbUserID.Name = "tbUserID";
            this.tbUserID.ReadOnly = true;
            this.tbUserID.Size = new System.Drawing.Size(500, 20);
            this.tbUserID.TabIndex = 1;
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(87, 97);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 0;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(431, 482);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 20);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // ChatCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnLogin);
            this.Name = "ChatCtrl";
            this.Size = new System.Drawing.Size(512, 506);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox tbSessionID;
        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbUserID;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnSend;
    }
}
