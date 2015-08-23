using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Vre.Server;
using Vre.Server.Model.Kmz;

namespace ModelPackageTester
{
    public partial class StructureImportForm : Form
    {
		private Kmz _model = null;
		private string _executablePath = null;
		private ModelImportSettings _settings;
        private string _displayModelFileName = null;

		private Dictionary<Control, Cursor> _controlState1 = new Dictionary<Control,Cursor>();
		private Dictionary<Control, bool> _controlState2 = new Dictionary<Control, bool>();

		private void importThread(object param)
		{
			string result = string.Empty;
			bool positive = false;

			string logFileName = Path.Combine(
				Path.GetDirectoryName(_displayModelFileName),
				Path.GetFileNameWithoutExtension(_displayModelFileName) + ".import.log.txt");

			try
			{
				ProcessStartInfo psi = new ProcessStartInfo(_executablePath, param as string);
				psi.WorkingDirectory = (_settings != null) ? _settings.BasePath : Path.GetDirectoryName(_displayModelFileName);
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

        private string buildImportComandLine()
        {
            StringBuilder commandLine = new StringBuilder("importstructure");

            commandLine.Append(" displaymodel=");
            insertPath(_displayModelFileName, ref commandLine);

            commandLine.Append(" structure=");
			insertPath((cbxStructures.SelectedItem as string).Trim(), ref commandLine);

			commandLine.Append(" strucLocName=");
			insertPath(tbLocalizedName.Text, ref commandLine);

            commandLine.AppendFormat(" dryrun={0}", cbDryRun.Checked ? "true" : "false");

			return commandLine.ToString();
        }

        private static void insertPath(string path, ref StringBuilder commandLine)
        {
            if (path.Contains(' ')) commandLine.AppendFormat("\"{0}\"", path);
            else commandLine.Append(path);
        }

		public StructureImportForm()
        {
            InitializeComponent();

            Icon = Properties.Resources.cloudservice;
        }

        public void Init(ModelImportSettings settings,
            string displayModelFileName, Kmz model,
            string executablePath, bool allowDrop, bool isProduction)
        {
			_settings = settings;

            _displayModelFileName = displayModelFileName;
            _executablePath = executablePath;
			_model = model;
            initInt(allowDrop, isProduction);

			string prop;

            prop = _settings.StructureName;
            if (prop != null) //tbStructureName.Text = prop;
				selectComboItem(cbxStructures, prop);

			btnImport.Enabled = true;// (tbStructureName.Text.Length > 0);
        }

		public void Init(string displayModelFileName, Kmz model,
			string executablePath, bool allowDrop, bool isProduction)
		{
			_settings = null;
			_displayModelFileName = displayModelFileName;
			_model = model;
			_executablePath = executablePath;
			initInt(allowDrop, isProduction);

			selectComboItem(cbxStructures, model.Model.Site.Structures.First().Name);

			btnImport.Enabled = true;// (tbStructureName.Text.Length > 0);
		}

		private void initInt(bool allowDrop, bool isProduction)
        {
            if (isProduction)
            {
                Text = Text + " - PRODUCTION";
                ForeColor = SystemColors.HighlightText;
                BackColor = SystemColors.Highlight;
            }

			cbxStructures.Items.Clear();
			foreach (Structure s in _model.Model.Site.Structures)
				cbxStructures.Items.Add(s.Name);

			string prop;

			prop = _settings.StructureName;
			selectComboItem(cbxStructures, prop);

			prop = _settings.StructureLocalizedName;
			if (prop != null) tbLocalizedName.Text = prop;
			else tbLocalizedName.Text = _settings.StructureName;
		}

		private void saveImportSettings()
		{
			if (null == _settings)
			{
				_settings = new ModelImportSettings(Path.Combine(
					Path.GetDirectoryName(_displayModelFileName),
					Path.GetFileNameWithoutExtension(_displayModelFileName) + ".import.txt"));
			}

			string basePath = _settings.BasePath;

			_settings.ImportMode = ModelImportSettings.Mode.Structure;

			_settings.StructureName = (cbxStructures.SelectedItem as string).Trim();

			_settings.StructureLocalizedName = tbLocalizedName.Text.Trim();

			_settings.DisplayModelFileName = makeRelativePath(_displayModelFileName, basePath);

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
		
		//private void tbSite_TextChanged(object sender, EventArgs e)
		//{
		//    btnImport.Enabled = (tbStructureName.Text.Length > 0);
		//}

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
    }
}
