using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CoreClasses;

namespace ConsoleSales
{
    public partial class ChangesReportForm : Form
    {
        string _siteName;
        string _buildingName;
        public ChangesReportForm(string report, string siteName, string buildingName)
        {
            InitializeComponent();
            Text = string.Format("Changes Report for site \'{0}\', building \'{1}\'", siteName, buildingName);
            _siteName = siteName;
            _buildingName = buildingName;
            textBoxReport.Text = report;
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
