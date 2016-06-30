namespace Client
{
    partial class ConversationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConversationForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnHangUp = new System.Windows.Forms.Button();
            this.btnCall = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSend = new System.Windows.Forms.Button();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnHangUp);
            this.groupBox1.Controls.Add(this.btnCall);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(556, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Voice call";
            // 
            // btnHangUp
            // 
            this.btnHangUp.BackColor = System.Drawing.Color.White;
            this.btnHangUp.Enabled = false;
            this.btnHangUp.Image = ((System.Drawing.Image)(resources.GetObject("btnHangUp.Image")));
            this.btnHangUp.Location = new System.Drawing.Point(307, 19);
            this.btnHangUp.Name = "btnHangUp";
            this.btnHangUp.Size = new System.Drawing.Size(243, 68);
            this.btnHangUp.TabIndex = 1;
            this.btnHangUp.UseVisualStyleBackColor = false;
            this.btnHangUp.Click += new System.EventHandler(this.btnHangUp_Click);
            // 
            // btnCall
            // 
            this.btnCall.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCall.BackColor = System.Drawing.Color.White;
            this.btnCall.Enabled = false;
            this.btnCall.Image = ((System.Drawing.Image)(resources.GetObject("btnCall.Image")));
            this.btnCall.Location = new System.Drawing.Point(6, 19);
            this.btnCall.Name = "btnCall";
            this.btnCall.Size = new System.Drawing.Size(243, 68);
            this.btnCall.TabIndex = 0;
            this.btnCall.UseVisualStyleBackColor = false;
            this.btnCall.Click += new System.EventHandler(this.btnCall_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.btnSend);
            this.groupBox2.Controls.Add(this.tbInput);
            this.groupBox2.Location = new System.Drawing.Point(12, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(556, 384);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Chat";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(6, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(544, 303);
            this.panel1.TabIndex = 1;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(457, 328);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(93, 50);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // tbInput
            // 
            this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInput.Location = new System.Drawing.Point(6, 328);
            this.tbInput.Multiline = true;
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(445, 50);
            this.tbInput.TabIndex = 0;
            this.tbInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbInput_KeyPress);
            // 
            // ConversationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 514);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ConversationForm";
            this.Text = "Rozmowa";
            this.Load += new System.EventHandler(this.ConversationForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCall;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnHangUp;
    }
}