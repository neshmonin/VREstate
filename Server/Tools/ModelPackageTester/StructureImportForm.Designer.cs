namespace ModelPackageTester
{
	partial class StructureImportForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbResults = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.cbAutoSaveSettings = new System.Windows.Forms.CheckBox();
			this.cbDryRun = new System.Windows.Forms.CheckBox();
			this.btnImport = new System.Windows.Forms.Button();
			this.cbxStructures = new System.Windows.Forms.ComboBox();
			this.tbLocalizedName = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 11);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Structure ID:";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(177, 34);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(200, 17);
			this.label2.TabIndex = 0;
			this.label2.Text = "(unique name and/or address)";
			// 
			// tbResults
			// 
			this.tbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbResults.Enabled = false;
			this.tbResults.Location = new System.Drawing.Point(20, 150);
			this.tbResults.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tbResults.Multiline = true;
			this.tbResults.Name = "tbResults";
			this.tbResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbResults.Size = new System.Drawing.Size(769, 553);
			this.tbResults.TabIndex = 2;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.cbAutoSaveSettings);
			this.panel1.Controls.Add(this.cbDryRun);
			this.panel1.Location = new System.Drawing.Point(20, 94);
			this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(445, 37);
			this.panel1.TabIndex = 1;
			// 
			// cbAutoSaveSettings
			// 
			this.cbAutoSaveSettings.AutoSize = true;
			this.cbAutoSaveSettings.Checked = true;
			this.cbAutoSaveSettings.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbAutoSaveSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cbAutoSaveSettings.Location = new System.Drawing.Point(201, 7);
			this.cbAutoSaveSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cbAutoSaveSettings.Name = "cbAutoSaveSettings";
			this.cbAutoSaveSettings.Size = new System.Drawing.Size(215, 21);
			this.cbAutoSaveSettings.TabIndex = 2;
			this.cbAutoSaveSettings.Text = "Auto-save import settings";
			this.cbAutoSaveSettings.UseVisualStyleBackColor = true;
			// 
			// cbDryRun
			// 
			this.cbDryRun.AutoSize = true;
			this.cbDryRun.Location = new System.Drawing.Point(9, 7);
			this.cbDryRun.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cbDryRun.Name = "cbDryRun";
			this.cbDryRun.Size = new System.Drawing.Size(77, 21);
			this.cbDryRun.TabIndex = 0;
			this.cbDryRun.Text = "Dry run";
			this.cbDryRun.UseVisualStyleBackColor = true;
			// 
			// btnImport
			// 
			this.btnImport.Enabled = false;
			this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnImport.Location = new System.Drawing.Point(479, 6);
			this.btnImport.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(312, 137);
			this.btnImport.TabIndex = 1;
			this.btnImport.Text = "Import";
			this.btnImport.UseVisualStyleBackColor = true;
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// cbxStructures
			// 
			this.cbxStructures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cbxStructures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxStructures.FormattingEnabled = true;
			this.cbxStructures.Location = new System.Drawing.Point(123, 6);
			this.cbxStructures.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cbxStructures.Name = "cbxStructures";
			this.cbxStructures.Size = new System.Drawing.Size(343, 24);
			this.cbxStructures.TabIndex = 0;
			// 
			// tbLocalizedName
			// 
			this.tbLocalizedName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbLocalizedName.Enabled = false;
			this.tbLocalizedName.Location = new System.Drawing.Point(203, 61);
			this.tbLocalizedName.Margin = new System.Windows.Forms.Padding(4);
			this.tbLocalizedName.Name = "tbLocalizedName";
			this.tbLocalizedName.Size = new System.Drawing.Size(263, 22);
			this.tbLocalizedName.TabIndex = 4;
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(16, 64);
			this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(171, 17);
			this.label20.TabIndex = 5;
			this.label20.Text = "Localized structure name:";
			// 
			// StructureImportForm
			// 
			this.AcceptButton = this.btnImport;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(807, 719);
			this.Controls.Add(this.tbLocalizedName);
			this.Controls.Add(this.label20);
			this.Controls.Add(this.btnImport);
			this.Controls.Add(this.cbxStructures);
			this.Controls.Add(this.tbResults);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MinimumSize = new System.Drawing.Size(821, 753);
			this.Name = "StructureImportForm";
			this.Text = "Import Structure - 3D Condo Explorer";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbResults;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnImport;
		private System.Windows.Forms.CheckBox cbDryRun;
		private System.Windows.Forms.CheckBox cbAutoSaveSettings;
		private System.Windows.Forms.ComboBox cbxStructures;
		private System.Windows.Forms.TextBox tbLocalizedName;
		private System.Windows.Forms.Label label20;
    }
}