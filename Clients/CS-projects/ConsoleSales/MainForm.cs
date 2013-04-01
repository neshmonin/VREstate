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

namespace ConsoleSales
{
    public partial class MainForm : Form
    {
        #region SetWindowPos declarations
        /// <summary>
        ///     Special window handles
        /// </summary>
        public enum SpecialWindowHandles
        {
            // ReSharper disable InconsistentNaming
            /// <summary>
            ///     Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
            /// </summary>
            HWND_TOP = 0,
            /// <summary>
            ///     Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
            /// </summary>
            HWND_BOTTOM = 1,
            /// <summary>
            ///     Places the window at the top of the Z order.
            /// </summary>
            HWND_TOPMOST = -1,
            /// <summary>
            ///     Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
            /// </summary>
            HWND_NOTOPMOST = -2
            // ReSharper restore InconsistentNaming
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            ///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000,

            /// <summary>
            ///     Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            SWP_DEFERERASE = 0x2000,

            /// <summary>
            ///     Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            SWP_DRAWFRAME = 0x0020,

            /// <summary>
            ///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,

            /// <summary>
            ///     Hides the window.
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,

            /// <summary>
            ///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOACTIVATE = 0x0010,

            /// <summary>
            ///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,

            /// <summary>
            ///     Retains the current position (ignores X and Y parameters).
            /// </summary>
            SWP_NOMOVE = 0x0002,

            /// <summary>
            ///     Does not change the owner window's position in the Z order.
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,

            /// <summary>
            ///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            SWP_NOREDRAW = 0x0008,

            /// <summary>
            ///     Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            SWP_NOREPOSITION = 0x0200,

            /// <summary>
            ///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400,

            /// <summary>
            ///     Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            SWP_NOSIZE = 0x0001,

            /// <summary>
            ///     Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOZORDER = 0x0004,

            /// <summary>
            ///     Displays the window.
            /// </summary>
            SWP_SHOWWINDOW = 0x0040,

            // ReSharper restore InconsistentNaming
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        #endregion // SetWindowPos declarations

        #region OnScreenKeyboard
        private const uint SW_SHOW = 5;
        private const uint SW_HIDE = 0;
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion

        private ListViewColumnSorter lvwColumnSorter;
        private Dictionary<string, Building> m_buildings = new Dictionary<string, Building>();
        private bool startOnSecondary = Properties.Settings.Default.StartOnSecondaryMonitor;
        private string importFromPath = Properties.Settings.Default.ImportFromPath;
        private string exportToPath = Properties.Settings.Default.ExportToPath;
        SuperServer m_superServer;
        // the plug-in instance
        private Developer m_currDeveloper = null;
        private Site m_currSite = null;
        private Building m_currBldng = null;

        private List<ChangingSuite> m_selectedSuites = null;
        private bool m_ignoreSelChange = false;
        private bool m_ignoreSelStatuseChange = false;
        private bool m_ignoreShowViewChange = false;
        private static bool m_vrEstateAppIsRunning = false;

        public MainForm()
        {
            if (startOnSecondary && Screen.AllScreens.Length > 1)
            {
                Rectangle secondMonitor = Screen.AllScreens[1].WorkingArea;
                SetWindowPos(this.Handle, (IntPtr)SpecialWindowHandles.HWND_TOP,
                             secondMonitor.Left, secondMonitor.Top, secondMonitor.Width,
                             secondMonitor.Height, 0);
            }

            InitializeComponent();
            this.Text += " (" + Properties.Settings.Default.serverEndPoint + ")";
            TextWriterTraceListener log = null;
            string logPath = string.Empty;
            try
            {
                logPath = Path.Combine(exportToPath, Properties.Settings.Default.DeveloperName + ".log");
                log = new TextWriterTraceListener(File.AppendText(logPath));
                Trace.Listeners.Add(log);
            }
            catch (DirectoryNotFoundException) { }

            lvwColumnSorter = new ListViewColumnSorter();
            listViewSuites.ListViewItemSorter = lvwColumnSorter;

            if (log == null)
            {
                try
                {
                    logPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    logPath = Path.Combine(logPath, Properties.Settings.Default.DeveloperName + ".log");
                    log = new TextWriterTraceListener(File.AppendText(logPath));
                    Trace.Listeners.Add(log);
                }
                catch (UnauthorizedAccessException) { }
            }

            if (log == null)
            {
                try
                {
                    logPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    logPath = Path.Combine(logPath, Properties.Settings.Default.DeveloperName + ".log");
                    log = new TextWriterTraceListener(File.AppendText(logPath));
                    Trace.Listeners.Add(log);
                }
                catch (Exception)
                {
                    MessageBox.Show("Cannot open file to log your activities. \n"+
                                    "The default tracing mechanism will be using.",
                                    "Logging Problem", 
                                    MessageBoxButtons.OK, 
                                    MessageBoxIcon.Asterisk);
                }
            }

            Trace.WriteLine("===============================================================");
            Trace.WriteLine(DateTime.Now.ToString() +
                "> Console " + Properties.Settings.Default.DeveloperName + " started", "Info");
            Trace.Flush();

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
                if (!loadModel())
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
                LoadOnScreenKeyboard();
            }
        }

