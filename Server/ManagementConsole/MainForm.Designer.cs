namespace Vre.Server.ManagementConsole
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
            this.pnlStartupShutdown = new System.Windows.Forms.Panel();
            this.lblStartupShutdown = new System.Windows.Forms.Label();
            this.tmrStartup = new System.Windows.Forms.Timer(this.components);
            this.mnuEstateDevelopers = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miAddEstateDeveloper = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEstateDeveloper = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miAddSite = new System.Windows.Forms.ToolStripMenuItem();
            this.miImportModelToDeveloper = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tvStructure = new System.Windows.Forms.TreeView();
            this.pnlPropPage = new System.Windows.Forms.Panel();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.mnuBuilding = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miImportModelToBuilding = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlStartupShutdown.SuspendLayout();
            this.mnuEstateDevelopers.SuspendLayout();
            this.mnuEstateDeveloper.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.mnuBuilding.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlStartupShutdown
            // 
            this.pnlStartupShutdown.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlStartupShutdown.Controls.Add(this.lblStartupShutdown);
            this.pnlStartupShutdown.Location = new System.Drawing.Point(124, 184);
            this.pnlStartupShutdown.Name = "pnlStartupShutdown";
            this.pnlStartupShutdown.Size = new System.Drawing.Size(94, 47);
            this.pnlStartupShutdown.TabIndex = 0;
            this.pnlStartupShutdown.Visible = false;
            // 
            // lblStartupShutdown
            // 
            this.lblStartupShutdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStartupShutdown.Location = new System.Drawing.Point(0, 0);
            this.lblStartupShutdown.Name = "lblStartupShutdown";
            this.lblStartupShutdown.Size = new System.Drawing.Size(90, 43);
            this.lblStartupShutdown.TabIndex = 0;
            this.lblStartupShutdown.Text = "label1";
            this.lblStartupShutdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmrStartup
            // 
            this.tmrStartup.Enabled = true;
            this.tmrStartup.Tick += new System.EventHandler(this.tmrStartup_Tick);
            // 
            // mnuEstateDevelopers
            // 
            this.mnuEstateDevelopers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAddEstateDeveloper});
            this.mnuEstateDevelopers.Name = "contextMenuStrip1";
            this.mnuEstateDevelopers.Size = new System.Drawing.Size(191, 26);
            // 
            // miAddEstateDeveloper
            // 
            this.miAddEstateDeveloper.Name = "miAddEstateDeveloper";
            this.miAddEstateDeveloper.Size = new System.Drawing.Size(190, 22);
            this.miAddEstateDeveloper.Text = "Add Estate Developer";
            this.miAddEstateDeveloper.Click += new System.EventHandler(this.miAddEstateDeveloper_Click);
            // 
            // mnuEstateDeveloper
            // 
            this.mnuEstateDeveloper.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAddSite,
            this.miImportModelToDeveloper});
            this.mnuEstateDeveloper.Name = "mnuEstateDeveloper";
            this.mnuEstateDeveloper.Size = new System.Drawing.Size(149, 48);
            // 
            // miAddSite
            // 
            this.miAddSite.Name = "miAddSite";
            this.miAddSite.Size = new System.Drawing.Size(148, 22);
            this.miAddSite.Text = "Add Site";
            this.miAddSite.Click += new System.EventHandler(this.miAddSite_Click);
            // 
            // miImportModelToDeveloper
            // 
            this.miImportModelToDeveloper.Name = "miImportModelToDeveloper";
            this.miImportModelToDeveloper.Size = new System.Drawing.Size(148, 22);
            this.miImportModelToDeveloper.Text = "Import model";
            this.miImportModelToDeveloper.Click += new System.EventHandler(this.miImportModelToDeveloper_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tbLog);
            this.splitContainer1.Size = new System.Drawing.Size(694, 582);
            this.splitContainer1.SplitterDistance = 417;
            this.splitContainer1.TabIndex = 12;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tvStructure);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pnlPropPage);
            this.splitContainer.Size = new System.Drawing.Size(694, 417);
            this.splitContainer.SplitterDistance = 333;
            this.splitContainer.TabIndex = 11;
            // 
            // tvStructure
            // 
            this.tvStructure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvStructure.Location = new System.Drawing.Point(0, 0);
            this.tvStructure.Name = "tvStructure";
            this.tvStructure.Size = new System.Drawing.Size(333, 417);
            this.tvStructure.TabIndex = 10;
            this.tvStructure.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvStructure_BeforeExpand);
            this.tvStructure.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvStructure_BeforeSelect);
            this.tvStructure.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvStructure_AfterSelect);
            this.tvStructure.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvStructure_NodeMouseClick);
            // 
            // pnlPropPage
            // 
            this.pnlPropPage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlPropPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPropPage.Location = new System.Drawing.Point(0, 0);
            this.pnlPropPage.Name = "pnlPropPage";
            this.pnlPropPage.Size = new System.Drawing.Size(357, 417);
            this.pnlPropPage.TabIndex = 0;
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Location = new System.Drawing.Point(0, 0);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(694, 161);
            this.tbLog.TabIndex = 0;
            // 
            // mnuBuilding
            // 
            this.mnuBuilding.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miImportModelToBuilding});
            this.mnuBuilding.Name = "mnuBuilding";
            this.mnuBuilding.Size = new System.Drawing.Size(153, 48);
            // 
            // miImportModelToBuilding
            // 
            this.miImportModelToBuilding.Name = "miImportModelToBuilding";
            this.miImportModelToBuilding.Size = new System.Drawing.Size(152, 22);
            this.miImportModelToBuilding.Text = "Import model";
            this.miImportModelToBuilding.Click += new System.EventHandler(this.miImportModelToBuilding_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 606);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlStartupShutdown);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "MainForm";
            this.Text = "Management Console - VRE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.pnlStartupShutdown.ResumeLayout(false);
            this.mnuEstateDevelopers.ResumeLayout(false);
            this.mnuEstateDeveloper.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.mnuBuilding.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlStartupShutdown;
        private System.Windows.Forms.Label lblStartupShutdown;
        private System.Windows.Forms.Timer tmrStartup;
        private System.Windows.Forms.ContextMenuStrip mnuEstateDevelopers;
        private System.Windows.Forms.ToolStripMenuItem miAddEstateDeveloper;
        private System.Windows.Forms.ContextMenuStrip mnuEstateDeveloper;
        private System.Windows.Forms.ToolStripMenuItem miAddSite;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView tvStructure;
        private System.Windows.Forms.Panel pnlPropPage;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.ToolStripMenuItem miImportModelToDeveloper;
        private System.Windows.Forms.ContextMenuStrip mnuBuilding;
        private System.Windows.Forms.ToolStripMenuItem miImportModelToBuilding;

    }
}

