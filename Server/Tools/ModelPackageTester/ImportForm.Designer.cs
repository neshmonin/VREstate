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
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tbSite = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.lblDisplayModelFile = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.tbResults = new System.Windows.Forms.TextBox();
			this.btnDisplayModelBrowse = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnImport = new System.Windows.Forms.Button();
			this.cbAutoSaveSettings = new System.Windows.Forms.CheckBox();
			this.cbDryRun = new System.Windows.Forms.CheckBox();
			this.cbxNewSuiteStatus = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.tbCountry = new System.Windows.Forms.TextBox();
			this.tbStateProvince = new System.Windows.Forms.TextBox();
			this.tbPostalCode = new System.Windows.Forms.TextBox();
			this.tbMunicipality = new System.Windows.Forms.TextBox();
			this.tbStreetAddress = new System.Windows.Forms.TextBox();
			this.cbxBuildings = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.cbSingleBuilding = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.cbxDeveloper = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.lblOverlayModelFile = new System.Windows.Forms.Label();
			this.btnOverlayModelBrowse = new System.Windows.Forms.Button();
			this.label15 = new System.Windows.Forms.Label();
			this.lblPoiModelFile = new System.Windows.Forms.Label();
			this.btnPoiModelBrowse = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.cbxNewBuildingStatus = new System.Windows.Forms.ComboBox();
			this.label16 = new System.Windows.Forms.Label();
			this.lblBubbleWebTemplateFile = new System.Windows.Forms.Label();
			this.btnBubbleWebTemplateBrowse = new System.Windows.Forms.Button();
			this.label17 = new System.Windows.Forms.Label();
			this.lblBubbleKioskTemplateFile = new System.Windows.Forms.Label();
			this.btnBubbleKioskTemplateBrowse = new System.Windows.Forms.Button();
			this.cbDoMlsImport = new System.Windows.Forms.CheckBox();
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
			this.label2.Location = new System.Drawing.Point(418, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "(unique name or ID)";
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
			this.label4.Location = new System.Drawing.Point(418, 35);
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
			this.tbSite.Size = new System.Drawing.Size(258, 20);
			this.tbSite.TabIndex = 1;
			this.tbSite.TextChanged += new System.EventHandler(this.tbSite_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 184);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(75, 13);
			this.label5.TabIndex = 3;
			this.label5.Text = "Display model:";
			// 
			// lblDisplayModelFile
			// 
			this.lblDisplayModelFile.AllowDrop = true;
			this.lblDisplayModelFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblDisplayModelFile.AutoEllipsis = true;
			this.lblDisplayModelFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblDisplayModelFile.Location = new System.Drawing.Point(151, 184);
			this.lblDisplayModelFile.Name = "lblDisplayModelFile";
			this.lblDisplayModelFile.Size = new System.Drawing.Size(259, 13);
			this.lblDisplayModelFile.TabIndex = 0;
			this.lblDisplayModelFile.Text = "drop file here or press Browse -->";
			this.lblDisplayModelFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragDrop);
			this.lblDisplayModelFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragEnter);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 195);
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
			this.tbResults.Location = new System.Drawing.Point(15, 418);
			this.tbResults.Multiline = true;
			this.tbResults.Name = "tbResults";
			this.tbResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbResults.Size = new System.Drawing.Size(578, 176);
			this.tbResults.TabIndex = 11;
			// 
			// btnDisplayModelBrowse
			// 
			this.btnDisplayModelBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDisplayModelBrowse.Location = new System.Drawing.Point(418, 179);
			this.btnDisplayModelBrowse.Name = "btnDisplayModelBrowse";
			this.btnDisplayModelBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnDisplayModelBrowse.TabIndex = 3;
			this.btnDisplayModelBrowse.Text = "Browse";
			this.btnDisplayModelBrowse.UseVisualStyleBackColor = true;
			this.btnDisplayModelBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.cbDoMlsImport);
			this.panel1.Controls.Add(this.btnImport);
			this.panel1.Controls.Add(this.cbAutoSaveSettings);
			this.panel1.Controls.Add(this.cbDryRun);
			this.panel1.Location = new System.Drawing.Point(15, 379);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(578, 31);
			this.panel1.TabIndex = 10;
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
			// cbAutoSaveSettings
			// 
			this.cbAutoSaveSettings.AutoSize = true;
			this.cbAutoSaveSettings.Checked = true;
			this.cbAutoSaveSettings.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbAutoSaveSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cbAutoSaveSettings.Location = new System.Drawing.Point(404, 7);
			this.cbAutoSaveSettings.Name = "cbAutoSaveSettings";
			this.cbAutoSaveSettings.Size = new System.Drawing.Size(169, 17);
			this.cbAutoSaveSettings.TabIndex = 3;
			this.cbAutoSaveSettings.Text = "Auto-save import settings";
			this.cbAutoSaveSettings.UseVisualStyleBackColor = true;
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
			this.cbDryRun.CheckedChanged += new System.EventHandler(this.cbDryRun_CheckedChanged);
			// 
			// cbxNewSuiteStatus
			// 
			this.cbxNewSuiteStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxNewSuiteStatus.FormattingEnabled = true;
			this.cbxNewSuiteStatus.Location = new System.Drawing.Point(154, 352);
			this.cbxNewSuiteStatus.Name = "cbxNewSuiteStatus";
			this.cbxNewSuiteStatus.Size = new System.Drawing.Size(162, 21);
			this.cbxNewSuiteStatus.TabIndex = 9;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 355);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(88, 13);
			this.label6.TabIndex = 3;
			this.label6.Text = "New suite status:";
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
			this.panel2.Size = new System.Drawing.Size(581, 114);
			this.panel2.TabIndex = 2;
			// 
			// tbCountry
			// 
			this.tbCountry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbCountry.Enabled = false;
			this.tbCountry.Location = new System.Drawing.Point(316, 85);
			this.tbCountry.Name = "tbCountry";
			this.tbCountry.Size = new System.Drawing.Size(82, 20);
			this.tbCountry.TabIndex = 6;
			// 
			// tbStateProvince
			// 
			this.tbStateProvince.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbStateProvince.Enabled = false;
			this.tbStateProvince.Location = new System.Drawing.Point(356, 59);
			this.tbStateProvince.Name = "tbStateProvince";
			this.tbStateProvince.Size = new System.Drawing.Size(42, 20);
			this.tbStateProvince.TabIndex = 4;
			// 
			// tbPostalCode
			// 
			this.tbPostalCode.Enabled = false;
			this.tbPostalCode.Location = new System.Drawing.Point(140, 85);
			this.tbPostalCode.Name = "tbPostalCode";
			this.tbPostalCode.Size = new System.Drawing.Size(118, 20);
			this.tbPostalCode.TabIndex = 5;
			// 
			// tbMunicipality
			// 
			this.tbMunicipality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbMunicipality.Enabled = false;
			this.tbMunicipality.Location = new System.Drawing.Point(140, 59);
			this.tbMunicipality.Name = "tbMunicipality";
			this.tbMunicipality.Size = new System.Drawing.Size(73, 20);
			this.tbMunicipality.TabIndex = 3;
			// 
			// tbStreetAddress
			// 
			this.tbStreetAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbStreetAddress.Enabled = false;
			this.tbStreetAddress.Location = new System.Drawing.Point(140, 33);
			this.tbStreetAddress.Name = "tbStreetAddress";
			this.tbStreetAddress.Size = new System.Drawing.Size(258, 20);
			this.tbStreetAddress.TabIndex = 2;
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
			this.cbxBuildings.Size = new System.Drawing.Size(258, 21);
			this.cbxBuildings.TabIndex = 1;
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
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(0, 88);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(66, 13);
			this.label11.TabIndex = 3;
			this.label11.Text = "Postal code:";
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(219, 62);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(131, 13);
			this.label10.TabIndex = 3;
			this.label10.Text = "State/province (two-char):";
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
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(0, 36);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(116, 13);
			this.label8.TabIndex = 3;
			this.label8.Text = "Building street address:";
			// 
			// cbxDeveloper
			// 
			this.cbxDeveloper.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cbxDeveloper.FormattingEnabled = true;
			this.cbxDeveloper.Location = new System.Drawing.Point(154, 5);
			this.cbxDeveloper.Name = "cbxDeveloper";
			this.cbxDeveloper.Size = new System.Drawing.Size(258, 21);
			this.cbxDeveloper.TabIndex = 0;
			this.cbxDeveloper.SelectedIndexChanged += new System.EventHandler(this.cbxDeveloper_TextChanged);
			this.cbxDeveloper.TextChanged += new System.EventHandler(this.cbxDeveloper_TextChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(12, 213);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(77, 13);
			this.label13.TabIndex = 3;
			this.label13.Text = "Overlay model:";
			// 
			// lblOverlayModelFile
			// 
			this.lblOverlayModelFile.AllowDrop = true;
			this.lblOverlayModelFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblOverlayModelFile.AutoEllipsis = true;
			this.lblOverlayModelFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblOverlayModelFile.Location = new System.Drawing.Point(151, 213);
			this.lblOverlayModelFile.Name = "lblOverlayModelFile";
			this.lblOverlayModelFile.Size = new System.Drawing.Size(259, 13);
			this.lblOverlayModelFile.TabIndex = 0;
			this.lblOverlayModelFile.Text = "drop file here or press Browse -->";
			this.lblOverlayModelFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragDrop);
			this.lblOverlayModelFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragEnter);
			// 
			// btnOverlayModelBrowse
			// 
			this.btnOverlayModelBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOverlayModelBrowse.Location = new System.Drawing.Point(418, 208);
			this.btnOverlayModelBrowse.Name = "btnOverlayModelBrowse";
			this.btnOverlayModelBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnOverlayModelBrowse.TabIndex = 4;
			this.btnOverlayModelBrowse.Text = "Browse";
			this.btnOverlayModelBrowse.UseVisualStyleBackColor = true;
			this.btnOverlayModelBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(12, 242);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(59, 13);
			this.label15.TabIndex = 3;
			this.label15.Text = "POI model:";
			// 
			// lblPoiModelFile
			// 
			this.lblPoiModelFile.AllowDrop = true;
			this.lblPoiModelFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblPoiModelFile.AutoEllipsis = true;
			this.lblPoiModelFile.Enabled = false;
			this.lblPoiModelFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblPoiModelFile.Location = new System.Drawing.Point(151, 242);
			this.lblPoiModelFile.Name = "lblPoiModelFile";
			this.lblPoiModelFile.Size = new System.Drawing.Size(259, 13);
			this.lblPoiModelFile.TabIndex = 0;
			this.lblPoiModelFile.Text = "drop file here or press Browse -->";
			this.lblPoiModelFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragDrop);
			this.lblPoiModelFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragEnter);
			// 
			// btnPoiModelBrowse
			// 
			this.btnPoiModelBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPoiModelBrowse.Enabled = false;
			this.btnPoiModelBrowse.Location = new System.Drawing.Point(418, 237);
			this.btnPoiModelBrowse.Name = "btnPoiModelBrowse";
			this.btnPoiModelBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnPoiModelBrowse.TabIndex = 5;
			this.btnPoiModelBrowse.Text = "Browse";
			this.btnPoiModelBrowse.UseVisualStyleBackColor = true;
			this.btnPoiModelBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(12, 328);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(102, 13);
			this.label14.TabIndex = 3;
			this.label14.Text = "New building status:";
			// 
			// cbxNewBuildingStatus
			// 
			this.cbxNewBuildingStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxNewBuildingStatus.FormattingEnabled = true;
			this.cbxNewBuildingStatus.Location = new System.Drawing.Point(154, 325);
			this.cbxNewBuildingStatus.Name = "cbxNewBuildingStatus";
			this.cbxNewBuildingStatus.Size = new System.Drawing.Size(162, 21);
			this.cbxNewBuildingStatus.TabIndex = 8;
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(12, 271);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(115, 13);
			this.label16.TabIndex = 3;
			this.label16.Text = "Bubble template (web):";
			// 
			// lblBubbleWebTemplateFile
			// 
			this.lblBubbleWebTemplateFile.AllowDrop = true;
			this.lblBubbleWebTemplateFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblBubbleWebTemplateFile.AutoEllipsis = true;
			this.lblBubbleWebTemplateFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblBubbleWebTemplateFile.Location = new System.Drawing.Point(151, 271);
			this.lblBubbleWebTemplateFile.Name = "lblBubbleWebTemplateFile";
			this.lblBubbleWebTemplateFile.Size = new System.Drawing.Size(259, 13);
			this.lblBubbleWebTemplateFile.TabIndex = 0;
			this.lblBubbleWebTemplateFile.Text = "drop file here or press Browse -->";
			this.lblBubbleWebTemplateFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportForm2_DragDrop);
			this.lblBubbleWebTemplateFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragEnter);
			// 
			// btnBubbleWebTemplateBrowse
			// 
			this.btnBubbleWebTemplateBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBubbleWebTemplateBrowse.Location = new System.Drawing.Point(418, 266);
			this.btnBubbleWebTemplateBrowse.Name = "btnBubbleWebTemplateBrowse";
			this.btnBubbleWebTemplateBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBubbleWebTemplateBrowse.TabIndex = 6;
			this.btnBubbleWebTemplateBrowse.Text = "Browse";
			this.btnBubbleWebTemplateBrowse.UseVisualStyleBackColor = true;
			this.btnBubbleWebTemplateBrowse.Click += new System.EventHandler(this.btnBrowse2_Click);
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(12, 301);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(120, 13);
			this.label17.TabIndex = 3;
			this.label17.Text = "Bubble template (kiosk):";
			// 
			// lblBubbleKioskTemplateFile
			// 
			this.lblBubbleKioskTemplateFile.AllowDrop = true;
			this.lblBubbleKioskTemplateFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblBubbleKioskTemplateFile.AutoEllipsis = true;
			this.lblBubbleKioskTemplateFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblBubbleKioskTemplateFile.Location = new System.Drawing.Point(151, 301);
			this.lblBubbleKioskTemplateFile.Name = "lblBubbleKioskTemplateFile";
			this.lblBubbleKioskTemplateFile.Size = new System.Drawing.Size(259, 13);
			this.lblBubbleKioskTemplateFile.TabIndex = 0;
			this.lblBubbleKioskTemplateFile.Text = "drop file here or press Browse -->";
			this.lblBubbleKioskTemplateFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportForm2_DragDrop);
			this.lblBubbleKioskTemplateFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragEnter);
			// 
			// btnBubbleKioskTemplateBrowse
			// 
			this.btnBubbleKioskTemplateBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBubbleKioskTemplateBrowse.Location = new System.Drawing.Point(418, 296);
			this.btnBubbleKioskTemplateBrowse.Name = "btnBubbleKioskTemplateBrowse";
			this.btnBubbleKioskTemplateBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBubbleKioskTemplateBrowse.TabIndex = 7;
			this.btnBubbleKioskTemplateBrowse.Text = "Browse";
			this.btnBubbleKioskTemplateBrowse.UseVisualStyleBackColor = true;
			this.btnBubbleKioskTemplateBrowse.Click += new System.EventHandler(this.btnBrowse2_Click);
			// 
			// cbDoMlsImport
			// 
			this.cbDoMlsImport.AutoSize = true;
			this.cbDoMlsImport.Checked = true;
			this.cbDoMlsImport.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbDoMlsImport.Enabled = false;
			this.cbDoMlsImport.Location = new System.Drawing.Point(218, 7);
			this.cbDoMlsImport.Name = "cbDoMlsImport";
			this.cbDoMlsImport.Size = new System.Drawing.Size(181, 17);
			this.cbDoMlsImport.TabIndex = 2;
			this.cbDoMlsImport.Text = "Include retrospective MLS import";
			this.cbDoMlsImport.UseVisualStyleBackColor = true;
			// 
			// ImportForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(605, 606);
			this.Controls.Add(this.cbxDeveloper);
			this.Controls.Add(this.cbxNewBuildingStatus);
			this.Controls.Add(this.cbxNewSuiteStatus);
			this.Controls.Add(this.btnPoiModelBrowse);
			this.Controls.Add(this.btnBubbleKioskTemplateBrowse);
			this.Controls.Add(this.btnBubbleWebTemplateBrowse);
			this.Controls.Add(this.btnOverlayModelBrowse);
			this.Controls.Add(this.btnDisplayModelBrowse);
			this.Controls.Add(this.tbResults);
			this.Controls.Add(this.tbSite);
			this.Controls.Add(this.lblPoiModelFile);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblBubbleKioskTemplateFile);
			this.Controls.Add(this.lblBubbleWebTemplateFile);
			this.Controls.Add(this.lblOverlayModelFile);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.lblDisplayModelFile);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.MinimumSize = new System.Drawing.Size(620, 620);
			this.Name = "ImportForm";
			this.Text = "Import Model - 3D Condo Explorer";
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbSite;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDisplayModelFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbResults;
        private System.Windows.Forms.Button btnDisplayModelBrowse;
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
        private System.Windows.Forms.ComboBox cbxDeveloper;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblOverlayModelFile;
        private System.Windows.Forms.Button btnOverlayModelBrowse;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lblPoiModelFile;
        private System.Windows.Forms.Button btnPoiModelBrowse;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cbxNewBuildingStatus;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblBubbleWebTemplateFile;
        private System.Windows.Forms.Button btnBubbleWebTemplateBrowse;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblBubbleKioskTemplateFile;
        private System.Windows.Forms.Button btnBubbleKioskTemplateBrowse;
		private System.Windows.Forms.CheckBox cbAutoSaveSettings;
		private System.Windows.Forms.CheckBox cbDoMlsImport;
    }
}