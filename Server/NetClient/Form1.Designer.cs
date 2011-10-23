namespace VRE_Client
{
    partial class Form1
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
            this.tmrStartup = new System.Windows.Forms.Timer(this.components);
            this.pnlStartupShutdown = new System.Windows.Forms.Panel();
            this.lblStartupShutdown = new System.Windows.Forms.Label();
            this.cbBuildingList = new System.Windows.Forms.ComboBox();
            this.lvSuiteList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnUiRevert = new System.Windows.Forms.Button();
            this.btnUiUpdate = new System.Windows.Forms.Button();
            this.cbxUiShowPanoramicView = new System.Windows.Forms.CheckBox();
            this.cbUiSaleStatus = new System.Windows.Forms.ComboBox();
            this.tbUiCeilingHeight = new System.Windows.Forms.TextBox();
            this.tbUiUnitPrice = new System.Windows.Forms.TextBox();
            this.tbUiUnitNumber = new System.Windows.Forms.TextBox();
            this.lblUiUnitType = new System.Windows.Forms.Label();
            this.lblUiFloorName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.pnlStartupShutdown.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmrStartup
            // 
            this.tmrStartup.Tick += new System.EventHandler(this.tmrStartup_Tick);
            // 
            // pnlStartupShutdown
            // 
            this.pnlStartupShutdown.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlStartupShutdown.Controls.Add(this.lblStartupShutdown);
            this.pnlStartupShutdown.Location = new System.Drawing.Point(353, 302);
            this.pnlStartupShutdown.Name = "pnlStartupShutdown";
            this.pnlStartupShutdown.Size = new System.Drawing.Size(200, 100);
            this.pnlStartupShutdown.TabIndex = 0;
            this.pnlStartupShutdown.Visible = false;
            // 
            // lblStartupShutdown
            // 
            this.lblStartupShutdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStartupShutdown.Location = new System.Drawing.Point(0, 0);
            this.lblStartupShutdown.Name = "lblStartupShutdown";
            this.lblStartupShutdown.Size = new System.Drawing.Size(196, 96);
            this.lblStartupShutdown.TabIndex = 0;
            this.lblStartupShutdown.Text = "label1";
            this.lblStartupShutdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbBuildingList
            // 
            this.cbBuildingList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBuildingList.FormattingEnabled = true;
            this.cbBuildingList.Location = new System.Drawing.Point(12, 12);
            this.cbBuildingList.Name = "cbBuildingList";
            this.cbBuildingList.Size = new System.Drawing.Size(292, 21);
            this.cbBuildingList.TabIndex = 0;
            this.cbBuildingList.SelectedIndexChanged += new System.EventHandler(this.cbBuildingList_SelectedIndexChanged);
            // 
            // lvSuiteList
            // 
            this.lvSuiteList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSuiteList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvSuiteList.FullRowSelect = true;
            this.lvSuiteList.HideSelection = false;
            this.lvSuiteList.Location = new System.Drawing.Point(12, 39);
            this.lvSuiteList.Name = "lvSuiteList";
            this.lvSuiteList.Size = new System.Drawing.Size(418, 510);
            this.lvSuiteList.TabIndex = 4;
            this.lvSuiteList.UseCompatibleStateImageBehavior = false;
            this.lvSuiteList.View = System.Windows.Forms.View.Details;
            this.lvSuiteList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvSuiteList_ColumnClick);
            this.lvSuiteList.SelectedIndexChanged += new System.EventHandler(this.lvSuiteList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Number";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Ceiling Height, ft";
            this.columnHeader2.Width = 90;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Status";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Current Price";
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Suite Type";
            this.columnHeader5.Width = 69;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.btnUiRevert);
            this.panel1.Controls.Add(this.btnUiUpdate);
            this.panel1.Controls.Add(this.cbxUiShowPanoramicView);
            this.panel1.Controls.Add(this.cbUiSaleStatus);
            this.panel1.Controls.Add(this.tbUiCeilingHeight);
            this.panel1.Controls.Add(this.tbUiUnitPrice);
            this.panel1.Controls.Add(this.tbUiUnitNumber);
            this.panel1.Controls.Add(this.lblUiUnitType);
            this.panel1.Controls.Add(this.lblUiFloorName);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(436, 39);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(227, 510);
            this.panel1.TabIndex = 5;
            // 
            // btnUiRevert
            // 
            this.btnUiRevert.Location = new System.Drawing.Point(133, 210);
            this.btnUiRevert.Name = "btnUiRevert";
            this.btnUiRevert.Size = new System.Drawing.Size(75, 23);
            this.btnUiRevert.TabIndex = 6;
            this.btnUiRevert.Text = "Revert";
            this.btnUiRevert.UseVisualStyleBackColor = true;
            this.btnUiRevert.Click += new System.EventHandler(this.btnUiRevert_Click);
            // 
            // btnUiUpdate
            // 
            this.btnUiUpdate.Location = new System.Drawing.Point(52, 210);
            this.btnUiUpdate.Name = "btnUiUpdate";
            this.btnUiUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUiUpdate.TabIndex = 5;
            this.btnUiUpdate.Text = "Update";
            this.btnUiUpdate.UseVisualStyleBackColor = true;
            this.btnUiUpdate.Click += new System.EventHandler(this.btnUiUpdate_Click);
            // 
            // cbxUiShowPanoramicView
            // 
            this.cbxUiShowPanoramicView.AutoSize = true;
            this.cbxUiShowPanoramicView.Location = new System.Drawing.Point(18, 177);
            this.cbxUiShowPanoramicView.Name = "cbxUiShowPanoramicView";
            this.cbxUiShowPanoramicView.Size = new System.Drawing.Size(130, 17);
            this.cbxUiShowPanoramicView.TabIndex = 4;
            this.cbxUiShowPanoramicView.Text = "Show panoramic view";
            this.cbxUiShowPanoramicView.UseVisualStyleBackColor = true;
            // 
            // cbUiSaleStatus
            // 
            this.cbUiSaleStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUiSaleStatus.FormattingEnabled = true;
            this.cbUiSaleStatus.Items.AddRange(new object[] {
            "Available",
            "On hold",
            "Sold"});
            this.cbUiSaleStatus.Location = new System.Drawing.Point(102, 146);
            this.cbUiSaleStatus.Name = "cbUiSaleStatus";
            this.cbUiSaleStatus.Size = new System.Drawing.Size(106, 21);
            this.cbUiSaleStatus.TabIndex = 3;
            // 
            // tbUiCeilingHeight
            // 
            this.tbUiCeilingHeight.Location = new System.Drawing.Point(102, 119);
            this.tbUiCeilingHeight.Name = "tbUiCeilingHeight";
            this.tbUiCeilingHeight.Size = new System.Drawing.Size(106, 20);
            this.tbUiCeilingHeight.TabIndex = 2;
            this.tbUiCeilingHeight.Validating += new System.ComponentModel.CancelEventHandler(this.tbUiCeilingHeight_Validating);
            // 
            // tbUiUnitPrice
            // 
            this.tbUiUnitPrice.Location = new System.Drawing.Point(102, 92);
            this.tbUiUnitPrice.Name = "tbUiUnitPrice";
            this.tbUiUnitPrice.Size = new System.Drawing.Size(106, 20);
            this.tbUiUnitPrice.TabIndex = 1;
            this.tbUiUnitPrice.Validating += new System.ComponentModel.CancelEventHandler(this.tbUiUnitPrice_Validating);
            // 
            // tbUiUnitNumber
            // 
            this.tbUiUnitNumber.Location = new System.Drawing.Point(102, 38);
            this.tbUiUnitNumber.Name = "tbUiUnitNumber";
            this.tbUiUnitNumber.Size = new System.Drawing.Size(106, 20);
            this.tbUiUnitNumber.TabIndex = 0;
            this.tbUiUnitNumber.Validating += new System.ComponentModel.CancelEventHandler(this.tbUiUnitNumber_Validating);
            // 
            // lblUiUnitType
            // 
            this.lblUiUnitType.AutoSize = true;
            this.lblUiUnitType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblUiUnitType.Location = new System.Drawing.Point(99, 68);
            this.lblUiUnitType.Name = "lblUiUnitType";
            this.lblUiUnitType.Size = new System.Drawing.Size(73, 13);
            this.lblUiUnitType.TabIndex = 0;
            this.lblUiUnitType.Text = "Floor name:";
            // 
            // lblUiFloorName
            // 
            this.lblUiFloorName.AutoSize = true;
            this.lblUiFloorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblUiFloorName.Location = new System.Drawing.Point(99, 14);
            this.lblUiFloorName.Name = "lblUiFloorName";
            this.lblUiFloorName.Size = new System.Drawing.Size(73, 13);
            this.lblUiFloorName.TabIndex = 0;
            this.lblUiFloorName.Text = "Floor name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Unit type:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 149);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Sale status:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 122);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Ceiling height, ft:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Unit price:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Unit number:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Floor name:";
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(310, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(114, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save to server";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExport
            // 
            this.btnExport.Enabled = false;
            this.btnExport.Location = new System.Drawing.Point(430, 10);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(511, 10);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 3;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 561);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lvSuiteList);
            this.Controls.Add(this.cbBuildingList);
            this.Controls.Add(this.pnlStartupShutdown);
            this.Name = "Form1";
            this.Text = "Form1";
            this.pnlStartupShutdown.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tmrStartup;
        private System.Windows.Forms.Panel pnlStartupShutdown;
        private System.Windows.Forms.Label lblStartupShutdown;
        private System.Windows.Forms.ComboBox cbBuildingList;
        private System.Windows.Forms.ListView lvSuiteList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbUiUnitNumber;
        private System.Windows.Forms.Label lblUiUnitType;
        private System.Windows.Forms.Label lblUiFloorName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbxUiShowPanoramicView;
        private System.Windows.Forms.ComboBox cbUiSaleStatus;
        private System.Windows.Forms.TextBox tbUiCeilingHeight;
        private System.Windows.Forms.TextBox tbUiUnitPrice;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnUiRevert;
        private System.Windows.Forms.Button btnUiUpdate;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImport;
    }
}

