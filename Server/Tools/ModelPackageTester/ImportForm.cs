using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vre.Server.Model.Kmz;
using System.IO;
using System.Diagnostics;

namespace ModelPackageTester
{
    public partial class ImportForm : Form
    {
        private Kmz _model = null;
        private string _modelFileName = null;
        private string _stiFileName = null;
        private string _floorPlanPath = null;
        private string _executablePath = null;
        private string _displayModelFileName = null;

        private void onNewModelFileName(string filename)
        {
            _displayModelFileName = filename;
            lblDisplayModelFile.Text = filename;
        }

        private string import()
        {
            string logFileName = Path.Combine(
                Path.GetDirectoryName(_modelFileName),
                Path.GetFileNameWithoutExtension(_modelFileName) + ".import.log.txt");

            StringBuilder commandLine = new StringBuilder("-importmodel");

            commandLine.Append(" infomodel=");
            insertPath(_modelFileName, ref commandLine);

            if (_displayModelFileName != null)
            {
                commandLine.Append(" displaymodel=");
                insertPath(_displayModelFileName, ref commandLine);
            }

            commandLine.Append(" sti=");
            insertPath(_stiFileName, ref commandLine);

            commandLine.Append(" ed=");
            insertPath(tbDeveloper.Text, ref commandLine);

            commandLine.Append(" site=");
            insertPath(tbSite.Text, ref commandLine);

            if (cbSingleBuilding.Checked)
            {
                commandLine.Append(" asbuilding=true");
                commandLine.Append(" building=");
                insertPath(cbxBuildings.SelectedValue as string, ref commandLine);
            }

            commandLine.AppendFormat(" dryrun={0}", cbDryRun.Checked ? "true" : "false");

            ProcessStartInfo psi = new ProcessStartInfo(_executablePath, commandLine.ToString());
            psi.WorkingDirectory = _floorPlanPath;

            File.Delete(logFileName);
            Process p = Process.Start(psi);
            p.WaitForExit();

            using (FileStream fs = File.OpenRead(logFileName))
                using (StreamReader rdr = new StreamReader(fs)) 
                    return rdr.ReadToEnd();
        }

        private static void insertPath(string path, ref StringBuilder commandLine)
        {
            if (path.Contains(' ')) commandLine.AppendFormat("\"{0}\"", path);
            else commandLine.Append(path);
        }

        public ImportForm()
        {
            InitializeComponent();
        }

        public void Init(string modelFileName, string stiFileName, string floorPlanPath, Kmz model, string executablePath)
        {
            _modelFileName = modelFileName;
            _stiFileName = stiFileName;
            _floorPlanPath = floorPlanPath;
            _model = model;
            _executablePath = executablePath;
        }

        private void ImportForm_Shown(object sender, EventArgs e)
        {
            foreach (Building b in _model.Model.Site.Buildings)
                cbxBuildings.Items.Add(b.Name);
        }

        private void cbSingleBuilding_CheckedChanged(object sender, EventArgs e)
        {
            cbxBuildings.Enabled = cbSingleBuilding.Checked;
        }

        private void tbDeveloper_TextChanged(object sender, EventArgs e)
        {
            btnImport.Enabled = (tbDeveloper.Text.Length > 0) && (tbSite.Text.Length > 0);
        }

        private void tbSite_TextChanged(object sender, EventArgs e)
        {
            btnImport.Enabled = (tbDeveloper.Text.Length > 0) && (tbSite.Text.Length > 0);
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
                            if (ext.Equals(".kmz") || ext.Equals(".kml")) onNewModelFileName(file);
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
            ofd.Filter = "Model files (*.kmz, *.kml)|*.kmz,*.kml|All Files (*.*)|*.*";
            ofd.Multiselect = false;
            //ofd.InitialDirectory = null;
            ofd.Title = "Select model file";

            if (DialogResult.OK == ofd.ShowDialog()) onNewModelFileName(ofd.FileName);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Cursor saved = Cursor.Current;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();

                tbResults.Text = import();
            }
            catch (Exception ex)
            {
                tbResults.Text = ex.Message + "\r\n" + ex.StackTrace;
            }
            finally
            {
                tbResults.Enabled = true;
                Cursor.Current = saved;
            }
        }
    }
}
