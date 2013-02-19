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
using Vre.Server.BusinessLogic;

namespace SuperAdminConsole
{
    public partial class NewAccount : Form
    {
        bool verificationSent = false;
        bool asSuperAdmin = false;
        public NewAccount(bool asSuperAdmin)
        {
            InitializeComponent();
            this.asSuperAdmin = asSuperAdmin;
            if (asSuperAdmin)
                tabPageNewDevAdmin.Visible = true;

            if (SuperServer.Developers.Count > 0)
            {
                int index = 0;
                foreach (var developer in SuperServer.Developers.Values)
                {
                    comboBoxDeveloper.Items.Add(developer);
                    if (developer.Name == "Resale")
                        index = comboBoxDeveloper.Items.Count-1;
                }

                comboBoxDeveloper.SelectedIndex = index;
            }

            UpdateState();
        }

        private void buttonSendVerify_Click(object sender, EventArgs e)
        {
            //  program?q=register&entity=user&role=sellingagent&email=<email address>
            string parameters = string.Format("q=register&entity=user&role=sellingagent&email={0}",
                                    textBoxEmail.Text);

            ServerResponse resp = ServerProxy.MakeGenericRequest(ServerProxy.RequestType.Get,
                                                            "program",
                                                            parameters, null);

            if (resp != null && HttpStatusCode.OK == resp.ResponseCode)
            {
                // Email verification message
                verificationSent = true;
                UpdateState();
                MessageBox.Show("Email verification message has been sent to the user \'" + textBoxEmail.Text + "\'");
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
                return;
            }

            string errMessage = string.Format("Failed creating DeveloperAdmin account for user \'{0}\': {1}",
                textBoxEmail.Text, resp.ResponseCode.ToString());
            MessageBox.Show(errMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateState();
        }

        private void UpdateState()
        {
            int indexOfAT = textBoxEmail.Text.IndexOf('@');
            int indexOfDOT = textBoxEmail.Text.LastIndexOf('.');
            switch (tabControAccountTypes.SelectedTab.Text)
            {
                case "New DeveAdmin":
                    buttonCreateDevAdmin.Enabled = !string.IsNullOrEmpty(textBoxLoginName.Text) &&
                                                   textBoxPassword.Text.Length > 6 &&
                                                   textBoxPassword.Text == textBoxPassword2.Text &&
                                                   (textBoxEmail.Text == string.Empty ||
                                                        (textBoxEmail.Text.Length != 0 &&
                                                         indexOfAT > 0 &&
                                                         indexOfDOT > indexOfAT + 3 &&
                                                         indexOfDOT < textBoxEmail.Text.Length - 2));
                    break;
                case "New Customer":
                    buttonSendVerify.Enabled = textBoxEmail.Text.Length != 0 &&
                                               indexOfAT > 0 &&
                                               indexOfDOT > indexOfAT + 3 &&
                                               indexOfDOT < textBoxEmail.Text.Length - 2;
                    textBoxEmail.Enabled = !verificationSent;
                    break;
            }
        }

        private void onTextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void buttonCreateDevAdmin_Click(object sender, EventArgs e)
        {
            // data/user?role=DeveloperAdmin&ed=Resale&uid=userX&pwd=myPassword
            // JSON -> {"nickName":"<text>", "primaryEmail":"<text>","personalInfo":"<text>"}
            Developer dev = comboBoxDeveloper.SelectedItem as Developer;
            //string parameters = string.Format("role=DeveloperAdmin&ed={0}&uid={1}&pwd={2}",
            //                            "6",//dev == null ? "6" : dev.Name,
            //                            textBoxLoginName.Text,
            //                            textBoxPassword.Text);

            Dictionary<string, object> JSON = new Dictionary<string, object>();
            JSON.Add("role", "DeveloperAdmin");
            JSON.Add("ed", dev.Name);
            JSON.Add("uid", textBoxLoginName.Text);
            JSON.Add("pwd", textBoxPassword.Text);
            if (textBoxNickname.Text != string.Empty)
                JSON.Add("nickName", textBoxNickname.Text);
            if (textBoxEmail.Text != string.Empty)
                JSON.Add("primaryEmail", textBoxEmail.Text);

            ClientData data = new ClientData(JSON);

            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Insert,
                                                          "user",
                                                          "", data);

            if (resp != null && HttpStatusCode.OK == resp.ResponseCode)
            {
                // Email verification message
                verificationSent = true;
                UpdateState();
                MessageBox.Show("New DeveloperAdmin account has been created for the user \'" + textBoxEmail.Text + "\'");
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
                return;
            }

            string errMessage = string.Format("Failed creating account for user \'{0}\': {1}",
                textBoxEmail.Text, resp.ResponseCode.ToString());
            MessageBox.Show(errMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateState();
        }
    }
}
