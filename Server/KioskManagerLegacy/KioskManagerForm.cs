using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VrEstate;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace KioskManager
{
    public partial class KioskManager : Form
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

        private Dictionary<string, Building> m_buildings = new Dictionary<string, Building>();
        private bool startOnSecondary = Properties.Settings.Default.StartOnSecondaryMonitor;
        private string importFromPath = Properties.Settings.Default.ImportFromPath;
        private string exportToPath = Properties.Settings.Default.ExportToPath;

        // the plug-in instance
        private SuperServer m_curServer = null;
        private Developer m_currDeveloper = null;
        private Site m_currSite = null;
        private Building m_currBldng = null;

        private List<Suite> m_selectedSuites = null;
        private bool m_documentChaned = false;
        private bool m_ignoreSelChange = false;
        private bool m_ignoreSelStatuseChange = false;
        private bool m_ignoreShowViewChange = false;
        private Timer m_timer = null;
        private static bool m_vrEstateAppIsRunning = false;

        public KioskManager()
        {
            if (startOnSecondary && Screen.AllScreens.Length > 1)
            {
                Rectangle secondMonitor = Screen.AllScreens[1].WorkingArea;
                SetWindowPos(this.Handle, (IntPtr)SpecialWindowHandles.HWND_TOP,
                             secondMonitor.Left, secondMonitor.Top, secondMonitor.Width,
                             secondMonitor.Height, 0);
            }

            Suite.ToStringSort = Suite.ToStringStyle.SortByFloor;

            InitializeComponent();
            SuperServer.WebRootLocalPath = Properties.Settings.Default.WebRootLocalPath;

            LoadOnScreenKeyboard();

            m_curServer = SuperServer.Login();
            loadModel();

            m_timer = new Timer();
            m_timer.Interval = 10000; // 10 seconds
            m_timer.Enabled = true;
            m_timer.Tick += new System.EventHandler(OnTimerEvent);
            m_timer.Start();
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

        private void loadModel()
        {
            comboDevelopers.Items.Clear();
            foreach (var dev in m_curServer.Developers)
                comboDevelopers.Items.Add(dev);

            comboDevelopers.SelectedIndex = 0;
        }

        private void comboDevelopers_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboSites.Enabled = true;
            m_currDeveloper = (Developer)comboDevelopers.SelectedItem;
            comboSites.Items.Clear();
            foreach (Site site in m_currDeveloper.Sites)
            {
                comboSites.Items.Add(site);
            }
        }

        private void comboSites_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBuildings.Enabled = true;
            m_currSite = (Site)comboSites.SelectedItem;
            comboBuildings.Items.Clear();
            foreach (Building building in m_currSite.Buildings.Values)
            {
                comboBuildings.Items.Add(building);
            }
        }

        private void comboBuildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxSuites.Enabled = true;
            groupBox2.Enabled = true;
            groupBox4.Enabled = true;
            populateBuilding();
        }

        private void populateBuilding()
        {
            listBoxSuites.Items.Clear();
            m_currBldng = (Building)comboBuildings.SelectedItem;

            Dictionary<string, Suite> suiteTypes = new Dictionary<string, Suite>();
            List<Suite> suitesList = new List<Suite>();
            foreach (Suite suite in m_currBldng.Suites.Values)
            {
                suitesList.Add(suite);

                foreach (var stat in suite.SaleStatuses)
                {
                    string suiteTypeName = suite.ClassId + "_" + stat.ToString();

                    if (suite.SaleStatuses.Count < 3 && !suiteTypes.ContainsKey(suiteTypeName))
                    {
                        suiteTypes.Add(suiteTypeName, suite);
                    }
                }
            }

            suitesList.Sort();
            listBoxSuites.Items.AddRange(suitesList.ToArray());

            var list = suiteTypes.Keys.ToList();
            list.Sort();
            Console.WriteLine("Building " + m_currBldng.Name);
            foreach (var key in list)
                Console.WriteLine("   {1}\t{2}\t{0}", key, suiteTypes[key].Name, suiteTypes[key].FloorNumber);
        }

        private void listBoxSuites_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateMultSuiteSelection();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (!m_documentChaned)
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
            m_selectedSuites = new List<Suite>();
            foreach (Suite s in listBoxSuites.SelectedItems) m_selectedSuites.Add(s);

            foreach (var suite in m_selectedSuites)
            {
                if (textSuiteName.Text != string.Empty)
                {
                    m_documentChaned = true;
                    suite.Name = textSuiteName.Text;
                }
                if (textFloorName.Text != string.Empty)
                {
                    m_documentChaned = true;
                    suite.FloorNumber = textFloorName.Text;
                }
                if (checkBoxShowPanoramicView.CheckState != CheckState.Indeterminate)
                {
                    m_documentChaned = true;
                    suite.ShowPanoramicView = checkBoxShowPanoramicView.Checked;
                }

                try
                {
                    if (textCellingHeight.Text != string.Empty)
                    {
                        m_documentChaned = true;
                        suite.CellingHeight = int.Parse(textCellingHeight.Text);
                    }
                }
                catch (System.FormatException) { }

                try
                {
                    if (textPrice.Text != string.Empty &&
                        textPrice.Text != "0")
                    {
                        m_documentChaned = true;
                        suite.Price = double.Parse(textPrice.Text);
                    }
                }
                catch (System.FormatException) { }

                if (comboSaleStatus.SelectedIndex != -1)
                {
                    m_documentChaned = true;
                    suite.Status = (Suite.SaleStatus)comboSaleStatus.Items[comboSaleStatus.SelectedIndex];
                }

                buttonSaveToModel.Enabled = m_documentChaned;

                //listBoxSuites.Update();
                if (m_documentChaned)
                {
                    m_ignoreSelChange = true;
                    int index = listBoxSuites.Items.IndexOf(suite);
                    listBoxSuites.Items.Remove(suite);
                    listBoxSuites.Items.Insert(index, suite);
                    listBoxSuites.SetSelected(index, true);
                    m_ignoreSelChange = false;
                }
            }
        }

        private void PopulateMultSuiteSelection()
        {
            if (m_ignoreSelChange)
                return;

            if (listBoxSuites.SelectedItems.Count == 0)
            {
                comboSaleStatus.Items.Clear();
                labelSuiteType.Text = string.Empty;
                textSuiteName.Text = string.Empty;
                textFloorName.Text = string.Empty;
                textCellingHeight.Text = string.Empty;
                textPrice.Text = string.Empty;
                m_ignoreShowViewChange = true;
                checkBoxShowPanoramicView.CheckState = CheckState.Indeterminate;
                m_ignoreShowViewChange = false;
                return;
            }

            comboSaleStatus.Items.Clear();
            m_selectedSuites = new List<Suite>();
            foreach (Suite s in listBoxSuites.SelectedItems) m_selectedSuites.Add(s);

            string groupSuiteType = labelSuiteType.Text = m_selectedSuites.ElementAt(0).ClassId;
            string groupSuiteName = textSuiteName.Text = m_selectedSuites.ElementAt(0).Name;
            string groupFloorName = textFloorName.Text = m_selectedSuites.ElementAt(0).FloorNumber;
            string groupCellingHeight = textCellingHeight.Text = m_selectedSuites.ElementAt(0).CellingHeight.ToString();
            m_ignoreShowViewChange = true;
            string groupPrice = textPrice.Text = m_selectedSuites.ElementAt(0).Price.ToString();
            checkBoxShowPanoramicView.Checked = m_selectedSuites.ElementAt(0).ShowPanoramicView;
            if (checkBoxShowPanoramicView.Checked)
                checkBoxShowPanoramicView.CheckState = CheckState.Checked;
            else
                checkBoxShowPanoramicView.CheckState = CheckState.Unchecked;
            m_ignoreShowViewChange = false;

            List<Suite.SaleStatus> groupSaleStatuses = new List<Suite.SaleStatus>();
            groupSaleStatuses.AddRange(m_selectedSuites.ElementAt(0).SaleStatuses);
            List<Suite.SaleStatus> groupSaleStatClone = new List<Suite.SaleStatus>(groupSaleStatuses);

            Suite.SaleStatus groupCurrSelectedSS = m_selectedSuites.ElementAt(0).Status;

            foreach (var suite in m_selectedSuites)
            {
                if (groupSuiteType != suite.ClassId) groupSuiteType = string.Empty;
                if (groupSuiteName != suite.Name) groupSuiteName = string.Empty;
                if (groupFloorName != suite.FloorNumber) groupFloorName = string.Empty;
                try { if (int.Parse(groupCellingHeight) != suite.CellingHeight) groupCellingHeight = string.Empty; } catch { }
                try { if (double.Parse(groupPrice) != suite.Price) groupPrice = string.Empty; } catch { }
                if (groupCurrSelectedSS != suite.Status) groupCurrSelectedSS = (Suite.SaleStatus)(-1);
                if (checkBoxShowPanoramicView.Checked != suite.ShowPanoramicView)
                {
                    m_ignoreShowViewChange = true;
                    checkBoxShowPanoramicView.CheckState = CheckState.Indeterminate;
                    m_ignoreShowViewChange = false;
                }

                foreach (Suite.SaleStatus grpSS in groupSaleStatClone)
                {
                    if (!suite.SaleStatuses.Contains<Suite.SaleStatus>(grpSS))
                        groupSaleStatuses.Remove(grpSS);
                }
            }

            labelSuiteType.Text = groupSuiteType;

            textSuiteName.Text = groupSuiteName;
            textFloorName.Text = groupFloorName;
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
        }

        private void saveChanges()
        {
            // Saving all accumulated changes back to the KMZ
            using (Kmz kmz = Kmz.Open("Model.kmz", System.IO.FileAccess.Read))
            {
                XmlDocument doc = kmz.GetColladaDoc();
                m_currSite.Save(ref doc);
                kmz.SaveColladaDoc(doc);
                Kmz.UpdateAndBackupKmz(kmz);
            }

            loadModel();
            m_documentChaned = false;
            buttonSaveToModel.Enabled = false;
        }

        private void buttonSaveToModel_Click(object sender, EventArgs e)
        {
            saveChanges();
            if (!m_vrEstateAppIsRunning) return;

            if (MessageBox.Show("Do you want to restart the Kiosk to immediately reflect your changes?",
                "Kiosk Admin", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                // Restart
                this.Cursor = Cursors.WaitCursor;
                KillVrEstateProcess();
                System.Threading.Thread.Sleep(5000);
                StartVrEstateProcess();
                this.Cursor = Cursors.Default;
            }
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
        private void textFloorName_Enter(object sender, EventArgs e) { lastFloorName = textFloorName.Text; }
        private void textFloorName_Leave(object sender, EventArgs e)
        {
            if (lastFloorName != textFloorName.Text)
                applySuiteChanges();
        }

        private string lastSuiteName;
        private void textSuiteName_Enter(object sender, EventArgs e) { lastSuiteName = textSuiteName.Text; }
        private void textSuiteName_Leave(object sender, EventArgs e)
        {
            if (lastSuiteName != textSuiteName.Text)
                applySuiteChanges();
        }

        private int lastSaleStatus;
        private void comboSaleStatus_Enter(object sender, EventArgs e) { lastSaleStatus = comboSaleStatus.SelectedIndex; }
        private void comboSaleStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_ignoreSelStatuseChange)
                return;

            if (lastSaleStatus != comboSaleStatus.SelectedIndex)
                applySuiteChanges();
        }

        private void checkBoxShowPanoramicView_CheckedChanged(object sender, EventArgs e)
        {
            if (m_ignoreShowViewChange)
                return;

            if (checkBoxShowPanoramicView.CheckState != CheckState.Indeterminate)
                applySuiteChanges();
        }

        private void radioButtonSortByType_CheckedChanged(object sender, EventArgs e)
        {
            if (Suite.ToStringSort == Suite.ToStringStyle.SortByType) return;

            Suite.ToStringSort = Suite.ToStringStyle.SortByType;
            populateBuilding();
        }

        private void radioButtonSortByFloor_CheckedChanged(object sender, EventArgs e)
        {
            if (Suite.ToStringSort == Suite.ToStringStyle.SortByFloor) return;

            Suite.ToStringSort = Suite.ToStringStyle.SortByFloor;
            populateBuilding();
        }

        private void radioButtonSortByStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (Suite.ToStringSort == Suite.ToStringStyle.SortByStatus) return;

            Suite.ToStringSort = Suite.ToStringStyle.SortByStatus;
            populateBuilding();
        }

        public void OnTimerEvent(object source, EventArgs e)
        {
            m_vrEstateAppIsRunning = getProcess("vrestate") != null;
            buttonStopApp.Enabled = m_vrEstateAppIsRunning;
            buttonStartApp.Enabled = !m_vrEstateAppIsRunning;
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

        private void KillVrEstateProcess()
        {
            KillProcess("vrestate");
            m_vrEstateAppIsRunning = false;
            buttonStopApp.Enabled = m_vrEstateAppIsRunning;
        }

        private void StartVrEstateProcess()
        {
            Process vrEstate = new System.Diagnostics.Process();

            vrEstate.StartInfo.FileName = Properties.Settings.Default.VrEstateExecutable;
            vrEstate.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(vrEstate.StartInfo.FileName);
            vrEstate.StartInfo.Arguments = "site=\"" + m_currSite.DirName +
                                        "\" dev=\"" + m_currDeveloper.Name;
            vrEstate.StartInfo.UseShellExecute = false;

            vrEstate.Start();
            vrEstate.ProcessorAffinity = (System.IntPtr)1;

            KillProcess("osk");

            m_vrEstateAppIsRunning = true;
            buttonStartApp.Enabled = !m_vrEstateAppIsRunning;
        }

        private void buttonStopApp_Click(object sender, EventArgs e)
        {
            KillVrEstateProcess();
        }

        private void buttonStartApp_Click(object sender, EventArgs e)
        {
            StartVrEstateProcess();
        }

        private void buttonExportToCSV_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv|Text file (*.txt)|*.txt";
            saveFileDialog.Title = "Save an Export File";
            saveFileDialog.InitialDirectory = exportToPath;
            saveFileDialog.FileName = DateTime.Now.ToLongDateString();
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
                        outStr += "# ----------------- " + building.Name + " -----------------,,,,,,\n";
                        outStr += "#Floor,#Suite,#Type,#Cellings,#Price,#Status,#Panoramic View\n";

                        var list = building.Suites.Values.ToList();
                        list.Sort();
                        foreach (var suite in list)
                        {
                            outStr += suite.ToCSV();
                        }
                    }

                    writeFile.Write(outStr);
                }
            }
        }

        private void buttonUpdateFromCSV_Click(object sender, EventArgs e)
        {
            openFileDialog.InitialDirectory = importFromPath;
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string import = string.Empty;
            FileStream stream = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
            using (StreamReader reader = new StreamReader(stream))
            {
                import = reader.ReadToEnd();
            }

            string[] split = import.Split('\n');
            foreach (var building in m_currSite.Buildings.Values)
            {
                foreach (var suite in building.Suites.Values)
                {
                    for (int i = 0; i < split.Length; i++ )
                    {
                        if (split[i] == string.Empty)
                            continue;

                        if (split[i].StartsWith("#"))
                        {
                            split[i] = string.Empty;
                            continue;
                        }

                        string uniqueKey = suite.UniqueKey;
                        if (suite.FromCSV(split[i]))
                        {
                            split[i] = string.Empty;
                            if (uniqueKey != suite.UniqueKey)
                            {
                                m_documentChaned = true;
                                buttonSaveToModel.Enabled = true;
                            }
                            break;
                        }
                    }
                }
            }
            if (m_documentChaned)
                populateBuilding();
        }
    }
}
