using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Julas.Utils;
using Julas.Utils.Collections;
using Julas.Utils.Extensions;
using TheArtOfDev.HtmlRenderer.WinForms;
using Ozeki.VoIP;
using VoipClient;


namespace Client
{
    public partial class ConversationForm : Form
    {
        private volatile bool _isInCall = false;

        private readonly string _thisUserId;
        private readonly string _otherUserId;
        private readonly HtmlPanel _htmlPanel;

        private readonly Color _textColor = Color.Black;
        private readonly Color _timestampColor = Color.DarkGray;
        private readonly Color _thisUserColor = Color.DodgerBlue;
        private readonly Color _otherUserColor = Color.DarkOrange;
        private readonly Color _enabledBtnColor = Color.FromArgb(255, 255, 255);
        private readonly Color _disabledBtnColor = Color.FromArgb(226, 226, 226);

        private readonly int _fontSize = 1;

        private readonly VoipClientModule _voipClient;

        public event Action<string> MessageSent;
        public event Action Call;
        public event Action HangUp;

        public ConversationForm(string thisUserId, string otherUserId, string hash_pass, VoipClientModule voipClient)
        {
            _thisUserId = thisUserId;
            _otherUserId = otherUserId;
            _voipClient = voipClient;
            InitializeComponent();
            this.Text = $"Conversation with {otherUserId}";
            _htmlPanel = new HtmlPanel();
            panel1.Controls.Add(_htmlPanel);
            _htmlPanel.Dock = DockStyle.Fill;           
            _voipClient.PhoneStateChanged += VoipClientOnPhoneStateChanged;
        }

        public new void Dispose()
        {
            _voipClient.PhoneStateChanged -= VoipClientOnPhoneStateChanged;
            base.Dispose(true);
        }

        private void VoipClientOnPhoneStateChanged(PhoneState phoneState)
        {
            Invoke(() =>
            {
                if (!phoneState.OtherUserId.IsOneOf(null, _otherUserId) || phoneState.Status.IsOneOf(PhoneStatus.Registering))
                {
                    btnCall.Enabled = false;
                    btnHangUp.Enabled = false;
                    btnCall.BackColor = _disabledBtnColor;
                    btnHangUp.BackColor = _disabledBtnColor;
                    return;
                }
                else
                {
                    switch (phoneState.Status)
                    {
                        case PhoneStatus.Calling:
                        {
                            btnCall.Enabled = false;
                            btnHangUp.Enabled = true;
                            btnCall.BackColor = _disabledBtnColor;
                            btnHangUp.BackColor = _enabledBtnColor;
                            break;
                        }
                        case PhoneStatus.InCall:
                        {
                            btnCall.Enabled = false;
                            btnHangUp.Enabled = true;
                            btnCall.BackColor = _disabledBtnColor;
                            btnHangUp.BackColor = _enabledBtnColor;
                            break;
                        }
                        case PhoneStatus.IncomingCall:
                        {
                            btnCall.Enabled = true;
                            btnHangUp.Enabled = true;
                            btnCall.BackColor = _enabledBtnColor;
                            btnHangUp.BackColor = _enabledBtnColor;
                            break;
                        }
                        case PhoneStatus.Registered:
                        {
                            btnCall.Enabled = true;
                            btnHangUp.Enabled = false;
                            btnCall.BackColor = _enabledBtnColor;
                            btnHangUp.BackColor = _disabledBtnColor;
                            break;
                        }
                    }
                }
            });
        }

        public void AppendMessageFromOtherUser(string message)
        {
            AppendMessage(message, _otherUserId, _otherUserColor);
        }

        private void AppendMessageFromThisUser(string message)
        {
            AppendMessage(message, _thisUserId, _thisUserColor);
        }

        private void AppendMessage(string msg, string from, Color nameColor)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<p>");
            sb.Append($"<b><font color=\"{GetHexColor(nameColor)}\" size=\"{_fontSize}\">{from}</font></b> ");
            sb.Append($"<font color=\"{GetHexColor(_timestampColor)}\" size=\"{_fontSize}\">{DateTime.Now.ToString("HH:mm:ss")}</font>");
            sb.Append("<br/>");
            sb.Append($"<font color=\"{GetHexColor(_textColor)}\" size=\"{_fontSize}\">{msg}</font>");
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

        private void ConversationForm_Load(object sender, EventArgs e)
        {
            VoipClientOnPhoneStateChanged(_voipClient.PhoneState);
        }

        private void Invoke(Action action)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(action));
            }
            else
            {
                action();
            }
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            btnCall.Enabled = false;
            btnHangUp.Enabled = false;
            btnCall.BackColor = _disabledBtnColor;
            btnHangUp.BackColor = _disabledBtnColor;

            switch (_voipClient.PhoneState.Status)
            {
                case PhoneStatus.IncomingCall:
                {
                    _voipClient.AnswerCall();
                    break;
                }
                case PhoneStatus.Registered:
                {
                    _voipClient.StartCall(_otherUserId);
                    break;
                }
            }
        }

        private void btnHangUp_Click(object sender, EventArgs e)
        {
            btnCall.Enabled = false;
            btnHangUp.Enabled = false;
            btnCall.BackColor = _disabledBtnColor;
            btnHangUp.BackColor = _disabledBtnColor;

            switch (_voipClient.PhoneState.Status)
            {
                case PhoneStatus.IncomingCall:
                {
                    _voipClient.RejectCall();
                    break;
                }
                case PhoneStatus.InCall:
                {
                    _voipClient.EndCall();
                    break;
                }
                case PhoneStatus.Calling:
                {
                    _voipClient.EndCall();
                    break;
                }
            }
        }
    }
}
