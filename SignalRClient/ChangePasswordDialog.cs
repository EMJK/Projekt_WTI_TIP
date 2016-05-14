using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignalRClient
{
    public partial class ChangePasswordDialog : Form
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        public ChangePasswordDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            OldPassword = tbOldPassword.Text;
            NewPassword = tbNewPassword.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
