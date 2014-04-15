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
    public partial class EmailSettings : Form
    {
        string initialSubject;
        string initialBody;

        public EmailSettings(string subject, string body,
                            List<string> words, 
                            List<string> conditions)
        {
            InitializeComponent();
            textBoxSubject.Text = subject;
            initialSubject = subject;
            textBody.Text = body;
            initialBody = body;
            foreach (string word in words)
                textBoxDictionary.Text += word + "\r\n";
            foreach (string condition in conditions)
                textBoxConditions.Text += condition + "\r\n";

            updateState();
        }

        private void updateState()
        {
            buttonClose.Enabled = initialSubject != textBoxSubject.Text ||
                                  initialBody != textBody.Text;
        }

        private void textBoxSubject_TextChanged(object sender, EventArgs e)
        {
            updateState();
        }

        private void textBody_TextChanged(object sender, EventArgs e)
        {
            updateState();
        }

        public string Subject
        {
            get { return textBoxSubject.Text; }
        }

        public string Body
        {
            get { return textBody.Text; }
        }
        
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

    }
}