        private void LoadOnScreenKeyboard()
        {
            if (!Properties.Settings.Default.UseOnScreenKeyboard) return;

            Process process = getProcess("osk");
            if (process != null)
                return;

            process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "c:\\windows\\system32\\osk.exe";
            process.StartInfo.Arguments = "";
            process.StartInfo.WorkingDirectory = "c:\\";
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.RedirectStandardError = false;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.RedirectStandardOutput = false;
            try
            {
                process.Start(); // Start Onscreen Keyboard
            }
            catch {}
        }

        private bool loadModel()
        {
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
            comboDevelopers.Items.Clear();
            if (SuperServer.Developers.Count > 0)
            {
                foreach (var developer in SuperServer.Developers.Values)
                {
                    comboDevelopers.Items.Add(developer);
                }

                comboDevelopers.SelectedIndex = 0;
            }

            return true;
        }

        private bool tryConnect()
        {
            string ep = Properties.Settings.Default.serverEndPoint;
            int timeout = Properties.Settings.Default.serverRequestTimeoutSec;

            m_superServer = ChangingSuperServer.Create(ep, timeout);
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
            string role = Properties.Settings.Default.role; //"developeradmin"; // "salesperson";
            string developerId = Properties.Settings.Default.DeveloperId;

            if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password))
            {
                if (role == "superadmin")
                {
                    if (m_superServer.SuperadminLogin(login, password))
                    {
                        Trace.WriteLine(DateTime.Now.ToString() + "> Connected to server " +
                                        Properties.Settings.Default.serverEndPoint +
                                        ", Login=" + login +
                                        ", role=" + role +
                                        ", SID=" + ServerProxy.SID, "Info");
                        Trace.Flush();
                        return true;
                    }
                }
                else
                {
                    if (m_superServer.Login(login, password, role, developerId))
                    {
                        Trace.WriteLine(DateTime.Now.ToString() + "> Connected to server " +
                                        Properties.Settings.Default.serverEndPoint +
                                        ", Login=" + login +
                                        ", role=" + role +
                                        ", developerId=" + developerId +
                                        ", SID=" + ServerProxy.SID, "Info");
                        Trace.Flush();
                        return true;
                    }
                }
            }

            Login lf = new Login(ServerProxy.ServerEndpoint, login, role);
            while (DialogResult.OK == lf.ShowDialog())
            {
                role = lf.cbxLoginTypes.SelectedItem.ToString();
                login = lf.tbLogin.Text;
                password = lf.tbPassword.Text;

                if (role == "superadmin")
                {
                    if (m_superServer.SuperadminLogin(login, password))
                    {
                        Trace.WriteLine(DateTime.Now.ToString() + "> Connected to server " +
                                        Properties.Settings.Default.serverEndPoint +
                                        ", Login=" + login +
                                        ", role=" + role +
                                        ", SID=" + ServerProxy.SID, "Info");
                        Trace.Flush();
                        return true;
                    }
                }
                else
                {
                    if (m_superServer.Login(login,
                                          password,
                                          role,
                                          developerId))
                    {
                        Trace.WriteLine(DateTime.Now.ToString() + "> Connected to server " +
                                        Properties.Settings.Default.serverEndPoint +
                                        ", Login=" + login +
                                        ", role=" + role +
                                        ", developerId=" + developerId +
                                        ", SID=" + ServerProxy.SID, "Info");
                        Trace.Flush();
                        return true;
                    }
                    else
                    {
                        Trace.WriteLine(DateTime.Now.ToString() + "> Failed to connected to server " +
                                        Properties.Settings.Default.serverEndPoint +
                                        ", Login=" + login, "Error");
                        Trace.Flush();

                        MessageBox.Show("Unknown login or bad password.\r\nPlease try again.",
                                        Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            return false;
        }

        private void comboDevelopers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ChangingSuite.AtLeastOneChanged)
            {
                switch (MessageBox.Show("Do you want to save your changes before switching to another Developer?",
                                        "3D Kiosk Manager",
                                        MessageBoxButtons.YesNoCancel,
                                        MessageBoxIcon.Question))
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        saveChanges();
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        ChangingSuite.DiscardChanges();
                        break; // go ahead - change it!
                    default: // i.e. "Cancel"
                        return;
                }
            }

            comboDevelopers.Enabled = comboDevelopers.Items.Count > 1;

            updateApplyButton();

            comboSites.Enabled = true;
            comboSites.Items.Clear();
            m_currDeveloper = (Developer)comboDevelopers.SelectedItem;
            m_currDeveloper.PopulateSiteList();
            listViewSuites.Items.Clear();
            comboBuildings.Items.Clear();
            listViewSuites.Enabled = false;
            groupBox4.Enabled = false;
            groupSuiteInfo.Enabled = false;
            foreach (Site site in m_currDeveloper.Sites)
            {
                comboSites.Items.Add(site);
            }

            //comboSites.SelectedIndex = 0;
        }

