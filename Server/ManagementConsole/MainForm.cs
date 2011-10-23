using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vre.Server.BusinessLogic;
using System.Diagnostics;
using Vre.Server.Dao;

namespace Vre.Server.ManagementConsole
{
    public partial class MainForm : Form
    {
        private const int NODE_TYPE_ESTATE_DEVELOPERS = 1;
        private const int NODE_TYPE_SUITE_TYPES = 2;
        private const int NODE_TYPE_BUILDINGS = 3;
        private const int NODE_TYPE_SUITES = 4;
        private const int NODE_TYPE_OPTIONS = 5;

        private bool _servicesStarted;
        private NHibernate.ISession _session;
        private User _administrativeSystemUser;
        private bool _inhibitNodeChangedTest = false;

        public MainForm()
        {
            _servicesStarted = false;
            _session = null;

            InitializeComponent();
            Cursor.Current = Cursors.WaitCursor;
            lblStartupShutdown.Text = "Please wait: connecting to database...";
            pnlStartupShutdown.BringToFront();
            pnlStartupShutdown.Dock = DockStyle.Fill;
            pnlStartupShutdown.Visible = true;
        }

        #region startup/shutdown/reload
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_servicesStarted)
            {
                if (!tryUnloadTreeView()) { e.Cancel = true; return; }

                Cursor saved = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                lblStartupShutdown.Text = "Please wait: shutting down...";
                pnlStartupShutdown.BringToFront();
                pnlStartupShutdown.Dock = DockStyle.Fill;
                pnlStartupShutdown.Visible = true;
                Update();

                try
                {
                    closeDbSession();

                    Vre.Server.StartupShutdown.PerformShutdown();
                    _servicesStarted = false;
                }
                finally
                {
                    Cursor.Current = saved;
                }
            }
        }

        private void tmrStartup_Tick(object sender, EventArgs e)
        {
            tmrStartup.Enabled = false;

            try
            {
                _servicesStarted = false;
                Vre.Server.StartupShutdown.PerformStartup(false);
                _servicesStarted = true;

                AppendLog("Conecting to " + NHibernateHelper.DisplayableConnectionString);

                openDbSession();
                loadTreeView();

                AppendLog("Conected to database");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to connect to database:\r\n" + ex.Message,
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                AppendLog("Unable to connect to database:\r\n" + ex.Message);
            }
            finally
            {
                pnlStartupShutdown.SendToBack();
                pnlStartupShutdown.Visible = false;
                Cursor.Current = Cursors.Default;
            }
        }

        private void reloadFromDb()
        {
            if (!tryUnloadTreeView()) return;

            tvStructure.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            Update();

            try
            {
                closeDbSession();
                openDbSession();
                loadTreeView();
            }
            finally
            {
                tvStructure.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void openDbSession()
        {
            _session = Vre.Server.NHibernateHelper.GetSession();
        }

        private void closeDbSession()
        {
            if (_session != null) { _session.Dispose(); _session = null; }
        }

        private void loadTreeView()
        {
            // TODO: Make this a login popup
            using (UserManager manager = new UserManager(_session))
            {
                _administrativeSystemUser = manager.Login(LoginType.Plain, "admin", "admin");
            }

            TreeNode root = new TreeNode("Estate Developers");
            root.Tag = NODE_TYPE_ESTATE_DEVELOPERS;
            root.ContextMenuStrip = mnuEstateDevelopers;
            EstateDeveloperProps props = new EstateDeveloperProps();

            using (Vre.Server.Dao.EstateDeveloperDao eddao = new Vre.Server.Dao.EstateDeveloperDao(_session))
            {
                foreach (var ed in eddao.GetAll())
                {
                    TreeNode edn = new TreeNode();
                    props.SetupNode(edn, ed);
                    edn.ContextMenuStrip = mnuEstateDeveloper;

                    root.Nodes.Add(edn);
                }
            }

            tvStructure.Nodes.Add(root);
            root.Expand();
        }

        private bool tryUnloadTreeView()
        {
            TreeViewCancelEventArgs tvea = new TreeViewCancelEventArgs(tvStructure.SelectedNode, false, TreeViewAction.Unknown);
            tvStructure_BeforeSelect(tvStructure, tvea);
            if (tvea.Cancel) return false;

            tvStructure.Nodes.Clear();

            return true;
        }
        #endregion

        public void AppendLog(string text)
        {
            tbLog.Text += text + "\r\n";
            tbLog.SelectionStart = tbLog.Text.Length - 1;
            tbLog.SelectionLength = 0;
        }

        #region lazy tree loading
        private void tvStructure_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            switch (e.Node.Level)
            {
                case 0:
                    expandSites(e);
                    break;

                case 1:
                    expandSiteProperties(e);
                    break;

                case 2:
                    expandLevel2(e);
                    break;

                case 3:
                    expandLevel3(e);
                    break;
            }
        }

        private void expandLevel3(TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag is Building)
            {
                foreach (TreeNode node in e.Node.Nodes)
                {
                    if (node.Nodes.Count > 0) continue;

                    TreeNode subnode;

                    subnode = new TreeNode("Suites");
                    subnode.Tag = NODE_TYPE_SUITES;
                    // TODO: Attach context menu
                    node.Nodes.Add(subnode);

                    subnode = new TreeNode("Options");
                    subnode.Tag = NODE_TYPE_OPTIONS;
                    // TODO: Attach context menu
                    node.Nodes.Add(subnode);
                }
            }
        }

        private void expandLevel2(TreeViewCancelEventArgs e)
        {
            foreach (TreeNode node in e.Node.Nodes)
            {
                if (node.Nodes.Count > 0) continue;

                if (node.Tag.Equals(NODE_TYPE_SUITE_TYPES))
                {
                    foreach (SuiteType st in ((Site)e.Node.Tag).SuiteTypes)
                    {
                        // TODO: Create by property page
                        TreeNode stn = new TreeNode(st.Name);
                        stn.Tag = st;
                        node.Nodes.Add(stn);
                    }
                }
                else if (node.Tag.Equals(NODE_TYPE_BUILDINGS))
                {
                    BuildingProps props = new BuildingProps();
                    foreach (Building b in ((Site)e.Node.Tag).Buildings)
                    {
                        TreeNode bn = new TreeNode();
                        props.SetupNode(bn, b);
                        bn.ContextMenuStrip = mnuBuilding;
                        node.Nodes.Add(bn);
                    }
                }
            }
        }

        private void expandSiteProperties(TreeViewCancelEventArgs e)
        {
            foreach (TreeNode node in e.Node.Nodes)
            {
                if (node.Nodes.Count > 0) continue;

                TreeNode subnode;

                subnode = new TreeNode("Suite types");
                subnode.Tag = NODE_TYPE_SUITE_TYPES;
                // TODO: Attach context menu
                node.Nodes.Add(subnode);

                subnode = new TreeNode("Buildings");
                subnode.Tag = NODE_TYPE_BUILDINGS;
                // TODO: Attach context menu
                node.Nodes.Add(subnode);
            }
        }

        private void expandSites(TreeViewCancelEventArgs e)
        {
            SiteProps props = new SiteProps();
            foreach (TreeNode node in e.Node.Nodes)
            {
                if (node.Nodes.Count > 0) continue;

                foreach (Site s in ((EstateDeveloper)node.Tag).Sites)
                {
                    TreeNode sn = new TreeNode();
                    props.SetupNode(sn, s);
                    // TODO: Attach context menu
                    //edn.ContextMenuStrip = mnuEstateDeveloper;
                    node.Nodes.Add(sn);
                }
            }
        }
        #endregion

        private void tvStructure_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (!_inhibitNodeChangedTest)
            {
                foreach (Control ctl in pnlPropPage.Controls)
                {
                    IPropertyPage pp = ctl as IPropertyPage;
                    if (pp != null)
                    {
                        if (pp.Changed)
                        {
                            switch (MessageBox.Show(
                                "Some values for " + pp.ObjectName + " have been changed.\r\nDo you want ot save changes?",
                                "Save changes - " + Text,
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                            {
                                case System.Windows.Forms.DialogResult.Yes:
                                    try
                                    {
                                        pp.Save(tvStructure.SelectedNode.Parent.Tag, _session);
                                        pp.SetupNode(tvStructure.SelectedNode);
                                    }
                                    catch (NHibernate.StaleObjectStateException)
                                    {
                                        MessageBox.Show(
                                            "Changes for " + pp.ObjectName + " have not been saved!\r\n" +
                                            "Concurent change from other user detected.\r\n" +
                                            "Please review updated values and try changing again.",
                                            "Save changes - " + Text,
                                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        _session.Refresh(tvStructure.SelectedNode.Tag);
                                        pp.SetObject(tvStructure.SelectedNode.Tag);
                                        pp.SetupNode(tvStructure.SelectedNode);
                                        e.Cancel = true;
                                        return;
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(
                                            "Changes for " + pp.ObjectName + " have not been saved!\r\n" +
                                            "Changes are lost.\r\n\r\n" + ex.Message,
                                            "Save changes - " + Text,
                                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    }
                                    break;

                                case System.Windows.Forms.DialogResult.No:
                                    {
                                        TreeNode node = tvStructure.SelectedNode;
                                        // Remove node in case it was new added node
                                        if (null == node.Tag)
                                        {
                                            _inhibitNodeChangedTest = true;
                                            node.Parent.Nodes.Remove(node);
                                            _inhibitNodeChangedTest = false;
                                        }
                                    }
                                    break;

                                case System.Windows.Forms.DialogResult.Cancel:
                                    e.Cancel = true;
                                    return;
                            }
                        }
                    }
                }
            }
            pnlPropPage.Controls.Clear();
        }

        private void tvStructure_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object o = e.Node.Tag;
            if (null == o) return;

            if (o is int)
            {
                switch ((int)o)
                {
                    case NODE_TYPE_BUILDINGS:
                        break;

                    case NODE_TYPE_ESTATE_DEVELOPERS:
                        break;

                    case NODE_TYPE_OPTIONS:
                        break;

                    case NODE_TYPE_SUITE_TYPES:
                        break;

                    case NODE_TYPE_SUITES:
                        break;

                    default:
                        break;
                }
            }
            else
            {
                IPropertyPage pp = PropertyPageFactory.GetPropertyPage(o.GetType());
                if (pp != null)
                {
                    pp.SetObject(o);
                    showPropertyPage(pp as Control);
                }
            }
        }

        private void showPropertyPage(Control ppg)
        {
            splitContainer.Panel2MinSize = ppg.MinimumSize.Width;
            pnlPropPage.Controls.Add(ppg);
            ppg.Dock = DockStyle.Fill;
        }

        private void tvStructure_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvStructure.Tag = e.Node;
        }

        private void miAddEstateDeveloper_Click(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode("???");
            tvStructure.SelectedNode.Nodes.Add(node);
            tvStructure.SelectedNode = node;

            EstateDeveloperProps edp = new EstateDeveloperProps();
            edp.SetObject(null);
            showPropertyPage(edp);
        }

        private void miAddSite_Click(object sender, EventArgs e)
        {

        }

        #region model file (KMZ) importing
        private void miImportModelToDeveloper_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = tvStructure.Tag as TreeNode;
            if (null == selectedNode) return;

            string newModelName = browseForModelFile();
            if (newModelName != null)
            {
                EstateDeveloper ed = selectedNode.Tag as EstateDeveloper;
                Debug.Assert(ed != null, "Invalid tree node selected!");

                Cursor.Current = Cursors.WaitCursor;
                Update();

                try
                {
                    doModelImport(newModelName, ed, null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Unable to import model:\r\n" + ex.Message,
                        "Model Import - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    AppendLog("Unable to import model:" + ex.Message);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void miImportModelToBuilding_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = tvStructure.Tag as TreeNode;
            if (null == selectedNode) return;

            string newModelName = browseForModelFile();
            if (newModelName != null)
            {
                Building b = selectedNode.Tag as Building;
                Debug.Assert(b != null, "Invalid tree node selected!");

                Cursor.Current = Cursors.WaitCursor;
                Update();

                try
                {
                    doModelImport(newModelName, b.ConstructionSite.Developer, b.ConstructionSite, b);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Unable to import model:\r\n" + ex.Message,
                        "Model Import - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    AppendLog("Unable to import model:" + ex.Message);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private string browseForModelFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DefaultExt = "kmz";
            ofd.DereferenceLinks = true;
            ofd.Filter = "KMZ files|*.kmz|All files|*.*";
            ofd.Multiselect = false;
            ofd.Title = "Select model file to import";
            if (DialogResult.OK == ofd.ShowDialog()) return ofd.FileName;
            else return null;
        }

        private void doModelImport(string modelFileName, EstateDeveloper dev, Site site, Building building)
        {
            //VrEstate.Kmz.DeveloperName = dev.Name;
            //VrEstate.Kmz.SiteName = "My Site";// site.Name;

            VrEstate.Site siteData;

            using (VrEstate.Kmz kmz = VrEstate.Kmz.Open(modelFileName, System.IO.FileAccess.Read))
            {
                VrEstate.Model.Setup(kmz.GetKmlDoc());
                siteData = new VrEstate.Site(kmz.GetColladaDoc());
            }

            using (Vre.Server.INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session))
            {
                site = dealWithImportedSite(dev, site, siteData);
                if (null == site) return;

                if (building != null)
                {
                    VrEstate.Building importing = null;
                    foreach (VrEstate.Building b in siteData.Buildings.Values)
                    {
                        if (b.Name.Trim().Equals(building.Name.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            importing = b;
                            break;
                        }
                    }

                    if (null == importing)
                    {
                        MessageBox.Show(
                            "Model file does not contain a building named '" + building.Name + "'.",
                            "Model Import - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    else
                    {
                        if (siteData.Buildings.Count > 1)
                        {
                            if (DialogResult.Cancel == MessageBox.Show(
                                "Model file contains more than one building;\r\ncontinue with importing a single building?",
                                "Model Import - " + Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                                return;
                        }
                        doImportBuilding(siteData, site, importing, building);
                    }

                }
                else
                {
                    foreach (VrEstate.Building b in siteData.Buildings.Values)
                    {
                        building = dealWithImportedBuilding(site, b.Name);
                        if (building != null) doImportBuilding(siteData, site, b, building);
                    }
                }
                _session.Refresh(site);

                tran.Commit();
            }
        }

        private Site dealWithImportedSite(EstateDeveloper dev, Site selected, VrEstate.Site importedSite)
        {
            Site result = null;
            bool createNewSite = false;

            if (selected != null)
            {
                if (!selected.Name.Trim().Equals(importedSite.Name.Trim(), StringComparison.InvariantCultureIgnoreCase))
                {
                    switch (MessageBox.Show(
                        "Selected site is '" + selected.Name + "' while model provides '" + importedSite.Name
                        + "'.\r\nUse selected site (override model's site)?",
                        "Model Import - " + Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        case DialogResult.Yes:
                            result = selected;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    result = selected;
                }
            }

            if (null == result)  // no site selected or selection cancelled
            {
                var sites = from s in dev.Sites
                            where s.Name.Trim().Equals(importedSite.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)
                            select s;
                if (1 == sites.Count())
                {
                    switch (MessageBox.Show(
                        "Site '" + importedSite.Name + "' already exists.\r\nUse existing site?"
                        + "\r\n\r\nWARNING: Answering 'NO' shall create a duplicate site with same name to be resolved manually.",
                        "Model Import - " + Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Yes:
                            result = sites.First();
                            break;

                        case DialogResult.No:
                            createNewSite = true;
                            break;

                        default:
                            break;
                    }
                }
                else if (0 == sites.Count())
                {
                    switch (MessageBox.Show(
                        "Site '" + importedSite.Name + "' does not exist.\r\nCreate new?",
                        "Model Import - " + Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.OK:
                            createNewSite = true;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    MessageBox.Show(
                        "ERROR:\r\nMultiple sites with same name (" + importedSite.Name
                        + ") exist.\r\nPlease fix names and try importing again.",
                        "Model Import - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    AppendLog("ERROR: Multiple sites with same name (" + importedSite.Name
                        + ") exist. Please fix names and try importing again.");
                }
            }  // no site selected or selection cancelled

            if ((null == result) && (createNewSite))
            {
                try
                {
                    result = new Site(dev, importedSite.Name);
                    //result.ExcursionModel;
                    //result.GenericInfoModel;
                    result.Location.Longitude = importedSite.Lon_d;
                    result.Location.Latitude = importedSite.Lat_d;
                    result.Location.Altitude = importedSite.Alt_m;
                    using (SiteDao dao = new SiteDao(_session)) dao.Create(result);
                    _session.Refresh(dev);
                }
                catch (Exception ex)
                {
                    result = null;
                    MessageBox.Show(
                        "ERROR: Creating site failed:\r\n" + ex.Message,
                        "Model Import - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    AppendLog("ERROR: Creating site failed:\r\n" + ex.Message);
                }
            }

            return result;
        }

        private Building dealWithImportedBuilding(Site site, string importedBuildingName)
        {
            Building result = null;
            bool createNewBuilding = false;

            if (site.Buildings != null)
            {
                var buildings = from b in site.Buildings
                                where b.Name.Trim().Equals(importedBuildingName.Trim(), StringComparison.InvariantCultureIgnoreCase)
                                select b;
                if (1 == buildings.Count())
                {
                    switch (MessageBox.Show(
                        "Building '" + importedBuildingName + "' already exists.\r\nOverwrite existing building?"
                        + "\r\n\r\nWARNING: Answering 'NO' shall create a duplicate building with same name to be resolved manually.",
                        "Model Import - " + Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Yes:
                            result = buildings.First();
                            break;

                        case DialogResult.No:
                            createNewBuilding = true;
                            break;

                        default:
                            break;
                    }
                }
                else if (0 == buildings.Count())
                {
                    createNewBuilding = true;
                }
                else
                {
                    MessageBox.Show(
                        "ERROR:\r\nMultiple buildings with same name (" + importedBuildingName
                        + ") exist.\r\nPlease fix names and try importing again.",
                        "Model Import - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    AppendLog("ERROR: Multiple buildings with same name (" + importedBuildingName
                        + ") exist. Please fix names and try importing again.");
                }
            }
            else
            {
                createNewBuilding = true;
            }

            if ((null == result) && (createNewBuilding))
            {
                try
                {
                    result = new Building(site, importedBuildingName);
                    using (BuildingDao dao = new BuildingDao(_session)) dao.Create(result);
                    _session.Refresh(site);
                }
                catch (Exception ex)
                {
                    result = null;
                    MessageBox.Show(
                        "ERROR: Creating building failed:\r\n" + ex.Message,
                        "Model Import - " + Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    AppendLog("ERROR: Creating building failed:\r\n" + ex.Message);
                }
            }

            return result;
        }

        private void doImportBuilding(VrEstate.Site model, Site siteReference, VrEstate.Building source, Building destination)
        {
            destination.Location.Longitude = source.Lon_d;
            destination.Location.Latitude = source.Lat_d;
            destination.Location.Altitude = source.Alt_m;

            int updatedCnt = 0, createdCnt = 0, deletedCnt = 0;

            using (SuiteDao dao = new SuiteDao(_session))
            {
                // delete possible existing suites
                //dao.DeleteSuites(destination);
                // prepopulate existing suite id list
                List<Suite> staleSuiteList = new List<Suite>(destination.Suites);

                foreach (VrEstate.Suite s in source.Suites.Values)
                {
                    int floor;
                    if (!int.TryParse(s.FloorNumber, out floor))
                    {
                        if (s.FloorNumber.Equals("PH", StringComparison.InvariantCultureIgnoreCase))  // penthouse
                            floor = 999998;
                        else if (s.FloorNumber.Equals("RG", StringComparison.InvariantCultureIgnoreCase))  // roof garden
                            floor = 999999;
                        else
                            throw new Exception("Floor number in model is not parsed: '" + s.FloorNumber + "'");
                    }

                    Suite suite = null;

                    var suites = from st in destination.Suites where st.SuiteName == s.Name select st;
                    if (1 == suites.Count())
                    {
                        // update existing suite
                        suite = suites.First();
                        staleSuiteList.Remove(suite);
                        updatedCnt++;
                    }
                    else
                    {
                        // if no such suite exists or multiple suites with this name exist
                        // - create new; existing ones shall be marked as deleted (staleSuiteList)
                        suite = new Suite(destination, floor, s.FloorNumber, s.Name);
                        destination.Suites.Add(suite);
                        createdCnt++;
                    }

                    switch (s.Status)
                    {
                        case VrEstate.Suite.SaleStatus.Available:
                            suite.Status = Suite.SalesStatus.Available;
                            break;

                        case VrEstate.Suite.SaleStatus.OnHold:
                            suite.Status = Suite.SalesStatus.OnHold;
                            break;

                        case VrEstate.Suite.SaleStatus.Sold:
                            suite.Status = Suite.SalesStatus.Sold;
                            break;

                        default:
                            throw new Exception("Suite status in model is not parsed: '" + s.Status + "'");
                    }

                    suite.Location.Longitude = s.Lon_d;
                    suite.Location.Latitude = s.Lat_d;
                    suite.Location.Altitude = s.Alt_m;
                    suite.Location.HorizontalHeading = s.Heading_d;
                    suite.CeilingHeight.SetValue(s.CellingHeight, ValueWithUM.Unit.Feet);
                    suite.ShowPanoramicView = s.ShowPanoramicView;

                    if (!dao.SafeCreateOrUpdate(suite))
                        throw new Exception("One of objects was updated by other user (0); please try again.");

                    bool suiteTypeResolved = false;
                    foreach (SuiteType st in siteReference.SuiteTypes)
                    {
                        if (st.Name.Equals(s.ClassId, StringComparison.InvariantCultureIgnoreCase))
                        {
                            suite.SuiteType = st;
                            dao.Update(suite);
                            suiteTypeResolved = true;
                            break;
                        }
                    }
                    if (!suiteTypeResolved)
                    {
                        SuiteType suiteType = new SuiteType(siteReference, s.ClassId);
                        // TODO: suiteType.Model
                        using (SuiteTypeDao stdao = new SuiteTypeDao(_session))
                        {
                            stdao.Create(suiteType);
                        }
                        suite.SuiteType = suiteType;
                        if (!dao.SafeUpdate(suite)) 
                            throw new Exception("One of objects was updated by other user (1); please try again.");
                    }

                    using (BuildingManager manager = new BuildingManager(_session))
                    {
                        if (!manager.SetSuitePrice(_administrativeSystemUser, suite, (float)s.Price))
                        {
                            throw new Exception("Cannot set suite price: value was updated by other user.");
                        }
                    }
                }

                // delete stale suites (not present in new model
                foreach (Suite suite in staleSuiteList)
                {
                    if (!dao.SafeDelete(suite))
                        throw new Exception("One of objects was updated by other user (1); please try again.");
                    deletedCnt++;
                }

                // refresh building state from database
                _session.Refresh(destination);

                // fixup floor levels
                //

                // gather floor numbers
                SortedSet<int> floors = new SortedSet<int>();
                foreach (Suite s in destination.Suites)
                    if (!floors.Contains(s.PhysicalLevelNumber)) floors.Add(s.PhysicalLevelNumber);

                // map to level numbers
                Dictionary<int, int> floor2level = new Dictionary<int, int>(floors.Count);
                for (int lvl = 0; lvl < floors.Count; lvl++) floor2level.Add(floors.ElementAt(lvl), lvl + 1);

                foreach (Suite s in destination.Suites)
                {
                    s.PhysicalLevelNumber = floor2level[s.PhysicalLevelNumber];
                    dao.Update(s);
                }
            }

            AppendLog(string.Format("Building {0} updated: {1} suites updated; {2} created; {3} deleted.",
                destination.ToString(), updatedCnt, createdCnt, deletedCnt));
        }
        #endregion
    }
}
