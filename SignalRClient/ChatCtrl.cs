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

        public ChatCtrl()
        {
            InitializeComponent();
            SetControlsToLoginState(false);
            _webApiClient = new WebApiClient("http://localhost:9922/");
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var response = _webApiClient.Account.Login(new LoginRequest() { UserID = tbUserID.Text, Password = tbPassword.Text});
                AppendLine($"[HTTP] User `{tbUserID.Text}` logged in.");
                tbSessionID.Text = response.SessionID;
                _chatApiClient = new ChatApiClient("http://localhost:9923/", tbUserID.Text, response.SessionID);
                _chatApiClient.Hub.SubscribeOn<MessageParam>(c => c.Message, msg =>
                {
                    AppendLine($"[CHAT] From {msg.SenderUserID}: {msg.Message}");
                });
                _chatApiClient.Hub.SubscribeOn<ClientListParam>(c => c.ClientList, msg =>
                {
                    
                    AppendLine($"[CHAT] Clients: {msg.Clients.Aggregate((a, b) => $"{a}, {b}")}");
                });

                SetControlsToLoginState(true);
            }
            catch (Exception ex)
            {
                AppendLine($"[HTTP] Could not log in: {ex.Message}");
            }
        }

        private void SetControlsToLoginState(bool isLoggedIn)
        {
            tbUserID.ReadOnly = isLoggedIn;
            tbPassword.ReadOnly = isLoggedIn;
            tbInput.Enabled = isLoggedIn;
            btnLogin.Enabled = !isLoggedIn;
            btnLogout.Enabled = isLoggedIn;
            btnChangePassword.Enabled = isLoggedIn;
            btnSend.Enabled = isLoggedIn;
            lbOtherUsers.Enabled = isLoggedIn;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                _webApiClient.Account.Logout(new LogoutRequest() {SessionID = tbSessionID.Text});
                AppendLine($"[HTTP] User `{tbUserID.Text}` logged out.");
                SetControlsToLoginState(false);
            }
            catch (Exception ex)
            {
                AppendLine($"[HTTP] Could not log out: {ex.Message}");
            }

            tbPassword.Clear();
            tbSessionID.Clear();
            tbInput.Clear();
            lbOtherUsers.Items.Clear();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (lbOtherUsers.SelectedItem == null) return;
            AppendLine($"[CHAT] To {tbUserID.Text}: {tbInput.Text}");
            _chatApiClient.Hub.Call(srv => srv.SendMessage(new SendMessageParam()
            {
                Message = tbInput.Text,
                DestinationUserID = (string)lbOtherUsers.SelectedItem
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

        private void FillUserList(IEnumerable<string> users)
        {
            Action action = () =>
            {
                var selected = (string)lbOtherUsers.SelectedItem;
                lbOtherUsers.Items.Clear();
                lbOtherUsers.Items.AddRange(users.Select(u => u.ToLower()).OrderBy(u => u).Cast<object>().ToArray());
                if (lbOtherUsers.Items.Cast<string>().Contains(selected))
                {
                    lbOtherUsers.SelectedItem = selected;
                }
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

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            var form = new ChangePasswordDialog();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _webApiClient.Account.ChangePassword(new ChangePasswordRequest()
                    {
                        OldPassword = form.OldPassword,
                        NewPassword = form.NewPassword,
                        SessionID = tbSessionID.Text,
                        UserID = tbUserID.Text
                    });
                    AppendLine($"[HTTP] Your password has been changed.");
                }
                catch (Exception ex)
                {
                    AppendLine($"[HTTP] Failed to change password: {ex.Message}");
                }
            }
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            var form = new CreateAccountDialog();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _webApiClient.Account.CreateAccount(new CreateAccountRequest()
                    {
                        UserID = form.UserName,
                        Password = form.Password
                    });
                    AppendLine($"[HTTP] New account has been created for user `{form.UserName}`.");
                }
                catch (Exception ex)
                {
                    AppendLine($"[HTTP] Failed to create account for user `{form.UserName}`: {ex.Message}");
                }
            }
        }
    }
}
