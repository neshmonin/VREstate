using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SuperAdminConsole
{
    public partial class UpdateDateTimeForm : Form
    {
        public DateTime ExpiredOn { private set; get; }

        public UpdateDateTimeForm(DateTime expiredOn)
        {
            InitializeComponent();
            ExpiredOn = expiredOn;
            dateTimePicker.Value = expiredOn;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            ExpiredOn = dateTimePicker.Value;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