        private void comboSites_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ChangingSuite.AtLeastOneChanged)
            {
                switch (MessageBox.Show("Do you want to save your changes before switching to different site?",
                                        "3D Kiosk Manager",
                                        MessageBoxButtons.YesNoCancel,
                                        MessageBoxIcon.Question))
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        saveChanges();
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        ChangingSuite.DiscardChanges();
                        break; // go ahead - change it!
                    default: // i.e. "Cancel"
                        return; 
                }
            }

            updateApplyButton();

            comboBuildings.Enabled = true;
            m_currSite = (Site)comboSites.SelectedItem;
            listViewSuites.Items.Clear(); 
            comboBuildings.Items.Clear();
            listViewSuites.Enabled = false;
            groupBox4.Enabled = false;
            groupSuiteInfo.Enabled = false;
            foreach (Building building in m_currSite.Buildings.Values)
            {
                comboBuildings.Items.Add(building);
            }
            Trace.WriteLine(DateTime.Now.ToString() +
                "> Site " + m_currSite.Name + " selected", "Info");
            Trace.Flush();

            comboBuildings.SelectedIndex = 0;
        }


        private void refreshFromServer()
        {
            if (ChangingSuite.AtLeastOneChanged)
            {
                switch (MessageBox.Show("Do you want to save your changes before switching to different building?",
                                        "3D Kiosk Manager",
                                        MessageBoxButtons.YesNoCancel,
                                        MessageBoxIcon.Question))
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        saveChanges();
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        ChangingSuite.DiscardChanges();
                        break; // go ahead - change it!
                    default: // i.e. "Cancel"
                        return; 
                }
            }

            updateApplyButton();

            listViewSuites.Enabled = true;
            groupBox4.Enabled = true;
            groupSuiteInfo.Enabled = true;
            ChangingSuite.Clear();
            populateBuilding();
        }

        private void comboBuildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshFromServer();
            if (m_currBldng != null)
            {
                Trace.WriteLine(DateTime.Now.ToString() +
                    "> Building " + m_currBldng.Name + " selected", "Info");
                Trace.Flush();
            }
        }

        private void populateBuilding()
        {
            listViewSuites.Items.Clear();
            listViewSuites.SelectedItems.Clear();
            m_currBldng = (Building)comboBuildings.SelectedItem;

            List<ChangingSuite> suitesList = new List<ChangingSuite>();
            foreach (ChangingSuite suite in m_currBldng.Suites.Values)
            {
                suitesList.Add(suite);
            }

            ChangingSuite.MergeChanges(suitesList);

            suitesList.Sort();
            foreach (ChangingSuite cs in suitesList)
            {
                ListViewItem lvItem = new ListViewItem(cs.ToStringArray());
                lvItem.Tag = cs;
                lvItem.UseItemStyleForSubItems = false;

                for (int i = 0; i < lvItem.SubItems.Count; i++)
                {
                    switch (i)
                    {
                        case 3: // CellingHeight
                            lvItem.SubItems[i].BackColor = cs.CellingHeightChanged ?
                                Color.LightSalmon:
                                Color.White;
                            break;
                        case 4: // CurrentPrice
                            lvItem.SubItems[i].BackColor = cs.PriceChanged ?
                                Color.LightSalmon :
                                Color.White;
                            break;
                        case 5: // Status
                            lvItem.SubItems[i].BackColor = cs.StatusChanged ?
                                Color.LightSalmon :
                                Color.White;
                            break;
                        case 6: // ShowPanoramicView
                            lvItem.SubItems[i].BackColor = cs.ShowPanoramicViewChanged ?
                                Color.LightSalmon :
                                Color.White;
                            break;
                    }
                }
                
                listViewSuites.Items.Add(lvItem);
            }
        }

        private void listViewSuites_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateMultSuiteSelection();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (!ChangingSuite.AtLeastOneChanged)
            {
                KillProcess("osk");
                Application.Exit();
                return;
            }

            switch (MessageBox.Show("Do you want to save your changes before exiting?",
                                    "3D Kiosk Manager",
                                    MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Question))
            {
                case System.Windows.Forms.DialogResult.Yes:
                    saveChanges();
                    KillProcess("osk");
                    Application.Exit();
                    break;
                case System.Windows.Forms.DialogResult.No:
                    KillProcess("osk");
                    Application.Exit();
                    break;
                default: // i.e. "Cancel"
                    break; // do nothing
            }
        }

        private void applySuiteChanges()
        {
            bool guiChanged = false;
            m_selectedSuites = new List<ChangingSuite>();
            foreach (ListViewItem s in listViewSuites.SelectedItems)
                m_selectedSuites.Add(s.Tag as ChangingSuite);
            foreach (var suite in m_selectedSuites)
            {
                if (checkBoxShowPanoramicView.CheckState != CheckState.Indeterminate)
                {
                    guiChanged = true;
                    suite.suite.ShowPanoramicView = checkBoxShowPanoramicView.Checked;
                }

                try
                {
                    if (textCellingHeight.Text != string.Empty)
                    {
                        guiChanged = true;
                        suite.suite.CellingHeight = int.Parse(textCellingHeight.Text);
                    }
                }
                catch (System.FormatException) { }

                try
                {
                    if (textPrice.Text != string.Empty/* &&
                        textPrice.Text != "0"*/)
                    {
                        guiChanged = true;
                        suite.suite.Price = double.Parse(textPrice.Text);
                    }
                }
                catch (System.FormatException) { }

                if (comboSaleStatus.SelectedIndex != -1)
                {
                    guiChanged = true;
                    suite.suite.Status = (Suite.SaleStatus)comboSaleStatus.Items[comboSaleStatus.SelectedIndex];
                }

                if (guiChanged)
                {
                    ChangingSuite.checkIn(suite);
                    reflectInGUI(suite);
                }
            }
            updateApplyButton();
        }

        private void reflectInGUI(ChangingSuite suite)
        {
            m_ignoreSelChange = true;
            listViewSuites.BeginUpdate();
            foreach (ListViewItem lvi in listViewSuites.Items)
            {
                string[] texts = suite.ToStringArray();
                ChangingSuite item = lvi.Tag as ChangingSuite;
                if (item.Equals(suite))
                {
                    for (int i = 0; i < lvi.SubItems.Count; i++)
                    {
                        switch (i)
                        {
                            case 3: // CellingHeight
                                lvi.SubItems[i].BackColor = suite.CellingHeightChanged ?
                                    Color.LightSalmon :
                                    Color.White;
                                break;
                            case 4: // CurrentPrice
                                lvi.SubItems[i].BackColor = suite.PriceChanged ?
                                    Color.LightSalmon :
                                    Color.White;
                                break;
                            case 5: // Status
                                lvi.SubItems[i].BackColor = suite.StatusChanged ?
                                    Color.LightSalmon :
                                    Color.White;
                                break;
                            case 6: // ShowPanoramicView
                                lvi.SubItems[i].BackColor = suite.ShowPanoramicViewChanged ?
                                    Color.LightSalmon :
                                    Color.White;
                                break;
                        }
                        lvi.SubItems[i].Text = texts[i];
                    }

                    break;
                }
            }
            listViewSuites.EndUpdate();
            m_ignoreSelChange = false;
        }

        private void PopulateMultSuiteSelection()
        {
            if (m_ignoreSelChange)
                return;

            if (listViewSuites.SelectedItems.Count == 0)
            {
                comboSaleStatus.Items.Clear();
                labelSuiteType.Text = string.Empty;
                textBoxBedrooms.Text = string.Empty;
                textBoxBathrooms.Text = string.Empty;
                textBoxBalcony.Text = string.Empty;
                textBoxTerrace.Text = string.Empty;
                textBoxArea.Text = string.Empty;
                textCellingHeight.Text = string.Empty;
                textPrice.Text = string.Empty;
                m_ignoreShowViewChange = true;
                checkBoxShowPanoramicView.CheckState = CheckState.Indeterminate;
                m_ignoreShowViewChange = false;
                return;
            }

            comboSaleStatus.Items.Clear();
            m_selectedSuites = new List<ChangingSuite>();
            foreach (ListViewItem s in listViewSuites.SelectedItems) 
                m_selectedSuites.Add(s.Tag as ChangingSuite);

            string groupSuiteType = labelSuiteType.Text = m_selectedSuites.ElementAt(0).suite.ClassId;
            string groupBedroomse = textBoxBedrooms.Text = m_selectedSuites.ElementAt(0).suite.SuiteClass.Bedrooms;
            string groupBathrooms = textBoxBathrooms.Text = m_selectedSuites.ElementAt(0).suite.SuiteClass.Bathrooms;
            string groupBalcony = textBoxBalcony.Text = m_selectedSuites.ElementAt(0).suite.SuiteClass.Balcony;
            string groupTerrace = textBoxTerrace.Text = m_selectedSuites.ElementAt(0).suite.SuiteClass.Terrace;
            string groupArea = textBoxArea.Text = m_selectedSuites.ElementAt(0).suite.SuiteClass.Area;
            
            string groupCellingHeight = textCellingHeight.Text = m_selectedSuites.ElementAt(0).suite.CellingHeight.ToString();
            m_ignoreShowViewChange = true;
            string groupPrice = textPrice.Text = m_selectedSuites.ElementAt(0).suite.Price.ToString();
            checkBoxShowPanoramicView.Checked = m_selectedSuites.ElementAt(0).suite.ShowPanoramicView;
            if (checkBoxShowPanoramicView.Checked)
                checkBoxShowPanoramicView.CheckState = CheckState.Checked;
            else
                checkBoxShowPanoramicView.CheckState = CheckState.Unchecked;
            m_ignoreShowViewChange = false;

            List<Suite.SaleStatus> groupSaleStatuses = new List<Suite.SaleStatus>();
            groupSaleStatuses.AddRange(m_selectedSuites.ElementAt(0).suite.SaleStatuses);
            List<Suite.SaleStatus> groupSaleStatClone = new List<Suite.SaleStatus>(groupSaleStatuses);

            Suite.SaleStatus groupCurrSelectedSS = m_selectedSuites.ElementAt(0).suite.Status;

            foreach (var suite in m_selectedSuites)
            {
                if (groupSuiteType != suite.suite.ClassId) groupSuiteType = string.Empty;
                if (groupBedroomse != suite.suite.SuiteClass.Bedrooms) groupBedroomse = string.Empty;
                if (groupBathrooms != suite.suite.SuiteClass.Bathrooms) groupBathrooms = string.Empty;
                if (groupBalcony != suite.suite.SuiteClass.Balcony) groupBalcony = string.Empty;
                if (groupTerrace != suite.suite.SuiteClass.Terrace) groupTerrace = string.Empty;
                if (groupArea != suite.suite.SuiteClass.Area) groupArea = string.Empty;

                int groupCellingHeightD = 0;
                try
                {
                    if (!string.IsNullOrEmpty(groupCellingHeight))
                        groupCellingHeightD = int.Parse(groupCellingHeight);
                }
                catch { }
                
                if (groupCellingHeightD != suite.suite.CellingHeight)
                    groupCellingHeight = string.Empty;

                double groupPriceD = 0.0;
                try
                {
                    if (!string.IsNullOrEmpty(groupPrice))
                        groupPriceD = double.Parse(groupPrice);
                }
                catch { }

                double suitePrice = suite.suite.Price;
                if (groupPriceD != suitePrice)
                {
                    groupPrice = string.Empty;
                    m_ignoreShowViewChange = true;
                    textPrice.Text = string.Empty;
                    m_ignoreShowViewChange = false;
                }

                if (groupCurrSelectedSS != suite.suite.Status) groupCurrSelectedSS = (Suite.SaleStatus)(-1);
                if (checkBoxShowPanoramicView.Checked != suite.suite.ShowPanoramicView)
                {
                    m_ignoreShowViewChange = true;
                    checkBoxShowPanoramicView.CheckState = CheckState.Indeterminate;
                    m_ignoreShowViewChange = false;
                }

                foreach (Suite.SaleStatus grpSS in groupSaleStatClone)
                {
                    if (!suite.suite.SaleStatuses.Contains<Suite.SaleStatus>(grpSS))
                        groupSaleStatuses.Remove(grpSS);
                }
            }

            labelSuiteType.Text = groupSuiteType;

            textBoxBedrooms.Text = groupBedroomse;
            textBoxBathrooms.Text = groupBathrooms;
            textBoxBalcony.Text = groupBalcony;
            textBoxTerrace.Text = groupTerrace;
            textBoxArea.Text = groupArea;
            textCellingHeight.Text = groupCellingHeight;
            textPrice.Text = groupPrice;

            int i = 0;
            foreach (var stat in groupSaleStatuses)
            {
                comboSaleStatus.Items.Add(stat);
                if (groupCurrSelectedSS == (Suite.SaleStatus)stat)
                {
                    m_ignoreSelStatuseChange = true;
                    comboSaleStatus.SelectedIndex = i;
                    m_ignoreSelStatuseChange = false;
                }

                i++;
            }

            updateApplyButton();
        }

        private void updateApplyButton()
        {
            buttonApplyChanges.Enabled = ChangingSuite.AtLeastOneChanged;

            int totalChanges = ChangingSuite.HowManyChanges;
            string title = string.Empty;
            if (totalChanges == 0)
                title = "No Changes";
            else
                title = string.Format("Apply Changes\n({0} record{1})", 
                                      totalChanges.ToString(),
                                      (totalChanges==1)?"":"s");

            buttonApplyChanges.Text = title;
        }

        private void saveChanges()
        {
            if (!ChangingSuite.AtLeastOneChanged) return;

            string report = ChangingSuite.GenerateChangesReport(false);
            if (MessageBox.Show("The following changes have been done:\n\n" + 
                                report +
                                "\n\nDo you want to Appy these changes to the Server?",
                                "Applying your Changes",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
                return;


            Cursor.Current = Cursors.WaitCursor;
            Vre.Server.BusinessLogic.ClientData changes = ChangingSuite.GenerateClientData();

            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Update,
                                                              "building/" + m_currBldng.ID,
                                                              null,
                                                              changes);
            int svCount = 0;

            switch (resp.ResponseCode)
            {
                case HttpStatusCode.OK:
                    svCount = resp.Data.GetProperty("updated", 0);
                    break;

                default:
                    MessageBox.Show("Technical problem detected on server.\nPlease try again",
                                    "Serverside Problem",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                     return;
            }

            ChangingSuite.PromoteChanges(delegate(ChangingSuite s)
            {
                reflectInGUI(s);
                m_currBldng.Suites[s.suite.UniqueKey] = s.suite;
            });
            Trace.WriteLine(DateTime.Now.ToString() + "> Changes sent to server:\n" +
                report, "Info");
            Trace.Flush();

            listViewSuites.SelectedItems.Clear();
            updateApplyButton();
            Cursor.Current = Cursors.Default;
        }

        private string lastPrice;
        private void textPrice_Enter(object sender, EventArgs e)
        {
            LoadOnScreenKeyboard();
            lastPrice = textPrice.Text;
            textPrice.SelectAll();
        }
        private void textPrice_Leave(object sender, EventArgs e)
        {
            if (lastPrice != textPrice.Text)
            {
                if (string.IsNullOrWhiteSpace(textPrice.Text))
                    textPrice.Text = "0";
                applySuiteChanges();
            }
        }
        private void textPrice_TextChanged(object sender, EventArgs e)
        {
            if (m_ignoreShowViewChange)
                return;

            if (string.IsNullOrWhiteSpace(textPrice.Text))
                textPrice.Text = "0";
            applySuiteChanges();
        }
        private string lastCellingHeight;
        private void textCellingHeight_Enter(object sender, EventArgs e)
        {
            LoadOnScreenKeyboard();
            lastCellingHeight = textCellingHeight.Text;
            textCellingHeight.SelectAll();
        }
        private void textCellingHeight_Leave(object sender, EventArgs e)
        {
            if (lastCellingHeight != textCellingHeight.Text)
            {
                if (string.IsNullOrWhiteSpace(textCellingHeight.Text))
                    textCellingHeight.Text = "0";
                applySuiteChanges();
            }
        }
        private string lastFloorName;
        private string lastSuiteName;
        private int lastSaleStatus;
        private void comboSaleStatus_Enter(object sender, EventArgs e) { lastSaleStatus = comboSaleStatus.SelectedIndex; }
        private void comboSaleStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_ignoreSelStatuseChange)
                return;

            if (lastSaleStatus != comboSaleStatus.SelectedIndex)
            {
                applySuiteChanges();
                listViewSuites.Focus();
            }
        }

        private void checkBoxShowPanoramicView_CheckedChanged(object sender, EventArgs e)
        {
            if (m_ignoreShowViewChange)
                return;

            if (checkBoxShowPanoramicView.CheckState != CheckState.Indeterminate)
                applySuiteChanges();
        }

        public void OnTimerEvent(object source, EventArgs e)
        {
            m_vrEstateAppIsRunning = getProcess("vrestate") != null;
        }

        private static System.Diagnostics.Process getProcess(string exeName)
        {
            System.Diagnostics.Process[] pArry = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process p in pArry)
            {
                string s = p.ProcessName;
                s = s.ToLower();
                if (s.CompareTo(exeName) == 0)
                    return p;
            }
            return null;
        }
        private bool KillProcess(string exeName)
        {
            Process process = getProcess(exeName);
            if (process == null)
                return false;

            process.Kill();
            return true;
        }

        private void buttonExportToCSV_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(importFromPath))
                saveFileDialog.InitialDirectory = exportToPath;
            else
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Displays a SaveFileDialog
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv|Text file (*.txt)|*.txt";
            saveFileDialog.Title = "Save an Export File";
            saveFileDialog.FileName = m_currSite.Name + " - " + DateTime.Now.ToLongDateString();
            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog.FileName != string.Empty)
            {
                string ext = Path.GetExtension(saveFileDialog.FileName);
                if (ext == string.Empty)
                {
                    switch (saveFileDialog.FilterIndex)
                    {
                        case 1:
                            ext = "csv";
                            break;
                        case 2:
                            ext = "txt";
                            break;
                    }

                    Path.ChangeExtension(saveFileDialog.FileName, ext);
                }
                // Save the Export
                FileStream stream = File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                using (StreamWriter writeFile = new StreamWriter(stream))
                {
                    string outStr = string.Empty;
                    foreach (var building in m_currSite.Buildings.Values)
                    {
                        outStr += building.ToCSV();
                    }

                    writeFile.Write(outStr);
                }
            }
        }

        private void buttonUpdateFromCSV_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(importFromPath))
                openFileDialog.InitialDirectory = importFromPath;
            else
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            openFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string import = string.Empty;
            try
            {
                FileStream stream = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                using (StreamReader reader = new StreamReader(stream))
                {
                    import = reader.ReadToEnd();
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Cannot access file " + openFileDialog.FileName, "CSV Import",
                                MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            //foreach (var building in m_currSite.Buildings.Values)
            //{
            //    building.FromCSV(import);
            //}
            m_currBldng.FromCSV(import);

            if (ChangingSuite.AtLeastOneChanged)
                populateBuilding();

            updateApplyButton();
        }

        private void buttonApplyChanges_Click(object sender, EventArgs e)
        {
            saveChanges();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            refreshFromServer();
        }

        private void restoreOriginalValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (var c in m_selectedSuites)
            {
                c.DiscardChange();
                ChangingSuite.checkIn(c);
                reflectInGUI(c);
            }

            PopulateMultSuiteSelection();

            Cursor.Current = Cursors.Default;
        }

        private void contextMenuUnit_Opening(object sender, CancelEventArgs e)
        {
            if (m_selectedSuites.Count < 1)
            {
                e.Cancel = true;
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            bool cancel = true;
            foreach (var c in m_selectedSuites)
            {
                if (c.changed)
                    cancel = false;
            }

            e.Cancel = cancel;
            Cursor.Current = Cursors.Default;
        }

        private void listViewSuites_ColumnClick(object sender, ColumnClickEventArgs e)
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
            listViewSuites.Sort();
        }
    }
}
