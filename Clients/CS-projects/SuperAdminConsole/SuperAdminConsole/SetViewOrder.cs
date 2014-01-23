using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using CoreClasses;
using Vre.Server.BusinessLogic;

namespace SuperAdminConsole
{
    public partial class SetViewOrder : Form
    {
        public enum ChangeReason
        {
            Creation,
            Transfer,
            Update
        }

        private ListViewColumnSorter lvwColumnSorter;

        const string EMPTY = "";
        const string WEB_PREFIX = @"http://";
        bool viewOrderUrlGenerated = false;
        bool emailSent = false;
        bool paymentSkip = false;
        public string paymentRefId { private set; get; }

        string initialVTourURL;
        string initialMoreInfoUrl;
        string initialNote;
        string initialMLS;

        public string ViewOrderPrice
        {
            get { return string.Format("${0:0.00}", numericUpDownPrice.Value); }
        }

        public string ListingPrice
        {
            get { return string.Format("${0:0.00}", richTextBoxListingPrice.Text); }
        }

        public string Tax
        {
            get { return string.Format("${0:0.00}", numericUpDownPrice.Value * numericUpDownTax.Value / 100M); }
        }

        public string ViewOrderPriceWithTax
        {
            get
            {
                return string.Format("${0:0.00}", TotalToBill);
            }
        }

        public bool ListingForSale
        {
            get { return radioButtonListingTypeSale.Checked;  }
        }

        public decimal TotalToBill
        {
            get
            {
                decimal price = numericUpDownPrice.Value;
                return price + price * numericUpDownTax.Value / 100M;
            }
        }

        public string MlsNum
        {
            get { return textBoxMLS.Text; }
        }

        public User theUser { private set; get; }

        ClientData theOrder = null;
        ChangeReason theReason;
        Developer m_developer = null;

        public SetViewOrder(User user, ClientData order, ChangeReason reason, Developer developer)
        {
            InitializeComponent();

            theReason = reason;
            m_developer = developer;

            cleanUp();

            textVTourURL.Text = EMPTY;
            textVTourURL.GotFocus += (sender, e) =>
            {
                if (textVTourURL.Text.Equals(EMPTY))
                {
                    textVTourURL.Text = WEB_PREFIX;
                    textVTourURL.SelectionStart = WEB_PREFIX.Length;
                }
            };
            textVTourURL.LostFocus += (sender, e) =>
            {
                if (textVTourURL.Text.Trim().Length == 0 || textVTourURL.Text == WEB_PREFIX)
                    textVTourURL.Text = EMPTY;
            };

            textMoreInfoUrl.Text = EMPTY;
            textMoreInfoUrl.GotFocus += (sender, e) =>
            {
                if (textMoreInfoUrl.Text.Equals(EMPTY))
                {
                    textMoreInfoUrl.Text = WEB_PREFIX;
                    textMoreInfoUrl.SelectionStart = WEB_PREFIX.Length;
                }
            };
            textMoreInfoUrl.LostFocus += (sender, e) =>
            {
                if (textMoreInfoUrl.Text.Trim().Length == 0 || textMoreInfoUrl.Text == WEB_PREFIX)
                    textMoreInfoUrl.Text = EMPTY;
            };


            theOrder = order;
            if (theReason != ChangeReason.Creation)
            {
                tabControlSteps.SelectTab("tabPageViewOrderOptions");
                addressVerified = true;
                ViewOrder.ViewOrderOptions options = theOrder.GetProperty<ViewOrder.ViewOrderOptions>("options", ViewOrder.ViewOrderOptions.FloorPlan);
                if (options != ViewOrder.ViewOrderOptions.FloorPlan)
                    textVTourURL.Text = theOrder["vTourUrl"] as string;

                PostalAddress = theOrder.GetProperty("label", string.Empty);
                textBoxMLS.Text = theOrder.GetProperty("mlsId", string.Empty);
                textMoreInfoUrl.Text = theOrder.GetProperty("infoUrl", string.Empty);
                textBoxNote.Text = theOrder.GetProperty("note", string.Empty);
            }

            paymentRefId = string.Empty;
            decimal price = 0M;

            switch (theReason)
            {
                case ChangeReason.Creation:
                    price = Properties.Settings.Default.PriceOf3DListing;
                    numericUpDownPrice.Maximum = decimal.Round(price * 2.0M);
                    this.Text = "Creating New viewOrder";
                    break;
                case ChangeReason.Transfer:
                    price = Properties.Settings.Default.PriceOf3DListing;
                    this.Text = "Transferring viewOrder to Another Account";
                    break;
                case ChangeReason.Update:
                    price = Properties.Settings.Default.ChangeViewOrderOptionsFee;
                    numericUpDownPrice.Maximum = decimal.Round(price * 4.0M);
                    this.Text = "Updating viewOrder Options";
                    break;
            }

            if (user.UserRole == User.Role.SellingAgent)
            {
                numericUpDownTax.Value = Properties.Settings.Default.SalesTaxValuePercent;
                numericUpDownPrice.Value = price;
                numericUpDownDaysValid.Value = Properties.Settings.Default.defaultDaysValidPermanent;
            }

            if (user.UserRole == User.Role.SuperAdmin ||
                user.UserRole == User.Role.DeveloperAdmin)
            {
                paymentSkip = true;
                numericUpDownDaysValid.Value = Properties.Settings.Default.defaultDaysValidTemp;
                numericUpDownPrice.Value = 0.00M;
                labelPrice.Visible = false;
                numericUpDownPrice.Visible = false;
            }

            theUser = user;
            comboBoxCountry.SelectedIndex = 0;

            initialVTourURL = textVTourURL.Text;
            initialMoreInfoUrl = textMoreInfoUrl.Text;
            initialMLS = textBoxMLS.Text;
            initialNote = textBoxNote.Text;

            lvwColumnSorter = new ListViewColumnSorter();
            listViewAddresses.ListViewItemSorter = lvwColumnSorter;
            textBoxMLS.ReadOnly = theReason != ChangeReason.Creation;
            //textMoreInfoUrl.ReadOnly = theReason != ChangeReason.Creation;
            //textBoxNote.Visible = theReason == ChangeReason.Creation;
            label11.Visible = theReason == ChangeReason.Creation;

            UpdateState();
        }

