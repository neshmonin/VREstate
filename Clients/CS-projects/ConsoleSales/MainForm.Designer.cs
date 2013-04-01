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
            this.labelSuiteType = new System.Windows.Forms.Label();
            this.groupSuiteInfo = new System.Windows.Forms.GroupBox();
            this.checkBoxShowPanoramicView = new System.Windows.Forms.CheckBox();
            this.comboSaleStatus = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textPrice = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textCellingHeight = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listViewSuites = new System.Windows.Forms.ListView();
            this.columnEditingState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSuiteName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSuiteType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnCealing = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnShowPanoramicView = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuUnit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreOriginalValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.groupBoxSuiteTypeInfo = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxBedrooms = new System.Windows.Forms.TextBox();
            this.textBoxBathrooms = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxBalcony = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxTerrace = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxArea = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupSuiteInfo.SuspendLayout();
            this.contextMenuUnit.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.pnlStartupShutdown.SuspendLayout();
            this.groupBoxSuiteTypeInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelModelFile
            // 
            this.labelModelFile.AutoSize = true;
            this.labelModelFile.Location = new System.Drawing.Point(486, 16);
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
            this.comboBuildings.Location = new System.Drawing.Point(486, 36);
            this.comboBuildings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBuildings.Name = "comboBuildings";
            this.comboBuildings.Size = new System.Drawing.Size(152, 23);
            this.comboBuildings.TabIndex = 2;
            this.comboBuildings.SelectedIndexChanged += new System.EventHandler(this.comboBuildings_SelectedIndexChanged);
            // 
            // labelSuiteType
            // 
            this.labelSuiteType.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSuiteType.Location = new System.Drawing.Point(13, 23);
            this.labelSuiteType.Name = "labelSuiteType";
            this.labelSuiteType.Size = new System.Drawing.Size(256, 36);
            this.labelSuiteType.TabIndex = 10;
            this.labelSuiteType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupSuiteInfo
            // 
            this.groupSuiteInfo.BackColor = System.Drawing.SystemColors.Control;
            this.groupSuiteInfo.Controls.Add(this.checkBoxShowPanoramicView);
            this.groupSuiteInfo.Controls.Add(this.comboSaleStatus);
            this.groupSuiteInfo.Controls.Add(this.label9);
            this.groupSuiteInfo.Controls.Add(this.textPrice);
            this.groupSuiteInfo.Controls.Add(this.label8);
            this.groupSuiteInfo.Controls.Add(this.textCellingHeight);
            this.groupSuiteInfo.Controls.Add(this.label2);
            this.groupSuiteInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupSuiteInfo.Location = new System.Drawing.Point(672, 313);
            this.groupSuiteInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupSuiteInfo.Name = "groupSuiteInfo";
            this.groupSuiteInfo.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupSuiteInfo.Size = new System.Drawing.Size(284, 222);
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
            this.checkBoxShowPanoramicView.Location = new System.Drawing.Point(13, 127);
            this.checkBoxShowPanoramicView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBoxShowPanoramicView.Name = "checkBoxShowPanoramicView";
            this.checkBoxShowPanoramicView.Size = new System.Drawing.Size(256, 28);
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
            this.comboSaleStatus.Location = new System.Drawing.Point(162, 78);
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
            this.label9.Location = new System.Drawing.Point(16, 84);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(132, 16);
            this.label9.TabIndex = 5;
            this.label9.Text = "Availability Status";
            // 
            // textPrice
            // 
            this.textPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textPrice.Location = new System.Drawing.Point(159, 32);
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
            this.label8.Location = new System.Drawing.Point(16, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 16);
            this.label8.TabIndex = 2;
            this.label8.Text = "Price";
            // 
            // textCellingHeight
            // 
            this.textCellingHeight.BackColor = System.Drawing.SystemColors.Window;
            this.textCellingHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textCellingHeight.Location = new System.Drawing.Point(159, 173);
            this.textCellingHeight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textCellingHeight.Name = "textCellingHeight";
            this.textCellingHeight.Size = new System.Drawing.Size(110, 26);
            this.textCellingHeight.TabIndex = 1;
            this.textCellingHeight.TabStop = false;
            this.textCellingHeight.Enter += new System.EventHandler(this.textCellingHeight_Enter);
            this.textCellingHeight.Leave += new System.EventHandler(this.textCellingHeight_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(16, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Cellings Height";
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
            // listViewSuites
            // 
            this.listViewSuites.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.listViewSuites.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnEditingState,
            this.columnSuiteName,
            this.columnSuiteType,
            this.columnCealing,
            this.columnPrice,
            this.columnStatus,
            this.columnShowPanoramicView});
            this.listViewSuites.ContextMenuStrip = this.contextMenuUnit;
            this.listViewSuites.Enabled = false;
            this.listViewSuites.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewSuites.FullRowSelect = true;
            this.listViewSuites.GridLines = true;
            this.listViewSuites.HideSelection = false;
            this.listViewSuites.Location = new System.Drawing.Point(17, 77);
            this.listViewSuites.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listViewSuites.Name = "listViewSuites";
            this.listViewSuites.ShowGroups = false;
            this.listViewSuites.Size = new System.Drawing.Size(639, 679);
            this.listViewSuites.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewSuites.TabIndex = 4;
            this.listViewSuites.UseCompatibleStateImageBehavior = false;
            this.listViewSuites.View = System.Windows.Forms.View.Details;
            this.listViewSuites.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewSuites_ColumnClick);
            this.listViewSuites.SelectedIndexChanged += new System.EventHandler(this.listViewSuites_SelectedIndexChanged);
            // 
            // columnEditingState
            // 
            this.columnEditingState.Text = " *";
            this.columnEditingState.Width = 40;
            // 
            // columnSuiteName
            // 
            this.columnSuiteName.Text = "Suite";
            this.columnSuiteName.Width = 75;
            // 
            // columnSuiteType
            // 
            this.columnSuiteType.Text = "Type";
            this.columnSuiteType.Width = 154;
            // 
            // columnCealing
            // 
            this.columnCealing.DisplayIndex = 4;
            this.columnCealing.Text = "Hght";
            this.columnCealing.Width = 57;
            // 
            // columnPrice
            // 
            this.columnPrice.DisplayIndex = 3;
            this.columnPrice.Text = "Price";
            this.columnPrice.Width = 122;
            // 
            // columnStatus
            // 
            this.columnStatus.Text = "Status";
            this.columnStatus.Width = 105;
            // 
            // columnShowPanoramicView
            // 
            this.columnShowPanoramicView.Text = "Show";
            this.columnShowPanoramicView.Width = 63;
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
            // buttonUpdateFromCSV
            // 
            this.buttonUpdateFromCSV.Location = new System.Drawing.Point(19, 22);
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
            this.buttonExportToCSV.Location = new System.Drawing.Point(158, 22);
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
            this.groupBox4.Location = new System.Drawing.Point(672, 17);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Size = new System.Drawing.Size(289, 64);
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
            this.comboDevelopers.Size = new System.Drawing.Size(214, 23);
            this.comboDevelopers.TabIndex = 18;
            this.comboDevelopers.SelectedIndexChanged += new System.EventHandler(this.comboDevelopers_SelectedIndexChanged);
            // 
            // comboSites
            // 
            this.comboSites.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSites.Enabled = false;
            this.comboSites.FormattingEnabled = true;
            this.comboSites.Location = new System.Drawing.Point(261, 36);
            this.comboSites.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboSites.Name = "comboSites";
            this.comboSites.Size = new System.Drawing.Size(198, 23);
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
            this.label6.Location = new System.Drawing.Point(258, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 15);
            this.label6.TabIndex = 21;
            this.label6.Text = "Project Name";
            // 
            // pnlStartupShutdown
            // 
            this.pnlStartupShutdown.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlStartupShutdown.Controls.Add(this.lblStartupShutdown);
            this.pnlStartupShutdown.Location = new System.Drawing.Point(317, 313);
            this.pnlStartupShutdown.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlStartupShutdown.Name = "pnlStartupShutdown";
            this.pnlStartupShutdown.Size = new System.Drawing.Size(321, 186);
            this.pnlStartupShutdown.TabIndex = 22;
            this.pnlStartupShutdown.Visible = false;
            // 
            // lblStartupShutdown
            // 
            this.lblStartupShutdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStartupShutdown.Location = new System.Drawing.Point(0, 0);
            this.lblStartupShutdown.Name = "lblStartupShutdown";
            this.lblStartupShutdown.Size = new System.Drawing.Size(317, 182);
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
            // groupBoxSuiteTypeInfo
            // 
            this.groupBoxSuiteTypeInfo.Controls.Add(this.textBoxArea);
            this.groupBoxSuiteTypeInfo.Controls.Add(this.label10);
            this.groupBoxSuiteTypeInfo.Controls.Add(this.textBoxTerrace);
            this.groupBoxSuiteTypeInfo.Controls.Add(this.label7);
            this.groupBoxSuiteTypeInfo.Controls.Add(this.textBoxBalcony);
            this.groupBoxSuiteTypeInfo.Controls.Add(this.label4);
            this.groupBoxSuiteTypeInfo.Controls.Add(this.textBoxBathrooms);
            this.groupBoxSuiteTypeInfo.Controls.Add(this.label3);
            this.groupBoxSuiteTypeInfo.Controls.Add(this.textBoxBedrooms);
            this.groupBoxSuiteTypeInfo.Controls.Add(this.label1);
            this.groupBoxSuiteTypeInfo.Controls.Add(this.labelSuiteType);
            this.groupBoxSuiteTypeInfo.Location = new System.Drawing.Point(673, 88);
            this.groupBoxSuiteTypeInfo.Name = "groupBoxSuiteTypeInfo";
            this.groupBoxSuiteTypeInfo.Size = new System.Drawing.Size(283, 184);
            this.groupBoxSuiteTypeInfo.TabIndex = 25;
            this.groupBoxSuiteTypeInfo.TabStop = false;
            this.groupBoxSuiteTypeInfo.Text = "Suite Type Info";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "Bedrooms:";
            // 
            // textBoxBedrooms
            // 
            this.textBoxBedrooms.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxBedrooms.Location = new System.Drawing.Point(90, 70);
            this.textBoxBedrooms.Name = "textBoxBedrooms";
            this.textBoxBedrooms.ReadOnly = true;
            this.textBoxBedrooms.Size = new System.Drawing.Size(57, 14);
            this.textBoxBedrooms.TabIndex = 12;
            this.textBoxBedrooms.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxBathrooms
            // 
            this.textBoxBathrooms.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxBathrooms.Location = new System.Drawing.Point(90, 97);
            this.textBoxBathrooms.Name = "textBoxBathrooms";
            this.textBoxBathrooms.ReadOnly = true;
            this.textBoxBathrooms.Size = new System.Drawing.Size(57, 14);
            this.textBoxBathrooms.TabIndex = 14;
            this.textBoxBathrooms.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 13;
            this.label3.Text = "Bathrooms";
            // 
            // textBoxBalcony
            // 
            this.textBoxBalcony.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxBalcony.Location = new System.Drawing.Point(90, 125);
            this.textBoxBalcony.Name = "textBoxBalcony";
            this.textBoxBalcony.ReadOnly = true;
            this.textBoxBalcony.Size = new System.Drawing.Size(57, 14);
            this.textBoxBalcony.TabIndex = 16;
            this.textBoxBalcony.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 15);
            this.label4.TabIndex = 15;
            this.label4.Text = "Balcony";
            // 
            // textBoxTerrace
            // 
            this.textBoxTerrace.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxTerrace.Location = new System.Drawing.Point(91, 154);
            this.textBoxTerrace.Name = "textBoxTerrace";
            this.textBoxTerrace.ReadOnly = true;
            this.textBoxTerrace.Size = new System.Drawing.Size(57, 14);
            this.textBoxTerrace.TabIndex = 18;
            this.textBoxTerrace.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 153);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 15);
            this.label7.TabIndex = 17;
            this.label7.Text = "Terrace";
            // 
            // textBoxArea
            // 
            this.textBoxArea.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxArea.Location = new System.Drawing.Point(214, 110);
            this.textBoxArea.Name = "textBoxArea";
            this.textBoxArea.ReadOnly = true;
            this.textBoxArea.Size = new System.Drawing.Size(57, 14);
            this.textBoxArea.TabIndex = 20;
            this.textBoxArea.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(178, 109);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 15);
            this.label10.TabIndex = 19;
            this.label10.Text = "Area";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(972, 771);
            this.ControlBox = false;
            this.Controls.Add(this.groupBoxSuiteTypeInfo);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonApplyChanges);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboSites);
            this.Controls.Add(this.comboDevelopers);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.listViewSuites);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupSuiteInfo);
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
            this.groupBox4.ResumeLayout(false);
            this.pnlStartupShutdown.ResumeLayout(false);
            this.groupBoxSuiteTypeInfo.ResumeLayout(false);
            this.groupBoxSuiteTypeInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelModelFile;
        private System.Windows.Forms.ComboBox comboBuildings;
        private System.Windows.Forms.Label labelSuiteType;
        private System.Windows.Forms.GroupBox groupSuiteInfo;
        private System.Windows.Forms.TextBox textPrice;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textCellingHeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboSaleStatus;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListView listViewSuites;
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
        private System.Windows.Forms.ColumnHeader columnEditingState;
        private System.Windows.Forms.ColumnHeader columnSuiteName;
        private System.Windows.Forms.ColumnHeader columnSuiteType;
        private System.Windows.Forms.ColumnHeader columnPrice;
        private System.Windows.Forms.ColumnHeader columnCealing;
        private System.Windows.Forms.ColumnHeader columnStatus;
        private System.Windows.Forms.ColumnHeader columnShowPanoramicView;
        private System.Windows.Forms.GroupBox groupBoxSuiteTypeInfo;
        private System.Windows.Forms.TextBox textBoxTerrace;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxBalcony;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxBathrooms;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxBedrooms;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxArea;
        private System.Windows.Forms.Label label10;
    }
}

