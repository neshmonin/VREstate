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
        const string EMPTY = "";
        const string WEB_PREFIX = @"http://";
        const string PHONE_PREFIX = "416 ";
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

            comboBoxCountry.SelectedIndex = 0;

            textBoxWebsite.Text = EMPTY;
            textBoxWebsite.GotFocus += (sender, e) =>
            {
                if (textBoxWebsite.Text.Equals(EMPTY))
                {
                    textBoxWebsite.Text = WEB_PREFIX;
                    textBoxWebsite.SelectionStart = EMPTY.Length;
                }
            };
            textBoxWebsite.LostFocus += (sender, e) =>
            {
                if (textBoxWebsite.Text.Trim().Length == 0 || textBoxWebsite.Text == WEB_PREFIX)
                    textBoxWebsite.Text = EMPTY;
            };

            textBoxPhone.Text = EMPTY;
            textBoxPhone.GotFocus += (sender, e) =>
            {
                if (textBoxPhone.Text.Equals(EMPTY))
                {
                    textBoxPhone.Text = PHONE_PREFIX;
                    textBoxPhone.SelectionStart = PHONE_PREFIX.Length;
                }
            };
            textBoxPhone.LostFocus += (sender, e) =>
            {
                if (textBoxPhone.Text.Trim().Length == 0 || textBoxPhone.Text == PHONE_PREFIX)
                    textBoxPhone.Text = EMPTY;
            };

            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                          "brokerage",
                                                          "", null);

            if (resp != null && HttpStatusCode.OK == resp.ResponseCode)
            {
                ClientData brokerageJASON = resp.Data;
                ClientData[] brokerages = brokerageJASON.GetNextLevelDataArray("brokerages");

                foreach (ClientData cd in brokerages)
                {
                    BrokerageInfo brokerage = new BrokerageInfo(cd);
                    comboBoxBrokerages.Items.Add(brokerage);
                }
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
                case "New Selling Agent":
                    buttonSendVerify.Enabled = textBoxEmail.Text.Length != 0 &&
                                               indexOfAT > 0 &&
                                               indexOfDOT > indexOfAT + 3 &&
                                               indexOfDOT < textBoxEmail.Text.Length - 2;
                    textBoxEmail.Enabled = !verificationSent;
                    break;
                case "New Agent":
                    buttonCreateAgent.Enabled = textBoxEmail.Text.Length != 0 &&
                                               indexOfAT > 0 &&
                                               indexOfDOT > indexOfAT + 3 &&
                                               indexOfDOT < textBoxEmail.Text.Length - 2 &&
                                               textBoxNickname.Text.Length > 1 &&
                                               comboBoxBrokerages.SelectedItem != null &&
                                               !string.IsNullOrEmpty(textBoxUID.Text) &&
                                                   textBoxPWD.Text.Length > 6 &&
                                                   textBoxPWD.Text == textBoxPWDconfirm.Text;
                    break;
                case "New Brokerage":
                    buttonCreateAgent.Enabled = textBoxEmail.Text.Length != 0 &&
                                               indexOfAT > 0 &&
                                               indexOfDOT > indexOfAT + 3 &&
                                               indexOfDOT < textBoxEmail.Text.Length - 2 &&
                                               textBoxNickname.Text.Length > 1;
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

        private void buttonCreateBrokerage_Click(object sender, EventArgs e)
        {
            // data/brokerage
            // JSON -> {"name":"RE/MAX CONDOS PLUS CORP. BROKERAGE", "streetAddress":"45 Harbour Square", 
            //  "city":"Toronto", "stateProvince":"ON", "postalCode":"M5J2G4", "country":"Canada", 
            // "phoneNumbers":["14162036636","14162031908"], 
            // "emails":["info@torontocondo.com","jjohnston@remaxcondosplus.com"], 
            // "webSite":"http://www.remaxcondosplus.com/"}
            BrokerageInfo brokerage = new BrokerageInfo(textBoxNickname.Text);
            brokerage.StreetAddress = textBoxStreet.Text;
            brokerage.City = textBoxCity.Text;
            brokerage.StateProvince = textBoxProvince.Text;
            brokerage.PostalCode = textBoxPostal.Text;
            brokerage.Country = comboBoxCountry.Text;
            brokerage.WebSite = textBoxWebsite.Text;
            brokerage.PhoneNumberList[0] = textBoxPhone.Text;
            brokerage.EmailList[0] = textBoxEmail.Text;

            ClientData data = brokerage.GetClientData();

            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Insert,
                                                          "brokerage",
                                                          "", data);

            if (resp != null && HttpStatusCode.OK == resp.ResponseCode)
            {
                // Email verification message
                verificationSent = true;
                UpdateState();
                MessageBox.Show("New Brokerage account has been created for \'" + textBoxNickname.Text + "\'",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
                return;
            }

            string errMessage = string.Format("Failed creating a Brokerage account for \'{0}\': {1}",
                textBoxNickname.Text, resp.ResponseCode.ToString());
            MessageBox.Show(errMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateState();
        }

        private void buttonCreateAgent_Click(object sender, EventArgs e)
        {
            // data/user
            // JSON -> {"name":"RE/MAX CONDOS PLUS CORP. BROKERAGE", "streetAddress":"45 Harbour Square", 
            //  "city":"Toronto", "stateProvince":"ON", "postalCode":"M5J2G4", "country":"Canada", 
            // "phoneNumbers":["14162036636","14162031908"], 
            // "emails":["info@torontocondo.com","jjohnston@remaxcondosplus.com"], 
            // "webSite":"http://www.remaxcondosplus.com/"}
            User agent = new User(6, User.Role.Agent);
            agent.NickName = textBoxNickname.Text;
            agent.PrimaryEmailAddress = textBoxEmail.Text;
            BrokerageInfo broker = comboBoxBrokerages.SelectedItem as BrokerageInfo;
            ClientData data = agent.GetClientData();
            data["brokerageId"] = broker.AutoID;
            data["uid"] = textBoxUID.Text;
            data["pwd"] = textBoxPWD.Text;

            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Insert,
                                                          "user",
                                                          "", data);

            if (resp != null && HttpStatusCode.OK == resp.ResponseCode)
            {
                // Email verification message
                verificationSent = true;
                UpdateState();
                MessageBox.Show("New Agent account has been created for \'" + textBoxNickname.Text + "\'",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
                return;
            }

            string errMessage = string.Format("Failed creating an Agent account for \'{0}\':\n{1}",
                textBoxNickname.Text, resp.ResponseCodeDescription);
            MessageBox.Show(errMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateState();
        }

        private void comboBoxBrokerages_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void textBoxUID_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void textBoxPWD_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void textBoxPWDconfirm_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }
    }
}
