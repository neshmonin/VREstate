using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using Vre.Server.BusinessLogic;
using CoreClasses;

namespace SuperAdminConsole
{
    public partial class EmailViewOrder : Form
    {
        public enum Template 
        {
            ListingPromotion,
            ListingCopyingAsPrivate,
            OrderConfirmation
        }

        static MailAddress fromAddress = new MailAddress(Properties.Settings.Default.emailFrom, "3D Condo Explorer");
        static Helpers.TransactionInfo trInfo;
        static User theUser;
        static ViewOrder theOrder;
        MailAddress toAddress;
        static string fromPassword = Properties.Settings.Default.emailPwd;
        static string paymentRefId;
        string subject = string.Empty;
        string body = string.Empty;
        static SmtpClient smtp;
        static string readableName;
        string email;
        const string ParameterOpen = "<";
        const string ParameterClose = ">";
        string Parameters = ParameterOpen + "..." + ParameterClose + ParameterOpen + "..." + ParameterClose;

        Dictionary<Word, string> wordsDictionary = new Dictionary<Word, string>();
        Dictionary<Condition, string> conditionsDictionary = new Dictionary<Condition, string>();

        public enum Word
        {
            ADDRESS_TO,
            VIEWORDER_ADDRESS,
            VIEWORDER_URL,
            PRICE_NO_TAX,
            DAYS_VALID,
            VALID_UNTIL_TIME_DAY,
            TODAYS_DATE,
            MLS_NUMBER,
            SIGNATURE,
            PRODUCT_NAME,
            PAYMENT_REF,
            VTOUR_URL,
            TAX,
            PAID_GROSS
        }

        public enum Condition
        {
            IF_EQUAL,
            IF_NOTEQUAL,
            ENDIF
        }

        public static EmailViewOrder Create(string viewOrderId, string payRefId, Template template)
        {
            readableName = Helpers.LabelFromViewOrder(viewOrderId, out theOrder, out theUser);
            trInfo = Helpers.TransactionDetails(payRefId);
            paymentRefId = payRefId;

            smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            EmailViewOrder form = new EmailViewOrder(template);
            return form;
        }

        public EmailViewOrder(Template template)
        {
            InitializeComponent();
            if (template == Template.OrderConfirmation)
            {
                email = string.IsNullOrEmpty(theUser.PrimaryEmailAddress) ? theUser.NickName : theUser.PrimaryEmailAddress;
                textBoxAddressTo.Text = email;
            }

            wordsDictionary.Add(Word.ADDRESS_TO, "{ADDRESS_TO}");
            wordsDictionary.Add(Word.VIEWORDER_ADDRESS, "{VIEWORDER_ADDRESS}");
            wordsDictionary.Add(Word.VIEWORDER_URL, "{VIEWORDER_URL}");
            wordsDictionary.Add(Word.PRICE_NO_TAX, "{PRICE_NO_TAX}");
            wordsDictionary.Add(Word.DAYS_VALID, "{DAYS_VALID}");
            wordsDictionary.Add(Word.VALID_UNTIL_TIME_DAY, "{VALID_UNTIL_TIME_DAY}");
            wordsDictionary.Add(Word.TODAYS_DATE, "{TODAYS_DATE}");
            wordsDictionary.Add(Word.MLS_NUMBER, "{MLS_NUMBER}");
            wordsDictionary.Add(Word.SIGNATURE, "{SIGNATURE}");
            wordsDictionary.Add(Word.PRODUCT_NAME, "{PRODUCT_NAME}");
            wordsDictionary.Add(Word.PAYMENT_REF, "{PAYMENT_REF}");
            wordsDictionary.Add(Word.VTOUR_URL, "{VTOUR_URL}");
            wordsDictionary.Add(Word.TAX, "{TAX}");
            wordsDictionary.Add(Word.PAID_GROSS, "{PAID_GROSS}");

            conditionsDictionary.Add(Condition.IF_EQUAL, "{IF_EQUAL}" + Parameters);
            conditionsDictionary.Add(Condition.IF_NOTEQUAL, "{IF_NOTEQUAL}" + Parameters);
            conditionsDictionary.Add(Condition.ENDIF, "{ENDIF}");

            loadFromTemplate(template);
            theTemplate = template;
            UpdateState();
        }

        private void loadFromTemplate(Template template)
        {
            switch (template)
            {
                case Template.OrderConfirmation:
                    body = Properties.Settings.Default.orderConfirmationTemplate;
                    subject = Properties.Settings.Default.defaultConfirmationSubject;
                    textBody.Focus();
                    break;
                case Template.ListingCopyingAsPrivate:
                    body = Properties.Settings.Default.listingCopyingAsPrivate;
                    subject = Properties.Settings.Default.defaultListingCopyingAsPrivateSubject;
                    textBody.Focus();
                    break;
                case Template.ListingPromotion:
                    body = Properties.Settings.Default.listingPromoTemplate;
                    subject = Properties.Settings.Default.defaultPromoSubject;
                    textBoxAddressTo.Focus();
                    break;
            }

            textBody.Text = PreprocessTemplate(body);
            textBoxSubject.Text = PreprocessTemplate(subject);
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                toAddress = new MailAddress(textBoxAddressTo.Text, string.IsNullOrEmpty(theUser.NickName) ? "unknown" : theUser.NickName);
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = textBoxSubject.Text,
                    Body = textBody.Text
                })
                {
                    smtp.Send(message);
                }
                Cursor.Current = Cursors.Default;

