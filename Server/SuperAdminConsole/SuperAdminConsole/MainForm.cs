using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CoreClasses;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Net;
using Vre.Server.BusinessLogic;

namespace SuperAdminConsole
{
    public partial class MainForm : Form
    {
        private ListViewColumnSorter lvwColumnSorter;
        private SuperServer m_superServer = null;
        string m_nickname = string.Empty;
        string m_email = string.Empty;

        Point lastNoteLocation;
        public Point LastNoteLocation
        {
            get { return lastNoteLocation; }
            set { lastNoteLocation = value; }
        }

        Size lastNoteSize;

        public Size LastNoteSize
        {
            get { return lastNoteSize; }
            set { lastNoteSize = value; }
        }

        public MainForm()
        {
            InitializeComponent();
            this.Text += " (" + Properties.Settings.Default.serverEndPoint + ")";

            Cursor.Current = Cursors.WaitCursor;
            lblStartupShutdown.Text = "Please wait: connecting to server...";
            pnlStartupShutdown.BringToFront();
            pnlStartupShutdown.Dock = DockStyle.Fill;
            pnlStartupShutdown.Visible = true;
            tmrStartup.Enabled = true;
            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            ListViewOrders.ListViewItemSorter = lvwColumnSorter;
            treeViewAccounts.TreeViewNodeSorter = new TreeNodeSorter();

            ServerProxy.Create(Properties.Settings.Default.serverEndPoint, 30);
            ServerProxy.OnSessionTreadExpired = new ServerProxy.SessionTreadExpired(delegate() 
                {
                    MessageBox.Show(
                            "Client session expired. Please restart the application",
                            "Session Expired", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                });

            //refreshUserAccounts();
            lastNoteLocation = new Point(this.ClientRectangle.Left, this.ClientRectangle.Top);
            lastNoteSize = new Size(560, 151);
        }

        #region Login
        private void tmrStartup_Tick(object sender, EventArgs e)
        {
            tmrStartup.Enabled = false;

            try
            {
                if (!tryConnect())
                    Application.Exit();
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
                tmrStartup.Stop();
                refreshUserAccounts();
            }
        }

        private bool tryConnect()
        {
            string ep = Properties.Settings.Default.serverEndPoint;
            int timeout = Properties.Settings.Default.serverRequestTimeoutSec;

            m_superServer = SuperServer.Create(ep, timeout);
            ServerProxy.UiUpdater = new ServerProxy.UpdateUiDelegate(delegate() 
                {
                    if (InvokeRequired)
                        Invoke(new ServerProxy.UpdateUiDelegate(this.Update));
                    else
                        this.Update();
                });

            if (!ServerProxy.Test())
            {
                MessageBox.Show(
                    "Server is not reachable.  Cannot continue.",
                    "Error - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                return false;
            }


            string uid = Properties.Settings.Default.clientLogin;
            string pwd = Properties.Settings.Default.clientPassword;

            if (!doLogin(uid, pwd))
            {
                MessageBox.Show(
                    "Login failed.  Cannot continue.",
                    "Error - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }

            return true;
        }

        private bool doLogin(string login, string password)
        {
            // attempt auto-login once with credentials passed
            if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password))
            {
                if (m_superServer.SuperadminLogin(login, password))
                {
                    Trace.WriteLine(DateTime.Now.ToString() + "> Connected to server " +
                                    Properties.Settings.Default.serverEndPoint +
                                    ", Login=" + login +
                                    ", SID=" + ServerProxy.SID, "Info");
                    Trace.Flush();
                    return true;
                }
            }

            LoginForm lf = new LoginForm(ServerProxy.ServerEndpoint, login);
            while (DialogResult.OK == lf.ShowDialog(this))
            {
                login = lf.tbLogin.Text;
                password = lf.tbPassword.Text;

                if (m_superServer.SuperadminLogin(login, password))
                {
                    Trace.WriteLine(DateTime.Now.ToString() + "> Connected to server " +
                                    Properties.Settings.Default.serverEndPoint +
                                    ", Login=" + login +
                                    ", SID=" + ServerProxy.SID, "Info");
                    Trace.Flush();
                    return true;
                }
            }
            return false;
        }
        #endregion /* Login */

        private void UpdateState()
        {
            if (treeViewAccounts.SelectedNode == null)
            {
                tabControlAccountProperty.SelectTab("tabPageEmpty");
                labelStreetName.Visible = false;
                textBoxFilter.Visible = false;
            }
            else
            {
                labelStreetName.Visible = treeViewAccounts.SelectedNode.Text == "ViewOrders";
                textBoxFilter.Visible = treeViewAccounts.SelectedNode.Text == "ViewOrders";
                if (treeViewAccounts.SelectedNode.Level == 0)
                    tabControlAccountProperty.SelectTab("tabPageInfo");
                else
                {
                    switch (treeViewAccounts.SelectedNode.Text)
                    {
                        case "ViewOrders":
                            tabControlAccountProperty.SelectTab("tabPageViewOrders");
                            break;
                        case "Banners":
                            tabControlAccountProperty.SelectTab("tabPageBanners");
                            break;
                        case "Logs":
                            tabControlAccountProperty.SelectTab("tabPageLogs");
                            break;
                    }
                }
            }
        }

        private void AddAccount_Click(object sender, EventArgs e)
        {
            Form newAccount = new NewAccount();
            if (newAccount.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                refreshUserAccounts();
            }
        }

        private void buttonAddNewViewOrder_Click(object sender, EventArgs e)
        {
            SetViewOrder newViewOrder = new SetViewOrder(m_agent, null, SetViewOrder.ChangeReason.Creation);
            newViewOrder.ShowDialog(this);
            refreshOrders();

            UpdateState();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            refreshUserAccounts();
        }

        public int MyID { private set; get; }

        private void refreshUserAccounts()
        {
            ListViewOrders.Items.Clear();
            treeViewAccounts.Nodes.Clear();

            // first we add a node for the superAdmin
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                              "user/" + ServerProxy.MyID,
                                                              "", null);
            User admin = new User(resp.Data);
            //TreeNode[] ch = new TreeNode[3] { new TreeNode("ViewOrders"),
            //                                  new TreeNode("Banners"),
            //                                  new TreeNode("Logs") };
            TreeNode[] ch = new TreeNode[2] { new TreeNode("ViewOrders"),
                                              new TreeNode("Logs") };
            TreeNode superAdminNode = new TreeNode("SELF (superAdmin)", ch);
            superAdminNode.Tag = admin;
            treeViewAccounts.Nodes.Add(superAdminNode);

            ///data/user?sid=<SID>[&genval=<generation>][&withdeleted={true|false}][&<hints>]
            resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                              "user",
                                                              "role=sellingagent", null);
            if (HttpStatusCode.OK != resp.ResponseCode)
            {
                MessageBox.Show("Failed querying the list of customers");
                return;
            }

            ClientData suiteTypeist = resp.Data;
            ClientData[] users = suiteTypeist.GetNextLevelDataArray("users");

            foreach (ClientData cd in users)
            {
                User user = new User(cd);
                string nodeName = string.Format("{0} <{1}> {2}", user.AutoID, user.NickName, user.PrimaryEmailAddress);
                //TreeNode[] children = new TreeNode[3] { new TreeNode("ViewOrders"),
                //                                        new TreeNode("Banners"),
                //                                        new TreeNode("Logs") };
                TreeNode[] children = new TreeNode[2] { new TreeNode("ViewOrders"),
                                                        new TreeNode("Logs") };
                TreeNode treeNode = new TreeNode(nodeName, children);
                treeNode.Tag = user;
                treeViewAccounts.Nodes.Add(treeNode);
            }
            UpdateState();
        }

