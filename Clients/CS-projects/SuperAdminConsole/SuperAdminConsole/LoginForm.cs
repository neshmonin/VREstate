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
        public LoginForm(string login)
        {
            InitializeComponent();
            tbLogin.Text = login;
            update();
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

        private void checkBoxIsSuperadmin_CheckedChanged(object sender, EventArgs e)
        {
            update();
        }

        public bool IsSuperAdmin { get; private set; }

        public bool IsMainServer 
        {
            get { return radioButtonMainServer.Checked; }
        }

        private void update()
        {
            IsSuperAdmin = !checkBoxIsSuperadmin.Checked;
            labelEstateDeveloper.Enabled = !IsSuperAdmin;
            textBoxEstateDeveloper.Enabled = !IsSuperAdmin;
            buttonLogin.Enabled = IsSuperAdmin ?
                tbLogin.Text.Length > 0 && tbPassword.Text.Length > 0 :
                tbLogin.Text.Length > 0 && tbPassword.Text.Length > 0 && textBoxEstateDeveloper.Text.Length > 0;
        }

        private void tbLogin_TextChanged(object sender, EventArgs e)
        {
            update();
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
            update();
        }

        private void textBoxEstateDeveloper_TextChanged(object sender, EventArgs e)
        {
            update();
        }
    }
}