                MessageBox.Show("Message sent successfully!", "Send email",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                textBody.Enabled = false;
                textBoxSubject.Enabled = false;
                buttonSend.Enabled = false;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (System.Net.Mail.SmtpException exception)
            {
                string errMsg = exception.Message;
                if (exception.InnerException != null &&
                    !string.IsNullOrEmpty(exception.InnerException.Message))
                {
                    errMsg += string.Format("\n{0}", exception.InnerException.Message);
                }

                Cursor.Current = Cursors.Default;
                MessageBox.Show(errMsg, "Failed to send email", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        Template theTemplate = Template.OrderConfirmation;
        
        private void UpdateState()
        {
            textBoxAddressTo.ReadOnly = theUser.UserRole == 
                User.Role.SellingAgent && !string.IsNullOrEmpty(email);
            int indexOfAT = textBoxAddressTo.Text.IndexOf('@');
            int indexOfDOT = textBoxAddressTo.Text.LastIndexOf('.');
            buttonSend.Enabled = textBoxAddressTo.Text.Length != 0 &&
                                       indexOfAT > 0 &&
                                       indexOfDOT > indexOfAT + 3 &&
                                       indexOfDOT < textBoxAddressTo.Text.Length - 2;
        }

        private void textBoxAddressTo_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private string ProcessCondition(string body, Condition cond)
        {
            /* ========== A sample of a condition =========
            {IF_NOTEQUAL}<{PAID_GROSS}><00.00>
            Price:    ${PRICE_NO_TAX}
            HST:      ${TAX}
            -------------------
            Total:    ${PAID_GROSS}
            {ENDIF}
            =============================================== */
            string strippedCondition = conditionsDictionary[cond].Replace(Parameters, "");
            while (body.Contains(strippedCondition))
            {
                int start_IF = body.IndexOf(strippedCondition, 0);
                body = body.Remove(start_IF, strippedCondition.Length);

                int start_ENDIF = body.IndexOf(conditionsDictionary[Condition.ENDIF], 0);
                body = body.Remove(start_ENDIF, conditionsDictionary[Condition.ENDIF].Length);

                string condition_body = body.Substring(start_IF, start_ENDIF - start_IF);
                body = body.Remove(start_IF, start_ENDIF - start_IF);

                // extract the first parameter:
                int start_param_1 = condition_body.IndexOf(ParameterOpen);
                int end_param_1 = condition_body.IndexOf(ParameterClose, start_param_1);
                string param_1 = condition_body.Substring(start_param_1 + 1, end_param_1 - start_param_1 - 1);
                param_1 = PreprocessTemplate(param_1);

                int start_param_2 = condition_body.IndexOf(ParameterOpen, end_param_1);
                int end_param_2 = condition_body.IndexOf(ParameterClose, start_param_2);
                string param_2 = condition_body.Substring(start_param_2 + 1, end_param_2 - start_param_2 - 1);
                param_2 = PreprocessTemplate(param_2);

                bool conditionValue = true;
                switch (cond)
                {
                    case Condition.IF_EQUAL:
                        conditionValue = param_1 == param_2;
                        break;
                    case Condition.IF_NOTEQUAL:
                        conditionValue = param_1 != param_2;
                        break;
                }

                if (conditionValue)
                {
                    string conditionText = condition_body.Substring(end_param_2+1);
                    conditionText = PreprocessTemplate(conditionText);
                    body = body.Insert(start_IF, conditionText);
                }
            }

            return body;
        }

        private string PreprocessTemplate(string body)
        {
            // First pass - to process the conditions (if any)
            body = ProcessCondition(body, Condition.IF_EQUAL);
            body = ProcessCondition(body, Condition.IF_NOTEQUAL);

            // TODO: get HST and Price from transaction
            decimal taxRate = Properties.Settings.Default.SalesTaxValuePercent / 100M;
            decimal price = 0M;
            decimal tax = 0M;
            if (trInfo != null)
            {
                price = decimal.Round(trInfo.GrossAmount / (1M + taxRate), 2);
                tax = trInfo.GrossAmount - price;
            }

            string product = string.Empty;
            switch (theOrder.Product)
            {
                case ViewOrder.ViewOrderProduct.PrivateListing:
                    product = "Interactive 3D Listing";
                    break;
                case ViewOrder.ViewOrderProduct.PublicListing:
                    product = "Interactive 3D MLS(TM)";
                    break;
                case ViewOrder.ViewOrderProduct.Building3DLayout:
                    product = "Interactive 3D Layout";
                    break;
            }

            string mlsId = theOrder.GetClientData().GetProperty("mlsId", string.Empty);
            if (!string.IsNullOrEmpty(mlsId)) mlsId += " ";

            string signature = string.IsNullOrEmpty(theUser.NickName) ?
                            Properties.Settings.Default.defaultSignature :
                            theUser.NickName;
            if (!signature.Contains('\r'))
            {
                string[] lines = signature.Split('\n');
                if (lines.Length > 1)
                {
                    signature = string.Empty;
                    foreach (string line in lines)
                        signature += line + "\r\n";
                }
            }

            if (body.Contains(wordsDictionary[Word.ADDRESS_TO]))
                body = body.Replace(wordsDictionary[Word.ADDRESS_TO], textBoxAddressTo.Text);
            if (body.Contains(wordsDictionary[Word.VIEWORDER_ADDRESS]))
                body = body.Replace(wordsDictionary[Word.VIEWORDER_ADDRESS], readableName);
            if (body.Contains(wordsDictionary[Word.VIEWORDER_URL]))
                body = body.Replace(wordsDictionary[Word.VIEWORDER_URL], theOrder.ViewOrderURL);
            if (body.Contains(wordsDictionary[Word.PRICE_NO_TAX]))
                body = body.Replace(wordsDictionary[Word.PRICE_NO_TAX], string.Format("{0:00.00}", Properties.Settings.Default.PriceOf3DListing));
            if (body.Contains(wordsDictionary[Word.DAYS_VALID]))
                body = body.Replace(wordsDictionary[Word.DAYS_VALID], ((theOrder.ExpiresOn - DateTime.Now).Days).ToString());
            if (body.Contains(wordsDictionary[Word.VALID_UNTIL_TIME_DAY]))
                body = body.Replace(wordsDictionary[Word.VALID_UNTIL_TIME_DAY], theOrder.ExpiresOn.ToShortTimeString() + 
                                                       " of " + theOrder.ExpiresOn.ToLongDateString());
            if (body.Contains(wordsDictionary[Word.TODAYS_DATE]))
                body = body.Replace(wordsDictionary[Word.TODAYS_DATE], DateTime.Now.ToShortDateString());
            if (body.Contains(wordsDictionary[Word.MLS_NUMBER]))
                body = body.Replace(wordsDictionary[Word.MLS_NUMBER], mlsId);
            if (body.Contains(wordsDictionary[Word.SIGNATURE]))
                body = body.Replace(wordsDictionary[Word.SIGNATURE], signature);
            if (body.Contains(wordsDictionary[Word.PRODUCT_NAME]))
                body = body.Replace(wordsDictionary[Word.PRODUCT_NAME], product);
            if (body.Contains(wordsDictionary[Word.PAYMENT_REF]))
                body = body.Replace(wordsDictionary[Word.PAYMENT_REF], paymentRefId);
            if (body.Contains(wordsDictionary[Word.VTOUR_URL]))
                body = body.Replace(wordsDictionary[Word.VTOUR_URL], theOrder.Options == ViewOrder.ViewOrderOptions.FloorPlan ? 
                                                    "<none>" : theOrder.VTourUrl);
            if (body.Contains(wordsDictionary[Word.TAX]))
                body = body.Replace(wordsDictionary[Word.TAX], string.Format("{0:00.00}", tax));
            if (body.Contains(wordsDictionary[Word.PAID_GROSS]))
                body = body.Replace(wordsDictionary[Word.PAID_GROSS], string.Format("{0:00.00}", trInfo != null ? 
                                                    trInfo.GrossAmount : 0.00M));
            return body;
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            EmailSettings emailSettings = new EmailSettings(subject, body, 
                                                    wordsDictionary.Values.ToList(),
                                                    conditionsDictionary.Values.ToList());

            if (emailSettings.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                switch (theTemplate)
                {
                    case Template.OrderConfirmation:
                        Properties.Settings.Default.orderConfirmationTemplate = emailSettings.Body;
                        Properties.Settings.Default.defaultConfirmationSubject = emailSettings.Subject;
                        break;
                    case Template.ListingCopyingAsPrivate:
                        Properties.Settings.Default.listingCopyingAsPrivate = emailSettings.Body;
                        Properties.Settings.Default.defaultListingCopyingAsPrivateSubject = emailSettings.Subject;
                        break;
                    case Template.ListingPromotion:
                        Properties.Settings.Default.listingPromoTemplate = emailSettings.Body;
                        Properties.Settings.Default.defaultPromoSubject = emailSettings.Subject;
                        break;
                }
                Properties.Settings.Default.Save();

                loadFromTemplate(theTemplate);

                UpdateState();
            }
        }
    }
}
