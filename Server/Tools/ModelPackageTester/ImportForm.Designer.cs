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
            this.cbSingleBuilding = new System.Windows.Forms.CheckBox();
            this.cbxBuildings = new System.Windows.Forms.ComboBox();
            this.cbDryRun = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblDisplayModelFile = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.tbResults = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
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
            this.label2.Location = new System.Drawing.Point(442, 9);
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
            this.tbDeveloper.Size = new System.Drawing.Size(282, 20);
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
            this.label4.Location = new System.Drawing.Point(442, 35);
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
            this.tbSite.Size = new System.Drawing.Size(282, 20);
            this.tbSite.TabIndex = 1;
            this.tbSite.TextChanged += new System.EventHandler(this.tbSite_TextChanged);
            // 
            // cbSingleBuilding
            // 
            this.cbSingleBuilding.AutoSize = true;
            this.cbSingleBuilding.Location = new System.Drawing.Point(15, 60);
            this.cbSingleBuilding.Name = "cbSingleBuilding";
            this.cbSingleBuilding.Size = new System.Drawing.Size(128, 17);
            this.cbSingleBuilding.TabIndex = 2;
            this.cbSingleBuilding.Text = "Import single Building:";
            this.cbSingleBuilding.UseVisualStyleBackColor = true;
            this.cbSingleBuilding.CheckedChanged += new System.EventHandler(this.cbSingleBuilding_CheckedChanged);
            // 
            // cbxBuildings
            // 
            this.cbxBuildings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxBuildings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBuildings.Enabled = false;
            this.cbxBuildings.FormattingEnabled = true;
            this.cbxBuildings.Location = new System.Drawing.Point(154, 58);
            this.cbxBuildings.Name = "cbxBuildings";
            this.cbxBuildings.Size = new System.Drawing.Size(282, 21);
            this.cbxBuildings.TabIndex = 2;
            // 
            // cbDryRun
            // 
            this.cbDryRun.AutoSize = true;
            this.cbDryRun.Checked = true;
            this.cbDryRun.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDryRun.Location = new System.Drawing.Point(15, 129);
            this.cbDryRun.Name = "cbDryRun";
            this.cbDryRun.Size = new System.Drawing.Size(60, 17);
            this.cbDryRun.TabIndex = 5;
            this.cbDryRun.Text = "Dry run";
            this.cbDryRun.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 88);
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
            this.lblDisplayModelFile.Location = new System.Drawing.Point(151, 88);
            this.lblDisplayModelFile.Name = "lblDisplayModelFile";
            this.lblDisplayModelFile.Size = new System.Drawing.Size(283, 13);
            this.lblDisplayModelFile.TabIndex = 0;
            this.lblDisplayModelFile.Text = "drop file here or press Browse -->";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(163, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "(for buildings not constructed yet)";
            // 
            // btnImport
            // 
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(154, 125);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 6;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // tbResults
            // 
            this.tbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResults.Enabled = false;
            this.tbResults.Location = new System.Drawing.Point(15, 154);
            this.tbResults.Multiline = true;
            this.tbResults.Name = "tbResults";
            this.tbResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResults.Size = new System.Drawing.Size(602, 253);
            this.tbResults.TabIndex = 7;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(445, 83);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 419);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.tbResults);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.cbDryRun);
            this.Controls.Add(this.cbxBuildings);
            this.Controls.Add(this.cbSingleBuilding);
            this.Controls.Add(this.tbSite);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbDeveloper);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblDisplayModelFile);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ImportForm";
            this.Text = "Import Model - 3D Condo Explorer";
            this.Shown += new System.EventHandler(this.ImportForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImportForm_DragEnter);
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
        private System.Windows.Forms.CheckBox cbSingleBuilding;
        private System.Windows.Forms.ComboBox cbxBuildings;
        private System.Windows.Forms.CheckBox cbDryRun;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDisplayModelFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.TextBox tbResults;
        private System.Windows.Forms.Button btnBrowse;
    }
}