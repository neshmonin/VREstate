using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using Vre.Client;
using Vre.Server.BusinessLogic;
using Vre.Server.BusinessLogic.Client;

namespace VRE_Client
{
    public partial class Form1 : Form
    {
        private ServerProxy _proxy;
        private ListViewItem _selectedSuiteItem;
        private Building _selectedBuilding;
        private List<SuiteEx> _changedItems = new List<SuiteEx>();

        public Form1()
        {
            InitializeComponent();

            Cursor.Current = Cursors.WaitCursor;
            lblStartupShutdown.Text = "Please wait: connecting to server...";
            pnlStartupShutdown.BringToFront();
            pnlStartupShutdown.Dock = DockStyle.Fill;
            pnlStartupShutdown.Visible = true;
            tmrStartup.Enabled = true;
        }

        private void tmrStartup_Tick(object sender, EventArgs e)
        {
            tmrStartup.Enabled = false;
            
            try
            {
                if (!tryConnect()) Application.Exit();
            }
            catch (Exception ex)
            {
                ex.Equals(this);
                Application.Exit();
            }
            finally
            {
                pnlStartupShutdown.SendToBack();
                pnlStartupShutdown.Visible = false;
                Cursor.Current = Cursors.Default;
            }
        }

        private bool tryConnect()
        {
            string ep = Program.Configuration.GetValue("serverEndPoint", "http://localhost:8000/vre/");
            int timeout = Program.Configuration.GetValue("serverRequestTimeoutSec", 15);

            if (_proxy != null) _proxy.Dispose();
            _proxy = new ServerProxy(ep, timeout, this);


            if (!_proxy.Test())
            {
                _proxy = null;

                MessageBox.Show(
                    "Server is not reachable.  Cannot continue.",
                    "Error - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                return false;
            }


            string uid = Program.Configuration.GetValue("clientLogin", string.Empty);
            string pwd = Program.Configuration.GetValue("clientPassword", string.Empty);

            if (!doLogin(_proxy, uid, pwd))
            {
                MessageBox.Show(
                    "Login failed.  Cannot continue.",
                    "Error - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }

            int siteId = Program.Configuration.GetValue("fixedSiteId", -1);
            ServerResponse resp = _proxy.MakeRestRequest(ServerProxy.RequestType.Get, "building", "site=" + siteId, null);
            if (HttpStatusCode.OK == resp.ResponseCode)
            {
                ClientData buildingList = resp.Data;

                foreach (ClientData cd in buildingList.GetNextLevelDataArray("buildings"))
                {
                    Building b = new Building(cd);

                    cbBuildingList.Items.Add(b);
                }
            }

            _selectedBuilding = null;
            showUnitDetails(null);

            return true;
        }

        private bool doLogin(ServerProxy proxy, string login, string password)
        {
            // attempt auto-login once with credentials passed
            if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password))
            {
                if (proxy.Login(login, password)) return true;
            }

            Login lf = new Login();
            lf.lblServer.Text = proxy.ServerEndpoint;
            lf.cbxLoginTypes.Items.AddRange(new string[] { "Plain" });
            lf.cbxLoginTypes.SelectedIndex = 0;
            lf.tbLogin.Text = login;
            while (DialogResult.OK == lf.ShowDialog())
            {
                if (proxy.Login(lf.tbLogin.Text, lf.tbPassword.Text)) return true;
                else MessageBox.Show("Unknown login or bad password.\r\nPlease try again.",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return false;
        }

        private void cbBuildingList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvSuiteList.Items.Clear();

            Building b = cbBuildingList.SelectedItem as Building;

            if (null == b) return;

            if (_selectedBuilding != null)
            {
                if (_changedItems.Count > 0)
                {
                    switch (MessageBox.Show(
                        "You have changed parameters for " + _changedItems.Count +
                        " units in " + _selectedBuilding + ".\r\n" +
                        "Do you want to save them?",
                        "Unit information not saved - " + Text,
                        MessageBoxButtons.YesNo, //MessageBoxButtons.YesNoCancel, 
                        MessageBoxIcon.Question))
                    {
                        case DialogResult.Yes:
                            btnSave_Click(btnSave, e);
                            break;

                        case DialogResult.No:
                            break;

                        case DialogResult.Cancel:
                            // TODO:
                            return;
                    }
                }
            }

            _selectedBuilding = null;
            _changedItems.Clear();

            ServerResponse resp = _proxy.MakeRestRequest(ServerProxy.RequestType.Get, "suite", "building=" + b.AutoID, null);
            if (HttpStatusCode.OK == resp.ResponseCode)
            {
                ClientData suiteList = resp.Data;

                foreach (ClientData cd in suiteList.GetNextLevelDataArray("suites"))
                {
                    SuiteEx s = new SuiteEx(cd);

                    ListViewItem lvi = new ListViewItem();
                    updateListViewItem(s, lvi);

                    lvi.Tag = s;
                    lvSuiteList.Items.Add(lvi);
                }

                if (null == lvSuiteList.ListViewItemSorter)
                    lvSuiteList.ListViewItemSorter = new ListViewColumnSorter();

                _selectedBuilding = b;
                btnSave.Enabled = false;
                btnExport.Enabled = true;
                btnImport.Enabled = true;
            }
            else
            {
                // TODO: Display error
            }
        }

        private void lvSuiteList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter sorter = lvSuiteList.ListViewItemSorter as ListViewColumnSorter;
            if (null == sorter) return;

            if (e.Column == sorter.ColumnToSort)
            {
                if (sorter.OrderOfSort == SortOrder.Ascending) sorter.OrderOfSort = SortOrder.Descending;
                else sorter.OrderOfSort = SortOrder.Ascending;
            }
            else
            {
                sorter.ColumnToSort = e.Column;
                sorter.OrderOfSort = SortOrder.Ascending;
            }

            lvSuiteList.Sort();
        }

        #region free text converstion and validation
        private static void updateListViewItem(SuiteEx unit, ListViewItem item)
        {
            item.SubItems.Clear();
            item.SubItems.Add(ceilingHeightToString(unit.CeilingHeight));
            item.SubItems.Add(unit.Status.ToString());
            item.SubItems.Add(priceToString(unit.CurrentPrice));
            item.SubItems.Add(unit.SuiteType.Name);
            item.Text = unit.SuiteName;
        }

        private static string priceToString(double price)
        {
            if (price > 0.0) return price.ToString("#.00");
            else return "---";
        }

        private static string ceilingHeightToString(ValueWithUM height)
        {
            double ft = height.ValueAs(ValueWithUM.Unit.Feet);
            if (ft > 0.0) return ft.ToString("#.00");
            else return "---";
        }

        private static int salesStatusToIndex(Suite.SalesStatus status)
        {
            switch (status)
            {
                case Suite.SalesStatus.Available:
                    return 0;
                case Suite.SalesStatus.OnHold:
                    return 1;
                case Suite.SalesStatus.Sold:
                    return 2;
                default:
                    return -1;
            }
        }

        private static double priceFromString(string text)
        {
            double result;
            if (double.TryParse(text, out result)) return result;
            return -1.0;
        }

        private static ValueWithUM ceilingHeightFromString(string text)
        {
            double result;
            if (double.TryParse(text, out result)) return new ValueWithUM(result, ValueWithUM.Unit.Feet);
            else return ValueWithUM.EmptyLinear;
        }

        private static Suite.SalesStatus salesStatusFromIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return Suite.SalesStatus.Available;
                case 1:
                    return Suite.SalesStatus.OnHold;
                case 2:
                    return Suite.SalesStatus.Sold;
                default:
                    throw new ApplicationException("Sales status combo box has unknown status item.");
            }
        }

