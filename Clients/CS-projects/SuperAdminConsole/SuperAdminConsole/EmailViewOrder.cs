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
            string email = string.IsNullOrEmpty(theUser.PrimaryEmailAddress) ? theUser.NickName : theUser.PrimaryEmailAddress;
            if (template == Template.OrderConfirmation)
            {
                textBoxAddressTo.Text = email;
                body = Properties.Settings.Default.orderConfirmationTemplate;
                subject = "Interactive 3D Listing Order Confirmation";
                textBody.Focus();
            }
            else if (template == Template.ListingPromotion)
            {
                body = Properties.Settings.Default.listingPromoTemplate;
                subject = PreprocessTemplate(Properties.Settings.Default.defaultPromoSubject);
                textBoxAddressTo.Focus();
            }

            textBody.Text = PreprocessTemplate(body);

            theTemplate = template;
            textBoxAddressTo.ReadOnly = theUser.UserRole == User.Role.SellingAgent && !string.IsNullOrEmpty(email);
            UpdateState();
            textBoxSubject.Text = subject;
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

        private string PreprocessTemplate(string body)
        {
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

            if (body.Contains("{ADDRESS_TO}"))
                body = body.Replace("{ADDRESS_TO}", textBoxAddressTo.Text);
            if (body.Contains("{VIEWORDER_ADDRESS}"))
                body = body.Replace("{VIEWORDER_ADDRESS}", readableName);
            if (body.Contains("{VIEWORDER_URL}"))
                body = body.Replace("{VIEWORDER_URL}", theOrder.ViewOrderURL);
            if (body.Contains("{PRICE_NO_TAX}"))
                body = body.Replace("{PRICE_NO_TAX}", string.Format("{0:00.00}", Properties.Settings.Default.PriceOf3DListing));
            if (body.Contains("{DAYS_VALID}"))
                body = body.Replace("{DAYS_VALID}", ((theOrder.ExpiresOn - DateTime.Now).Days + 1).ToString());
            if (body.Contains("{VALID_UNTIL_TIME_DAY}"))
                body = body.Replace("{VALID_UNTIL_TIME_DAY}", theOrder.ExpiresOn.ToShortTimeString() + 
                                                       " of " + theOrder.ExpiresOn.ToLongDateString());
            if (body.Contains("{TODAYS_DATE}"))
                body = body.Replace("{TODAYS_DATE}", DateTime.Now.ToShortDateString());
            if (body.Contains("{MLS_NUMBER}"))
                body = body.Replace("{MLS_NUMBER}", mlsId);
            if (body.Contains("{SIGNATURE}"))
                body = body.Replace("{SIGNATURE}", signature);
            if (body.Contains("{PRODUCT_NAME}"))
                body = body.Replace("{PRODUCT_NAME}", product);
            if (body.Contains("{PAYMENT_REF}"))
                body = body.Replace("{PAYMENT_REF}", paymentRefId);
            if (body.Contains("{VTOUR_URL}"))
                body = body.Replace("{VTOUR_URL}", theOrder.Options == ViewOrder.ViewOrderOptions.FloorPlan ? 
                                                    "<none>" : theOrder.VTourUrl);
            if (body.Contains("{TAX}"))
                body = body.Replace("{TAX}", string.Format("{0:00.00}", tax));
            if (body.Contains("{PAID_GROSS}"))
                body = body.Replace("{PAID_GROSS}", string.Format("{0:00.00}", trInfo != null ? 
                                                    trInfo.GrossAmount : 0.00M));
            return body;
        }
    }
}
