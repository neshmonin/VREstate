using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using CoreClasses;
using Vre.Server.BusinessLogic;

namespace SuperAdminConsole
{
    public partial class SuitesTableForm : Form
    {
        public Vre.Server.BusinessLogic.SuiteInventory Suite
        {
            private set;
            get;
        }

        public Vre.Server.BusinessLogic.Building Building
        {
            private set;
            get;
        }

        public string PostalAddress
        {
            private set;
            get;
        }

        public string MLS
        {
            private set;
            get;
        }

        public string VTourURL
        {
            private set;
            get;
        }

        public string MoreInfoURL
        {
            private set;
            get;
        }

        public SuitesTableForm(ClientData building)
        {
            InitializeComponent();
            Building = new Vre.Server.BusinessLogic.Building(building);
            this.Text = "Suites for " + Building.ToString();
            int ID = building.GetProperty("id", 0);
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                              "inventory",
                                                              "building=" + ID, null);

            if (HttpStatusCode.OK != resp.ResponseCode)
                return;

            ClientData[] suites = resp.Data.GetNextLevelDataArray("inventory");

            Array.Sort(suites, delegate(ClientData first, ClientData second)
            {

                int int1 = NumberFromFloorName(first["floorName"] as string);
                int int2 = NumberFromFloorName(second["floorName"] as string);

                if (int1 != -1 && int2 != -1)
                {
                    if (int1 > int2)
                        return 1;
                    else if (int1 < int2)
                        return -1;
                }

                string str1 = first["name"] as string;
                string str2 = second["name"] as string;

                return str1.CompareTo(str2);
            });

            //    id, 7432
            //    version, System.Int32[]
            //    buildingId, 365
            //    levelNumber, -1
            //    floorName, 01
            //    name, 119
            //    showPanoramicView, True
            //    status, Available
            //    suiteTypeId, 2662
            //    currentPrice, -1

            string floor = string.Empty;
            Point label = new Point(labelFloorNo.Location.X, labelFloorNo.Location.Y);
            Point button = new Point(labelUnits.Location.X, labelUnits.Location.Y);
            const int ButtonIncrement = 50;
            const int lineIncrement = 20;
            Size maxSize = new Size(button);
            int index = 0;
            int tabIndex = 0;

            foreach (ClientData suite in suites)
            {
                SuiteInventory theSuite = new SuiteInventory(suite);
                if (floor != suite["floorName"] as string)
                {
                    floor = suite["floorName"] as string;
                    label.Y += lineIncrement;
                    button.X = labelUnits.Location.X;
                    button.Y += lineIncrement;
                    if (maxSize.Height < button.Y + lineIncrement)
                        maxSize.Height = button.Y + lineIncrement;

                    Label lbl = new Label();
                    lbl.Location = label;
                    lbl.Size = labelFloorNo.Size;
                    lbl.Text = floor;
                    lbl.Visible = true;
                    lbl.Enabled = true;
                    lbl.TabIndex = tabIndex++;
                    this.Controls.Add(lbl);
                }
                Button btn = new Button();
                btn.Location = button;
                btn.Size = new Size(ButtonIncrement - 4, 23);
                btn.Text = theSuite.SuiteName;
                btn.Tag = theSuite;
                btn.Visible = true;
                //btn.Enabled = theSuite.Status == Vre.Server.BusinessLogic.Suite.SalesStatus.Sold;
                if (theSuite.Status == Vre.Server.BusinessLogic.Suite.SalesStatus.Available)
                    btn.BackColor = Color.FromArgb(200, 255, 200);
                else if (theSuite.Status == Vre.Server.BusinessLogic.Suite.SalesStatus.ResaleAvailable)
                    btn.BackColor = Color.FromArgb(200, 255, 255);
                else if (theSuite.Status == Vre.Server.BusinessLogic.Suite.SalesStatus.AvailableRent)
                    btn.BackColor = Color.FromArgb(255, 220, 255);
                else if (theSuite.Status == Vre.Server.BusinessLogic.Suite.SalesStatus.OnHold)
                    btn.BackColor = Color.FromArgb(255, 255, 200);
                else if (theSuite.Status == Vre.Server.BusinessLogic.Suite.SalesStatus.Sold)
                    // TODO: change to 'theSuite.Status == Vre.Server.BusinessLogic.Suite.SalesStatus.Resale' when it ready
                    btn.BackColor = Color.FromArgb(255, 250, 250);

                //btn.UseVisualStyleBackColor = true;
                btn.TabIndex = tabIndex++;
                btn.Click += new System.EventHandler(delegate(object o, EventArgs e)
                    {
                        Button bttn = o as Button;
                        Suite = bttn.Tag as Vre.Server.BusinessLogic.SuiteInventory;
                        string addressLine1 = building.GetProperty("addressLine1", string.Empty);
                        string addressLine2 = building.GetProperty("addressLine2", string.Empty);
                        string city = building.GetProperty("city", string.Empty);
                        string stateProvince = building.GetProperty("stateProvince", string.Empty);
                        string country = building.GetProperty("country", string.Empty);
                        string postal = building.GetProperty("postalCode", string.Empty);
                        PostalAddress = string.Format("{0}-{1}, {2}{3}, {4}, {5}, {6}",
                                                        Suite.SuiteName,
                                                        addressLine1,
                                                        addressLine2 != string.Empty ? ", " + addressLine2 : string.Empty,
                                                        city,
                                                        stateProvince,
                                                        postal,
                                                        country);

                        MLS = theSuite.MLS;
                        VTourURL = theSuite.VirtualTourURL;
                        MoreInfoURL = theSuite.MoreInfoURL;

                        this.AcceptButton = bttn;
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                        Close();
                    });
                this.Controls.Add(btn);

                button.X += ButtonIncrement;
                index++;

                if (maxSize.Width < button.X + ButtonIncrement)
                    maxSize.Width = button.X + ButtonIncrement;
            }
            maxSize.Height += this.Size.Height;
            this.Size = maxSize;
        }

        private int NumberFromFloorName(string floor)
        {
            if (string.IsNullOrEmpty(floor))
                return -1;

            floor = floor.ToLower();
            int int1 = -1;
            if (floor.StartsWith("ph"))
                int1 = 1000;
            else
            if (floor.StartsWith("gf"))
                int1 = 0;
            else
            if (floor.StartsWith("th"))
                int1 = 0;

            if (int1 != -1)
            {
                int firstDigit = floor.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                if (firstDigit != -1)
                {
                    int lastDigit = floor.LastIndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                    floor = floor.Substring(firstDigit, lastDigit - firstDigit + 1);
                    int1 += int.Parse(floor);
                }
            }
            else
            {
                try
                {
                    int1 = int.Parse(floor);
                }
                catch (Exception)
                { // probably "RG" or something like this
                }
            }
            return int1;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Escape)
            {
                this.Close();
                return true; // Handled.
            }

            return base.ProcessDialogKey(keyData);
        }

    }
}
