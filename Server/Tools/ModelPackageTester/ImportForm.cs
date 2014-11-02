using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vre.Server.Model.Kmz;
using Vre.Server;
using System.Collections.Generic;
using System.Threading;

namespace ModelPackageTester
{
    public partial class ImportForm : Form
    {
        private Kmz _model = null;
		private ModelImportSettings _settings;
        private string _modelFileName = null;
        private string _stiFileName = null;
        private string _floorPlanPath = null;
        private string _executablePath = null;
        private string _displayModelFileName = null;
        private string _overlayModelFileName = null;
        private string _poiModelFileName = null;
        private string _bubbleWebTemplateFileName = null;
        private string _bubbleKioskTemplateFileName = null;

		private Dictionary<Control, Cursor> _controlState1 = new Dictionary<Control,Cursor>();
		private Dictionary<Control, bool> _controlState2 = new Dictionary<Control, bool>();
		
		private void importThread(object param)
		{
			string result = string.Empty;
			bool positive = false;

			string logFileName = Path.Combine(
				Path.GetDirectoryName(_modelFileName),
				Path.GetFileNameWithoutExtension(_modelFileName) + ".import.log.txt");

			try
			{
				ProcessStartInfo psi = new ProcessStartInfo(_executablePath, param as string);
				psi.WorkingDirectory = _floorPlanPath;
				psi.Verb = "runas";  // trigger process rights elevation

				File.Delete(logFileName);
				Process p = Process.Start(psi);
				p.WaitForExit();

				if (0 == p.ExitCode)
				{
					using (FileStream fs = File.OpenRead(logFileName))
						using (StreamReader rdr = new StreamReader(fs))
							result = rdr.ReadToEnd();
					positive = true;
				}
				else
				{
					result = string.Format("Import tool failed: {0}", p.ExitCode);
				}
			}
			catch (Exception ex)
			{
				result = string.Format("Import tool run failed: {0}\r\n{1}",
					ex.Message, ex.StackTrace);
			}

			Invoke((Action)delegate { finishWithImport(result, positive); });
		}

		private void finishWithImport(string result, bool positive)
		{ 
            tbResults.Text = result;

			foreach (var kvp in _controlState1.Reverse()) kvp.Key.Cursor = kvp.Value;
			foreach (var kvp in _controlState2.Reverse()) kvp.Key.Enabled = kvp.Value;

			_controlState1.Clear();
			_controlState2.Clear();

			if (positive && cbAutoSaveSettings.Checked) saveImportSettings();
		}

		private static void saveChangeControls(Control root, 
			Dictionary<Control, Cursor> save1,
			Dictionary<Control, bool> save2, Cursor newValue)
		{
			foreach (var octl in root.Controls)
			{
				var ctl = octl as Control;
				if (null == ctl) continue;
				if (ctl.Controls.Count > 0) saveChangeControls(ctl, save1, save2, newValue);
				save1[ctl] = ctl.Cursor;
				save2[ctl] = ctl.Enabled;
				ctl.Cursor = newValue;
				ctl.Enabled = false;
			}
		}

        private void onNewModelFileName(object sender, string filename)
        {
            if (sender.Equals(btnDisplayModelBrowse) || sender.Equals(lblDisplayModelFile))
            {
                _displayModelFileName = filename;
                lblDisplayModelFile.Text = filename;
            }
            else if (sender.Equals(btnOverlayModelBrowse) || sender.Equals(lblOverlayModelFile))
            {
                _overlayModelFileName = filename;
                lblOverlayModelFile.Text = filename;
            }
            else if (sender.Equals(btnPoiModelBrowse) || sender.Equals(lblPoiModelFile))
            {
                _poiModelFileName = filename;
                lblPoiModelFile.Text = filename;
            }
            else if (sender.Equals(btnBubbleWebTemplateBrowse) || sender.Equals(lblBubbleWebTemplateFile))
            {
                _bubbleWebTemplateFileName = filename;
                lblBubbleWebTemplateFile.Text = filename;
            }
            else if (sender.Equals(btnBubbleKioskTemplateBrowse) || sender.Equals(lblBubbleKioskTemplateFile))
            {
                _bubbleKioskTemplateFileName = filename;
                lblBubbleKioskTemplateFile.Text = filename;
            }
        }

