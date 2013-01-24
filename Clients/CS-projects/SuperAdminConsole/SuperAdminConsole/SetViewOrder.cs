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

        const string DEFAULT = @"http://3rdpartylink.com";
        bool viewOrderUrlGenerated = false;
        bool emailSent = false;
        bool paymentSkip = false;
        public string paymentRefId { private set; get; }
        public string validatedUrl = string.Empty;

        bool initialFloorPlanOption;
        string initialExternalLinkOption;
        string initialMlsUrl;
        string initialNote;
        string initialMLS;

        public string ViewOrderPrice
        {
            get { return string.Format("${0:0.00}", numericUpDownPrice.Value); }
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

        public SetViewOrder(User user, ClientData order, ChangeReason reason)
        {
            InitializeComponent();
            theReason = reason;

            textExternalLink.Text = DEFAULT;
            textExternalLink.GotFocus += (sender, e) =>
            {
                if (textExternalLink.Text.Equals(DEFAULT))
                {
                    textExternalLink.Text = @"http://";
                    textExternalLink.SelectionStart = DEFAULT.Length;
                }
            };
            textExternalLink.LostFocus += (sender, e) =>
            {
                if (textExternalLink.Text.Trim().Length == 0 || textExternalLink.Text == @"http://")
                    textExternalLink.Text = DEFAULT;
            };


            theOrder = order;
            if (theReason != ChangeReason.Creation)
            {
                tabControlSteps.SelectTab("tabPageViewOrderOptions");
                addressVerified = true;
                ViewOrder.ViewOrderOptions options = theOrder.GetProperty<ViewOrder.ViewOrderOptions>("options", ViewOrder.ViewOrderOptions.FloorPlan);
                if (options == ViewOrder.ViewOrderOptions.FloorPlan)
                    radioButtonFloorplan.Checked = true;
                else
                {
                    radioButtonExternalLink.Checked = true;
                    textExternalLink.Text = theOrder["vTourUrl"] as string;
                    validatedUrl = textExternalLink.Text;
                }
                PostalAddress = theOrder.GetProperty("label", string.Empty);
                textBoxMLS.Text = theOrder.GetProperty("mlsId", string.Empty);
                textBoxMlsUrl.Text = theOrder.GetProperty("infoUrl", string.Empty);
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

            if (user.UserRole == User.Role.SuperAdmin)
            {
                paymentSkip = true;
                numericUpDownDaysValid.Value = Properties.Settings.Default.defaultDaysValidTemp;
                numericUpDownPrice.Value = 0.00M;
                labelPrice.Visible = false;
                numericUpDownPrice.Visible = false;
            }

            theUser = user;
            comboBoxCountry.SelectedIndex = 0;

            initialFloorPlanOption = radioButtonFloorplan.Checked;
            initialExternalLinkOption = textExternalLink.Text;
            initialMLS = textBoxMLS.Text;
            initialMlsUrl = textBoxMlsUrl.Text;
            initialNote = textBoxNote.Text;

            lvwColumnSorter = new ListViewColumnSorter();
            listViewAddresses.ListViewItemSorter = lvwColumnSorter;
            textBoxMLS.ReadOnly = theReason != ChangeReason.Creation;
            textBoxMlsUrl.ReadOnly = theReason != ChangeReason.Creation;
            textBoxNote.Visible = theReason == ChangeReason.Creation;
            label11.Visible = theReason == ChangeReason.Creation;

            UpdateState();
        }

        private bool haveOptionsChanged()
        {
            return initialFloorPlanOption != radioButtonFloorplan.Checked ||
                   initialExternalLinkOption != textExternalLink.Text;
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

                    break;
                case "Address":
                    buttonPrev.Visible = true;
                    buttonPrev.Enabled = true;
                    buttonNext.Enabled = addressVerified;
                    buttonCheckAddress.Enabled = !addressVerified;
                    buttonFinish.Enabled = false;
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
                    buttonCancel.Visible = paymentRefId == string.Empty;
                    buttonFinish.Visible = true;

                    if (theReason == ChangeReason.Update)
                    {
                        buttonPayment.Enabled = haveOptionsChanged() && 
                                                numericUpDownPrice.Value > 0M &&
                                                paymentRefId == string.Empty &&
                                                    (radioButtonFloorplan.Checked ||
                                                            (validatedUrl != string.Empty &&
                                                             validatedUrl == textExternalLink.Text));

                        buttonFinish.Enabled =  haveOptionsChanged() && 
                                                  (paymentRefId != string.Empty ||
                                                   numericUpDownPrice.Value == 0M) ||
                                                initialMLS != textBoxMLS.Text ||
                                                initialMlsUrl != textBoxMlsUrl.Text ||
                                                initialNote != textBoxNote.Text;
                    }
                    else
                    {
                        buttonPayment.Enabled = !paymentSkip &&
                                                numericUpDownPrice.Value > 0M &&
                                                 paymentRefId == string.Empty &&
                                                    (radioButtonFloorplan.Checked ||
                                                            (validatedUrl != string.Empty &&
                                                             validatedUrl == textExternalLink.Text));
                        buttonFinish.Enabled = theReason == ChangeReason.Transfer;
                    }

                    groupBoxListingOptions.Enabled = paymentRefId == string.Empty;

                    numericUpDownTax.Visible = numericUpDownPrice.Value != 0.00M;
                    labelTax.Visible = numericUpDownPrice.Value != 0.00M;
                    labelPercent.Visible = numericUpDownPrice.Value != 0.00M;
                    textBoxTotal.Visible = numericUpDownPrice.Value != 0.00M;

                    textExternalLink.Enabled = radioButtonExternalLink.Checked;
                    numericUpDownPrice.Enabled = true;
                    buttonCheckLink.Enabled = textExternalLink.Enabled &&
                                              textExternalLink.Text.ToLower().Contains("http") &&
                                              textExternalLink.Text.ToLower().Contains(@"://") &&
                                              textExternalLink.Text.Length > 8 &&
                                              textExternalLink.Text.Contains(".") &&
                                              validatedUrl != textExternalLink.Text;

                    textBoxTotal.Text = ViewOrderPriceWithTax;
                    buttonCancel.Visible = paymentRefId == string.Empty;
                    break;
                case "Generate":
                    buttonCancel.Visible = paymentSkip && !viewOrderUrlGenerated;
                    buttonPrev.Visible = paymentSkip && !viewOrderUrlGenerated;
                    buttonSendEmailk.Enabled = viewOrderUrlGenerated && !emailSent;
                    buttonSendEmailk.Visible = viewOrderUrlGenerated;
                    buttonFinish.Enabled = theUser.UserRole == User.Role.SuperAdmin &&
                                                viewOrderUrlGenerated;
                    buttonGenerateURL.Enabled = !viewOrderUrlGenerated;
                    buttonNext.Visible = false;
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
            // https://vrt.3dcondox.com/vre/program?
            //                                      q=check&entity=building&
            //                                      ad_co=CANADA&
            //                                      ad_stn=QUEENS&
            //                                      ad_bn=650&
            //                                      ad_ibn=0508&
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
        public decimal DaysValid { get { return numericUpDownDaysValid.Value; } }


        private void buttonGenerateURL_Click(object sender, EventArgs e)
        {
            if (!viewOrderUrlGenerated)
            {
                // https://vrt.3dcondox.com/vre/program?
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
                if (radioButtonPrivateListing.Checked)
                    product = "prl";
                else if (radioButtonSharedListing.Checked)
                    product = "pul";
                else if (radioButton3DLayout.Checked)
                    product = "b3dl";

                string parameters = string.Format("q=register&" +
                                                  "entity=viewOrder&" +
                                                  "ownerId={0}&" +
                                                  "{1}" +
                                                  "daysValid={2}&" +
                                                  "product={10}&" +
                                                  "options={3}" +
                                                  "{4}&" +
                                                  "mls_id={7}&" +
                                                  "mls_url={8}&" +
                                                  "note=\"{9}\"&" +
                                                  "propertyType={5}&" +
                                                  "propertyId={6}",
                                                  theUser.AutoID,
                                                  numericUpDownPrice.Value==0?"":"pr="+paymentRefId+"&",
                                                  DaysValid,
                                                  radioButtonFloorplan.Checked?"fp":"evt",
                                                  radioButtonExternalLink.Checked ? "&evt_url=" + HttpUtility.UrlEncodeUnicode(textExternalLink.Text) : string.Empty,
                                                  PropertyType,
                                                  PropertyID,
                                                  textBoxMLS.Text,
                                                  HttpUtility.UrlEncodeUnicode(textBoxMlsUrl.Text),
                                                  HttpUtility.UrlEncodeUnicode(textBoxNote.Text),
                                                  product);
                ServerResponse resp = ServerProxy.MakeGenericRequest(ServerProxy.RequestType.Get,
                                                                  "program",
                                                                  parameters, null);
                if (HttpStatusCode.OK != resp.ResponseCode)
                {
                    MessageBox.Show("Cannot Create viewOrder");
                    DialogResult = System.Windows.Forms.DialogResult.Abort;
                    Close();
                    return;
                }

                
                // Response: JSON
                //
                //  {
                //      ”viewOrder-url”:<URL of direct link to created viewOrder>,
                //      ”button-url”:<URL of graphics for button>
                //  }
                // 
                textBoxViewOrderURL.Text = resp.Data["viewOrder-url"] as string;
                viewOrderUrlGenerated = true;
                UpdateState();
                MessageBox.Show("The ViewOrder has been generated successfully!\n" +
                                "Please send the link to the customer",
                                "ViewOrder is Ready!", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
                ViewOrderURL = textBoxViewOrderURL.Text;
                ViewOrderID = (resp.Data["viewOrder-id"] as string).Replace("-", string.Empty);

                Clipboard.SetText(ViewOrderURL);

                string the300xgars = Properties.Settings.Default.the300charsTemplate;
                textBox300Chars.Text = string.Format(the300xgars, 
                                                        ViewOrderURL,
                                                        DaysValid,
                                                        Properties.Settings.Default.PriceOf3DListing);
            }
            UpdateState();
        }

        public string ViewOrderID { private set; get; }
        public string ViewOrderURL { private set; get; }

        private void radioButtonExternalLink_CheckedChanged(object sender, EventArgs e)
        {
            textExternalLink.Focus();
            UpdateState();
        }

        private void textExternalLink_TextChanged(object sender, EventArgs e)
        {
            validatedUrl = string.Empty;
            UpdateState();
        }

        private void buttonCheckLink_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(textExternalLink.Text) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TURE if the Status code == 200
                if (response.StatusCode == HttpStatusCode.OK)
                    validatedUrl = textExternalLink.Text;
            }
            catch (Exception exception)
            {
                //Any exception will returns false.
                string error = string.Format("The provided External Link cannot be validated. \nError \'{0}\'",
                    exception.Message);
                MessageBox.Show(error, "Cannot Validate the Link Provided", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                validatedUrl = string.Empty;
                textExternalLink.Focus();
                textExternalLink.SelectionStart = 7;
                textExternalLink.SelectionLength = int.MaxValue;
            }
            Cursor.Current = Cursors.Default;
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
                DateTime expiresOn = DateTime.Now.AddDays((double)numericUpDownDaysValid.Value+1);
                expiresOn = System.TimeZone.CurrentTimeZone.ToUniversalTime(expiresOn.Date);
                theOrder["expiresOn"] = expiresOn;
                theOrder["ownerId"] = theUser.AutoID;
            }

            if (theReason != ChangeReason.Creation)
            {
                ViewOrder.ViewOrderOptions options = radioButtonFloorplan.Checked ? ViewOrder.ViewOrderOptions.FloorPlan : 
                                                                                    ViewOrder.ViewOrderOptions.ExternalTour;
                //bool changed = false;
                //theOrder.UpdateProperty("options", options, ref changed);
                theOrder["options"] = (int)options;
                theOrder["vTourUrl"] = radioButtonExternalLink.Checked ? textExternalLink.Text : string.Empty;
                theOrder["mlsId"] = textBoxMLS.Text;
                theOrder["infoUrl"] = textBoxMlsUrl.Text;
                theOrder["mlsNote"] = textBoxNote.Text;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void buttonSendEmailk_Click(object sender, EventArgs e)
        {
            EmailViewOrder.Template templ = EmailViewOrder.Template.OrderConfirmation;
            if (theReason == ChangeReason.Creation && theUser.UserRole == User.Role.SuperAdmin)
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
            string scopeParam = string.Format("scopeType={0}&ad_mu={1}",
                                              scopeType,
                                              comboBoxCity.SelectedItem);
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

        private void listViewAddresses_SelectedIndexChanged(object sender, EventArgs e)
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

            if (radioButtonPrivateListing.Checked || radioButtonSharedListing.Checked)
            {
                Cursor.Current = Cursors.WaitCursor;
                ClientData building = item.Tag as ClientData;
                if (suitesTableForm != null)
                    suitesTableForm.Dispose();
                suitesTableForm = new SuitesTableForm(building);
                suitesTableForm.FormClosed += SuitesTableFormClosedEvent;
                suitesTableForm.ResizeEnd += new EventHandler(delegate(object o, EventArgs ee)
                {
                    Cursor.Current = Cursors.Default;
                });
                suitesTableForm.Show(this);
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
            PostalAddress = objSrc.PostalAddress;
            PropertyType = "suite";
            PropertyID = objSrc.Suite.AutoID;

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

    }
}
