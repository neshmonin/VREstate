namespace KioskManager
{
    partial class KioskManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KioskManager));
            this.labelModelFile = new System.Windows.Forms.Label();
            this.comboBuildings = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonStartApp = new System.Windows.Forms.Button();
            this.buttonStopApp = new System.Windows.Forms.Button();
            this.buttonSaveToModel = new System.Windows.Forms.Button();
            this.listBoxSuites = new System.Windows.Forms.ListBox();
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
            this.groupSuiteInfo.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelModelFile
            // 
            this.labelModelFile.AutoSize = true;
            this.labelModelFile.Location = new System.Drawing.Point(15, 67);
            this.labelModelFile.Name = "labelModelFile";
            this.labelModelFile.Size = new System.Drawing.Size(78, 17);
            this.labelModelFile.TabIndex = 1;
            this.labelModelFile.Text = "Building Name";
            this.labelModelFile.UseCompatibleTextRendering = true;
            // 
            // comboBuildings
            // 
            this.comboBuildings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBuildings.Enabled = false;
            this.comboBuildings.FormattingEnabled = true;
            this.comboBuildings.Location = new System.Drawing.Point(15, 83);
            this.comboBuildings.Name = "comboBuildings";
            this.comboBuildings.Size = new System.Drawing.Size(131, 21);
            this.comboBuildings.TabIndex = 2;
            this.comboBuildings.SelectedIndexChanged += new System.EventHandler(this.comboBuildings_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Suites";
            // 
            // labelSuiteType
            // 
            this.labelSuiteType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelSuiteType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSuiteType.Location = new System.Drawing.Point(492, 109);
            this.labelSuiteType.Name = "labelSuiteType";
            this.labelSuiteType.Size = new System.Drawing.Size(146, 32);
            this.labelSuiteType.TabIndex = 10;
            this.labelSuiteType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(403, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Suite Type";
            // 
            // groupSuiteInfo
            // 
            this.groupSuiteInfo.Controls.Add(this.checkBoxShowPanoramicView);
            this.groupSuiteInfo.Controls.Add(this.comboSaleStatus);
            this.groupSuiteInfo.Controls.Add(this.label9);
            this.groupSuiteInfo.Controls.Add(this.textPrice);
            this.groupSuiteInfo.Controls.Add(this.label8);
            this.groupSuiteInfo.Controls.Add(this.textCellingHeight);
            this.groupSuiteInfo.Controls.Add(this.label2);
            this.groupSuiteInfo.Location = new System.Drawing.Point(389, 211);
            this.groupSuiteInfo.Name = "groupSuiteInfo";
            this.groupSuiteInfo.Size = new System.Drawing.Size(252, 139);
            this.groupSuiteInfo.TabIndex = 5;
            this.groupSuiteInfo.TabStop = false;
            this.groupSuiteInfo.Text = "Suite Editable Info";
            // 
            // checkBoxShowPanoramicView
            // 
            this.checkBoxShowPanoramicView.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxShowPanoramicView.Checked = true;
            this.checkBoxShowPanoramicView.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBoxShowPanoramicView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBoxShowPanoramicView.Location = new System.Drawing.Point(15, 78);
            this.checkBoxShowPanoramicView.Name = "checkBoxShowPanoramicView";
            this.checkBoxShowPanoramicView.Size = new System.Drawing.Size(175, 24);
            this.checkBoxShowPanoramicView.TabIndex = 4;
            this.checkBoxShowPanoramicView.Text = "Show Panoramic View";
            this.checkBoxShowPanoramicView.UseVisualStyleBackColor = true;
            this.checkBoxShowPanoramicView.CheckedChanged += new System.EventHandler(this.checkBoxShowPanoramicView_CheckedChanged);
            // 
            // comboSaleStatus
            // 
            this.comboSaleStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSaleStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboSaleStatus.FormattingEnabled = true;
            this.comboSaleStatus.Location = new System.Drawing.Point(142, 103);
            this.comboSaleStatus.Name = "comboSaleStatus";
            this.comboSaleStatus.Size = new System.Drawing.Size(94, 24);
            this.comboSaleStatus.Sorted = true;
            this.comboSaleStatus.TabIndex = 6;
            this.comboSaleStatus.SelectedIndexChanged += new System.EventHandler(this.comboSaleStatus_SelectedIndexChanged);
            this.comboSaleStatus.Enter += new System.EventHandler(this.comboSaleStatus_Enter);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(17, 111);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Sale Status";
            // 
            // textPrice
            // 
            this.textPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textPrice.Location = new System.Drawing.Point(140, 51);
            this.textPrice.Name = "textPrice";
            this.textPrice.Size = new System.Drawing.Size(95, 22);
            this.textPrice.TabIndex = 3;
            this.textPrice.TextChanged += new System.EventHandler(this.textPrice_TextChanged);
            this.textPrice.Enter += new System.EventHandler(this.textPrice_Enter);
            this.textPrice.Leave += new System.EventHandler(this.textPrice_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(17, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Price";
            // 
            // textCellingHeight
            // 
            this.textCellingHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textCellingHeight.Location = new System.Drawing.Point(140, 20);
            this.textCellingHeight.Name = "textCellingHeight";
            this.textCellingHeight.Size = new System.Drawing.Size(97, 22);
            this.textCellingHeight.TabIndex = 1;
            this.textCellingHeight.Enter += new System.EventHandler(this.textCellingHeight_Enter);
            this.textCellingHeight.Leave += new System.EventHandler(this.textCellingHeight_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(17, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Cellings Height";
            // 
            // textFloorName
            // 
            this.textFloorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textFloorName.Location = new System.Drawing.Point(526, 175);
            this.textFloorName.Name = "textFloorName";
            this.textFloorName.ReadOnly = true;
            this.textFloorName.Size = new System.Drawing.Size(97, 22);
            this.textFloorName.TabIndex = 14;
            this.textFloorName.Enter += new System.EventHandler(this.textFloorName_Enter);
            this.textFloorName.Leave += new System.EventHandler(this.textFloorName_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(404, 178);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Floor Number/Name";
            // 
            // textSuiteName
            // 
            this.textSuiteName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textSuiteName.Location = new System.Drawing.Point(526, 149);
            this.textSuiteName.Name = "textSuiteName";
            this.textSuiteName.ReadOnly = true;
            this.textSuiteName.Size = new System.Drawing.Size(97, 22);
            this.textSuiteName.TabIndex = 12;
            this.textSuiteName.Enter += new System.EventHandler(this.textSuiteName_Enter);
            this.textSuiteName.Leave += new System.EventHandler(this.textSuiteName_Leave);
            // 
            // labelSuiteNumber
            // 
            this.labelSuiteNumber.AutoSize = true;
            this.labelSuiteNumber.Location = new System.Drawing.Point(403, 152);
            this.labelSuiteNumber.Name = "labelSuiteNumber";
            this.labelSuiteNumber.Size = new System.Drawing.Size(104, 13);
            this.labelSuiteNumber.TabIndex = 11;
            this.labelSuiteNumber.Text = "Suite Number/Name";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(572, 622);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(69, 26);
            this.buttonCancel.TabIndex = 17;
            this.buttonCancel.Text = "Exit";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonStartApp);
            this.groupBox1.Controls.Add(this.buttonStopApp);
            this.groupBox1.Location = new System.Drawing.Point(430, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 72);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Kiosk Execution Control";
            // 
            // buttonStartApp
            // 
            this.buttonStartApp.Enabled = false;
            this.buttonStartApp.Location = new System.Drawing.Point(114, 23);
            this.buttonStartApp.Name = "buttonStartApp";
            this.buttonStartApp.Size = new System.Drawing.Size(76, 29);
            this.buttonStartApp.TabIndex = 1;
            this.buttonStartApp.Text = "Start";
            this.buttonStartApp.UseVisualStyleBackColor = true;
            this.buttonStartApp.Click += new System.EventHandler(this.buttonStartApp_Click);
            // 
            // buttonStopApp
            // 
            this.buttonStopApp.Enabled = false;
            this.buttonStopApp.Location = new System.Drawing.Point(17, 23);
            this.buttonStopApp.Name = "buttonStopApp";
            this.buttonStopApp.Size = new System.Drawing.Size(73, 29);
            this.buttonStopApp.TabIndex = 0;
            this.buttonStopApp.Text = "Terminate";
            this.buttonStopApp.UseVisualStyleBackColor = true;
            this.buttonStopApp.Click += new System.EventHandler(this.buttonStopApp_Click);
            // 
            // buttonSaveToModel
            // 
            this.buttonSaveToModel.Enabled = false;
            this.buttonSaveToModel.Location = new System.Drawing.Point(392, 358);
            this.buttonSaveToModel.Name = "buttonSaveToModel";
            this.buttonSaveToModel.Size = new System.Drawing.Size(249, 37);
            this.buttonSaveToModel.TabIndex = 6;
            this.buttonSaveToModel.Text = "Save all Accumulated Changes to the Model";
            this.buttonSaveToModel.UseVisualStyleBackColor = true;
            this.buttonSaveToModel.Click += new System.EventHandler(this.buttonSaveToModel_Click);
            // 
            // listBoxSuites
            // 
            this.listBoxSuites.Enabled = false;
            this.listBoxSuites.FormattingEnabled = true;
            this.listBoxSuites.Location = new System.Drawing.Point(15, 131);
            this.listBoxSuites.Name = "listBoxSuites";
            this.listBoxSuites.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxSuites.Size = new System.Drawing.Size(361, 524);
            this.listBoxSuites.TabIndex = 4;
            this.listBoxSuites.SelectedIndexChanged += new System.EventHandler(this.listBoxSuites_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonSortByStatus);
            this.groupBox2.Controls.Add(this.radioButtonSortByFloor);
            this.groupBox2.Controls.Add(this.radioButtonSortByType);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(161, 65);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(215, 54);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sort the Suites by:";
            // 
            // radioButtonSortByStatus
            // 
            this.radioButtonSortByStatus.AutoSize = true;
            this.radioButtonSortByStatus.Location = new System.Drawing.Point(149, 26);
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
            this.radioButtonSortByFloor.Location = new System.Drawing.Point(13, 26);
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
            this.radioButtonSortByType.Location = new System.Drawing.Point(81, 26);
            this.radioButtonSortByType.Name = "radioButtonSortByType";
            this.radioButtonSortByType.Size = new System.Drawing.Size(49, 17);
            this.radioButtonSortByType.TabIndex = 1;
            this.radioButtonSortByType.Text = "Type";
            this.radioButtonSortByType.UseVisualStyleBackColor = true;
            this.radioButtonSortByType.CheckedChanged += new System.EventHandler(this.radioButtonSortByType_CheckedChanged);
            // 
            // buttonUpdateFromCSV
            // 
            this.buttonUpdateFromCSV.Location = new System.Drawing.Point(12, 19);
            this.buttonUpdateFromCSV.Name = "buttonUpdateFromCSV";
            this.buttonUpdateFromCSV.Size = new System.Drawing.Size(117, 26);
            this.buttonUpdateFromCSV.TabIndex = 0;
            this.buttonUpdateFromCSV.Text = "Update from CSV...";
            this.buttonUpdateFromCSV.UseVisualStyleBackColor = true;
            this.buttonUpdateFromCSV.Click += new System.EventHandler(this.buttonUpdateFromCSV_Click);
            // 
            // buttonExportToCSV
            // 
            this.buttonExportToCSV.Location = new System.Drawing.Point(135, 19);
            this.buttonExportToCSV.Name = "buttonExportToCSV";
            this.buttonExportToCSV.Size = new System.Drawing.Size(105, 26);
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
            this.groupBox4.Location = new System.Drawing.Point(391, 456);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(252, 55);
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
            this.comboDevelopers.Location = new System.Drawing.Point(15, 31);
            this.comboDevelopers.Name = "comboDevelopers";
            this.comboDevelopers.Size = new System.Drawing.Size(180, 21);
            this.comboDevelopers.TabIndex = 18;
            this.comboDevelopers.SelectedIndexChanged += new System.EventHandler(this.comboDevelopers_SelectedIndexChanged);
            // 
            // comboSites
            // 
            this.comboSites.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSites.Enabled = false;
            this.comboSites.FormattingEnabled = true;
            this.comboSites.Location = new System.Drawing.Point(201, 31);
            this.comboSites.Name = "comboSites";
            this.comboSites.Size = new System.Drawing.Size(174, 21);
            this.comboSites.TabIndex = 19;
            this.comboSites.SelectedIndexChanged += new System.EventHandler(this.comboSites_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Developer Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(201, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Project Name";
            // 
            // KioskManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(655, 668);
            this.ControlBox = false;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboSites);
            this.Controls.Add(this.comboDevelopers);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.listBoxSuites);
            this.Controls.Add(this.buttonSaveToModel);
            this.Controls.Add(this.labelSuiteType);
            this.Controls.Add(this.textFloorName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textSuiteName);
            this.Controls.Add(this.labelSuiteNumber);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupSuiteInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBuildings);
            this.Controls.Add(this.labelModelFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KioskManager";
            this.Text = "3D Kiosk Manager";
            this.groupSuiteInfo.ResumeLayout(false);
            this.groupSuiteInfo.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelModelFile;
        private System.Windows.Forms.ComboBox comboBuildings;
        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonStartApp;
        private System.Windows.Forms.Button buttonStopApp;
        private System.Windows.Forms.ComboBox comboSaleStatus;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonSaveToModel;
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
    }
}

