using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SuperAdminConsole
{
    public partial class LoginForm : Form
    {
        public LoginForm(string server, string login)
        {
            InitializeComponent();
            lblServer.Text = server;
            tbLogin.Text = login;
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            if (tbLogin.Text.Length > 0) tbPassword.Focus();
            else tbLogin.Focus();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

    }
}
