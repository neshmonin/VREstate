namespace SuperAdminConsole
{
    partial class EmailSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBody = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSubject = new System.Windows.Forms.TextBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textBoxDictionary = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxConditions = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(529, 592);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(77, 23);
            this.buttonCancel.TabIndex = 19;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // textBody
            // 
            this.textBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBody.Location = new System.Drawing.Point(11, 55);
            this.textBody.MinimumSize = new System.Drawing.Size(410, 188);
            this.textBody.Multiline = true;
            this.textBody.Name = "textBody";
            this.textBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBody.Size = new System.Drawing.Size(507, 559);
            this.textBody.TabIndex = 20;
            this.textBody.TextChanged += new System.EventHandler(this.textBody_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Subject:";
            // 
            // textBoxSubject
            // 
            this.textBoxSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSubject.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxSubject.Location = new System.Drawing.Point(11, 20);
            this.textBoxSubject.Name = "textBoxSubject";
            this.textBoxSubject.Size = new System.Drawing.Size(507, 20);
            this.textBoxSubject.TabIndex = 21;
            this.textBoxSubject.TextChanged += new System.EventHandler(this.textBoxSubject_TextChanged);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(628, 592);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 23;
            this.buttonClose.Text = "OK";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textBoxDictionary
            // 
            this.textBoxDictionary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDictionary.Location = new System.Drawing.Point(527, 21);
            this.textBoxDictionary.Multiline = true;
            this.textBoxDictionary.Name = "textBoxDictionary";
            this.textBoxDictionary.ReadOnly = true;
            this.textBoxDictionary.Size = new System.Drawing.Size(175, 393);
            this.textBoxDictionary.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(530, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Dictionary";
            // 
            // textBoxConditions
            // 
            this.textBoxConditions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxConditions.Location = new System.Drawing.Point(527, 437);
            this.textBoxConditions.Multiline = true;
            this.textBoxConditions.Name = "textBoxConditions";
            this.textBoxConditions.ReadOnly = true;
            this.textBoxConditions.Size = new System.Drawing.Size(175, 149);
            this.textBoxConditions.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(530, 417);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Logical Operations";
            // 
            // EmailSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(715, 626);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxConditions);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxDictionary);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.textBody);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxSubject);
            this.Controls.Add(this.buttonCancel);
            this.Name = "EmailSettings";
            this.Text = "Email Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBody;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSubject;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textBoxDictionary;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxConditions;
        private System.Windows.Forms.Label label3;
    }
}