        private string buildImportComandLine()
        {
            StringBuilder commandLine = new StringBuilder("importmodel");

            commandLine.Append(" infomodel=");
            insertPath(_modelFileName, ref commandLine);

            if (_displayModelFileName != null)
            {
                commandLine.Append(" displaymodel=");
                insertPath(_displayModelFileName, ref commandLine);
            }

            if (_overlayModelFileName != null)
            {
                commandLine.Append(" overlaymodel=");
                insertPath(_overlayModelFileName, ref commandLine);
            }

            commandLine.Append(" sti=");
            insertPath(_stiFileName, ref commandLine);

            commandLine.Append(" ed=");
            insertPath(cbxDeveloper.Text.Trim(), ref commandLine);

            commandLine.Append(" site=");
            insertPath(tbSite.Text.Trim(), ref commandLine);

            if (cbSingleBuilding.Checked)
            {
                commandLine.Append(" asbuilding=true");
                commandLine.Append(" building=");
                insertPath((cbxBuildings.SelectedItem as string).Trim(), ref commandLine);

                commandLine.Append(" ad_sta=");
                insertPath(tbStreetAddress.Text.Trim(), ref commandLine);

                commandLine.Append(" ad_mu=");
                insertPath(tbMunicipality.Text.Trim(), ref commandLine);

                commandLine.Append(" ad_stpr=");
                insertPath(tbStateProvince.Text.Trim(), ref commandLine);

                commandLine.Append(" ad_po=");
                insertPath(tbPostalCode.Text.Trim(), ref commandLine);

                commandLine.Append(" ad_co=");
                insertPath(tbCountry.Text.Trim(), ref commandLine);

                if (_poiModelFileName != null)
                {
                    commandLine.Append(" poimodel=");
                    insertPath(_poiModelFileName, ref commandLine);
                }
            }

            if (_bubbleWebTemplateFileName != null)
            {
                commandLine.Append(" bubblewebtemplate=");
                insertPath(_bubbleWebTemplateFileName, ref commandLine);
            }

            if (_bubbleKioskTemplateFileName != null)
            {
                commandLine.Append(" bubblekiosktemplate=");
                insertPath(_bubbleKioskTemplateFileName, ref commandLine);
            }

            commandLine.AppendFormat(" buildingStatus={0}", cbxNewBuildingStatus.SelectedItem as string);

            commandLine.AppendFormat(" suiteStatus={0}", cbxNewSuiteStatus.SelectedItem as string);

			commandLine.AppendFormat(" currency={0}", cbxCurrency.SelectedItem as string);

			switch (cbxMeasurementSystem.SelectedIndex)
			{
				default:
				case 0:
					commandLine.AppendFormat(" heightunit={0}", _settings.HeightUnit.ToString());
					commandLine.AppendFormat(" floorareaunit={0}", _settings.FloorAreaUnit.ToString());
					break;

				case 1:
					commandLine.AppendFormat(" heightunit={0}", ModelImportSettings.LengthUnit.Feet.ToString());
					commandLine.AppendFormat(" floorareaunit={0}", ModelImportSettings.AreaUnit.SqFeet.ToString());
					break;

				case 2:
					commandLine.AppendFormat(" heightunit={0}", ModelImportSettings.LengthUnit.Meters.ToString());
					commandLine.AppendFormat(" floorareaunit={0}", ModelImportSettings.AreaUnit.SqMeters.ToString());
					break;
			}

	
			if (!cbDryRun.Checked)
				commandLine.AppendFormat(" mlsimport={0}", cbDoMlsImport.Checked ? "true" : "false");

            commandLine.AppendFormat(" dryrun={0}", cbDryRun.Checked ? "true" : "false");

			return commandLine.ToString();
        }

        private static void insertPath(string path, ref StringBuilder commandLine)
        {
            if (path.Contains(' ')) commandLine.AppendFormat("\"{0}\"", path);
            else commandLine.Append(path);
        }

        public ImportForm()
        {
            InitializeComponent();

            Icon = Properties.Resources.cloudservice;

            cbxNewSuiteStatus.Items.Clear();
            cbxNewSuiteStatus.Items.Add("Available");
            cbxNewSuiteStatus.Items.Add("OnHold");
            cbxNewSuiteStatus.Items.Add("Sold");
            cbxNewSuiteStatus.Items.Add("ResaleAvailable");
            cbxNewSuiteStatus.SelectedIndex = 2;

            cbxNewBuildingStatus.Items.Clear();
            cbxNewBuildingStatus.Items.Add("InProject");
            cbxNewBuildingStatus.Items.Add("Constructing");
            cbxNewBuildingStatus.Items.Add("Built");
            cbxNewBuildingStatus.Items.Add("Sold");
            cbxNewBuildingStatus.Items.Add("ResaleAvailable");
            cbxNewBuildingStatus.SelectedIndex = 2;

			cbxMeasurementSystem.Items.Clear();
			cbxMeasurementSystem.Items.Add("Mixed (config)");
			cbxMeasurementSystem.Items.Add("Imperial");
			cbxMeasurementSystem.Items.Add("Metric");
			cbxMeasurementSystem.SelectedIndex = 1;

			cbxCurrency.Items.Clear();
			cbxCurrency.Items.Add("CAD");
			cbxCurrency.Items.Add("RUB");
			cbxCurrency.SelectedIndex = 0;
		}

