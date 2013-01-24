using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CoreClasses;
using Vre.Server.BusinessLogic;

namespace SuperAdminConsole
{
    public partial class AccountPropertyForm : Form
    {
        object oldValue;
        Type propType;
        ClientData theData;
        string theKey;

        public object NewValue
        {
            private set;
            get;
        }

        public AccountPropertyForm(ClientData data, string key)
        {
            InitializeComponent();
            theData = data;
            theKey = key;
            Text = "Editing Value for Property \'" + key +"\'";
            oldValue = data[key];
            propType = data[key].GetType();
            textBoxValue.Text = oldValue.ToString();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            NewValue = textBoxValue.Text;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UpdateIt()
        {
            buttonApply.Enabled = oldValue.ToString() != textBoxValue.Text;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            string keyDataStr = keyData.ToString();

            if ((keyData & Keys.KeyCode) == Keys.Escape)
            {
                this.Close();
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                return true; // Handled.
            }
            //else
            //    if (keyData == (Keys.Shift | Keys.Enter))
            //        return base.ProcessDialogKey(keyData);
            //    else
            //        if ((keyData & Keys.KeyCode) == Keys.Enter)
            //        {
            //            this.Close();
            //            DialogResult = System.Windows.Forms.DialogResult.OK;
            //            return true; // Handled.
            //        }

            return base.ProcessDialogKey(keyData);
        }


        private void textBoxValue_TextChanged(object sender, EventArgs e)
        {
            UpdateIt();
        }
    }
}
