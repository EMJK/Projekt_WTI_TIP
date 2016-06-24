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
using Ozeki.VoIP;


namespace Client
{
    public partial class ConversationForm : Form
    {
        private readonly string _thisUserId;
        private readonly string _otherUserId;
        private readonly HtmlPanel _htmlPanel;

        private readonly Color _textColor = Color.Black;
        private readonly Color _timestampColor = Color.DarkGray;
        private readonly Color _thisUserColor = Color.DodgerBlue;
        private readonly Color _otherUserColor = Color.DarkOrange;
        private readonly int _fontSize = 1;
        static ConversationForm me;

        public event Action<string> MessageSent;
        public event Action Call;
        public event Action HangUp;

        private static Softphone _mySoftphone; // softphone object

        public ConversationForm(string thisUserId, string otherUserId, string hash_pass)
        {
            _thisUserId = thisUserId;
            _otherUserId = otherUserId;
            InitializeComponent();
            this.Text = $"Conversation with {otherUserId}";
            _htmlPanel = new HtmlPanel();
            panel1.Controls.Add(_htmlPanel);
            me = this;
            _htmlPanel.Dock = DockStyle.Fill;           
            InitSoftphone();
            ReadRegisterInfos(thisUserId, hash_pass);
        }
        
        public void btnCallEnable(bool on)
        {
            btnCall.Enabled = on;
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

        private void btnCall_Click(object sender, EventArgs e)
        {
            _mySoftphone.StartCall(_otherUserId);
        }

        private static void InitSoftphone()
        {
            _mySoftphone = new Softphone();
            _mySoftphone.PhoneLineStateChanged += mySoftphone_PhoneLineStateChanged;
            _mySoftphone.CallStateChanged += mySoftphone_CallStateChanged;
            _mySoftphone.IncomingCall += mySoftphone_IncomingCall;
        }

        private static void ReadRegisterInfos(string user, string pass)
        {
            var registrationRequired = true;
            var authenticationId = user;

            var userName = user;

            var displayName = user;

            var registerPassword = pass;

            var domainHost = "10.0.0.3";

            int domainPort = 5060;

            // When every information has been given, we are trying to register to the serer with the softphone's Register() method.
            _mySoftphone.Register(registrationRequired, displayName, userName, authenticationId, registerPassword,
                                 domainHost, domainPort);
        }

        private static void mySoftphone_CallStateChanged(object sender, CallStateChangedArgs e)
        {
            Console.WriteLine("Call state changed: {0}", e.State);

            if (e.State.IsInCall())
                me.btnCallEnable(false);
            else
                me.btnCallEnable(true);
        }

        static void mySoftphone_PhoneLineStateChanged(object sender, RegistrationStateChangedArgs e)
        {
            Console.WriteLine("Phone line state changed: {0}", e.State);

            if (e.State == RegState.Error || e.State == RegState.NotRegistered)
            {
                MessageBox.Show("Failed to register, insert valid data");
            }
            else if (e.State == RegState.RegistrationSucceeded)
            {
                me.btnCallEnable(true);
            }
        }

        static void mySoftphone_IncomingCall(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("New incoming call from" + me._otherUserId + ". Answer?", "Incoming Call", MessageBoxButtons.YesNo);
            Console.WriteLine("\nIncoming call!");
            Console.WriteLine("Call accepted.");
            if (result == DialogResult.Yes)
                _mySoftphone.AcceptCall();
            else
                _mySoftphone.HangUp();
        }
    }
}