        public void Init(ModelImportSettings settings,
            string modelFileName, string stiFileName, string floorPlanPath, Kmz model,
            string executablePath, bool allowDrop, bool isProduction)
        {
			_settings = settings;

            _modelFileName = modelFileName;
            _stiFileName = stiFileName;
            _floorPlanPath = floorPlanPath;
            _model = model;
            _executablePath = executablePath;
            initInt(allowDrop, isProduction);

            string prop, path;

            prop = _settings.EstateDeveloperName;
            if (prop != null) selectComboItem(cbxDeveloper, prop);

            prop = _settings.SiteName;
            if (prop != null) tbSite.Text = prop;

            prop = _settings.DisplayModelFileName;
            if (prop != null)
            {
                path = Path.Combine(_settings.BasePath, prop);
                if (File.Exists(path))
                {
                    _displayModelFileName = path;
                    lblDisplayModelFile.Text = path;
                }
            }

            prop = _settings.OverlayModelFileName;
            if (prop != null)
            {
                path = Path.Combine(_settings.BasePath, prop);
                if (File.Exists(path))
                {
                    _overlayModelFileName = path;
                    lblOverlayModelFile.Text = path;
                }
            }

            prop = _settings.PoiModelFileName;
            if (prop != null)
            {
                path = Path.Combine(_settings.BasePath, prop);
                if (File.Exists(path))
                {
                    _poiModelFileName = path;
                    lblPoiModelFile.Text = path;
                }
            }

            prop = _settings.BubbleWebTemplateFileName;
            if (prop != null)
            {
                path = Path.Combine(_settings.BasePath, prop);
                if (File.Exists(path))
                {
                    _bubbleWebTemplateFileName = path;
                    lblBubbleWebTemplateFile.Text = path;
                }
            }

            prop = _settings.BubbleKioskTemplateFileName;
            if (prop != null)
            {
                path = Path.Combine(_settings.BasePath, prop);
                if (File.Exists(path))
                {
                    _bubbleKioskTemplateFileName = path;
                    lblBubbleKioskTemplateFile.Text = path;
                }
            }

            prop = _settings.NewBuildingStatusName;
            if (prop != null) selectComboItem(cbxNewBuildingStatus, prop);

            prop = _settings.NewSuiteStatusName;
            if (prop != null) selectComboItem(cbxNewSuiteStatus, prop);

			prop = _settings.Currency;
			if (prop != null) selectComboItem(cbxCurrency, prop);

			if ((_settings.FloorAreaUnit == ModelImportSettings.AreaUnit.SqFeet)
				&& (_settings.HeightUnit == ModelImportSettings.LengthUnit.Feet))
				cbxMeasurementSystem.SelectedIndex = 1;
			else if ((_settings.FloorAreaUnit == ModelImportSettings.AreaUnit.SqMeters)
				&& (_settings.HeightUnit == ModelImportSettings.LengthUnit.Meters))
				cbxMeasurementSystem.SelectedIndex = 2;
			else
				cbxMeasurementSystem.SelectedIndex = 0;

            if (_settings.ImportMode == ModelImportSettings.Mode.Building)
            {
                cbSingleBuilding.Checked = true;
                cbSingleBuilding_CheckedChanged(this, null);

				prop = _settings.BuildingToImportName;
				selectComboItem(cbxBuildings, prop);

                prop = _settings.BuildingStreetAddress;
                if (prop != null) tbStreetAddress.Text = prop;

                prop = _settings.BuildingMunicipality;
                if (prop != null) tbMunicipality.Text = prop;

                prop = _settings.BuildingStateProvince;
                if (prop != null) tbStateProvince.Text = prop;

                prop = _settings.BuildingPostalCode;
                if (prop != null) tbPostalCode.Text = prop;

                prop = _settings.BuildingCountry;
                if (prop != null) tbCountry.Text = prop;
            }
            else
            {
                cbSingleBuilding.Checked = false;
                cbSingleBuilding_CheckedChanged(this, null);
            }

            btnImport.Enabled = (cbxDeveloper.Text.Length > 0) && (tbSite.Text.Length > 0);
        }