        private void tbUiUnitPrice_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool validated = false;
            double result;
            if (double.TryParse(tbUiUnitPrice.Text, out result))
            {
                validated = ((result > 0.0) && (result < 100000000.0));  // TODO: should be configurable
            }
            else if (tbUiUnitPrice.Text.Equals("---"))
            {
                validated = true;
            }

            e.Cancel = !validated;
        }

        private void tbUiCeilingHeight_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool validated = false;
            double result;
            if (double.TryParse(tbUiCeilingHeight.Text, out result))
            {
                validated = ((result > 0.0) && (result < 30.0));  // TODO: should be configurable
            }
            else if (tbUiCeilingHeight.Text.Equals("---"))
            {
                validated = true;
            }

            e.Cancel = !validated;
        }

        private void tbUiUnitNumber_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool validated = false;

            validated = (tbUiUnitNumber.Text.Length <= 16);

            e.Cancel = !validated;
        }
        #endregion

        private void showUnitDetails(SuiteEx unit)
        {
            if (unit != null)
            {
                lblUiFloorName.Text = unit.FloorName; lblUiFloorName.Enabled = true;
                tbUiUnitNumber.Text = unit.SuiteName; tbUiUnitNumber.Enabled = true;
                lblUiUnitType.Text = unit.SuiteType.Name; lblUiUnitType.Enabled = true;
                tbUiUnitPrice.Text = priceToString(unit.CurrentPrice); tbUiUnitPrice.Enabled = true;
                tbUiCeilingHeight.Text = ceilingHeightToString(unit.CeilingHeight); tbUiCeilingHeight.Enabled = true;
                cbUiSaleStatus.SelectedIndex = salesStatusToIndex(unit.Status); cbUiSaleStatus.Enabled = true;
                cbxUiShowPanoramicView.Checked = unit.ShowPanoramicView; cbxUiShowPanoramicView.Enabled = true;
            }
            else
            {
                lblUiFloorName.Text = string.Empty; lblUiFloorName.Enabled = false;
                tbUiUnitNumber.Text = string.Empty; tbUiUnitNumber.Enabled = false;
                lblUiUnitType.Text = string.Empty; lblUiUnitType.Enabled = false;
                tbUiUnitPrice.Text = string.Empty; tbUiUnitPrice.Enabled = false;
                tbUiCeilingHeight.Text = string.Empty; tbUiCeilingHeight.Enabled = false;
                cbUiSaleStatus.SelectedItem = -1; cbUiSaleStatus.Enabled = false;
                cbxUiShowPanoramicView.Checked = false; cbxUiShowPanoramicView.Enabled = false;
            }
        }

        private bool suiteDataOnUiChanged(SuiteEx toCompare)
        {
            bool result = false;

            if (!result && !tbUiUnitNumber.Text.Equals(toCompare.SuiteName)) result = true;
            if (!result && !priceFromString(tbUiUnitPrice.Text).Equals(toCompare.CurrentPrice)) result = true;
            if (!result && !ceilingHeightFromString(tbUiCeilingHeight.Text).Equals(toCompare.CeilingHeight)) result = true;
            if (!result && !salesStatusFromIndex(cbUiSaleStatus.SelectedIndex).Equals(toCompare.Status)) result = true;
            if (!result && (cbxUiShowPanoramicView.Checked != toCompare.ShowPanoramicView)) result = true;

            return result;
        }

        private void lvSuiteList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SuiteEx unit = null;
            ListViewItem item = null;
            if (1 == lvSuiteList.SelectedItems.Count) item = lvSuiteList.SelectedItems[0];
            if (item != null) unit = item.Tag as SuiteEx;

            //if ((item != null) && _revertUnitSelection)
            //{
            //    _revertUnitSelection = false;
            //    item.Selected = false;
            //    _selectedSuiteItem.Selected = true;
            //    return;
            //}

            if ((_selectedSuiteItem != null) && (null == item))
            {
                SuiteEx selected = _selectedSuiteItem.Tag as SuiteEx;
                Debug.Assert(selected != null, "Selected list item has no associated SuiteEx in tag!");
                if (suiteDataOnUiChanged(selected))
                {
                    switch (MessageBox.Show(
                        "You have changed parameters for unit " + selected.SuiteName + ".\r\n" +
                        "Do you want to save them?",
                        "Unit information not saved - " + Text,
                        MessageBoxButtons.YesNo, //MessageBoxButtons.YesNoCancel, 
                        MessageBoxIcon.Question))
                    {
                        case DialogResult.Yes:
                            btnUiUpdate_Click(btnUiUpdate, e);
                            break;

                        case DialogResult.No:
                            break;

                        case DialogResult.Cancel:
                            // TODO
                            //_selectedSuiteItem.Selected = true;
                            //item.Selected = false;
                            //_revertUnitSelection = true;
                            return;
                    }
                }
            }

            showUnitDetails(unit);            
            _selectedSuiteItem = item;
        }

        private void btnUiUpdate_Click(object sender, EventArgs e)
        {
            SuiteEx unit = _selectedSuiteItem.Tag as SuiteEx;
            Debug.Assert(unit != null, "Selected list item has no associated SuiteEx in tag!");

            unit.SuiteName = tbUiUnitNumber.Text;
            unit.CurrentPrice = priceFromString(tbUiUnitPrice.Text);
            unit.CeilingHeight.SetValue(ceilingHeightFromString(tbUiCeilingHeight.Text).ValueAs(ValueWithUM.Unit.Feet), ValueWithUM.Unit.Feet);
            unit.Status = salesStatusFromIndex(cbUiSaleStatus.SelectedIndex);
            unit.ShowPanoramicView = cbxUiShowPanoramicView.Checked;

            updateListViewItem(unit, _selectedSuiteItem);

            if (!_changedItems.Contains(unit)) _changedItems.Add(unit);
            btnSave.Enabled = true;
        }

        private void btnUiRevert_Click(object sender, EventArgs e)
        {
            lvSuiteList_SelectedIndexChanged(lvSuiteList, e);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int svCount = 0;

            int idx = _changedItems.Count;
            ClientData[] units = new ClientData[idx];
            for (idx--; idx >= 0; idx--) units[idx] = _changedItems[idx].GetClientData();

            ClientData changes = new ClientData();
            changes.Add("suites", units);

            ServerResponse resp = _proxy.MakeRestRequest(ServerProxy.RequestType.Update, "building/" + _selectedBuilding.AutoID, null, changes);
            switch (resp.ResponseCode)
            {
                case HttpStatusCode.OK:
                    svCount = resp.Data.GetProperty("updated", 0);
                    break;

                default:
                    break;
            }

            _changedItems.Clear();
            btnSave.Enabled = false;
            MessageBox.Show("Successfully posted " + svCount + " unit information updates to server.",
                "Unit information update - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {

        }
    }
}
