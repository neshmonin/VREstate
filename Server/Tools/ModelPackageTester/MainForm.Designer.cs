namespace ModelPackageTester
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.lblModelPath = new System.Windows.Forms.Label();
            this.btnBrowseModel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSuiteTypeInfoPath = new System.Windows.Forms.Label();
            this.btnBrowseSuiteTypeInfo = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lblFloorPlansPath = new System.Windows.Forms.Label();
            this.btnBrowseFloorPlanFolder = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.tbResults = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGuessSuiteTypeInfo = new System.Windows.Forms.Button();
            this.btnGuessFloorPlanFolder = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnImport = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "KMZ file:";
            // 
            // lblModelPath
            // 
            this.lblModelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblModelPath.AutoEllipsis = true;
            this.lblModelPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblModelPath.Location = new System.Drawing.Point(107, 9);
            this.lblModelPath.Name = "lblModelPath";
            this.lblModelPath.Size = new System.Drawing.Size(661, 13);
            this.lblModelPath.TabIndex = 0;
            this.lblModelPath.Text = "drop file here or press Browse -->";
            // 
            // btnBrowseModel
            // 
            this.btnBrowseModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseModel.Location = new System.Drawing.Point(806, 4);
            this.btnBrowseModel.Name = "btnBrowseModel";
            this.btnBrowseModel.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseModel.TabIndex = 0;
            this.btnBrowseModel.Text = "Browse";
            this.btnBrowseModel.UseVisualStyleBackColor = true;
            this.btnBrowseModel.Click += new System.EventHandler(this.btnBrowseModel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "CSV file:";
            // 
            // lblSuiteTypeInfoPath
            // 
            this.lblSuiteTypeInfoPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSuiteTypeInfoPath.AutoEllipsis = true;
            this.lblSuiteTypeInfoPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSuiteTypeInfoPath.Location = new System.Drawing.Point(107, 35);
            this.lblSuiteTypeInfoPath.Name = "lblSuiteTypeInfoPath";
            this.lblSuiteTypeInfoPath.Size = new System.Drawing.Size(661, 13);
            this.lblSuiteTypeInfoPath.TabIndex = 0;
            this.lblSuiteTypeInfoPath.Text = "drop file here or press Browse -->";
            // 
            // btnBrowseSuiteTypeInfo
            // 
            this.btnBrowseSuiteTypeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSuiteTypeInfo.Location = new System.Drawing.Point(806, 30);
            this.btnBrowseSuiteTypeInfo.Name = "btnBrowseSuiteTypeInfo";
            this.btnBrowseSuiteTypeInfo.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseSuiteTypeInfo.TabIndex = 2;
            this.btnBrowseSuiteTypeInfo.Text = "Browse";
            this.btnBrowseSuiteTypeInfo.UseVisualStyleBackColor = true;
            this.btnBrowseSuiteTypeInfo.Click += new System.EventHandler(this.btnBrowseSuiteTypeInfo_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Floorplans folder:";
            // 
            // lblFloorPlansPath
            // 
            this.lblFloorPlansPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFloorPlansPath.AutoEllipsis = true;
            this.lblFloorPlansPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblFloorPlansPath.Location = new System.Drawing.Point(107, 61);
            this.lblFloorPlansPath.Name = "lblFloorPlansPath";
            this.lblFloorPlansPath.Size = new System.Drawing.Size(661, 13);
            this.lblFloorPlansPath.TabIndex = 0;
            this.lblFloorPlansPath.Text = "drop folder here or press Browse -->";
            // 
            // btnBrowseFloorPlanFolder
            // 
            this.btnBrowseFloorPlanFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseFloorPlanFolder.Location = new System.Drawing.Point(806, 56);
            this.btnBrowseFloorPlanFolder.Name = "btnBrowseFloorPlanFolder";
            this.btnBrowseFloorPlanFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFloorPlanFolder.TabIndex = 4;
            this.btnBrowseFloorPlanFolder.Text = "Browse";
            this.btnBrowseFloorPlanFolder.UseVisualStyleBackColor = true;
            this.btnBrowseFloorPlanFolder.Click += new System.EventHandler(this.btnBrowseFloorPlanFolder_Click);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Enabled = false;
            this.btnTest.Location = new System.Drawing.Point(806, 85);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 6;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // tbResults
            // 
            this.tbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResults.Enabled = false;
            this.tbResults.Location = new System.Drawing.Point(15, 114);
            this.tbResults.Multiline = true;
            this.tbResults.Name = "tbResults";
            this.tbResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResults.Size = new System.Drawing.Size(866, 312);
            this.tbResults.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Test results:";
            // 
            // btnGuessSuiteTypeInfo
            // 
            this.btnGuessSuiteTypeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuessSuiteTypeInfo.Enabled = false;
            this.btnGuessSuiteTypeInfo.Location = new System.Drawing.Point(773, 30);
            this.btnGuessSuiteTypeInfo.Name = "btnGuessSuiteTypeInfo";
            this.btnGuessSuiteTypeInfo.Size = new System.Drawing.Size(27, 23);
            this.btnGuessSuiteTypeInfo.TabIndex = 1;
            this.btnGuessSuiteTypeInfo.Text = "?";
            this.btnGuessSuiteTypeInfo.UseVisualStyleBackColor = true;
            this.btnGuessSuiteTypeInfo.Click += new System.EventHandler(this.btnGuessSuiteTypeInfo_Click);
            // 
            // btnGuessFloorPlanFolder
            // 
            this.btnGuessFloorPlanFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuessFloorPlanFolder.Enabled = false;
            this.btnGuessFloorPlanFolder.Location = new System.Drawing.Point(773, 56);
            this.btnGuessFloorPlanFolder.Name = "btnGuessFloorPlanFolder";
            this.btnGuessFloorPlanFolder.Size = new System.Drawing.Size(27, 23);
            this.btnGuessFloorPlanFolder.TabIndex = 3;
            this.btnGuessFloorPlanFolder.Text = "?";
            this.btnGuessFloorPlanFolder.UseVisualStyleBackColor = true;
            this.btnGuessFloorPlanFolder.Click += new System.EventHandler(this.btnGuessFloorPlanFolder_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 200;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 429);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(893, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(487, 17);
            this.toolStripStatusLabel1.Text = "Comments and info: andrey.masliuk@3dcondox.com CC: alex.neshmonin@3dcondox.com";
            // 
            // btnImport
            // 
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(725, 85);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 5;
            this.btnImport.Text = "Import...";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 451);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnGuessFloorPlanFolder);
            this.Controls.Add(this.btnGuessSuiteTypeInfo);
            this.Controls.Add(this.tbResults);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnBrowseFloorPlanFolder);
            this.Controls.Add(this.btnBrowseSuiteTypeInfo);
            this.Controls.Add(this.btnBrowseModel);
            this.Controls.Add(this.lblFloorPlansPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblSuiteTypeInfoPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblModelPath);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(550, 270);
            this.Name = "MainForm";
            this.Text = "Model Package Tester - 3D Condo Explorer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblModelPath;
        private System.Windows.Forms.Button btnBrowseModel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSuiteTypeInfoPath;
        private System.Windows.Forms.Button btnBrowseSuiteTypeInfo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblFloorPlansPath;
        private System.Windows.Forms.Button btnBrowseFloorPlanFolder;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.TextBox tbResults;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnGuessSuiteTypeInfo;
        private System.Windows.Forms.Button btnGuessFloorPlanFolder;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btnImport;
    }
}