        public void Init(string modelFileName, string stiFileName, string floorPlanPath, Kmz model,
            string executablePath, bool allowDrop, bool isProduction)
        {
			_settings = null;
            _modelFileName = modelFileName;
            _stiFileName = stiFileName;
            _floorPlanPath = floorPlanPath;
            _model = model;
            _executablePath = executablePath;
            initInt(allowDrop, isProduction);
        }

        private void initInt(bool allowDrop, bool isProduction)
        {
            string edListFile = Path.Combine(Path.GetDirectoryName(_executablePath), "estate-developer-list.txt");
            if (File.Exists(edListFile))
            {
                cbxDeveloper.Items.Clear();
                using (StreamReader rdr = File.OpenText(edListFile))
                {
                    while (!rdr.EndOfStream)
                    {
                        string line = rdr.ReadLine().Trim();
                        if (line.Length > 0) cbxDeveloper.Items.Add(line);
                    }
                }
                selectComboItem(cbxDeveloper, "Resale");
            }

            if (!allowDrop)
            {
                //AllowDrop = false;
                lblDisplayModelFile.Text = string.Empty;
                lblOverlayModelFile.Text = string.Empty;
                lblPoiModelFile.Text = string.Empty;
                lblBubbleWebTemplateFile.Text = string.Empty;
                lblDisplayModelFile.AllowDrop = false;
                lblOverlayModelFile.AllowDrop = false;
                lblPoiModelFile.AllowDrop = false;
                lblBubbleWebTemplateFile.AllowDrop = false;
            }

            if (isProduction)
            {
                Text = Text + " - PRODUCTION";
                ForeColor = SystemColors.HighlightText;
                BackColor = SystemColors.Highlight;
            }

            cbxBuildings.Items.Clear();
            foreach (Building b in _model.Model.Site.Buildings)
                cbxBuildings.Items.Add(b.Name);
        }

		private void saveImportSettings()
		{
			if (null == _settings)
			{
				var path = Path.GetDirectoryName(_modelFileName);
				var name = Path.GetFileName(path);  // gets name of last level folder 
				if (string.IsNullOrEmpty(name))
				{
					if (cbSingleBuilding.Checked) name = cbxBuildings.SelectedItem as string;
					else name = tbSite.Text.Trim();
				}
				_settings = new ModelImportSettings(Path.Combine(path, name + ".import.txt"));
			}

			string basePath = _settings.BasePath;

			_settings.ModelFileName = makeRelativePath(_modelFileName, basePath);
			_settings.SuiteTypeInfoFileName = makeRelativePath(_stiFileName, basePath);
			_settings.FloorplanDirectoryName = makeRelativePath(_floorPlanPath, basePath);

			_settings.EstateDeveloperName = cbxDeveloper.Text.Trim();
			_settings.SiteName = tbSite.Text.Trim();

			_settings.DisplayModelFileName = makeRelativePath(_displayModelFileName, basePath);
			_settings.OverlayModelFileName = makeRelativePath(_overlayModelFileName, basePath);
			_settings.PoiModelFileName = makeRelativePath(_poiModelFileName, basePath);
			_settings.BubbleWebTemplateFileName = makeRelativePath(_bubbleWebTemplateFileName, basePath);
			_settings.BubbleKioskTemplateFileName = makeRelativePath(_bubbleKioskTemplateFileName, basePath);
	
			_settings.NewBuildingStatusName = cbxNewBuildingStatus.SelectedItem as string;
			_settings.NewSuiteStatusName = cbxNewSuiteStatus.SelectedItem as string;

			_settings.Currency = cbxCurrency.SelectedItem as string;

			switch (cbxMeasurementSystem.SelectedIndex)
			{
				default:
				case 0:
					break;

				case 1:
					_settings.FloorAreaUnit = ModelImportSettings.AreaUnit.SqFeet;
					_settings.HeightUnit = ModelImportSettings.LengthUnit.Feet;
					break;

				case 2:
					_settings.FloorAreaUnit = ModelImportSettings.AreaUnit.SqMeters;
					_settings.HeightUnit = ModelImportSettings.LengthUnit.Meters;
					break;
			}

			if (cbSingleBuilding.Checked)
			{
				_settings.ImportMode = ModelImportSettings.Mode.Building;
				_settings.BuildingToImportName = cbxBuildings.SelectedItem as string;
				_settings.BuildingStreetAddress = tbStreetAddress.Text.Trim();
				_settings.BuildingMunicipality = tbMunicipality.Text.Trim();
				_settings.BuildingStateProvince = tbStateProvince.Text.Trim();
				_settings.BuildingPostalCode = tbPostalCode.Text.Trim();
				_settings.BuildingCountry = tbCountry.Text.Trim();
			}
			else
			{
				_settings.ImportMode = ModelImportSettings.Mode.Site;
				_settings.BuildingToImportName = null;
			}

			_settings.Save();
		}