        private User m_agent = null;
        private User m_rigthMouseAgent = null;

        private void treeViewAccounts_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode rigthMouseNode = treeViewAccounts.GetNodeAt(e.X, e.Y);
                if (rigthMouseNode != null && rigthMouseNode.Tag != null)
                {
                    if (!rigthMouseNode.Text.StartsWith("SELF"))
                    {
                        m_rigthMouseAgent = rigthMouseNode.Tag as User;
                        return;
                    }
                }
            }
            m_rigthMouseAgent = null;
        }


        private void treeViewAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            m_agent = null;
            if (e.Node.Tag != null)
                m_agent = e.Node.Tag as User;
            else
            if (e.Node.Parent != null && e.Node.Parent.Tag != null)
                m_agent = e.Node.Parent.Tag as User;

            if (m_agent != null)
            {
                m_email = m_agent.PrimaryEmailAddress == string.Empty?"no email":m_agent.PrimaryEmailAddress;
                m_nickname = m_agent.NickName == string.Empty?"superAdmin":m_agent.NickName;
            }

            if (e.Node.Parent == null)
            {
                refreshAccountInfo();
            }
            else if (e.Node.Text == "ViewOrders")
            {
                refreshOrders();
            }
            else if (e.Node.Text == "Logs")
            {
                listViewLogs.Items.Clear();
                // https://vrt.3dcondox.com/vre/data/ft?userId=<id>&sid=<SID>
                ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                                  "ft",
                                                                  "userId=" + m_agent.AutoID, null);
                if (HttpStatusCode.OK != resp.ResponseCode)
                {
                    MessageBox.Show("Failed retreiving log for customer \'" + m_agent.NickName + "\'");
                    UpdateState();
                    return;
                }

                ClientData[] logs = resp.Data.GetNextLevelDataArray("transactions");
                //ClientData[] logs = orders.GetNextLevelDataArray("orders");
                foreach (ClientData log in logs)
                {
                    //    created = {13/10/2012 6:11:57 AM}                         System.DateTime
                    //    paymentSystem = CondoExplorer                             string
                    //    paymentRefId = 3DCX001                                    string
                    //    initiatorId = 4                                           int
                    //    account = User                                            string
                    //    accountId = 8                                             int
                    //    operation = Debit                                         string
                    //    amount = 50                                               decimal
                    //    subject = View"                                           string
                    //    target = Suite"                                           string
                    //    targetId = 8564                                           int
                    //    extraTargetInfo = 96D7DB20-DD22-4F66-B153-C7053484ED3C    string
                    string[] subitems = new string[7];
                    DateTime time = log.GetProperty("created", DateTime.Now);
                    subitems[0] = time.Date.ToShortDateString() + ", " + time.ToLongTimeString();
                    subitems[1] = log.GetProperty("paymentSystem", string.Empty) == "CondoExplorer"?"CE":"PP";
                    string paymentRefId = log.GetProperty("paymentRefId", string.Empty);
                    subitems[2] = paymentRefId;
                    subitems[3] = log.GetProperty("operation", string.Empty);
                    double amount = log.GetProperty("amount", 0.00);
                    subitems[4] = string.Format("${0:00.00}", amount);
                    subitems[5] = log.GetProperty("target", string.Empty);
                    subitems[6] = log.GetProperty("targetId", 0).ToString();

                    ListViewItem order = new ListViewItem(subitems);

                    order.Tag = log;
                    listViewLogs.Items.Add(order);
                }
            }

            UpdateState();
        }

        void refreshAccountInfo()
        {
            listViewAccountInfo.Items.Clear();
            if (m_agent == null)
                return;

            foreach (var prop in m_agent.GetClientData())
            {
                if (prop.Key == "version" || prop.Key == "estateDeveloperId")
                    continue;

                string[] subitems = new string[2];
                subitems[0] = prop.Key.ToString();
                subitems[1] = prop.Value.ToString();

                ListViewItem acctInfo = new ListViewItem(subitems);
                listViewAccountInfo.Items.Add(acctInfo);
            }
        }

        private void contextMenuStripAccountProperty_Opening(object sender, CancelEventArgs e)
        {
            if (listViewAccountInfo.SelectedItems.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            string prop = listViewAccountInfo.SelectedItems[0].SubItems[0].Text;
            if (prop == "id" || prop == "version" || prop == "role")
            {
                e.Cancel = true;
                return;
            }
        }

        private void changePropertyValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string prop = listViewAccountInfo.SelectedItems[0].SubItems[0].Text;
            ClientData data = m_agent.GetClientData();
            AccountPropertyForm editForm = new AccountPropertyForm(data, prop);
            if (editForm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                data[prop] = editForm.NewValue;

                ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                                  "user/" + m_agent.AutoID,
                                                                  "", data);
                if (HttpStatusCode.OK != resp.ResponseCode)
                {
                    MessageBox.Show("Failed updating property \'" + prop + "\'");
                    return;
                }
                refreshUserAccounts();
            }
        }

        void FilterViewOrders()
        {
            ListViewOrders.Items.Clear();
            foreach (ClientData viewOrder in viewOrdersOfCurrentUser)
            {
                if (viewOrder.GetProperty("deleted", false))
                    continue;

                string filter = textBoxFilter.Text.ToLower();
                string label = viewOrder.GetProperty("label", string.Empty);
                String note = viewOrder.GetProperty("note", string.Empty);
                if (!label.ToLower().Contains(filter) &&
                    !note.ToLower().Contains(filter))
                    continue;

                //id = 20a6b755-58b2-4616-89de-92faf1ed6814
                //version = Vre.Server.BusinessLogic.ClientData[]
                //ownerId = 8
                //expiresOn = 01/11/2012 12:00:00 AM
                //enabled = True
                //product = 0
                //mlsId = 
                //productUrl = 
                //targetObjectType = 0
                //targetObjectId = 8516
                //requestCounter = 0
                //lastRequestTime = 12/10/2012 9:26:40 AM
                string[] subitems = new string[5];
                //[9] = {[targetObjectId, 8516]}

                if (label == string.Empty)
                {
                    ViewOrder theOrder;
                    User theUser;
                    label = Helpers.LabelFromViewOrder(viewOrder.GetProperty("id", string.Empty),
                                                            out theOrder, out theUser);
                    subitems[0] = "-";
                    subitems[1] = label;
                }
                else
                {
                    int separatorIndex = label.IndexOf(" - ");
                    string aptNum = label.Substring(0, separatorIndex);
                    string buildingAddr = label.Substring(separatorIndex + 3);
                    subitems[0] = aptNum;
                    subitems[1] = buildingAddr;
                }

                switch (viewOrder.GetProperty("targetObjectType", 0))
                {
                    case 0:
                        subitems[2] = "PrL";
                        break;
                    case 1:
                        subitems[2] = "Bld";
                        break;
                    case 2:
                        subitems[2] = "Site";
                        break;
                    case 3:
                        subitems[2] = "GS";
                        break;
                    case 4:
                        subitems[2] = "3DL";
                        break;
                }
                DateTime expiredOn = viewOrder.GetProperty("expiresOn", DateTime.Now);
                subitems[3] = expiredOn.ToShortDateString();
                subitems[4] = viewOrder.GetProperty("mlsId", string.Empty);

                string productUrl = viewOrder.GetProperty("productUrl", string.Empty);
                if (productUrl != string.Empty)
                {
                    int i = 0;
                }

                ListViewItem orderItem = new ListViewItem(subitems);
                orderItem.Font = ListViewOrders.Font.Clone() as Font;
                if (expiredOn.CompareTo(DateTime.Now.AddDays(2.0)) < 0)
                    orderItem.ForeColor = Color.Red;
                else
                    orderItem.ForeColor = Color.Green;

                if (expiredOn.CompareTo(DateTime.Now) < 0)
                {
                    orderItem.ForeColor = Color.Red;
                    orderItem.Font = new Font(orderItem.Font, FontStyle.Strikeout);
                }

                if (!viewOrder.GetProperty("enabled", true))
                    orderItem.ForeColor = Color.LightGray;

                orderItem.Tag = viewOrder;
                if (!string.IsNullOrEmpty(note))
                    orderItem.ToolTipText = note;

                ListViewOrders.Items.Add(orderItem);
            }
        }

        int lastSelectedIndex = -1;
        ClientData[] viewOrdersOfCurrentUser = null;
        void refreshOrders()
        {
            if (ListViewOrders.SelectedIndices.Count > 0)
                lastSelectedIndex = ListViewOrders.SelectedIndices[0];

            //https://vrt.3dcondox.com/vre/data/viewOrder?
            //                                         userId=<selling agent id>&
            //                                         sid=<SID>
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                                "viewOrder",
                                                                "userId=" + m_agent.AutoID +
                                                                "&verbose=true", null);
            if (HttpStatusCode.OK != resp.ResponseCode)
            {
                MessageBox.Show("Failed querying ViewOrders for customer \'" + m_agent.NickName + "\'");
                UpdateState();
                return;
            }

            //ClientData orders = resp.Data.GetNextLevelDataItem("orders");
            ClientData orders = resp.Data;
            viewOrdersOfCurrentUser = orders.GetNextLevelDataArray("viewOrders");
            FilterViewOrders();

            if (lastSelectedIndex != -1)
            {
                if (lastSelectedIndex >= ListViewOrders.Items.Count)
                    lastSelectedIndex = ListViewOrders.Items.Count - 1;

                if (lastSelectedIndex != -1)
                {
                    ListViewOrders.Items[lastSelectedIndex].Selected = true;
                    ListViewOrders.Select();
                    ListViewOrders.SelectedItems[0].EnsureVisible();
                    ListViewOrders.TopItem = ListViewOrders.Items[lastSelectedIndex];
                }
            }
        }

        private void contextMenuStripViewOrder_Opening(object sender, CancelEventArgs e)
        {
            if (ListViewOrders.SelectedItems.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            ClientData viewOrder = ListViewOrders.SelectedItems[0].Tag as ClientData;
            bool enabled = viewOrder.GetProperty("enabled", true);
            toolStripMenuItemEnableViewOrder.Text = enabled ? "Disable" : "Enable";
        }

        private void toolStripMenuItemEnableViewOrder_Click(object sender, EventArgs e)
        {
            ClientData viewOrder = ListViewOrders.SelectedItems[0].Tag as ClientData;
            // https://vrt.3dcondox.com/vre/data/viewOrder/<viewOrder ID>?sid=<SID>
            //    (HTTP PUT, in JSON should be "enabled"="frue")
            bool enabled = viewOrder.GetProperty("enabled", true);
            if (enabled)
                viewOrder["enabled"] = false;
            else
                viewOrder["enabled"] = true;

            string viewOrderId = viewOrder.GetProperty("id", string.Empty);
            if (viewOrderId != string.Empty)
            {
                // ClientData agent = treeViewAccounts.SelectedNode.Tag as ClientData;

                viewOrderId = viewOrderId.Replace("-", string.Empty); 
                ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                                  "viewOrder/" + viewOrderId,
                                                                  "", viewOrder);
                refreshOrders();
            }
            UpdateState();
        }

        private void toolStripMenuItemExtend_Click(object sender, EventArgs e)
        {
            ClientData viewOrder = ListViewOrders.SelectedItems[0].Tag as ClientData;
            string viewOrderId = viewOrder.GetProperty("id", string.Empty);
            if (viewOrderId != string.Empty)
            {
                DateTime expiredOn = viewOrder.GetProperty("expiresOn", DateTime.Now);
                UpdateDateTimeForm dialog = new UpdateDateTimeForm(expiredOn);
                if (dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                    return;

                viewOrder["expiresOn"] = dialog.ExpiredOn;

                viewOrderId = viewOrderId.Replace("-", string.Empty);
                ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                                  "viewOrder/" + viewOrderId,
                                                                  "", viewOrder);
                refreshOrders();
            }
            UpdateState();
        }

        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to permanently delete this viewOrder?",
                                "Deleting the viewOrder",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Stop)
                                                != System.Windows.Forms.DialogResult.Yes)
                return;

            // https://vrt.3dcondox.com/vre/data/viewOrder/<viewOrder ID>?sid=<SID>
            //     (HTTP DELETE)
            ClientData viewOrder = ListViewOrders.SelectedItems[0].Tag as ClientData;
            string viewOrderId = viewOrder.GetProperty("id", "");
            viewOrderId = viewOrderId.Replace("-", string.Empty); 
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Delete,
                                                              "viewOrder/" + viewOrderId,
                                                              "", null);
            refreshOrders();
            UpdateState();
        }

        private void listViewOrders_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_agent.UserRole != User.Role.SuperAdmin)
                return;

            if (ListViewOrders.SelectedItems.Count != 0)
                ListViewOrders.DoDragDrop(ListViewOrders.SelectedItems[0], DragDropEffects.Move);
        }

        private void listViewOrders_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void treeViewAccounts_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void treeViewAccounts_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode nodeToDropIn = treeViewAccounts.GetNodeAt(
                treeViewAccounts.PointToClient(new Point(e.X, e.Y)));
            if (nodeToDropIn == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            User newUser = null;
            if (nodeToDropIn.Level == 0)
            {
                newUser = nodeToDropIn.Tag as User;
                nodeToDropIn.Expand();
                nodeToDropIn = nodeToDropIn.Nodes[0];
            }
            else if (nodeToDropIn.Level == 1)
            {
                TreeNode parent = nodeToDropIn.Parent;
                newUser = parent.Tag as User;
                parent.Expand();
                nodeToDropIn = parent.Nodes[0];
            }

            if (newUser == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (newUser.UserRole == User.Role.SuperAdmin)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            m_agent = newUser;

            ListViewItem lvItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            if (lvItem == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            ClientData viewOrderToMove = lvItem.Tag as ClientData;
            if (viewOrderToMove == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            SetViewOrder updateViewOrder = new SetViewOrder(m_agent, viewOrderToMove, SetViewOrder.ChangeReason.Transfer);
            if (updateViewOrder.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            //viewOrderToMove["ownerId"] = m_agent.AutoID;
            //DateTime expiresOn = DateTime.Now.AddDays(90.0);
            //viewOrderToMove["expiresOn"] = expiresOn;

            string viewOrderId = viewOrderToMove.GetProperty("id", string.Empty);
            if (viewOrderId == string.Empty)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            viewOrderId = viewOrderId.Replace("-", string.Empty);
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                                "viewOrder/" + viewOrderId,
                                                                "pr=" + updateViewOrder.paymentRefId,
                                                                viewOrderToMove);

            if (HttpStatusCode.OK != resp.ResponseCode)
            {
                MessageBox.Show("Cannot transfer this viewOrder to this account");
                UpdateState();
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }
            }

            EmailViewOrder form = EmailViewOrder.Create(viewOrderId,
                                                        updateViewOrder.paymentRefId,
                                                        EmailViewOrder.Template.OrderConfirmation);
            if (form == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                UpdateState();
                MessageBox.Show("The viewOrder has been transferred successfully!", "ViewOrder Transferred",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            treeViewAccounts.SelectedNode = nodeToDropIn;
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (transactionInfo == null)
                return;
            
            MessageBox.Show(transactionInfo.Formatted, "PayPal Transaction Details");
            transactionInfo = null;
        }

        private Helpers.TransactionInfo transactionInfo = null;

        private void contextMenuStripLogs_Opening(object sender, CancelEventArgs e)
        {
            if (listViewLogs.SelectedItems.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            ClientData log = listViewLogs.SelectedItems[0].Tag as ClientData;
            string paymentRefId = log.GetProperty("paymentRefId", string.Empty);

            transactionInfo = Helpers.TransactionDetails(paymentRefId);
            if (transactionInfo == null)
            {
                e.Cancel = true;
                return;
            }
        }

        private void toolStripMenuItemChangeOrderView_Click(object sender, EventArgs e)
        {
            ClientData viewOrderToUpdate = ListViewOrders.SelectedItems[0].Tag as ClientData;
            SetViewOrder updateViewOrder = new SetViewOrder(m_agent, 
                                                            viewOrderToUpdate, 
                                                            SetViewOrder.ChangeReason.Update);
            if (updateViewOrder.ShowDialog(this) == DialogResult.OK)
            {
                string viewOrderId = viewOrderToUpdate.GetProperty("id", string.Empty);
                viewOrderId = viewOrderId.Replace("-", string.Empty);
                string param = string.IsNullOrEmpty(updateViewOrder.paymentRefId) ? string.Empty : "pr=" + updateViewOrder.paymentRefId;
                ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                                    "viewOrder/" + viewOrderId,
                                                                    param,
                                                                    viewOrderToUpdate);
                refreshOrders();
            }
        }

        private void loadTheLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientData viewOrder = ListViewOrders.SelectedItems[0].Tag as ClientData;
            System.Diagnostics.Process.Start(viewOrder["viewOrder-url"] as string);
        }

        private void ListViewOrders_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if ( e.Column == lvwColumnSorter.SortColumn )
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
            ListViewOrders.Sort();

        }

        private void editNotesForThisItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientData viewOrder = ListViewOrders.SelectedItems[0].Tag as ClientData;
            String note = viewOrder.GetProperty("note", string.Empty);
            string caption = "Notes for \'" + viewOrder.GetProperty("label", string.Empty) + "\'";
            Note noteDlg = new Note(caption, note, this);
            noteDlg.Location = LastNoteLocation;
            noteDlg.Size = LastNoteSize;
            if (noteDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string viewOrderId = viewOrder.GetProperty("id", string.Empty);
                if (viewOrderId != string.Empty)
                {
                    viewOrder["note"] = noteDlg.Notes;

                    viewOrderId = viewOrderId.Replace("-", string.Empty);
                    ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                                        "viewOrder/" + viewOrderId,
                                                                        "", viewOrder);
                }
                refreshOrders();
            }
        }

        private void editMLSURLForThisItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientData viewOrder = ListViewOrders.SelectedItems[0].Tag as ClientData;
            String mlsUrl = viewOrder.GetProperty("mlsUrl", string.Empty);
            string caption = "MLS URL for \'" + viewOrder.GetProperty("label", string.Empty) + "\'";
            Note noteDlg = new Note(caption, mlsUrl, this);
            noteDlg.Location = LastNoteLocation;
            noteDlg.Size = LastNoteSize;
            if (noteDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string viewOrderId = viewOrder.GetProperty("id", string.Empty);
                if (viewOrderId != string.Empty)
                {
                    viewOrder["mlsUrl"] = noteDlg.Notes;

                    viewOrderId = viewOrderId.Replace("-", string.Empty);
                    ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                                        "viewOrder/" + viewOrderId,
                                                                        "", viewOrder);
                }
                refreshOrders();
            }
        }

        private void composePromoEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientData viewOrder = ListViewOrders.SelectedItems[0].Tag as ClientData;
            string viewOrderId = viewOrder.GetProperty("id", string.Empty);
            EmailViewOrder email = EmailViewOrder.Create(viewOrderId,
                                                         null,
                                                         EmailViewOrder.Template.ListingPromotion);
            if (email == null)
                return;

            if (email.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                UpdateState();
            }
        }

        private void ListViewOrders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (ListViewOrders.SelectedItems.Count == 1)
            {
                ClientData viewOrder = ListViewOrders.SelectedItems[0].Tag as ClientData;
                String mlsUrl = viewOrder.GetProperty("mlsUrl", string.Empty);
                if (!string.IsNullOrEmpty(mlsUrl))
                    System.Diagnostics.Process.Start(mlsUrl);
            }
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            lastSelectedIndex = -1;
            FilterViewOrders();
        }

        private void deleteThisAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_rigthMouseAgent == null)
                return;

            if (MessageBox.Show("WARNING: By deleting account\r\n\r\n    " + 
                                m_rigthMouseAgent.PrimaryEmailAddress.ToUpper() + 
                                "\r\n\r\nyou'll remove all orders associated with it.\r\n\r\n\r\n" + 
                                "Do you really want to delete this Account?",
                                "Removing an account",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Hand) == DialogResult.Yes)
            {
                // first of all we delete all viewOrders on this account
                ListViewOrders.Items.Clear();
                viewOrdersOfCurrentUser = null;
                ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                                    "viewOrder",
                                                                    "userId=" + m_rigthMouseAgent.AutoID +
                                                                    "&verbose=true", null);
                ClientData orders = resp.Data;
                ClientData[] viewOrders = orders.GetNextLevelDataArray("viewOrders");

                foreach (ClientData order in viewOrders)
                {
                    if (order.GetProperty("deleted", false))
                        continue;

                    string viewOrderId = order.GetProperty("id", string.Empty);
                    viewOrderId = viewOrderId.Replace("-", string.Empty);
                    resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Delete,
                                                        "viewOrder/" + viewOrderId, null, null);
                    if (HttpStatusCode.OK != resp.ResponseCode)
                    {
                        MessageBox.Show("Cannot delete a viewOrder!", "Deleting ViewOrders",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }

                // then we delete the account
                // https://vrt.3dcondox.com/data/user/<userid>?sid=<SID>
                if (HttpStatusCode.OK != resp.ResponseCode)
                {
                    MessageBox.Show("Cannot delete this user!", "Deleting User", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                } else {
                    resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Delete,
                                                        "user/" + m_rigthMouseAgent.AutoID, null, null);

                    refreshUserAccounts();
                }
            }
        }

        private void contextMenuStripUserAccount_Opening(object sender, CancelEventArgs e)
        {
            if (m_rigthMouseAgent == null)
            {
                e.Cancel = true;
                return;
            }
        }

    }
}