        private bool haveOptionsChanged()
        {
            return initialVTourURL != textVTourURL.Text ||
                   initialMoreInfoUrl != textMoreInfoUrl.Text ||
                   initialMLS != textBoxMLS.Text ||
                   initialNote != textBoxNote.Text;
        }

        private string generateTitle()
        {
            string tittle = string.Empty;
            if (radioButtonPrivateListing.Checked)
                tittle = "Ordered: Private Listing";
            else if (radioButtonSharedListing.Checked)
                tittle = "Ordered: Public Listing";
            else if (radioButton3DLayout.Checked)
                tittle = "Ordered: 3D Layout";

            switch (tabControlSteps.SelectedTab.Text)
            {
                case "Product":
                case "Address":
                    return tittle;
                case "Options":
                case "Generate":
                    if (PostalAddress != "") 
                        tittle += " \'" + PostalAddress + "\"";

                    break;
            }

            return tittle;
        }

        private void UpdateState()
        {
            Text = generateTitle();
            switch (tabControlSteps.SelectedTab.Text)
            {
                case "Product":
                    buttonPrev.Visible = false;
                    buttonNext.Enabled = true;
                    buttonFinish.Enabled = false;
                    buttonOneMore.Visible = false;
                    buttonCancel.Visible = true;
                    break;
                case "Address":
                    buttonPrev.Visible = true;
                    buttonPrev.Enabled = true;
                    buttonNext.Enabled = addressVerified;
                    buttonCheckAddress.Enabled = !addressVerified;
                    buttonFinish.Enabled = false;
                    buttonOneMore.Visible = false;
                    buttonCancel.Visible = true;
                    break;
                case "Options":
                    groupBoxListingOptions.Visible = radioButtonPrivateListing.Checked || 
                                                     radioButtonSharedListing.Checked;
                    labelValidFor.Visible = theReason != ChangeReason.Update;
                    numericUpDownDaysValid.Visible = theReason != ChangeReason.Update;
                    labelDays.Visible = theReason != ChangeReason.Update;

                    buttonPrev.Enabled = paymentRefId == string.Empty;
                    buttonPrev.Visible = paymentRefId == string.Empty && theReason == ChangeReason.Creation;
                    buttonNext.Enabled = paymentRefId != string.Empty || paymentSkip;
                    buttonNext.Visible = theReason == ChangeReason.Creation;
                    buttonFinish.Visible = true;

                    if (theReason == ChangeReason.Update)
                    {
                        buttonPayment.Enabled = haveOptionsChanged() && 
                                                numericUpDownPrice.Value > 0M &&
                                                paymentRefId == string.Empty;

                        buttonFinish.Enabled =  haveOptionsChanged() && 
                                                  (paymentRefId != string.Empty ||
                                                   numericUpDownPrice.Value == 0M) ||
                                                initialMLS != textBoxMLS.Text ||
                                                initialMoreInfoUrl != textMoreInfoUrl.Text ||
                                                initialNote != textBoxNote.Text;
                    }
                    else
                    {
                        buttonPayment.Enabled = !paymentSkip &&
                                                numericUpDownPrice.Value > 0M &&
                                                 paymentRefId == string.Empty;
                        buttonFinish.Enabled = theReason == ChangeReason.Transfer;
                    }

                    groupBoxListingOptions.Enabled = paymentRefId == string.Empty;

                    numericUpDownTax.Visible = numericUpDownPrice.Value != 0.00M;
                    labelTax.Visible = numericUpDownPrice.Value != 0.00M;
                    labelPercent.Visible = numericUpDownPrice.Value != 0.00M;
                    textBoxTotal.Visible = numericUpDownPrice.Value != 0.00M;

                    numericUpDownPrice.Enabled = true;

                    textBoxTotal.Text = ViewOrderPriceWithTax;
                    buttonCancel.Visible = paymentRefId == string.Empty;
                    buttonOneMore.Visible = false;
                    break;
                case "Generate":
                    buttonCancel.Visible = paymentSkip && !viewOrderUrlGenerated;
                    buttonPrev.Visible = paymentSkip && !viewOrderUrlGenerated;
                    buttonSendEmailk.Enabled = viewOrderUrlGenerated && !emailSent;
                    buttonSendEmailk.Visible = viewOrderUrlGenerated;
                    buttonFinish.Enabled = (theUser.UserRole == User.Role.SuperAdmin ||
                                            theUser.UserRole == User.Role.DeveloperAdmin) &&
                                                viewOrderUrlGenerated;
                    buttonGenerateURL.Enabled = !viewOrderUrlGenerated;
                    buttonNext.Visible = false;
                    buttonOneMore.Visible = theReason == ChangeReason.Creation &&
                                            viewOrderUrlGenerated;
                    break;
            }
        }

#region CheckPostalAddres
        string savedAddressString = string.Empty;
        string AddressStringFromUI
        {
            get
            {
                string retStr = textStreet.Text +
                                textBuildingNo.Text +
                                textUnitNo.Text +
                                textCity.Text +
                                textProvince.Text +
                                textPostalCode.Text;
                return retStr.ToLower();
            }
        }

