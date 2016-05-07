using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RegistrarChatApiClient;
using RegistrarWebApiClient;
using RegistrarWebApiClient.Models.Account;

namespace SignalRClient
{
    public partial class ChatCtrl : UserControl
    {
        private WebApiClient _webApiClient;
        private ChatApiClient _chatApiClient;
        private string _otherUserID;

        public ChatCtrl()
        {
            InitializeComponent();
            _webApiClient = new WebApiClient("http://localhost:9922/");
        }

        public void SetUserUserIDs(string thisUser, string otherUser)
        {
            tbUserID.Text = thisUser.ToLower();
            _otherUserID = otherUser.ToLower();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var response = _webApiClient.Account.Login(new LoginRequest() { UserID = tbUserID.Text });
            tbSessionID.Text = response.SessionID;
            _chatApiClient = new ChatApiClient("http://localhost:9923/", tbUserID.Text, response.SessionID);
            _chatApiClient.Hub.SubscribeOn<MessageParam>(c => c.Message, msg =>
            {
                AppendLine($"{msg.SenderUserID}: {msg.Message}");
            });
            _chatApiClient.Hub.SubscribeOn<ClientListParam>(c => c.ClientList, msg =>
            {
                AppendLine($"Clients: {msg.Clients.Aggregate((a,b) => $"{a}, {b}")}");
            });
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            _webApiClient.Account.Logout(new LogoutRequest() { SessionID = tbSessionID.Text });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            AppendLine($"{tbUserID.Text}: {tbInput.Text}");
            _chatApiClient.Hub.Call(srv => srv.SendMessage(new SendMessageParam()
            {
                Message = tbInput.Text,
                DestinationUserID = _otherUserID
            }));
            tbInput.Clear();
        }

        private void AppendLine(string str)
        {
            Action action = () =>
            {
                tbOutput.Text += $"{str}{Environment.NewLine}";
            };
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
