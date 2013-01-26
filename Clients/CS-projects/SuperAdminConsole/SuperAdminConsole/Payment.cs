using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
//using System.Web;

namespace SuperAdminConsole
{
    public partial class Payment : Form
    {
        public string PaymentRefId { private set; get; }
        bool paymentSucceed = false;

        public decimal TotalPayed { private set; get; }
        public Payment(decimal priceWithTax)
        {
            InitializeComponent();

            decimal roundedGrossTotal = decimal.Round(priceWithTax, 2);
            comboBoxCurrency.SelectedIndex = 0;
            comboBoxCountry.SelectedIndex = 0;
            textBoxAmount.Text = roundedGrossTotal.ToString();
            radioButtonVISA.Checked = true;
            //textBoxPoNum.Text = "LSTNG-" + (DateTime.Now.Ticks / 10000).ToString();

#region AUTO TextBoxes
            const string DEFAULT_CARDNUMBER = @"4512709886670826";
            const string DEFAULT_CVV2 = @"321";
            const string DEFAULT_FIRSTNAME = @"John";
            const string DEFAULT_LASTNAME = @"Smith";
            const string DEFAULT_FULLNAME = @"John Smith";
            const string DEFAULT_STREET1 = @"120 Main St., Apt. 1210";
            const string DEFAULT_POSTAL = @"M6B1B1";

            textBoxCardNo.Text = DEFAULT_CARDNUMBER;
            textBoxCvv2.Text = DEFAULT_CVV2;
            textBoxFirstName.Text = DEFAULT_FIRSTNAME;
            textBoxLastName.Text = DEFAULT_LASTNAME;
            textBoxName.Text = DEFAULT_FULLNAME;
            textBoxStreet.Text = DEFAULT_STREET1;
            textBoxPostal.Text = DEFAULT_POSTAL;

            textBoxCardNo.GotFocus += (sender, e) =>
            {
                if (textBoxCardNo.Text.Equals(DEFAULT_CARDNUMBER))
                    textBoxCardNo.Text = string.Empty;
            };
            textBoxCardNo.LostFocus += (sender, e) =>
            {
                if (textBoxCardNo.Text.Trim().Length == 0)
                    textBoxCardNo.Text = DEFAULT_CARDNUMBER;
            };

            textBoxCvv2.GotFocus += (sender, e) =>
            {
                if (textBoxCvv2.Text.Equals(DEFAULT_CVV2))
                    textBoxCvv2.Text = string.Empty;
            };
            textBoxCvv2.LostFocus += (sender, e) =>
            {
                if (textBoxCvv2.Text.Trim().Length == 0)
                    textBoxCvv2.Text = DEFAULT_CVV2;
            };

            textBoxFirstName.GotFocus += (sender, e) =>
            {
                if (textBoxFirstName.Text.Equals(DEFAULT_FIRSTNAME))
                    textBoxFirstName.Text = string.Empty;
            };
            textBoxFirstName.LostFocus += (sender, e) =>
            {
                if (textBoxFirstName.Text.Trim().Length == 0)
                    textBoxFirstName.Text = DEFAULT_FIRSTNAME;
            };

            textBoxLastName.GotFocus += (sender, e) =>
            {
                if (textBoxLastName.Text.Equals(DEFAULT_LASTNAME))
                    textBoxLastName.Text = string.Empty;
            };
            textBoxLastName.LostFocus += (sender, e) =>
            {
                if (textBoxLastName.Text.Trim().Length == 0)
                    textBoxLastName.Text = DEFAULT_LASTNAME;
            };

            textBoxName.GotFocus += (sender, e) =>
            {
                if (textBoxName.Text.Equals(DEFAULT_FULLNAME))
                {
                    textBoxName.Text = textBoxFirstName.Text + " " + textBoxLastName.Text;
                }
            };
            textBoxName.LostFocus += (sender, e) =>
            {
                if (textBoxName.Text.Trim().Length == 0)
                    textBoxName.Text = DEFAULT_FULLNAME;
            };

            textBoxStreet.GotFocus += (sender, e) =>
            {
                if (textBoxStreet.Text.Equals(DEFAULT_STREET1))
                    textBoxStreet.Text = string.Empty;
            };
            textBoxStreet.LostFocus += (sender, e) =>
            {
                if (textBoxStreet.Text.Trim().Length == 0)
                    textBoxStreet.Text = DEFAULT_STREET1;
            };

            textBoxPostal.GotFocus += (sender, e) =>
            {
                if (textBoxPostal.Text.Equals(DEFAULT_POSTAL))
                    textBoxPostal.Text = string.Empty;
            };
            textBoxPostal.LostFocus += (sender, e) =>
            {
                if (textBoxPostal.Text.Trim().Length == 0)
                    textBoxPostal.Text = DEFAULT_POSTAL;
            };
#endregion // AUTO TextBoxes

            UpdateState();
        }