        bool addressVerified
        {
            get
            {
                return savedAddressString != string.Empty &&
                       savedAddressString == AddressStringFromUI;
            }
            set
            {
                if (value == false)
                    savedAddressString = string.Empty;
                else
                    savedAddressString = AddressStringFromUI;
            }
        }

        private void buttonCheckAddress_Click(object sender, EventArgs e)
        {
            // https://vrt.3dcondox.com/program?
            //                                      q=check&entity=address&
            //                                      ad_stn=QUEENS&
            //                                      ad_bn=650&
            //                                      ad_ibn=0508&
            //                                      ad_co=CANADA&
            //                                      sid=<SID>
            //{
            //    "result":true,
            //    "propertyType":suite,
            //    "propertyId":12345678,
            //    "normalizedAddress":
            //    {
            //        "ad_ibn":"0508",
            //        "ad_ibt":"apt",
            //        "ad_bn":"650",
            //        "ad_std":"w",
            //        "ad_stt":"other",
            //        "ad_stn":"Queens Quay",
            //        "ad_co":"CANADA",
            //        "ad_po":"M5V3N2",
            //        "ad_stpr":"ON",
            //        "ad_mu":"TORONTO"
            //    },
            //    "readableAddress":"0508, 650 Queens Quay W, Toronto, ON, M5V3N2, Canada"
            //}
            
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                string parameters = string.Format("q=check&entity=address&"+
                                                  "ad_stn={0}&" +
                                                  "ad_bn={1}&" +
                                                  "ad_ibn={2}&" +
                                                  "ad_mu={3}&" +
                                                  "ad_co={4}",
                                                  textStreet.Text,
                                                  textBuildingNo.Text,
                                                  textUnitNo.Text,
                                                  textCity.Text,
                                                  comboBoxCountry.SelectedItem);
                ServerResponse response = ServerProxy.MakeGenericRequest(ServerProxy.RequestType.Get,
                                                                  "program",
                                                                  parameters,
                                                                  null);

                if (HttpStatusCode.OK == response.ResponseCode)
                {
                    bool result = response.Data.GetProperty("result", false);
                    if (result)
                    {
                        PostalAddress = response.Data.GetProperty("readableAddress", "");
                        PropertyType = response.Data.GetProperty("propertyType", "");
                        PropertyID = response.Data.GetProperty("propertyId", 0);
                        ClientData picd = response.Data.GetNextLevelDataItem("normalizedAddress");
                        textStreet.Text = picd.GetProperty("ad_stn", "");
                        textBuildingNo.Text = picd.GetProperty("ad_bn", "");
                        textUnitNo.Text = picd.GetProperty("ad_ibn", "");
                        textCity.Text = picd.GetProperty("ad_mu", "");
                        textProvince.Text = picd.GetProperty("ad_stpr", "");
                        textPostalCode.Text = picd.GetProperty("ad_po", "");

                        addressVerified = true;
                    }
                }
                Cursor.Current = Cursors.Default;

                if (!addressVerified)
                    MessageBox.Show("This address is not available", "Address Verification", 
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception exception)
            {
                Cursor.Current = Cursors.Default;
                //Any exception will returns false.
                string error = string.Format("Address Verification failed: \n\'{0}\'",
                    exception.Message);
                MessageBox.Show(error, "Address Verification", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            UpdateState();
        }

#endregion // CheckPostalAddres

        public string PostalAddress { private set; get; }
        public string PropertyType { private set; get; }
        public int PropertyID { private set; get; }
        public int BuildingID { private set; get; }
        public decimal DaysValid { get { return numericUpDownDaysValid.Value; } }


        private void buttonGenerateURL_Click(object sender, EventArgs e)
        {
            if (!viewOrderUrlGenerated)
            {
                // https://vrt.3dcondox.com/program?
                //                                      q=register&
                //                                      entity=viewOrder&
                //                                      ownerId=<user ID>&
                //                                      pr=<payment reference #>&
                //                                      daysValid=<number of days>&
                //                                      options={fp|evt|3dt}&
                //                                      product={prl|pul|b3dl&
                //                                      [evt_url=<External Virtual Tour URL>]&
                //                                      mls_id=<MLS#>?
                //                                      propertyType=suite&
                //                                      propertyId=<suite ID>&
                //                                      sid=<SID>
                string product = string.Empty;

                string parameters;
                if (radioButton3DLayout.Checked)
                {
                    string noteParam = string.IsNullOrEmpty(textBoxNote.Text) ? "" :
                                "note=\"" + HttpUtility.UrlEncodeUnicode(textBoxNote.Text) + "\"&";
                    string infoUrlParam = string.IsNullOrEmpty(textMoreInfoUrl.Text) ? "" :
                                "mls_url=\"" + HttpUtility.UrlEncodeUnicode(textMoreInfoUrl.Text) + "\"&";
                    parameters = string.Format("q=register&entity=viewOrder&product=b3dl&" +
                                               "ownerId={0}&" +
                                               "{1}" +
                                               "{2}" +
                                               "daysValid={3}&" +
                                               "propertyType={4}&" +
                                               "propertyId={5}",
                                               theUser.AutoID,
                                               noteParam,
                                               infoUrlParam,
                                               DaysValid,
                                               PropertyType,
                                               PropertyID);
                }
                else
                {
                    if (radioButtonPrivateListing.Checked)
                        product = "prl";
                    else if (radioButtonSharedListing.Checked)
                        product = "pul";
                    parameters = string.Format("q=register&" +
                                               "entity=viewOrder&" +
                                               "ownerId={0}&" +
                                               "{1}" +
                                               "daysValid={2}&" +
                                               "product={9}&" +
                                               "options={3}&" +
                                               "mls_id={6}&" +
                                               "mls_url={7}&" +
                                               "note=\"{8}\"&" +
                                               "propertyType={4}&" +
                                               "propertyId={5}",
                                               theUser.AutoID + (theUser.UserRole != User.Role.SuperAdmin ?
                                                                        "&ed=" + theUser.EstateDeveloperID : 
                                                                        string.Empty),
                                               numericUpDownPrice.Value == 0 ? "" : "pr=" + paymentRefId + "&",
                                               DaysValid,
                                               textVTourURL.Text == string.Empty ? "fp" :
                                                    "evt&evt_url=" + HttpUtility.UrlEncodeUnicode(textVTourURL.Text),
                                               PropertyType,
                                               PropertyID,
                                               textBoxMLS.Text,
                                               string.IsNullOrEmpty(textMoreInfoUrl.Text) ? "" : 
                                                    HttpUtility.UrlEncodeUnicode(textMoreInfoUrl.Text),
                                               HttpUtility.UrlEncodeUnicode(textBoxNote.Text),
                                               product);
                }
                ServerResponse resp = ServerProxy.MakeGenericRequest(ServerProxy.RequestType.Get,
                                                                  "program",
                                                                  parameters, null);
                if (HttpStatusCode.OK == resp.ResponseCode)
                {
                    string viewOrderID = (resp.Data["viewOrder-id"] as string).Replace("-", string.Empty);
                    string viewOrderUrl = resp.Data["viewOrder-url"] as string;
                    // Response: JSON
                    //  {
                    //      ”viewOrder-url”:<URL of direct link to created viewOrder>,
                    //      ”viewOrder-id":<string ID>
                    //      ”ref”:<ref number>
                    //  }

                    if (HttpStatusCode.OK == resp.ResponseCode)
                    {
                        resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                                            "suite/" + PropertyID,
                                                                            "",
                                                                            null);
                        if (HttpStatusCode.OK == resp.ResponseCode)
                        {
                            ClientData suiteData = resp.Data;
                            string price = richTextBoxListingPrice.Text.Replace("$", "");
                            price = price.Replace(",", "");
                            decimal cp = 0.0m;
                            if (!string.IsNullOrEmpty(price))
                                try { cp = Decimal.Parse(price); } catch (Exception) {}

			                if (cp >= 0.0m)
                            {
                                Money? currentPrice = new Money(cp);
                                suiteData["currentPrice"] = currentPrice.Value.ToString("F");
                                suiteData["currentPriceCurrency"] = currentPrice.Value.Currency.Iso3LetterCode;
                            }
                            suiteData["status"] = radioButtonListingTypeSale.Checked ? 
                                ClientData.ConvertProperty<Vre.Server.BusinessLogic.Suite.SalesStatus>(Vre.Server.BusinessLogic.Suite.SalesStatus.ResaleAvailable) :
                                ClientData.ConvertProperty<Vre.Server.BusinessLogic.Suite.SalesStatus>(Vre.Server.BusinessLogic.Suite.SalesStatus.AvailableRent);

                            resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                                                "suite/" + PropertyID,
                                                                                "",
                                                                                suiteData);
                            if (HttpStatusCode.OK == resp.ResponseCode)
                            {
                                viewOrderUrlGenerated = true;
                                textBoxViewOrderURL.Text = viewOrderUrl;
                                ViewOrderID = viewOrderID;
                                ViewOrderURL = textBoxViewOrderURL.Text;
                                UpdateState();
                                MessageBox.Show("The ViewOrder has been generated successfully!\n" +
                                                "Please send the link to the customer",
                                                "ViewOrder is Ready!",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                            }
                            else
                            {
                                ServerResponse undo = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Delete,
                                                                                    "viewOrder/" + viewOrderID,
                                                                                    "userId=" + this.theUser.AutoID +
                                                                                    "&ed=" +
                                                                                    this.m_developer.Name, null);
                            }
                        }
                    }
                }

                if (!viewOrderUrlGenerated)
                {
                    MessageBox.Show("Cannot Create viewOrder (ResponseCode=" + resp.ResponseCode + ")");
                    DialogResult = System.Windows.Forms.DialogResult.Abort;
                    Close();
                    return;
                }

                Clipboard.SetText(ViewOrderURL);

                string the300xgars = Properties.Settings.Default.the300charsTemplate;
                textBox300Chars.Text = PreprocessTemplate(the300xgars);
            }
            UpdateState();
        }