		private static readonly string _directorySeparatorChar = new string(Path.DirectorySeparatorChar, 1);

		private static string makeRelativePath(string path, string basePath)
		{
			string result;
			if ((path != null) && path.StartsWith(basePath))
			{
				result = path.Substring(basePath.Length);
				if (result.StartsWith(_directorySeparatorChar))
					result = result.Substring(1);
			}
			else
			{
				result = path;
			}
			if ((result != null) && (0 == result.Length)) result = ".";  // directory case
			return result;
		}

        private static void selectComboItem(ComboBox target, string desirableItem)
        {
            int idx = target.Items.IndexOf(desirableItem);
            if (idx >= 0) target.SelectedIndex = idx;
        }

        private void cbSingleBuilding_CheckedChanged(object sender, EventArgs e)
        {
            cbxBuildings.Enabled = cbSingleBuilding.Checked;
            tbStreetAddress.Enabled = cbSingleBuilding.Checked;
            tbMunicipality.Enabled = cbSingleBuilding.Checked;
            tbStateProvince.Enabled = cbSingleBuilding.Checked;
            tbPostalCode.Enabled = cbSingleBuilding.Checked;
            tbCountry.Enabled = cbSingleBuilding.Checked;

            lblPoiModelFile.Enabled = cbSingleBuilding.Checked;
            btnPoiModelBrowse.Enabled = cbSingleBuilding.Checked;

            if (cbSingleBuilding.Checked && (0 == tbStreetAddress.Text.Length)
                && (0 == tbMunicipality.Text.Length) && (0 == tbStateProvince.Text.Length)
                && (0 == tbPostalCode.Text.Length) && (0 == tbCountry.Text.Length))
            {
                tbMunicipality.Text = "Toronto";
                tbStateProvince.Text = "ON";
                tbPostalCode.Text = "H0H0H0";
                tbCountry.Text = "Canada";
            }
        }

        private void cbxDeveloper_TextChanged(object sender, EventArgs e)
        {
            btnImport.Enabled = (cbxDeveloper.Text.Length > 0) && (tbSite.Text.Length > 0);
        }

        private void tbSite_TextChanged(object sender, EventArgs e)
        {
            btnImport.Enabled = (cbxDeveloper.Text.Length > 0) && (tbSite.Text.Length > 0);
        }

        private void ImportForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void ImportForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] myFiles = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (myFiles != null)
                {
                    foreach (string file in myFiles)
                    {
                        if (!File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                        {
                            string ext = Path.GetExtension(file).ToLowerInvariant();
                            if (ext.Equals(".kmz") || ext.Equals(".kml")) onNewModelFileName(sender, file);
                            else MessageBox.Show("Unknown file type dropped.",
                                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void ImportForm2_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] myFiles = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (myFiles != null)
                {
                    foreach (string file in myFiles)
                    {
                        if (!File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                        {
                            string ext = Path.GetExtension(file).ToLowerInvariant();
                            if (ext.Equals(".htm") || ext.Equals(".html")) onNewModelFileName(sender, file);
                            else MessageBox.Show("Unknown file type dropped.",
                                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.DefaultExt = "kmz";
            ofd.Filter = "Model files (*.kmz, *.kml)|*.kmz;*.kml|All Files (*.*)|*.*";
            ofd.Multiselect = false;
            //ofd.InitialDirectory = null;
            ofd.Title = "Select model file";

            if (DialogResult.OK == ofd.ShowDialog()) onNewModelFileName(sender, ofd.FileName);
        }

        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.DefaultExt = "html";
            ofd.Filter = "Html files (*.htm, *.html)|*.htm;*.html|All Files (*.*)|*.*";
            ofd.Multiselect = false;
            //ofd.InitialDirectory = null;
            ofd.Title = "Select template file";

            if (DialogResult.OK == ofd.ShowDialog()) onNewModelFileName(sender, ofd.FileName);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
			saveChangeControls(this, _controlState1, _controlState2, Cursors.WaitCursor);
			_controlState1[this] = this.Cursor;
			this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            try
            {
				string cmdLn = buildImportComandLine();
				new Thread(importThread).Start(cmdLn);
            }
            catch (Exception ex)
            {
				finishWithImport(ex.Message + "\r\n" + ex.StackTrace, false);
            }
        }

		private void cbDryRun_CheckedChanged(object sender, EventArgs e)
		{
			cbDoMlsImport.Enabled = !cbDryRun.Checked;
		}
    }
}
