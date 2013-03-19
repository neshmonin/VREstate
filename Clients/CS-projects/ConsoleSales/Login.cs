using System;
using System.Windows.Forms;
//using CoreClasses;

namespace ConsoleSales
{
    public partial class Login : Form
    {
        //public delegate void LoginProc(string serverInfo, string loginType, string login, string password);

        public Login(string server, string login, string role)
        {
            InitializeComponent();
            lblServer.Text = server;
            cbxLoginTypes.Items.AddRange(new string[] { "superadmin", "developeradmin", "salesperson" });
            int index = cbxLoginTypes.FindStringExact(role);
            if (index != -1)
                cbxLoginTypes.SelectedIndex = index;
            tbLogin.Text = login;
        }

        private void Login_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ('\x0D' == e.KeyChar)
            {
                e.Handled = true;
                DialogResult = DialogResult.OK;
            }
            else if ('\x1B' == e.KeyChar)
            {
                e.Handled = true;
                DialogResult = DialogResult.Cancel;
            }
        }

        private void Login_Shown(object sender, EventArgs e)
        {
            if (tbLogin.Text.Length > 0) tbPassword.Focus();
            else tbLogin.Focus();
        }

        //public bool PerformLogin(string serverInfo, string[] loginTypes, LoginProc proc)
        //{
        //    lblServer.Text = serverInfo;

        //    cbxLoginTypes.Items.Clear();
        //    cbxLoginTypes.Items.AddRange(loginTypes);

        //    if (DialogResult.OK == ShowDialog())
        //    {
        //        proc(serverInfo, cbxLoginTypes.SelectedItem as string, tbLogin.Text, tbPassword.Text);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