        private string PreprocessTemplate(string body)
        {
            if (body.Contains("{VIEWORDER_ADDRESS}"))
                body = body.Replace("{VIEWORDER_ADDRESS}", this.PostalAddress);
            if (body.Contains("{VIEWORDER_URL}"))
                body = body.Replace("{VIEWORDER_URL}", ViewOrderURL);
            if (body.Contains("{DAYS_VALID}"))
                body = body.Replace("{DAYS_VALID}", DaysValid.ToString());
            if (body.Contains("{MLS_NUMBER}"))
                body = body.Replace("{MLS_NUMBER}", MlsNum);
            if (body.Contains("{PRICE_NO_TAX}"))
                body = body.Replace("{PRICE_NO_TAX}", Properties.Settings.Default.PriceOf3DListing.ToString());
            return body;
        }

        public string ViewOrderID { private set; get; }
        public string ViewOrderURL { private set; get; }

        private void radioButtonVTourURL_CheckedChanged(object sender, EventArgs e)
        {
            textVTourURL.Focus();
            UpdateState();
        }

        private void numericUpDownPrice_ValueChanged(object sender, EventArgs e)
        {
            paymentSkip = numericUpDownPrice.Value == 0;
            UpdateState();
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            tabControlSteps.SelectedIndex = tabControlSteps.SelectedIndex - 1;
            UpdateState();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            tabControlSteps.SelectedIndex = tabControlSteps.SelectedIndex + 1;
            UpdateState();
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            if (theReason == ChangeReason.Transfer)
            {
                DateTime expiresOn = DateTime.Now.AddDays((double)numericUpDownDaysValid.Value + 1);
                expiresOn = System.TimeZone.CurrentTimeZone.ToUniversalTime(expiresOn.Date);
                theOrder["expiresOn"] = expiresOn;
                theOrder["ownerId"] = theUser.AutoID;
                theOrder["note"] = textBoxNote.Text;
            }

            if (theReason != ChangeReason.Creation)
            {
                ViewOrder.ViewOrderOptions options = 
                    textVTourURL.Text == string.Empty ? ViewOrder.ViewOrderOptions.FloorPlan :
                                                        ViewOrder.ViewOrderOptions.ExternalTour;
                //bool changed = false;
                //theOrder.UpdateProperty("options", options, ref changed);
                theOrder["options"] = (int)options;
                theOrder["vTourUrl"] = textVTourURL.Text;
                theOrder["mlsId"] = textBoxMLS.Text;
                theOrder["infoUrl"] = textMoreInfoUrl.Text;
                theOrder["note"] = textBoxNote.Text;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void buttonSendEmailk_Click(object sender, EventArgs e)
        {
            EmailViewOrder.Template templ = EmailViewOrder.Template.OrderConfirmation;
            if (theReason == ChangeReason.Creation && 
                    (theUser.UserRole == User.Role.SuperAdmin ||
                     theUser.UserRole == User.Role.DeveloperAdmin))
                templ = EmailViewOrder.Template.ListingPromotion;

            EmailViewOrder email = EmailViewOrder.Create(ViewOrderID,
                                                         paymentRefId,
                                                         templ);
            if (email == null)
                return;

            if (email.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                emailSent = true;
                UpdateState();
                MessageBox.Show("The viewOrder has been created successfully!", "ViewOrder Created",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }

        }

        private void textPostalCode_TextChanged(object sender, EventArgs e)
        {
            addressVerified = false;
            UpdateState();
        }

        private void buttonPayment_Click(object sender, EventArgs e)
        {
            Payment payment = new Payment(TotalToBill);
            if (payment.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                paymentRefId = payment.PaymentRefId;
            }
            UpdateState();
        }

        private void initListViewAddresses(string scopeType)
        {
            string scopeParam = string.Format("scopeType={0}&ad_mu={1}&ed={2}",
                                              scopeType,
                                              comboBoxCity.SelectedItem,
                                              m_developer.Name);
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                                "building",
                                                                scopeParam, null);

            if (HttpStatusCode.OK != resp.ResponseCode)
            {
                MessageBox.Show("Failed to extract a list of all buildings");
                UpdateState();
                return;
            }

            buildingsOfTheSelectedCity = resp.Data.GetNextLevelDataArray("buildings");
            filterAddresses();
        }

        ClientData[] buildingsOfTheSelectedCity = null;
        private void filterAddresses()
        {
            listViewAddresses.Items.Clear();
            //int count = 1;
            foreach (ClientData building in buildingsOfTheSelectedCity)
            {
                string filter = textBoxStreetName.Text.ToLower();
                string buildingName = building.GetProperty("name", string.Empty);
                string addressLine1 = building.GetProperty("addressLine1", string.Empty);
                string postal = building.GetProperty("postalCode", string.Empty);
                if (!buildingName.ToLower().Contains(filter) &&
                    !addressLine1.ToLower().Contains(filter) &&
                    !postal.ToLower().Contains(filter)) 
                    continue;

                //    id = 368
                //    version = ClientData[]
                //    siteId = 23
                //    name = Atrium
                //    status = 0
                //    openingDate = 
                //    altitudeAdjustment = 0
                //    maxSuiteAltitude = 26.869617929887028
                //    initialView = 
                //    position = ClientData
                //    center = ClientData
                //    addressLine1 = 650 Queens Quay W
                //    city = Toronto
                //    stateProvince = ON
                //    postalCode = M5V3N2
                //    country = Canada
                string[] subitems = new string[5];
                subitems[0] = buildingName;
                int separator = addressLine1.IndexOfAny(new char[] { ' ', ',' });
                subitems[1] = addressLine1.Substring(0, separator + 1);
                subitems[2] = addressLine1.Substring(separator + 1);
                subitems[3] = building.GetProperty("city", string.Empty);
                subitems[4] = postal;

                //// ---- Uncomment the following code to get a report on all buildings ----
                //string buildingInfo = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                //    count++,
                //    buildingName,
                //    addressLine1.Substring(0, separator + 1),
                //    addressLine1.Substring(separator + 1),
                //    building.GetProperty("city", string.Empty),
                //    postal);
                //System.Diagnostics.Trace.WriteLine(buildingInfo);
                //System.Diagnostics.Trace.Flush();
                //// ---- End of the buildings report code ----

                ListViewItem buildingItem = new ListViewItem(subitems);
                buildingItem.Tag = building;
                listViewAddresses.Items.Add(buildingItem);
            }
        }

        private void tabControlAddressPicking_SelectedTabChanged(object sender, EventArgs e)
        {
            if (tabControlAddressPicking.SelectedTab.Name == "tabPagePick")
                comboBoxCity.SelectedIndex = 0;

            UpdateState();
        }

        SuitesTableForm suitesTableForm = null;

        private void listViewAddresses_MouseClicked(object sender, MouseEventArgs e)
        {
            if (listViewAddresses.SelectedItems == null || listViewAddresses.SelectedItems.Count == 0)
            {
                UpdateState();
                return;
            }

            ListViewItem item = listViewAddresses.SelectedItems[0];
            textBuildingNo.Text = item.SubItems[1].Text;
            textStreet.Text = item.SubItems[2].Text;
            textUnitNo.Text = string.Empty;
            textCity.Text = item.SubItems[3].Text;
            textPostalCode.Text = item.SubItems[4].Text;

            ClientData building = item.Tag as ClientData;
            if (radioButtonPrivateListing.Checked || radioButtonSharedListing.Checked)
            {
                Cursor.Current = Cursors.WaitCursor;
                if (suitesTableForm != null)
                    suitesTableForm.Dispose();
                suitesTableForm = new SuitesTableForm(building);
                suitesTableForm.FormClosed += SuitesTableFormClosedEvent;
                suitesTableForm.ResizeEnd += new EventHandler(delegate(object o, EventArgs ee)
                {
                    Cursor.Current = Cursors.Default;
                });
                suitesTableForm.Show(this);
                BuildingID = building.GetProperty("id", -1);
            }
            else if (radioButton3DLayout.Checked)
            {
                PostalAddress = textStreet.Text;
                PropertyType = "building";
                PropertyID = building.GetProperty("id", -1);

                addressVerified = true;
            }

            UpdateState();
        }

        private void SuitesTableFormClosedEvent(object sender, EventArgs e)
        {
            SuitesTableForm objSrc = sender as SuitesTableForm;
            if (null == objSrc)
                return;

            if (objSrc.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;

            textUnitNo.Text = objSrc.Suite.SuiteName;
            ClientData suiteCD = objSrc.Suite.GetClientData();
            textBoxMLS.Text = objSrc.MLS;
            textVTourURL.Text = objSrc.VTourURL;
            textMoreInfoUrl.Text = objSrc.MoreInfoURL;
            PostalAddress = objSrc.PostalAddress;
            PropertyType = "suite";
            PropertyID = objSrc.Suite.AutoID;
            richTextBoxListingPrice.Text = objSrc.Suite.CurrentPrice.HasValue ? objSrc.Suite.CurrentPrice.Value.ToString() : string.Empty;
            switch (objSrc.Suite.Status)
            {
                case Vre.Server.BusinessLogic.Suite.SalesStatus.AvailableRent:
                    radioButtonListingTypeRent.Checked = true;
                    break;
                default:
                    radioButtonListingTypeSale.Checked = true;
                    break;
            }

            addressVerified = true;
            tabControlSteps.SelectedIndex = tabControlSteps.SelectedIndex + 1;
            UpdateState();
        }

        private void NewViewOrder_Load(object sender, EventArgs e)
        {
            comboBoxCity.SelectedIndex = 0;
            UpdateState();
        }

        private void contextMenuStripSuites_Opening(object sender, CancelEventArgs e)
        {
            if (listViewAddresses.SelectedItems.Count == 0)
                e.Cancel = true;
        }

        private void comboBoxCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxStreetName.Text = string.Empty;
            initListViewAddresses("address");
        }

        private void listViewAddresses_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            listViewAddresses.Sort();
        }

        private void textBoxStreetName_TextChanged(object sender, EventArgs e)
        {
            filterAddresses();
        }

        private void updateStateEvent(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void buttonOneMore_Click(object sender, EventArgs e)
        {
            cleanUp();

            tabControlSteps.SelectTab("tabPageCheckAddress");
            UpdateState();
        }

        private void cleanUp()
        {
            textBoxMLS.Text = string.Empty;
            textMoreInfoUrl.Text = string.Empty;
            textBoxNote.Text = string.Empty;
            textVTourURL.Text = string.Empty;
            textBoxViewOrderURL.Text = string.Empty;
            textBox300Chars.Text = string.Empty;
            richTextBoxListingPrice.Text = string.Empty;
            theOrder = null;
            viewOrderUrlGenerated = false;
            if (m_developer.Name == "Demo")
            {
                textVTourURL.Text = "https://vrt.3dcondox.com/templates/images/Demo-VIRTUALTOUR.png";
                textMoreInfoUrl.Text = "https://vrt.3dcondox.com/templates/images/Demo-REALTOR_CA.png";
                numericUpDownDaysValid.Maximum = 3651;
                numericUpDownDaysValid.Value = 3650;
                numericUpDownPrice.Value = 0;
            }
        }

        private void radioButtonListingTypeSale_CheckedChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void radioButtonListingTypeRent_CheckedChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

    }
}
