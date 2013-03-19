namespace ConsoleSales
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.labelModelFile = new System.Windows.Forms.Label();
            this.comboBuildings = new System.Windows.Forms.ComboBox();
            this.labelSuitesTitle = new System.Windows.Forms.Label();
            this.labelSuiteType = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupSuiteInfo = new System.Windows.Forms.GroupBox();
            this.checkBoxShowPanoramicView = new System.Windows.Forms.CheckBox();
            this.comboSaleStatus = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textPrice = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textCellingHeight = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textFloorName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textSuiteName = new System.Windows.Forms.TextBox();
            this.labelSuiteNumber = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listBoxSuites = new System.Windows.Forms.ListBox();
            this.contextMenuUnit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreOriginalValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonSortByStatus = new System.Windows.Forms.RadioButton();
            this.radioButtonSortByFloor = new System.Windows.Forms.RadioButton();
            this.radioButtonSortByType = new System.Windows.Forms.RadioButton();
            this.buttonUpdateFromCSV = new System.Windows.Forms.Button();
            this.buttonExportToCSV = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.comboDevelopers = new System.Windows.Forms.ComboBox();
            this.comboSites = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pnlStartupShutdown = new System.Windows.Forms.Panel();
            this.lblStartupShutdown = new System.Windows.Forms.Label();
            this.tmrStartup = new System.Windows.Forms.Timer(this.components);
            this.buttonApplyChanges = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.groupSuiteInfo.SuspendLayout();
            this.contextMenuUnit.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.pnlStartupShutdown.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelModelFile
            // 
            this.labelModelFile.AutoSize = true;
            this.labelModelFile.Location = new System.Drawing.Point(17, 77);
            this.labelModelFile.Name = "labelModelFile";
            this.labelModelFile.Size = new System.Drawing.Size(85, 19);
            this.labelModelFile.TabIndex = 1;
            this.labelModelFile.Text = "Building Name";
            this.labelModelFile.UseCompatibleTextRendering = true;
            // 
            // comboBuildings
            // 
            this.comboBuildings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBuildings.Enabled = false;
            this.comboBuildings.FormattingEnabled = true;
            this.comboBuildings.Location = new System.Drawing.Point(17, 96);
            this.comboBuildings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBuildings.Name = "comboBuildings";
            this.comboBuildings.Size = new System.Drawing.Size(152, 23);
            this.comboBuildings.TabIndex = 2;
            this.comboBuildings.SelectedIndexChanged += new System.EventHandler(this.comboBuildings_SelectedIndexChanged);
            // 
            // labelSuitesTitle
            // 
            this.labelSuitesTitle.AutoSize = true;
            this.labelSuitesTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSuitesTitle.Location = new System.Drawing.Point(15, 143);
            this.labelSuitesTitle.Name = "labelSuitesTitle";
            this.labelSuitesTitle.Size = new System.Drawing.Size(61, 24);
            this.labelSuitesTitle.TabIndex = 3;
            this.labelSuitesTitle.Text = "Suites";
            // 
            // labelSuiteType
            // 
            this.labelSuiteType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelSuiteType.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSuiteType.Location = new System.Drawing.Point(778, 119);
            this.labelSuiteType.Name = "labelSuiteType";
            this.labelSuiteType.Size = new System.Drawing.Size(170, 36);
            this.labelSuiteType.TabIndex = 10;
            this.labelSuiteType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(674, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Suite Type";
            // 
            // groupSuiteInfo
            // 
            this.groupSuiteInfo.BackColor = System.Drawing.SystemColors.Control;
            this.groupSuiteInfo.Controls.Add(this.checkBoxShowPanoramicView);
            this.groupSuiteInfo.Controls.Add(this.comboSaleStatus);
            this.groupSuiteInfo.Controls.Add(this.label9);
            this.groupSuiteInfo.Controls.Add(this.textPrice);
            this.groupSuiteInfo.Controls.Add(this.label8);
            this.groupSuiteInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupSuiteInfo.Location = new System.Drawing.Point(662, 286);
            this.groupSuiteInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupSuiteInfo.Name = "groupSuiteInfo";
            this.groupSuiteInfo.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupSuiteInfo.Size = new System.Drawing.Size(294, 251);
            this.groupSuiteInfo.TabIndex = 5;
            this.groupSuiteInfo.TabStop = false;
            this.groupSuiteInfo.Text = "Suite Editable Info";
            // 
            // checkBoxShowPanoramicView
            // 
            this.checkBoxShowPanoramicView.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxShowPanoramicView.Checked = true;
            this.checkBoxShowPanoramicView.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxShowPanoramicView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBoxShowPanoramicView.Location = new System.Drawing.Point(17, 118);
            this.checkBoxShowPanoramicView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBoxShowPanoramicView.Name = "checkBoxShowPanoramicView";
            this.checkBoxShowPanoramicView.Size = new System.Drawing.Size(163, 28);
            this.checkBoxShowPanoramicView.TabIndex = 4;
            this.checkBoxShowPanoramicView.Text = "Show Panoramic View";
            this.checkBoxShowPanoramicView.UseVisualStyleBackColor = true;
            this.checkBoxShowPanoramicView.CheckedChanged += new System.EventHandler(this.checkBoxShowPanoramicView_CheckedChanged);
            // 
            // comboSaleStatus
            // 
            this.comboSaleStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSaleStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboSaleStatus.FormattingEnabled = true;
            this.comboSaleStatus.Location = new System.Drawing.Point(166, 185);
            this.comboSaleStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboSaleStatus.Name = "comboSaleStatus";
            this.comboSaleStatus.Size = new System.Drawing.Size(109, 28);
            this.comboSaleStatus.Sorted = true;
            this.comboSaleStatus.TabIndex = 6;
            this.comboSaleStatus.SelectedIndexChanged += new System.EventHandler(this.comboSaleStatus_SelectedIndexChanged);
            this.comboSaleStatus.Enter += new System.EventHandler(this.comboSaleStatus_Enter);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(20, 193);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(132, 16);
            this.label9.TabIndex = 5;
            this.label9.Text = "Availability Status";
            // 
            // textPrice
            // 
            this.textPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textPrice.Location = new System.Drawing.Point(163, 51);
            this.textPrice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textPrice.Name = "textPrice";
            this.textPrice.Size = new System.Drawing.Size(110, 26);
            this.textPrice.TabIndex = 3;
            this.textPrice.TextChanged += new System.EventHandler(this.textPrice_TextChanged);
            this.textPrice.Enter += new System.EventHandler(this.textPrice_Enter);
            this.textPrice.Leave += new System.EventHandler(this.textPrice_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(20, 53);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 16);
            this.label8.TabIndex = 2;
            this.label8.Text = "Price";
            // 
            // textCellingHeight
            // 
            this.textCellingHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textCellingHeight.Location = new System.Drawing.Point(819, 238);
            this.textCellingHeight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textCellingHeight.Name = "textCellingHeight";
            this.textCellingHeight.ReadOnly = true;
            this.textCellingHeight.Size = new System.Drawing.Size(113, 26);
            this.textCellingHeight.TabIndex = 1;
            this.textCellingHeight.TabStop = false;
            this.textCellingHeight.Enter += new System.EventHandler(this.textCellingHeight_Enter);
            this.textCellingHeight.Leave += new System.EventHandler(this.textCellingHeight_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(676, 239);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Cellings Height";
            // 
            // textFloorName
            // 
            this.textFloorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textFloorName.Location = new System.Drawing.Point(818, 205);
            this.textFloorName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textFloorName.Name = "textFloorName";
            this.textFloorName.ReadOnly = true;
            this.textFloorName.Size = new System.Drawing.Size(113, 26);
            this.textFloorName.TabIndex = 14;
            this.textFloorName.TabStop = false;
            this.textFloorName.Enter += new System.EventHandler(this.textFloorName_Enter);
            this.textFloorName.Leave += new System.EventHandler(this.textFloorName_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(676, 209);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 15);
            this.label3.TabIndex = 13;
            this.label3.Text = "Floor Number/Name";
            // 
            // textSuiteName
            // 
            this.textSuiteName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textSuiteName.Location = new System.Drawing.Point(818, 172);
            this.textSuiteName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textSuiteName.Name = "textSuiteName";
            this.textSuiteName.ReadOnly = true;
            this.textSuiteName.Size = new System.Drawing.Size(113, 26);
            this.textSuiteName.TabIndex = 12;
            this.textSuiteName.TabStop = false;
            this.textSuiteName.Enter += new System.EventHandler(this.textSuiteName_Enter);
            this.textSuiteName.Leave += new System.EventHandler(this.textSuiteName_Leave);
            // 
            // labelSuiteNumber
            // 
            this.labelSuiteNumber.AutoSize = true;
            this.labelSuiteNumber.Location = new System.Drawing.Point(674, 176);
            this.labelSuiteNumber.Name = "labelSuiteNumber";
            this.labelSuiteNumber.Size = new System.Drawing.Size(120, 15);
            this.labelSuiteNumber.TabIndex = 11;
            this.labelSuiteNumber.Text = "Suite Number/Name";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(876, 730);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(81, 30);
            this.buttonCancel.TabIndex = 17;
            this.buttonCancel.Text = "Exit";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // listBoxSuites
            // 
            this.listBoxSuites.ContextMenuStrip = this.contextMenuUnit;
            this.listBoxSuites.Enabled = false;
            this.listBoxSuites.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxSuites.FormattingEnabled = true;
            this.listBoxSuites.ItemHeight = 24;
            this.listBoxSuites.Location = new System.Drawing.Point(17, 176);
            this.listBoxSuites.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBoxSuites.Name = "listBoxSuites";
            this.listBoxSuites.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxSuites.Size = new System.Drawing.Size(627, 580);
            this.listBoxSuites.TabIndex = 4;
            this.listBoxSuites.SelectedIndexChanged += new System.EventHandler(this.listBoxSuites_SelectedIndexChanged);
            // 
            // contextMenuUnit
            // 
            this.contextMenuUnit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreOriginalValueToolStripMenuItem});
            this.contextMenuUnit.Name = "contextMenuUnit";
            this.contextMenuUnit.Size = new System.Drawing.Size(191, 48);
            this.contextMenuUnit.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuUnit_Opening);
            // 
            // restoreOriginalValueToolStripMenuItem
            // 
            this.restoreOriginalValueToolStripMenuItem.Name = "restoreOriginalValueToolStripMenuItem";
            this.restoreOriginalValueToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.restoreOriginalValueToolStripMenuItem.Text = "Restore Original Value";
            this.restoreOriginalValueToolStripMenuItem.Click += new System.EventHandler(this.restoreOriginalValueToolStripMenuItem_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonSortByStatus);
            this.groupBox2.Controls.Add(this.radioButtonSortByFloor);
            this.groupBox2.Controls.Add(this.radioButtonSortByType);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(345, 71);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(250, 62);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sort the Suites by:";
            // 
            // radioButtonSortByStatus
            // 
            this.radioButtonSortByStatus.AutoSize = true;
            this.radioButtonSortByStatus.Location = new System.Drawing.Point(174, 30);
            this.radioButtonSortByStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButtonSortByStatus.Name = "radioButtonSortByStatus";
            this.radioButtonSortByStatus.Size = new System.Drawing.Size(55, 17);
            this.radioButtonSortByStatus.TabIndex = 2;
            this.radioButtonSortByStatus.Text = "Status";
            this.radioButtonSortByStatus.UseVisualStyleBackColor = true;
            this.radioButtonSortByStatus.CheckedChanged += new System.EventHandler(this.radioButtonSortByStatus_CheckedChanged);
            // 
            // radioButtonSortByFloor
            // 
            this.radioButtonSortByFloor.AutoSize = true;
            this.radioButtonSortByFloor.Checked = true;
            this.radioButtonSortByFloor.Location = new System.Drawing.Point(16, 30);
            this.radioButtonSortByFloor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButtonSortByFloor.Name = "radioButtonSortByFloor";
            this.radioButtonSortByFloor.Size = new System.Drawing.Size(48, 17);
            this.radioButtonSortByFloor.TabIndex = 0;
            this.radioButtonSortByFloor.TabStop = true;
            this.radioButtonSortByFloor.Text = "Floor";
            this.radioButtonSortByFloor.UseVisualStyleBackColor = true;
            this.radioButtonSortByFloor.CheckedChanged += new System.EventHandler(this.radioButtonSortByFloor_CheckedChanged);
            // 
            // radioButtonSortByType
            // 
            this.radioButtonSortByType.AutoSize = true;
            this.radioButtonSortByType.Location = new System.Drawing.Point(95, 30);
            this.radioButtonSortByType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButtonSortByType.Name = "radioButtonSortByType";
            this.radioButtonSortByType.Size = new System.Drawing.Size(49, 17);
            this.radioButtonSortByType.TabIndex = 1;
            this.radioButtonSortByType.Text = "Type";
            this.radioButtonSortByType.UseVisualStyleBackColor = true;
            this.radioButtonSortByType.CheckedChanged += new System.EventHandler(this.radioButtonSortByType_CheckedChanged);
            // 
            // buttonUpdateFromCSV
            // 
            this.buttonUpdateFromCSV.Location = new System.Drawing.Point(10, 26);
            this.buttonUpdateFromCSV.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonUpdateFromCSV.Name = "buttonUpdateFromCSV";
            this.buttonUpdateFromCSV.Size = new System.Drawing.Size(128, 30);
            this.buttonUpdateFromCSV.TabIndex = 0;
            this.buttonUpdateFromCSV.Text = "Update from CSV...";
            this.buttonUpdateFromCSV.UseVisualStyleBackColor = true;
            this.buttonUpdateFromCSV.Click += new System.EventHandler(this.buttonUpdateFromCSV_Click);
            // 
            // buttonExportToCSV
            // 
            this.buttonExportToCSV.Location = new System.Drawing.Point(171, 26);
            this.buttonExportToCSV.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonExportToCSV.Name = "buttonExportToCSV";
            this.buttonExportToCSV.Size = new System.Drawing.Size(114, 30);
            this.buttonExportToCSV.TabIndex = 1;
            this.buttonExportToCSV.Text = "Export to CSV...";
            this.buttonExportToCSV.UseVisualStyleBackColor = true;
            this.buttonExportToCSV.Click += new System.EventHandler(this.buttonExportToCSV_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonExportToCSV);
            this.groupBox4.Controls.Add(this.buttonUpdateFromCSV);
            this.groupBox4.Enabled = false;
            this.groupBox4.Location = new System.Drawing.Point(662, 17);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Size = new System.Drawing.Size(299, 64);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "CSV Export/Import";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // comboDevelopers
            // 
            this.comboDevelopers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDevelopers.FormattingEnabled = true;
            this.comboDevelopers.Location = new System.Drawing.Point(17, 36);
            this.comboDevelopers.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboDevelopers.Name = "comboDevelopers";
            this.comboDevelopers.Size = new System.Drawing.Size(301, 23);
            this.comboDevelopers.TabIndex = 18;
            this.comboDevelopers.SelectedIndexChanged += new System.EventHandler(this.comboDevelopers_SelectedIndexChanged);
            // 
            // comboSites
            // 
            this.comboSites.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSites.Enabled = false;
            this.comboSites.FormattingEnabled = true;
            this.comboSites.Location = new System.Drawing.Point(345, 36);
            this.comboSites.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboSites.Name = "comboSites";
            this.comboSites.Size = new System.Drawing.Size(251, 23);
            this.comboSites.TabIndex = 19;
            this.comboSites.SelectedIndexChanged += new System.EventHandler(this.comboSites_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 15);
            this.label5.TabIndex = 20;
            this.label5.Text = "Developer Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(345, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 15);
            this.label6.TabIndex = 21;
            this.label6.Text = "Project Name";
            // 
            // pnlStartupShutdown
            // 
            this.pnlStartupShutdown.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlStartupShutdown.Controls.Add(this.lblStartupShutdown);
            this.pnlStartupShutdown.Location = new System.Drawing.Point(373, 334);
            this.pnlStartupShutdown.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlStartupShutdown.Name = "pnlStartupShutdown";
            this.pnlStartupShutdown.Size = new System.Drawing.Size(393, 186);
            this.pnlStartupShutdown.TabIndex = 22;
            this.pnlStartupShutdown.Visible = false;
            // 
            // lblStartupShutdown
            // 
            this.lblStartupShutdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStartupShutdown.Location = new System.Drawing.Point(0, 0);
            this.lblStartupShutdown.Name = "lblStartupShutdown";
            this.lblStartupShutdown.Size = new System.Drawing.Size(389, 182);
            this.lblStartupShutdown.TabIndex = 0;
            this.lblStartupShutdown.Text = "labelText";
            this.lblStartupShutdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmrStartup
            // 
            this.tmrStartup.Tick += new System.EventHandler(this.tmrStartup_Tick);
            // 
            // buttonApplyChanges
            // 
            this.buttonApplyChanges.Enabled = false;
            this.buttonApplyChanges.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonApplyChanges.Location = new System.Drawing.Point(673, 586);
            this.buttonApplyChanges.Margin = new System.Windows.Forms.Padding(2);
            this.buttonApplyChanges.Name = "buttonApplyChanges";
            this.buttonApplyChanges.Size = new System.Drawing.Size(284, 118);
            this.buttonApplyChanges.TabIndex = 23;
            this.buttonApplyChanges.Text = "No Changes";
            this.buttonApplyChanges.UseVisualStyleBackColor = true;
            this.buttonApplyChanges.Click += new System.EventHandler(this.buttonApplyChanges_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(676, 730);
            this.buttonRefresh.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(89, 32);
            this.buttonRefresh.TabIndex = 24;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Visible = false;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(972, 771);
            this.ControlBox = false;
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonApplyChanges);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboSites);
            this.Controls.Add(this.textCellingHeight);
            this.Controls.Add(this.comboDevelopers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.listBoxSuites);
            this.Controls.Add(this.labelSuiteType);
            this.Controls.Add(this.textFloorName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textSuiteName);
            this.Controls.Add(this.labelSuiteNumber);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupSuiteInfo);
            this.Controls.Add(this.labelSuitesTitle);
            this.Controls.Add(this.comboBuildings);
            this.Controls.Add(this.labelModelFile);
            this.Controls.Add(this.pnlStartupShutdown);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "VRT Manager";
            this.groupSuiteInfo.ResumeLayout(false);
            this.groupSuiteInfo.PerformLayout();
            this.contextMenuUnit.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.pnlStartupShutdown.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelModelFile;
        private System.Windows.Forms.ComboBox comboBuildings;
        private System.Windows.Forms.Label labelSuitesTitle;
        private System.Windows.Forms.Label labelSuiteType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupSuiteInfo;
        private System.Windows.Forms.TextBox textPrice;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textCellingHeight;
        private System.Windows.Forms.TextBox textFloorName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textSuiteName;
        private System.Windows.Forms.Label labelSuiteNumber;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboSaleStatus;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListBox listBoxSuites;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonSortByStatus;
        private System.Windows.Forms.RadioButton radioButtonSortByFloor;
        private System.Windows.Forms.RadioButton radioButtonSortByType;
        private System.Windows.Forms.CheckBox checkBoxShowPanoramicView;
        private System.Windows.Forms.Button buttonUpdateFromCSV;
        private System.Windows.Forms.Button buttonExportToCSV;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ComboBox comboDevelopers;
        private System.Windows.Forms.ComboBox comboSites;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pnlStartupShutdown;
        private System.Windows.Forms.Label lblStartupShutdown;
        private System.Windows.Forms.Timer tmrStartup;
        private System.Windows.Forms.Button buttonApplyChanges;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.ContextMenuStrip contextMenuUnit;
        private System.Windows.Forms.ToolStripMenuItem restoreOriginalValueToolStripMenuItem;
    }
}

