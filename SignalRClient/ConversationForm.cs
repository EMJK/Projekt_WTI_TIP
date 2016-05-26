using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Julas.Utils.Collections;
using Julas.Utils.Extensions;
using TheArtOfDev.HtmlRenderer.WinForms;

namespace Client
{
    public partial class ConversationForm : Form
    {
        private readonly string _thisUserId;
        private readonly string _otherUserId;
        private readonly HtmlPanel _htmlPanel;

        private readonly Color TextColor = Color.Black;
        private readonly Color TimestampColor = Color.DarkGray;
        private readonly Color ThisUserColor = Color.DodgerBlue;
        private readonly Color OtherUserColor = Color.DarkOrange;
        private readonly int FontSize = 1;

        public event Action<string> MessageSent;
        public event Action Call;
        public event Action HangUp;

        public ConversationForm(string thisUserID, string otherUserId)
        {
            _thisUserId = thisUserID;
            _otherUserId = otherUserId;
            InitializeComponent();
            this.Text = $"Conversation with {otherUserId}";
            _htmlPanel = new HtmlPanel();
            panel1.Controls.Add(_htmlPanel);
            _htmlPanel.Dock = DockStyle.Fill;
        }

        public void AppendMessageFromOtherUser(string message)
        {
            AppendMessage(message, _otherUserId, OtherUserColor);
        }

        private void AppendMessageFromThisUser(string message)
        {
            AppendMessage(message, _thisUserId, ThisUserColor);
        }

        private void AppendMessage(string msg, string from, Color nameColor)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<p>");
            sb.Append($"<b><font color=\"{GetHexColor(nameColor)}\" size=\"{FontSize}\">{from}</font></b> ");
            sb.Append($"<font color=\"{GetHexColor(TimestampColor)}\" size=\"{FontSize}\">{DateTime.Now.ToString("HH:mm:ss")}</font>");
            sb.Append("<br/>");
            sb.Append($"<font color=\"{GetHexColor(TextColor)}\" size=\"{FontSize}\">{msg}</font>");
            sb.Append("</p>");
            _htmlPanel.Text += sb.ToString();
            _htmlPanel.VerticalScroll.Value = _htmlPanel.VerticalScroll.Maximum;
        }

        private string GetHexColor(Color color)
        {
            return $"#{color.R.ToString("x2").ToUpper()}{color.G.ToString("x2").ToUpper()}{color.B.ToString("x2").ToUpper()}";
        }

        private void SendMessage()
        {
            if (!tbInput.Text.IsNullOrWhitespace())
            {
                MessageSent?.Invoke(tbInput.Text.Trim());
                AppendMessageFromThisUser(tbInput.Text.Trim());
                tbInput.Text = "";
                tbInput.SelectionStart = 0;
            }
        }

        private void tbInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' || e.KeyChar == '\n')
            {
                e.Handled = true;
                SendMessage();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            Call?.Invoke();
        }
    }
}