        private void UpdateState()
        {
            textBoxAmount.ReadOnly = paymentSucceed;
            buttonPayNow.Enabled = !paymentSucceed;
            comboBoxCurrency.Enabled = !paymentSucceed;
            radioButtonVISA.Enabled = !paymentSucceed;
            radioButtonMasterCard.Enabled = !paymentSucceed;
            groupBoxCardInfo.Enabled = !paymentSucceed;
            groupBoxBillingAddress.Enabled = !paymentSucceed;
        }

        private void radioButtonVISA_CheckedChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void radioButtonMasterCard_CheckedChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void buttonPayNow_Click(object sender, EventArgs e)
        {
            buttonPayNow.Enabled = false;
            // Create request object
            DoDirectPaymentRequestType request = new DoDirectPaymentRequestType();
            DoDirectPaymentRequestDetailsType requestDetails = new DoDirectPaymentRequestDetailsType();
            request.DoDirectPaymentRequestDetails = requestDetails;

            requestDetails.PaymentAction = PaymentActionCodeType.SALE;

            // Populate card requestDetails
            CreditCardDetailsType creditCard = new CreditCardDetailsType();
            requestDetails.CreditCard = creditCard;
            PayerInfoType payer = new PayerInfoType();
            PersonNameType name = new PersonNameType();
            name.FirstName = textBoxName.Text;
            name.LastName = textBoxLastName.Text;
            payer.PayerName = name;
            creditCard.CardOwner = payer;

            creditCard.CreditCardNumber = textBoxCardNo.Text;
            if (radioButtonVISA.Checked)
                creditCard.CreditCardType = CreditCardTypeType.VISA;
            else if (radioButtonMasterCard.Checked)
                creditCard.CreditCardType = CreditCardTypeType.MASTERCARD;

            creditCard.CVV2 = textBoxCvv2.Text;
            creditCard.ExpMonth = (int)numericUpDownMM.Value;
            creditCard.ExpYear = (int)numericUpDownYY.Value;

            requestDetails.PaymentDetails = new PaymentDetailsType();
            AddressType billingAddr = new AddressType();
            if (textBoxName.Text != string.Empty && textBoxStreet2.Text != string.Empty
                && textBoxStreet.Text != string.Empty && comboBoxCountry.SelectedText != string.Empty)
            {
                billingAddr.Name = textBoxName.Text;
                billingAddr.Street1 = textBoxStreet.Text;
                billingAddr.Street2 = textBoxStreet2.Text;
                billingAddr.CityName = textBoxCity.Text;
                billingAddr.StateOrProvince = textBoxProvince.Text;
                billingAddr.Country = (CountryCodeType)Enum.Parse(typeof(CountryCodeType), comboBoxCountry.SelectedText);
                billingAddr.PostalCode = textBoxPostal.Text;

                //Fix for release
                billingAddr.Phone = string.Empty;

                payer.Address = billingAddr;
            }

            // Populate payment requestDetails
            CurrencyCodeType currency = (CurrencyCodeType)
                Enum.Parse(typeof(CurrencyCodeType), comboBoxCurrency.SelectedItem as string);
            BasicAmountType paymentAmount = new BasicAmountType(currency, textBoxAmount.Text);
            requestDetails.PaymentDetails.OrderTotal = paymentAmount;


            // Invoke the API
            DoDirectPaymentReq wrapper = new DoDirectPaymentReq();
            wrapper.DoDirectPaymentRequest = request;
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService();
            DoDirectPaymentResponseType response = service.DoDirectPayment(wrapper);

            // Check for API return status
            //setKeyResponseObjects(service, response);

            /**************************************************************/
            if (response.Ack.Equals(AckCodeType.FAILURE) ||
                (response.Errors != null && response.Errors.Count > 0))
            {
                string Error = string.Empty;
                foreach (ErrorType error in response.Errors)
                {
                    string parameters = string.Empty;
                    foreach (ErrorParameterType parameter in error.ErrorParameters)
                    {
                        parameters += parameter.Value + " ";
                    }
                    Error += string.Format("Error Code: {0}{1}\n" +
                                           "{2} ({3})\n" +
                                           "----------------------------------\n",
                                           error.ErrorCode,
                                           error.ErrorParameters.Count == 0 ? "" : "; Error Parameters: " + parameters,
                                           error.ShortMessage,
                                           error.LongMessage);
                }

                MessageBox.Show("Transaction FAILURE:\n" + Error,
                                "Payment incomplete",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                //this.DialogResult = System.Windows.Forms.DialogResult.Abort;
                TotalPayed = 0.0M;
                PaymentRefId = null;
            }
            else
            {
                paymentSucceed = true;
                textBoxPoNum.Text = response.TransactionID;
                UpdateState();
                string paymentStatus = response.PaymentStatus.ToString();
                string msg = string.Format("Payment Completed!\n\n" +
                                           "    Transaction Id: {0}\n" +
                                           "    Correlation Id: {1}{2}",
                                           response.TransactionID,
                                           response.CorrelationID,
                                           paymentStatus == "" ? "" : "\n\n    Payment status: " + paymentStatus);
                MessageBox.Show(msg, "Payment Completed",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                PaymentRefId = response.TransactionID;
                TotalPayed = decimal.Parse(textBoxAmount.Text);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            UpdateState();
        }

        //private void setKeyResponseObjects(PayPalAPIInterfaceServiceService service,
        //    DoDirectPaymentResponseType response)
        //{
        //    HttpContext CurrContext = HttpContext.Current;
        //    CurrContext.Items.Add("Response_apiName", "DoDirectPayment");
        //    CurrContext.Items.Add("Response_redirectURL", null);
        //    CurrContext.Items.Add("Response_requestPayload", service.getLastRequest());
        //    CurrContext.Items.Add("Response_responsePayload", service.getLastResponse());

        //    Dictionary<string, string> responseParams = new Dictionary<string, string>();
        //    responseParams.Add("Correlation Id", response.CorrelationID);
        //    responseParams.Add("API Result", response.Ack.ToString());

        //    if (response.Ack.Equals(AckCodeType.FAILURE) ||
        //        (response.Errors != null && response.Errors.Count > 0))
        //    {
        //        CurrContext.Items.Add("Response_error", response.Errors);
        //    }
        //    else
        //    {
        //        CurrContext.Items.Add("Response_error", null);
        //        responseParams.Add("Transaction Id", response.TransactionID);
        //        responseParams.Add("Payment status", response.PaymentStatus.ToString());
        //        if (response.PendingReason != null)
        //        {
        //            responseParams.Add("Pending reason", response.PendingReason.ToString());
        //        }
        //    }
        //    CurrContext.Items.Add("Response_keyResponseObject", responseParams);
        //    // Server.Transfer("../APIResponse.aspx");

        //}
    }

}
