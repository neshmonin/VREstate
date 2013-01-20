using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vre.Server.BusinessLogic;
using CoreClasses;
using System.Diagnostics;

namespace SuperAdminConsole
{
    public partial class Note : Form
    {
        string originalText = string.Empty;
        MainForm parent = null;

        public Note(string captionText, string text, MainForm parent)
        {
            InitializeComponent();
            originalText = text;
            richTextBox.Text = text;
            this.parent = parent;
            Text = captionText;
        }

        public string Notes
        {
            get { return richTextBox.Text; }
        }

        private void Note_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.LastNoteLocation = this.Location;
            parent.LastNoteSize = this.Size;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            string keyDataStr = keyData.ToString();
            Trace.WriteLine(keyDataStr);

            if ((keyData & Keys.KeyCode) == Keys.Escape)
            {
                this.Close();
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                return true; // Handled.
            }
            else
            if (keyData == (Keys.Shift|Keys.Enter))
                return base.ProcessDialogKey(keyData);
            else
            if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                this.Close();
                DialogResult = System.Windows.Forms.DialogResult.OK;
                return true; // Handled.
            }

            return base.ProcessDialogKey(keyData);
        }

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

    }
}
