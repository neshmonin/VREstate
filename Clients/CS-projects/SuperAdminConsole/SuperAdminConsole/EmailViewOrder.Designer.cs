namespace SuperAdminConsole
{
    partial class EmailViewOrder
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
            this.textBoxAddressTo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSubject = new System.Windows.Forms.TextBox();
            this.textBody = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxAddressTo
            // 
            this.textBoxAddressTo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxAddressTo.Location = new System.Drawing.Point(12, 30);
            this.textBoxAddressTo.Name = "textBoxAddressTo";
            this.textBoxAddressTo.Size = new System.Drawing.Size(293, 20);
            this.textBoxAddressTo.TabIndex = 2;
            this.textBoxAddressTo.TextChanged += new System.EventHandler(this.textBoxAddressTo_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Addtess To:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Subject:";
            // 
            // textBoxSubject
            // 
            this.textBoxSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSubject.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxSubject.Location = new System.Drawing.Point(12, 73);
            this.textBoxSubject.Name = "textBoxSubject";
            this.textBoxSubject.Size = new System.Drawing.Size(646, 20);
            this.textBoxSubject.TabIndex = 1;
            // 
            // textBody
            // 
            this.textBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBody.Location = new System.Drawing.Point(12, 108);
            this.textBody.MinimumSize = new System.Drawing.Size(410, 188);
            this.textBody.Multiline = true;
            this.textBody.Name = "textBody";
            this.textBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBody.Size = new System.Drawing.Size(646, 557);
            this.textBody.TabIndex = 0;
            // 
            // buttonSend
            // 
            this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSend.Location = new System.Drawing.Point(345, 4);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(313, 34);
            this.buttonSend.TabIndex = 3;
            this.buttonSend.Text = "Send this email";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(498, 44);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(160, 23);
            this.buttonSettings.TabIndex = 4;
            this.buttonSettings.Text = "Change the defaults...";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // EmailViewOrder
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(670, 677);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBody);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxSubject);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxAddressTo);
            this.MinimumSize = new System.Drawing.Size(450, 344);
            this.Name = "EmailViewOrder";
            this.Text = "Email viewOrder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxAddressTo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSubject;
        private System.Windows.Forms.TextBox textBody;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonSettings;
    }
}