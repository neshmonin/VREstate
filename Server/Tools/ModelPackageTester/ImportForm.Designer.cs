namespace ModelPackageTester
{
    partial class ImportForm
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
            this.tbDeveloper = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbSite = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblDisplayModelFile = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbResults = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnImport = new System.Windows.Forms.Button();
            this.cbDryRun = new System.Windows.Forms.CheckBox();
            this.cbxNewSuiteStatus = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbxBuildings = new System.Windows.Forms.ComboBox();
            this.cbSingleBuilding = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbStreetAddress = new System.Windows.Forms.TextBox();
            this.tbMunicipality = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tbStateProvince = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tbPostalCode = new System.Windows.Forms.TextBox();
            this.tbCountry = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Import to Estate Developer:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(478, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "(unique name or ID)";
            // 
            // tbDeveloper
            // 
            this.tbDeveloper.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDeveloper.Location = new System.Drawing.Point(154, 6);
            this.tbDeveloper.Name = "tbDeveloper";
            this.tbDeveloper.Size = new System.Drawing.Size(318, 20);
            this.tbDeveloper.TabIndex = 0;
            this.tbDeveloper.TextChanged += new System.EventHandler(this.tbDeveloper_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Import to (or replace) Site:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(478, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(180, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "(name within Estate Developer or ID)";
            // 
            // tbSite
            // 
            this.tbSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSite.Location = new System.Drawing.Point(154, 32);
            this.tbSite.Name = "tbSite";
            this.tbSite.Size = new System.Drawing.Size(318, 20);
            this.tbSite.TabIndex = 1;
            this.tbSite.TextChanged += new System.EventHandler(this.tbSite_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 184);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Display model:";
            // 
            // lblDisplayModelFile
            // 
            this.lblDisplayModelFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDisplayModelFile.AutoEllipsis = true;
            this.lblDisplayModelFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDisplayModelFile.Location = new System.Drawing.Point(148, 184);
            this.lblDisplayModelFile.Name = "lblDisplayModelFile";
            this.lblDisplayModelFile.Size = new System.Drawing.Size(319, 13);
            this.lblDisplayModelFile.TabIndex = 0;
            this.lblDisplayModelFile.Text = "drop file here or press Browse -->";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 197);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(163, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "(for buildings not constructed yet)";
            // 
            // tbResults
            // 
            this.tbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResults.Enabled = false;
            this.tbResults.Location = new System.Drawing.Point(15, 296);
            this.tbResults.Multiline = true;
            this.tbResults.Name = "tbResults";
            this.tbResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResults.Size = new System.Drawing.Size(638, 111);
            this.tbResults.TabIndex = 4;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(478, 179);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.btnImport);
            this.panel1.Controls.Add(this.cbDryRun);
            this.panel1.Location = new System.Drawing.Point(12, 249);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(638, 31);
            this.panel1.TabIndex = 8;
            // 
            // btnImport
            // 
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(137, 3);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // cbDryRun
            // 
            this.cbDryRun.AutoSize = true;
            this.cbDryRun.Checked = true;
            this.cbDryRun.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDryRun.Location = new System.Drawing.Point(7, 6);
            this.cbDryRun.Name = "cbDryRun";
            this.cbDryRun.Size = new System.Drawing.Size(60, 17);
            this.cbDryRun.TabIndex = 0;
            this.cbDryRun.Text = "Dry run";
            this.cbDryRun.UseVisualStyleBackColor = true;
            // 
            // cbxNewSuiteStatus
            // 
            this.cbxNewSuiteStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxNewSuiteStatus.FormattingEnabled = true;
            this.cbxNewSuiteStatus.Location = new System.Drawing.Point(151, 219);
            this.cbxNewSuiteStatus.Name = "cbxNewSuiteStatus";
            this.cbxNewSuiteStatus.Size = new System.Drawing.Size(121, 21);
            this.cbxNewSuiteStatus.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 222);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "New suite sale status:";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.tbCountry);
            this.panel2.Controls.Add(this.tbStateProvince);
            this.panel2.Controls.Add(this.tbPostalCode);
            this.panel2.Controls.Add(this.tbMunicipality);
            this.panel2.Controls.Add(this.tbStreetAddress);
            this.panel2.Controls.Add(this.cbxBuildings);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.cbSingleBuilding);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Location = new System.Drawing.Point(12, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(641, 114);
            this.panel2.TabIndex = 10;
            // 
            // cbxBuildings
            // 
            this.cbxBuildings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxBuildings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBuildings.Enabled = false;
            this.cbxBuildings.FormattingEnabled = true;
            this.cbxBuildings.Location = new System.Drawing.Point(140, 6);
            this.cbxBuildings.Name = "cbxBuildings";
            this.cbxBuildings.Size = new System.Drawing.Size(318, 21);
            this.cbxBuildings.TabIndex = 1;
            // 
            // cbSingleBuilding
            // 
            this.cbSingleBuilding.AutoSize = true;
            this.cbSingleBuilding.Location = new System.Drawing.Point(3, 8);
            this.cbSingleBuilding.Name = "cbSingleBuilding";
            this.cbSingleBuilding.Size = new System.Drawing.Size(127, 17);
            this.cbSingleBuilding.TabIndex = 0;
            this.cbSingleBuilding.Text = "Import single building:";
            this.cbSingleBuilding.UseVisualStyleBackColor = true;
            this.cbSingleBuilding.CheckedChanged += new System.EventHandler(this.cbSingleBuilding_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(116, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Building street address:";
            // 
            // tbStreetAddress
            // 
            this.tbStreetAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStreetAddress.Enabled = false;
            this.tbStreetAddress.Location = new System.Drawing.Point(140, 33);
            this.tbStreetAddress.Name = "tbStreetAddress";
            this.tbStreetAddress.Size = new System.Drawing.Size(318, 20);
            this.tbStreetAddress.TabIndex = 2;
            // 
            // tbMunicipality
            // 
            this.tbMunicipality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMunicipality.Enabled = false;
            this.tbMunicipality.Location = new System.Drawing.Point(140, 59);
            this.tbMunicipality.Name = "tbMunicipality";
            this.tbMunicipality.Size = new System.Drawing.Size(133, 20);
            this.tbMunicipality.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(118, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Municipality (town/city):";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(279, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(131, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "State/province (two-char):";
            // 
            // tbStateProvince
            // 
            this.tbStateProvince.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStateProvince.Enabled = false;
            this.tbStateProvince.Location = new System.Drawing.Point(416, 59);
            this.tbStateProvince.Name = "tbStateProvince";
            this.tbStateProvince.Size = new System.Drawing.Size(42, 20);
            this.tbStateProvince.TabIndex = 4;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(0, 88);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(66, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Postal code:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(264, 88);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "Country:";
            // 
            // tbPostalCode
            // 
            this.tbPostalCode.Enabled = false;
            this.tbPostalCode.Location = new System.Drawing.Point(140, 85);
            this.tbPostalCode.Name = "tbPostalCode";
            this.tbPostalCode.Size = new System.Drawing.Size(118, 20);
            this.tbPostalCode.TabIndex = 5;
            // 
            // tbCountry
            // 
            this.tbCountry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCountry.Enabled = false;
            this.tbCountry.Location = new System.Drawing.Point(316, 85);
            this.tbCountry.Name = "tbCountry";
            this.tbCountry.Size = new System.Drawing.Size(142, 20);
            this.tbCountry.TabIndex = 6;
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 419);
            this.Controls.Add(this.cbxNewSuiteStatus);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.tbResults);
            this.Controls.Add(this.tbSite);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbDeveloper);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblDisplayModelFile);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "ImportForm";
            this.Text = "Import Model - 3D Condo Explorer";
            this.Shown += new System.EventHandler(this.ImportForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragEnter);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDeveloper;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbSite;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDisplayModelFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbResults;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.CheckBox cbDryRun;
        private System.Windows.Forms.ComboBox cbxNewSuiteStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox tbMunicipality;
        private System.Windows.Forms.TextBox tbStreetAddress;
        private System.Windows.Forms.ComboBox cbxBuildings;
        private System.Windows.Forms.CheckBox cbSingleBuilding;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbCountry;
        private System.Windows.Forms.TextBox tbStateProvince;
        private System.Windows.Forms.TextBox tbPostalCode;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
    }
}