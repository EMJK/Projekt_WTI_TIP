using System;
using System.Collections.Concurrent;
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

namespace Client
{
    public partial class ChatCtrl : UserControl
    {
        private WebApiClient _webApiClient;
        private ChatApiClient _chatApiClient;
        private ConcurrentDictionary<string, ConversationForm> _forms; 

        public ChatCtrl()
        {
            InitializeComponent();
            SetControlsToLoginState(false);
            _webApiClient = new WebApiClient("http://localhost:9922/");
            _forms = new ConcurrentDictionary<string, ConversationForm>();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var response = _webApiClient.Account.Login(new LoginRequest() { UserID = tbUserID.Text, Password = tbPassword.Text});
                AppendLine($"User `{tbUserID.Text}` logged in.");
                tbSessionID.Text = response.SessionID;
                _chatApiClient = new ChatApiClient("http://localhost:9923/", tbUserID.Text, response.SessionID);
                _chatApiClient.Hub.SubscribeOn<MessageParam>(c => c.Message, msg =>
                {
                    Invoke(() =>
                    {
                        ConversationForm form = StartConversation(msg.SenderUserID);
                        form.AppendMessageFromOtherUser(msg.Message);
                    });
                });
                _chatApiClient.Hub.SubscribeOn<ClientListParam>(c => c.ClientList, msg =>
                {
                    Invoke(() => FillUserList(msg.Clients));
                });

                SetControlsToLoginState(true);
            }
            catch (WebApiException ex)
            {
                AppendLine($"Could not log in: {ex.ResponseCode} {ex.ResponseMessage}");
            }
        }

        private void SetControlsToLoginState(bool isLoggedIn)
        {
            tbUserID.ReadOnly = isLoggedIn;
            tbPassword.ReadOnly = isLoggedIn;
            btnLogin.Enabled = !isLoggedIn;
            btnLogout.Enabled = isLoggedIn;
            btnChangePassword.Enabled = isLoggedIn;
            lbOtherUsers.Enabled = isLoggedIn;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                _webApiClient.Account.Logout(new LogoutRequest() {SessionID = tbSessionID.Text});
                //AppendLine($"User `{tbUserID.Text}` logged out.");
                SetControlsToLoginState(false);
                tbPassword.Clear();
                tbSessionID.Clear();
                lbOtherUsers.Items.Clear();
                tbLog.Clear();
            }
            catch (WebApiException ex)
            {
                AppendLine($"Could not log out: {ex.ResponseCode} {ex.ResponseMessage}");
            }
        }

        private void FillUserList(IEnumerable<string> users)
        {
            Invoke(() =>
            {
                var selected = (string)lbOtherUsers.SelectedItem;
                lbOtherUsers.Items.Clear();
                lbOtherUsers.Items.AddRange(users
                    .Select(u => u.ToLower())
                    .Where(u => u != tbUserID.Text.ToLower())
                    .OrderBy(u => u)
                    .Cast<object>()
                    .ToArray());
                if (lbOtherUsers.Items.Cast<string>().Contains(selected))
                {
                    lbOtherUsers.SelectedItem = selected;
                }
            });
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
                    AppendLine("Your password has been changed.");
                }
                catch (WebApiException ex)
                {
                    AppendLine($"Failed to change password: {ex.ResponseCode} {ex.ResponseMessage}");
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
                    AppendLine($"New account has been created for user `{form.UserName}`.");
                }
                catch (WebApiException ex)
                {
                    AppendLine($"Failed to create account for user `{form.UserName}`: {ex.ResponseCode} {ex.ResponseMessage}");
                }
            }
        }

        private void AppendLine(string str)
        {
            tbLog.AppendText($"{str}{Environment.NewLine}");
            tbLog.ScrollToCaret();
        }

        private void tbUserID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) btnLogin_Click(null, e);
        }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) btnLogin_Click(null, e);
        }

        private void lbOtherUsers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var person = lbOtherUsers.SelectedItem as string;
            if (person == null) return;
            StartConversation(person);
        }

        private ConversationForm StartConversation(string person)
        {
            ConversationForm chatForm = null;
            Invoke(() =>
            {
                chatForm = _forms.GetOrAdd(person, _ =>
                {
                    var form = new ConversationForm(tbUserID.Text, person);
                    _forms[person] = form;
                    var msgSent = new Action<string>(msg =>
                    {
                        _chatApiClient.Hub.Call(c => c.SendMessage(new SendMessageParam()
                        {
                            DestinationUserID = person,
                            Message = msg
                        }));
                    });
                    form.MessageSent += msgSent;
                    form.Closed += (sender, args) =>
                    {
                        form.MessageSent -= msgSent;
                        ConversationForm tmp;
                        _forms.TryRemove(person, out tmp);
                    };
                    form.Show();
                    return form;
                });
                chatForm.BringToFront();
            });
            return chatForm;
        }

        private void Invoke(Action action)
        {
            if (this.InvokeRequired) this.Invoke(new MethodInvoker(action));
            else action();
        }
    }
}
