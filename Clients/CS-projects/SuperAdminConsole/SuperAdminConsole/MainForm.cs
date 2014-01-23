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
        private Developer m_currentDeveloper = null;
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

        LoginForm loginForm = null;
        private bool doLogin(string login, string password)
        {
            // attempt auto-login once with credentials passed
            //if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password))
            //{
            //    if (m_superServer.SuperadminLogin(login, password))
            //    {
            //        Trace.WriteLine(DateTime.Now.ToString() + "> Connected to server " +
            //                        Properties.Settings.Default.serverEndPoint +
            //                        ", Login=" + login +
            //                        ", SID=" + ServerProxy.SID, "Info");
            //        Trace.Flush();
            //        return true;
            //    }
            //}

            loginForm = new LoginForm(login);
            while (DialogResult.OK == loginForm.ShowDialog(this))
            {
                if (loginForm.IsMainServer)
                    ServerProxy.ServerEndpoint = Properties.Settings.Default.serverEndPoint;
                else
                    ServerProxy.ServerEndpoint = Properties.Settings.Default.serverEndPoint + "vre/"; 
                
                login = loginForm.tbLogin.Text;
                password = loginForm.tbPassword.Text;
                myRole = loginForm.IsSuperAdmin ? "SuperAdmin" : "Admin";

                if (loginForm.IsSuperAdmin)
                {
                    if (m_superServer.SuperadminLogin(login, password))
                    {
                        Trace.WriteLine(DateTime.Now.ToString() + "> Connected to server " +
                                        Properties.Settings.Default.serverEndPoint +
                                        ", Login=" + login +
                                        ", SID=" + ServerProxy.SID, "Info");
                        Trace.Flush();
                        comboBoxEstateDeveloper.Items.Clear();
                        if (SuperServer.Developers.Count > 0)
                        {
                            foreach (var developer in SuperServer.Developers.Values)
                                comboBoxEstateDeveloper.Items.Add(developer);

                            comboBoxEstateDeveloper.SelectedItem = SuperServer.Developers["Resale"];
                        }

                        return true;
                    }
                }
                else
                {
                    string developerId = loginForm.textBoxEstateDeveloper.Text;
                    if (m_superServer.Login(login, password, "developeradmin", developerId))
                    {
                        Trace.WriteLine(DateTime.Now.ToString() + "> Connected to server " +
                                        Properties.Settings.Default.serverEndPoint +
                                        ", Login=" + login +
                                        ", role=developeradmin" +
                                        ", developerId=" + developerId +
                                        ", SID=" + ServerProxy.SID, "Info");
                        Trace.Flush();
                        comboBoxEstateDeveloper.Items.Add(SuperServer.Developers[developerId]);
                        comboBoxEstateDeveloper.SelectedIndex = 0;
                        return true;
                    }
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
                labelStreetName.Visible = treeViewAccounts.SelectedNode.Text == "ViewOrders" ||
                                          treeViewAccounts.SelectedNode.Text == "-MLS-";
                textBoxFilter.Visible = treeViewAccounts.SelectedNode.Text == "ViewOrders" ||
                                          treeViewAccounts.SelectedNode.Text == "-MLS-";
                if (treeViewAccounts.SelectedNode.Tag as UpdateableBase != null)
                {
                    if (treeViewAccounts.SelectedNode.Text == "-MLS-")
                        tabControlAccountProperty.SelectTab("tabPageViewOrders");
                    else
                        tabControlAccountProperty.SelectTab("tabPageInfo");
                }
                else
                {
                    switch (treeViewAccounts.SelectedNode.Text)
                    {
                        case "-MLS-":
                        case "ViewOrders":
                            tabControlAccountProperty.SelectTab("tabPageViewOrders");
                            break;
                        case "Banners":
                            tabControlAccountProperty.SelectTab("tabPageBanners");
                            break;
                        case "Logs":
                            tabControlAccountProperty.SelectTab("tabPageLogs");
                            break;
                        default:
                            tabControlAccountProperty.SelectTab("tabPageEmpty");
                            labelStreetName.Visible = false;
                            textBoxFilter.Visible = false;
                            break;
                    }
                }
            }
        }

        private void AddAccount_Click(object sender, EventArgs e)
        {
            Form newAccount = new NewAccount(myRole == "SuperAdmin");
            if (newAccount.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                refreshUserAccounts();
            }
        }

        private void buttonAddNewViewOrder_Click(object sender, EventArgs e)
        {
            SetViewOrder newViewOrder = new SetViewOrder(m_agent, 
                                                        null, 
                                                        SetViewOrder.ChangeReason.Creation,
                                                        m_currentDeveloper);
            newViewOrder.ShowDialog(this);
            refreshOrders();

            UpdateState();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            refreshUserAccounts();
        }

        public int MyID { private set; get; }

        string myRole;
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

            TreeNode topNode = new TreeNode(string.Format("SELF ({0})", myRole), ch);
            topNode.Tag = admin;
            treeViewAccounts.Nodes.Add(topNode);

            ///data/user?sid=<SID>[&genval=<generation>][&withdeleted={true|false}][&<hints>]
            resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                "user",
                                                "role=DeveloperAdmin&ed=" +
                                                m_currentDeveloper.Name, 
                                                null);
            if (HttpStatusCode.OK != resp.ResponseCode)
            {
                MessageBox.Show("Failed querying the list of customers");
                return;
            }

            ClientData adminsJASON = resp.Data;
            ClientData[] admins = adminsJASON.GetNextLevelDataArray("users");

            TreeNode[] adminNodes = new TreeNode[admins.Length-1];
            int i = 0;
            foreach (ClientData cd in admins)
            {
                User user = new User(cd);
                string nodeName = string.Empty;
                if (string.IsNullOrEmpty(user.NickName))
                    nodeName = string.Format("ADMIN <{0}>", user.AutoID);
                else
                {
                    if (user.NickName == "mlsImport")
                    {
                        nodeName = "-MLS-";
                        TreeNode mlsNode = new TreeNode(nodeName);
                        mlsNode.Tag = user;
                        treeViewAccounts.Nodes.Add(mlsNode);
                        continue;
                    }
                    else
                        nodeName = string.Format("{0}<{1}>", user.NickName, user.AutoID);
                }
                //TreeNode[] children = new TreeNode[3] { new TreeNode("ViewOrders"),
                //                                        new TreeNode("Banners"),
                //                                        new TreeNode("Logs") };
                TreeNode[] children = new TreeNode[2] { new TreeNode("ViewOrders"),
                                                    new TreeNode("Logs") };
                adminNodes[i] = new TreeNode(nodeName, children);
                adminNodes[i].Tag = user;
                i++;
            }
            topNode = new TreeNode(string.Format("Dev Admins ({0})", admins.Length), adminNodes);
            treeViewAccounts.Nodes.Add(topNode);

            ///data/user?sid=<SID>[&genval=<generation>][&withdeleted={true|false}][&<hints>]
            resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                               "user",
                                               "role=sellingagent",
                                               null);
            if (HttpStatusCode.OK != resp.ResponseCode)
            {
                MessageBox.Show("Failed querying the list of customers");
                return;
            }

            ClientData usersJSON = resp.Data;
            ClientData[] users = usersJSON.GetNextLevelDataArray("users");

            TreeNode[] userNodes = new TreeNode[users.Length];
            i = 0;
            foreach (ClientData cd in users)
            {
                User user = new User(cd);
                string nodeName = string.Format("{0} <{1}> {2}", user.AutoID, user.NickName, user.PrimaryEmailAddress);
                //TreeNode[] children = new TreeNode[3] { new TreeNode("ViewOrders"),
                //                                        new TreeNode("Banners"),
                //                                        new TreeNode("Logs") };
                TreeNode[] children = new TreeNode[2] { new TreeNode("ViewOrders"),
                                                        new TreeNode("Logs") };
                userNodes[i] = new TreeNode(nodeName, children);
                userNodes[i].Tag = user;
                i++;
            }
            topNode = new TreeNode(string.Format("Selling Agents ({0})", users.Length), userNodes);
            treeViewAccounts.Nodes.Add(topNode);

            resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                               "user",
                                               "role=anonymous",
                                               null);
            if (HttpStatusCode.OK != resp.ResponseCode)
            {
                MessageBox.Show("Failed querying the list of customers");
                return;
            }

            usersJSON = resp.Data;
            users = usersJSON.GetNextLevelDataArray("users");

            userNodes = new TreeNode[users.Length];
            i = 0;
            foreach (ClientData cd in users)
            {
                User user = new User(cd);
                string nodeName = string.Format("<{0}> {1}", user.AutoID, user.PrimaryEmailAddress);
                TreeNode[] children = new TreeNode[2] { new TreeNode("ViewOrders"),
                                                        new TreeNode("Logs")  };
                userNodes[i] = new TreeNode(nodeName, children);
                userNodes[i].Tag = user;
                i++;
            }
            topNode = new TreeNode(string.Format("Anonymous Users ({0})", users.Length), userNodes);
            treeViewAccounts.Nodes.Add(topNode);


            resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                               "brokerage",
                                               "",
                                               null);
            if (HttpStatusCode.OK != resp.ResponseCode)
            {
                MessageBox.Show("Failed querying the list of brokerages");
                return;
            }

            ClientData brokerageJASON = resp.Data;
            ClientData[] brokerages = brokerageJASON.GetNextLevelDataArray("brokerages");

            TreeNode[] brokerageNodes = new TreeNode[brokerages.Length];
            i = 0;
            foreach (ClientData cd in brokerages)
            {
                BrokerageInfo brokerage = new BrokerageInfo(cd);
                string nodeName = string.Format("<{0}> {1} (?)", brokerage.AutoID, brokerage.Name);
                TreeNode overwritable = new TreeNode("expand");
                brokerageNodes[i] = new TreeNode(nodeName, new TreeNode[1]{overwritable});
                brokerageNodes[i].Tag = brokerage;
                i++;
            }
            topNode = new TreeNode(string.Format("Brokerages ({0})", brokerages.Length), brokerageNodes);
            treeViewAccounts.Nodes.Add(topNode);

            //TreeNode[] lastch = new TreeNode[1] { new TreeNode("ViewOrders") };
            //TreeNode lastNode = new TreeNode("ANONIMOUS", lastch);
            //lastNode.Tag = new User(m_currentDeveloper.ID, User.Role.Anonymous);
            //treeViewAccounts.Nodes.Add(lastNode);

            UpdateState();
        }

        private User m_agent = null;
        private User m_rigthMouseAgent = null;
        private BrokerageInfo m_brokerage = null;

        private void treeViewAccounts_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode rigthMouseNode = treeViewAccounts.GetNodeAt(e.X, e.Y);
                if (rigthMouseNode != null && rigthMouseNode.Tag != null)
                {
                    m_rigthMouseAgent = rigthMouseNode.Tag as User;
                    return;
                }
            }
            m_rigthMouseAgent = null;
        }


        private void treeViewAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            m_agent = null;
            if (e.Node.Tag != null)
            {
                m_agent = e.Node.Tag as User;
                m_brokerage = e.Node.Tag as BrokerageInfo;
            }
            else
            if (e.Node.Parent != null && e.Node.Parent.Tag != null)
            {
                m_agent = e.Node.Parent.Tag as User;
                m_brokerage = e.Node.Tag as BrokerageInfo;
            }

            if (m_agent != null)
            {
                m_email = m_agent.PrimaryEmailAddress == string.Empty?"no email":m_agent.PrimaryEmailAddress;
                m_nickname = m_agent.NickName == string.Empty ? myRole : m_agent.NickName;
            }

            if (e.Node.Text == "ViewOrders" || e.Node.Text == "-MLS-")
                refreshOrders();
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
                    //    theObject = View"                                           string
                    //    target = Suite"                                           string
                    //    targetId = 8564                                           int
                    //    extraTargetInfo = 96D7DB20-DD22-4F66-B153-C7053484ED3C    string
                    string[] subitems = new string[7];
                    DateTime time = log.GetProperty("created", DateTime.Now);
                    subitems[0] = time.Date.ToShortDateString() + ", " + time.ToLongTimeString();
                    subitems[1] = log.GetProperty("paymentSystem", string.Empty) == "CondoExplorer" ? "CE" : "PP";
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
            else
            {
                listViewAccountInfo.Items.Clear();
                UpdateableBase item = e.Node.Tag as UpdateableBase;
                if (item != null)
                {
                    ClientData cd = item.GetClientData();
                    foreach (var prop in cd)
                    {
                        if (prop.Key == "version" || prop.Key == "estateDeveloperId")
                            continue;

                        string[] subitems = new string[2];
                        ListViewItem acctInfo = null;
                        subitems[0] = prop.Key.ToString();
                        if (prop.Value != null)
                        {
                            subitems[1] = prop.Value.ToString();
                            acctInfo = new ListViewItem(subitems);
                        }
                        else
                        {
                            subitems[0] = prop.Key.ToString() + "[]";
                            subitems[1] = "<empty>";
                            acctInfo = new ListViewItem(subitems);
                            //ClientData[] array = cd.GetNextLevelDataArray(prop.Key);
                            //if (array != null)
                            //{
                            //    if (array.Length == 0)
                            //    {
                            //        subitems[0] = prop.Key.ToString() + "[]";
                            //        subitems[1] = "<empty>";
                            //        acctInfo = new ListViewItem(subitems);
                            //    }
                            //    else
                            //    {
                            //        int i = 0;
                            //        foreach (var elt in array)
                            //        {

                            //        }
                            //    }
                            //}
                        }

                        listViewAccountInfo.Items.Add(acctInfo);
                    }
                }
                listViewLocalPP.Items.Clear();
                if (m_agent != null)
                    PopulatePricingPolicy(m_agent, "agent");
                else
                if (m_brokerage != null)
                    PopulatePricingPolicy(m_brokerage, "brokerage");


                listViewCurrentBalance.Items.Clear();
            }

            UpdateState();
        }

        private void PopulatePricingPolicy(UpdateableBase theObject, string subject)
        {
            if (theObject == null) return;

            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                                "pp",
                                                                "defaults=false&" +
                                                                "subject=" + subject + "&" +
                                                                "subjectid=" +
                                                                    theObject.AutoID.ToString(),
                                                                null);

            ClientData[] policies = resp.Data.GetNextLevelDataArray("policies");
            if (policies == null || policies.Length == 0)
            {
                listViewLocalPP.Visible = false;
                buttonDeleteLocalPP.Visible = false;
                buttonAddLocalPP.Visible = true;
                buttonAddLocalPP.Text = "Add Local Pricing Policy...";
                if (buttonAddLocalPP.Tag == null)
                {
                    buttonAddLocalPP.Tag = this;
                    buttonAddLocalPP.Click += new System.EventHandler(
                        delegate(object o, EventArgs evnt)
                        {
                            AddPricingPolicy(theObject, subject);
                        });
                }
            }
            else
            {
                listViewLocalPP.Visible = true;
                buttonDeleteLocalPP.Visible = true;
                buttonAddLocalPP.Visible = false;
                buttonDeleteLocalPP.Text = "Delete Local Pricing Policy";
                if (buttonDeleteLocalPP.Tag == null)
                {
                    buttonDeleteLocalPP.Tag = this;
                    buttonDeleteLocalPP.Click += new System.EventHandler(
                        delegate(object o, EventArgs evnt)
                        {
                            if (MessageBox.Show("Are you sure you want to delete local Pricing Policy for " +
                                theObject.ToString() + "?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                DeletePricingPolicy(theObject, subject);
                        });
                }
                foreach (ClientData policy in policies)
                {
                    string[] subitems = new string[2];
                    subitems[0] = policy.GetProperty("service", "<unknown>");
                    subitems[1] = policy.GetProperty("unitPrice", 0.00m).ToString();
                    ListViewItem pi = new ListViewItem(subitems);
                    listViewLocalPP.Items.Add(pi);
                }
            }
        }

        private void AddPricingPolicy(UpdateableBase theObject, string subject)
        {
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                                "pp",
                                                                "defaults=true&" +
                                                                "subject=" + subject + "&" +
                                                                "subjectid=0",
                                                                null);
            ClientData[] policies = resp.Data.GetNextLevelDataArray("policies");
            PricingPolicy[] pricingPolicies = new PricingPolicy[policies.Length];

            for (int j = 0; j < policies.Length; j++)
            {
                pricingPolicies[j] = new PricingPolicy(policies[j]);
                string[] subitems = new string[2];
                subitems[0] = policies[j].GetProperty("service", "<unknown>");
                subitems[1] = policies[j].GetProperty("unitPrice", 0.00m).ToString();
                ListViewItem pi = new ListViewItem(subitems);
                listViewLocalPP.Items.Add(pi);
            }
            PricingPolicy pp =
                new PricingPolicy(subject == "agent" ?
                                        PricingPolicy.SubjectType.Agent :
                                        PricingPolicy.SubjectType.Brokerage,
                                  theObject.AutoID,
                                  PricingPolicy.ServiceType.ActiveAgentMontly,
                                  5.00m);

            resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Insert,
                                                                "pp",
                                                                "subject=" + subject + "&" +
                                                                "subjectid=" +
                                                                    theObject.AutoID.ToString(),
                                                                pp.GetClientData());
            PopulatePricingPolicy(theObject, subject);
        }

        private void DeletePricingPolicy(UpdateableBase theObject, string subject)
        {
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Delete,
                                                                "pp",
                                                                "subject=" + subject + "&" +
                                                                "subjectid=" +
                                                                    theObject.AutoID.ToString(),
                                                                null);
            PopulatePricingPolicy(theObject, subject);
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
            ClientData data = null;
            if (m_agent != null)
                data = m_agent.GetClientData();
            else if (m_brokerage != null)
                data = m_brokerage.GetClientData();

            AccountPropertyForm editForm = new AccountPropertyForm(data, prop);
            if (editForm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                data[prop] = editForm.NewValue;

                ServerResponse resp = null;
                if (m_agent != null)
                    resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                       "user/" + m_agent.AutoID,
                                                       "", data);
                else
                    resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                       "brokerage/" + m_brokerage.AutoID,
                                                       "", data);

                if (HttpStatusCode.OK != resp.ResponseCode)
                {
                    MessageBox.Show("Failed updating property \'" + 
                                    prop + "\':\n" + 
                                    resp.ResponseCodeDescription);
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
                //vTourUrl = 
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
                    string aptNum;
                    string buildingAddr;
                    if (separatorIndex != -1)
                    {
                        aptNum = label.Substring(0, separatorIndex);
                        buildingAddr = label.Substring(separatorIndex + 3);
                    }
                    else
                    {
                        aptNum = "---";
                        buildingAddr = label;
                    }
                    subitems[0] = aptNum;
                    subitems[1] = buildingAddr;
                }

                switch (viewOrder.GetProperty<ViewOrder.ViewOrderProduct>("product", ViewOrder.ViewOrderProduct.PublicListing))
                {
                    case ViewOrder.ViewOrderProduct.PrivateListing:
                        subitems[2] = "PrL";
                        break;
                    case ViewOrder.ViewOrderProduct.PublicListing:
                        subitems[2] = "ShL";
                        break;
                    case ViewOrder.ViewOrderProduct.Building3DLayout:
                        subitems[2] = "3DL";
                        break;
                }
                DateTime expiredOn = viewOrder.GetProperty("expiresOn", DateTime.Now);
                subitems[3] = expiredOn.ToShortDateString();
                subitems[4] = viewOrder.GetProperty("mlsId", string.Empty);

                string vTourUrl = viewOrder.GetProperty("vTourUrl", string.Empty);
                if (vTourUrl != string.Empty)
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
            Cursor current = Cursor.Current;
            Cursor = Cursors.WaitCursor;
            if (ListViewOrders.SelectedIndices.Count > 0)
                lastSelectedIndex = ListViewOrders.SelectedIndices[0];

            //https://vrt.3dcondox.com/vre/data/viewOrder?
            //                                         userId=<selling theObject id>&
            //                                         sid=<SID>
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                                "viewOrder",
                                                                "userId=" + m_agent.AutoID +
                                                                "&ed=" +
                                                                m_currentDeveloper.Name +
                                                                "&verbose=true", null);

            Cursor = current;
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
            switch (viewOrder.GetProperty<ViewOrder.ViewOrderProduct>("product", ViewOrder.ViewOrderProduct.PublicListing))
            {
                case ViewOrder.ViewOrderProduct.PrivateListing:
                    toolStripMenuItemConvert.Visible = true;
                    toolStripMenuItemConvert.Text = "Convert to Shared Listing";
                    break;
                case ViewOrder.ViewOrderProduct.PublicListing:
                    toolStripMenuItemConvert.Visible = true;
                    toolStripMenuItemConvert.Text = "Convert to Private Listing";
                    break;
                default:
                    toolStripMenuItemConvert.Visible = false;
                    break;
            }
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
                // ClientData theObject = treeViewAccounts.SelectedNode.Tag as ClientData;

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

            //if (m_agent.UserRole != User.Role.SuperAdmin &&
            //    m_agent.UserRole != User.Role.DeveloperAdmin)
            //    return;

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

            //if (newUser.UserRole == User.Role.SuperAdmin ||
            //    newUser.UserRole == User.Role.DeveloperAdmin)
           // {
           //     e.Effect = DragDropEffects.None;
           //     return;
           // }

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

            SetViewOrder updateViewOrder = new SetViewOrder(m_agent, 
                                                            viewOrderToMove, 
                                                            SetViewOrder.ChangeReason.Transfer,
                                                            m_currentDeveloper);
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
                                                            SetViewOrder.ChangeReason.Update,
                                                            m_currentDeveloper);
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
            String infoUrl = viewOrder.GetProperty("infoUrl", string.Empty);
            string caption = "MLS URL for \'" + viewOrder.GetProperty("label", string.Empty) + "\'";
            Note noteDlg = new Note(caption, infoUrl, this);
            noteDlg.Location = LastNoteLocation;
            noteDlg.Size = LastNoteSize;
            if (noteDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string viewOrderId = viewOrder.GetProperty("id", string.Empty);
                if (viewOrderId != string.Empty)
                {
                    viewOrder["infoUrl"] = noteDlg.Notes;

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
                String infoUrl = viewOrder.GetProperty("infoUrl", string.Empty);
                if (!string.IsNullOrEmpty(infoUrl))
                    System.Diagnostics.Process.Start(infoUrl);
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

        private void comboBoxEstateDeveloper_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentDeveloper = comboBoxEstateDeveloper.SelectedItem as Developer;
            refreshUserAccounts();
        }

        private void copyURLToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientData viewOrder = ListViewOrders.SelectedItems[0].Tag as ClientData;
            Clipboard.SetText(viewOrder["viewOrder-url"] as string);
        }

        private void toolStripMenuItemConvert_Click(object sender, EventArgs e)
        {
            ClientData viewOrder = ListViewOrders.SelectedItems[0].Tag as ClientData;
            // https://vrt.3dcondox.com/vre/data/viewOrder/<viewOrder ID>?sid=<SID>
            //    (HTTP PUT, in JSON should be "enabled"="frue")
            switch (viewOrder.GetProperty<ViewOrder.ViewOrderProduct>("product", ViewOrder.ViewOrderProduct.PublicListing))
            {
                case ViewOrder.ViewOrderProduct.PrivateListing:
                    viewOrder["product"] = ViewOrder.ViewOrderProduct.PublicListing;
                    break;
                case ViewOrder.ViewOrderProduct.PublicListing:
                    viewOrder["product"] = ViewOrder.ViewOrderProduct.PrivateListing;
                    break;
                default:
                    toolStripMenuItemConvert.Visible = false;
                    break;
            }

            string viewOrderId = viewOrder.GetProperty("id", string.Empty);
            if (viewOrderId != string.Empty)
            {
                // ClientData theObject = treeViewAccounts.SelectedNode.Tag as ClientData;

                viewOrderId = viewOrderId.Replace("-", string.Empty);
                ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                                  "viewOrder/" + viewOrderId,
                                                                  "", viewOrder);
                refreshOrders();
            }
            UpdateState();
        }

        private void treeViewAccounts_AfterExpand(object sender, TreeViewEventArgs e)
        {
            BrokerageInfo brokerage = e.Node.Tag as BrokerageInfo;
            if (brokerage == null)
                return;

            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get,
                                                              "user",
                                                              "role=Agent&brokerageId=" + brokerage.AutoID,
                                                              null);
            ClientData usersJSON = resp.Data;
            ClientData[] users = usersJSON.GetNextLevelDataArray("users");
            e.Node.Text = string.Format("<{0}> {1} ({2})",
                brokerage.AutoID,
                brokerage.Name,
                users.Length);

            foreach (ClientData cd in users)
            {
                User user = new User(cd);
                string nodeName = string.Format("{0} <{1}> {2}",
                    user.AutoID,
                    user.NickName,
                    user.PrimaryEmailAddress);
                //TreeNode[] children = new TreeNode[3] { new TreeNode("ViewOrders"),
                //                                        new TreeNode("Banners"),
                //                                        new TreeNode("Logs") };
                TreeNode[] children = new TreeNode[2] { new TreeNode("ViewOrders"),
                                                        new TreeNode("Logs") };
                TreeNode userNode = new TreeNode(nodeName, children);
                userNode.Tag = user;
                TreeNode overwritable = e.Node.FirstNode;
                if (overwritable.Tag == null)
                    e.Node.Nodes.Remove(overwritable);

                e.Node.Nodes.Add(userNode);
            }
        }

    }
}
