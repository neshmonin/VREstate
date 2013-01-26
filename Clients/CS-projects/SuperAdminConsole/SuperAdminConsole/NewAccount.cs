using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CoreClasses;
using System.Net;

namespace SuperAdminConsole
{
    public partial class NewAccount : Form
    {
        bool verificationSent = false;
        public NewAccount()
        {
            InitializeComponent();
            UpdateState();
        }

        private void buttonSendVerify_Click(object sender, EventArgs e)
        {
            //  program?
            //           q=register&
            //           entity=user&
            //           role=sellingagent&
            //           email=<email address>
            string parameters = string.Format("q=register&entity=user&role=sellingagent&email={0}",
                textBoxEmail.Text);
            ServerResponse resp = ServerProxy.MakeGenericRequest(ServerProxy.RequestType.Get, 
                                                              "program",
                                                              parameters, null);
            if (HttpStatusCode.OK == resp.ResponseCode)
            {
                // Email verification message
                verificationSent = true;
                UpdateState();
                MessageBox.Show("Email verification message has been sent to the user \'" + textBoxEmail.Text + "\'");
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
                return;
            }

            string errMessage = string.Format("Failed creating account for user \'{0}\': {1}",
                textBoxEmail.Text, resp.ResponseCode.ToString());
            MessageBox.Show(errMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateState();
        }

        private void UpdateState()
        {
            int indexOfAT = textBoxEmail.Text.IndexOf('@');
            int indexOfDOT = textBoxEmail.Text.LastIndexOf('.');
            buttonSendVerify.Enabled = textBoxEmail.Text.Length != 0 &&
                                       indexOfAT > 0 &&
                                       indexOfDOT > indexOfAT+3 &&
                                       indexOfDOT < textBoxEmail.Text.Length - 2;
            textBoxEmail.Enabled = !verificationSent;
        }

        private void textBoxNickname_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void textBoxEmail_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }
    }
}
