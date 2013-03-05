namespace SuperAdminConsole
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
            this.AddAccount = new System.Windows.Forms.Button();
            this.contextMenuStripViewOrder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemEnableViewOrder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExtend = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemChangeOrderView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadTheLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.editNotesForThisItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMLSURLForThisItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.composePromoEmailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.pnlStartupShutdown = new System.Windows.Forms.Panel();
            this.lblStartupShutdown = new System.Windows.Forms.Label();
            this.tmrStartup = new System.Windows.Forms.Timer(this.components);
            this.treeViewAccounts = new System.Windows.Forms.TreeView();
            this.contextMenuStripUserAccount = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteThisAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripLogs = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripAccountProperty = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changePropertyValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.labelStreetName = new System.Windows.Forms.Label();
            this.comboBoxEstateDeveloper = new System.Windows.Forms.ComboBox();
            this.tabControlAccountProperty = new SuperAdminConsole.WizardPages();
            this.tabPageInfo = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.listViewAccountInfo = new System.Windows.Forms.ListView();
            this.columnHeaderProperty = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageEmpty = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageViewOrders = new System.Windows.Forms.TabPage();
            this.buttonAddNewViewOrder = new System.Windows.Forms.Button();
            this.ListViewOrders = new System.Windows.Forms.ListView();
            this.aptNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buildingAdd = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PropertyType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ExpiresOn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.More = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageBanners = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPageLogs = new System.Windows.Forms.TabPage();
            this.listViewLogs = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStripViewOrder.SuspendLayout();
            this.pnlStartupShutdown.SuspendLayout();
            this.contextMenuStripUserAccount.SuspendLayout();
            this.contextMenuStripLogs.SuspendLayout();
            this.contextMenuStripAccountProperty.SuspendLayout();
            this.tabControlAccountProperty.SuspendLayout();
            this.tabPageInfo.SuspendLayout();
            this.tabPageEmpty.SuspendLayout();
            this.tabPageViewOrders.SuspendLayout();
            this.tabPageBanners.SuspendLayout();
            this.tabPageLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddAccount
            // 
            this.AddAccount.Location = new System.Drawing.Point(12, 38);
            this.AddAccount.Name = "AddAccount";
            this.AddAccount.Size = new System.Drawing.Size(118, 27);
            this.AddAccount.TabIndex = 2;
            this.AddAccount.Text = "Add New Account...";
            this.AddAccount.UseVisualStyleBackColor = true;
            this.AddAccount.Click += new System.EventHandler(this.AddAccount_Click);
            // 
            // contextMenuStripViewOrder
            // 
            this.contextMenuStripViewOrder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemEnableViewOrder,
            this.toolStripMenuItemExtend,
            this.toolStripSeparator2,
            this.toolStripMenuItemChangeOrderView,
            this.toolStripSeparator1,
            this.toolStripMenuItemDelete,
            this.LoadTheLinkToolStripMenuItem,
            this.toolStripSeparator3,
            this.editNotesForThisItemToolStripMenuItem,
            this.editMLSURLForThisItemToolStripMenuItem,
            this.toolStripSeparator4,
            this.composePromoEmailToolStripMenuItem});
            this.contextMenuStripViewOrder.Name = "contextMenuStripViewOrder";
            this.contextMenuStripViewOrder.Size = new System.Drawing.Size(224, 204);
            this.contextMenuStripViewOrder.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripViewOrder_Opening);
            // 
            // toolStripMenuItemEnableViewOrder
            // 
            this.toolStripMenuItemEnableViewOrder.Name = "toolStripMenuItemEnableViewOrder";
            this.toolStripMenuItemEnableViewOrder.Size = new System.Drawing.Size(223, 22);
            this.toolStripMenuItemEnableViewOrder.Text = "Enabled";
            this.toolStripMenuItemEnableViewOrder.Click += new System.EventHandler(this.toolStripMenuItemEnableViewOrder_Click);
            // 
            // toolStripMenuItemExtend
            // 
            this.toolStripMenuItemExtend.Name = "toolStripMenuItemExtend";
            this.toolStripMenuItemExtend.Size = new System.Drawing.Size(223, 22);
            this.toolStripMenuItemExtend.Text = "Extend...";
            this.toolStripMenuItemExtend.Click += new System.EventHandler(this.toolStripMenuItemExtend_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(220, 6);
            // 
            // toolStripMenuItemChangeOrderView
            // 
            this.toolStripMenuItemChangeOrderView.Name = "toolStripMenuItemChangeOrderView";
            this.toolStripMenuItemChangeOrderView.Size = new System.Drawing.Size(223, 22);
            this.toolStripMenuItemChangeOrderView.Text = "Change Options...";
            this.toolStripMenuItemChangeOrderView.Click += new System.EventHandler(this.toolStripMenuItemChangeOrderView_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(220, 6);
            // 
            // toolStripMenuItemDelete
            // 
            this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
            this.toolStripMenuItemDelete.Size = new System.Drawing.Size(223, 22);
            this.toolStripMenuItemDelete.Text = "Delete";
            this.toolStripMenuItemDelete.Click += new System.EventHandler(this.toolStripMenuItemDelete_Click);
            // 
            // LoadTheLinkToolStripMenuItem
            // 
            this.LoadTheLinkToolStripMenuItem.Name = "LoadTheLinkToolStripMenuItem";
            this.LoadTheLinkToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.LoadTheLinkToolStripMenuItem.Text = "Load in Default Browser";
            this.LoadTheLinkToolStripMenuItem.Click += new System.EventHandler(this.loadTheLinkToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(220, 6);
            // 
            // editNotesForThisItemToolStripMenuItem
            // 
            this.editNotesForThisItemToolStripMenuItem.Name = "editNotesForThisItemToolStripMenuItem";
            this.editNotesForThisItemToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.editNotesForThisItemToolStripMenuItem.Text = "Edit Notes for This Item...";
            this.editNotesForThisItemToolStripMenuItem.Click += new System.EventHandler(this.editNotesForThisItemToolStripMenuItem_Click);
            // 
            // editMLSURLForThisItemToolStripMenuItem
            // 
            this.editMLSURLForThisItemToolStripMenuItem.Name = "editMLSURLForThisItemToolStripMenuItem";
            this.editMLSURLForThisItemToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.editMLSURLForThisItemToolStripMenuItem.Text = "Edit MLS URL for This Item...";
            this.editMLSURLForThisItemToolStripMenuItem.Click += new System.EventHandler(this.editMLSURLForThisItemToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(220, 6);
            // 
            // composePromoEmailToolStripMenuItem
            // 
            this.composePromoEmailToolStripMenuItem.Name = "composePromoEmailToolStripMenuItem";
            this.composePromoEmailToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.composePromoEmailToolStripMenuItem.Text = "Compose Promo Email...";
            this.composePromoEmailToolStripMenuItem.Click += new System.EventHandler(this.composePromoEmailToolStripMenuItem_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonRefresh.Location = new System.Drawing.Point(136, 40);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(96, 25);
            this.buttonRefresh.TabIndex = 5;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseMnemonic = false;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // pnlStartupShutdown
            // 
            this.pnlStartupShutdown.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlStartupShutdown.Controls.Add(this.lblStartupShutdown);
            this.pnlStartupShutdown.Location = new System.Drawing.Point(95, 221);
            this.pnlStartupShutdown.Name = "pnlStartupShutdown";
            this.pnlStartupShutdown.Size = new System.Drawing.Size(363, 165);
            this.pnlStartupShutdown.TabIndex = 6;
            this.pnlStartupShutdown.Visible = false;
            // 
            // lblStartupShutdown
            // 
            this.lblStartupShutdown.AutoSize = true;
            this.lblStartupShutdown.Location = new System.Drawing.Point(36, 67);
            this.lblStartupShutdown.Name = "lblStartupShutdown";
            this.lblStartupShutdown.Size = new System.Drawing.Size(0, 13);
            this.lblStartupShutdown.TabIndex = 0;
            // 
            // tmrStartup
            // 
            this.tmrStartup.Tick += new System.EventHandler(this.tmrStartup_Tick);
            // 
            // treeViewAccounts
            // 
            this.treeViewAccounts.AllowDrop = true;
            this.treeViewAccounts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewAccounts.ContextMenuStrip = this.contextMenuStripUserAccount;
            this.treeViewAccounts.HideSelection = false;
            this.treeViewAccounts.Location = new System.Drawing.Point(12, 71);
            this.treeViewAccounts.Name = "treeViewAccounts";
            this.treeViewAccounts.Size = new System.Drawing.Size(220, 555);
            this.treeViewAccounts.TabIndex = 7;
            this.treeViewAccounts.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewAccounts_AfterSelect);
            this.treeViewAccounts.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewAccounts_DragDrop);
            this.treeViewAccounts.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewAccounts_DragEnter);
            this.treeViewAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeViewAccounts_MouseUp);
            // 
            // contextMenuStripUserAccount
            // 
            this.contextMenuStripUserAccount.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteThisAccountToolStripMenuItem});
            this.contextMenuStripUserAccount.Name = "contextMenuStripUserAccount";
            this.contextMenuStripUserAccount.Size = new System.Drawing.Size(176, 26);
            this.contextMenuStripUserAccount.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripUserAccount_Opening);
            // 
            // deleteThisAccountToolStripMenuItem
            // 
            this.deleteThisAccountToolStripMenuItem.Name = "deleteThisAccountToolStripMenuItem";
            this.deleteThisAccountToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.deleteThisAccountToolStripMenuItem.Text = "Delete this account";
            this.deleteThisAccountToolStripMenuItem.Click += new System.EventHandler(this.deleteThisAccountToolStripMenuItem_Click);
            // 
            // contextMenuStripLogs
            // 
            this.contextMenuStripLogs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showDetailsToolStripMenuItem});
            this.contextMenuStripLogs.Name = "contextMenuStripLogs";
            this.contextMenuStripLogs.Size = new System.Drawing.Size(157, 26);
            this.contextMenuStripLogs.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripLogs_Opening);
            // 
            // showDetailsToolStripMenuItem
            // 
            this.showDetailsToolStripMenuItem.Name = "showDetailsToolStripMenuItem";
            this.showDetailsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.showDetailsToolStripMenuItem.Text = "PayPal Details...";
            this.showDetailsToolStripMenuItem.Click += new System.EventHandler(this.showDetailsToolStripMenuItem_Click);
            // 
            // contextMenuStripAccountProperty
            // 
            this.contextMenuStripAccountProperty.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changePropertyValueToolStripMenuItem});
            this.contextMenuStripAccountProperty.Name = "contextMenuStripAccountProperty";
            this.contextMenuStripAccountProperty.Size = new System.Drawing.Size(205, 26);
            this.contextMenuStripAccountProperty.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripAccountProperty_Opening);
            // 
            // changePropertyValueToolStripMenuItem
            // 
            this.changePropertyValueToolStripMenuItem.Name = "changePropertyValueToolStripMenuItem";
            this.changePropertyValueToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.changePropertyValueToolStripMenuItem.Text = "Change Property Value...";
            this.changePropertyValueToolStripMenuItem.Click += new System.EventHandler(this.changePropertyValueToolStripMenuItem_Click);
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Location = new System.Drawing.Point(518, 12);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(152, 20);
            this.textBoxFilter.TabIndex = 10;
            this.textBoxFilter.Visible = false;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            // 
            // labelStreetName
            // 
            this.labelStreetName.AutoSize = true;
            this.labelStreetName.Location = new System.Drawing.Point(400, 15);
            this.labelStreetName.Name = "labelStreetName";
            this.labelStreetName.Size = new System.Drawing.Size(102, 13);
            this.labelStreetName.TabIndex = 9;
            this.labelStreetName.Text = "Type string to filter...";
            this.labelStreetName.Visible = false;
            // 
            // comboBoxEstateDeveloper
            // 
            this.comboBoxEstateDeveloper.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEstateDeveloper.FormattingEnabled = true;
            this.comboBoxEstateDeveloper.Location = new System.Drawing.Point(12, 7);
            this.comboBoxEstateDeveloper.Name = "comboBoxEstateDeveloper";
            this.comboBoxEstateDeveloper.Size = new System.Drawing.Size(220, 21);
            this.comboBoxEstateDeveloper.TabIndex = 11;
            this.comboBoxEstateDeveloper.SelectedIndexChanged += new System.EventHandler(this.comboBoxEstateDeveloper_SelectedIndexChanged);
            // 
            // tabControlAccountProperty
            // 
            this.tabControlAccountProperty.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlAccountProperty.Controls.Add(this.tabPageInfo);
            this.tabControlAccountProperty.Controls.Add(this.tabPageEmpty);
            this.tabControlAccountProperty.Controls.Add(this.tabPageViewOrders);
            this.tabControlAccountProperty.Controls.Add(this.tabPageBanners);
            this.tabControlAccountProperty.Controls.Add(this.tabPageLogs);
            this.tabControlAccountProperty.Location = new System.Drawing.Point(246, 40);
            this.tabControlAccountProperty.Name = "tabControlAccountProperty";
            this.tabControlAccountProperty.SelectedIndex = 0;
            this.tabControlAccountProperty.Size = new System.Drawing.Size(429, 586);
            this.tabControlAccountProperty.TabIndex = 8;
            // 
            // tabPageInfo
            // 
            this.tabPageInfo.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageInfo.Controls.Add(this.label2);
            this.tabPageInfo.Controls.Add(this.listViewAccountInfo);
            this.tabPageInfo.Location = new System.Drawing.Point(4, 22);
            this.tabPageInfo.Name = "tabPageInfo";
            this.tabPageInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInfo.Size = new System.Drawing.Size(421, 560);
            this.tabPageInfo.TabIndex = 4;
            this.tabPageInfo.Text = "Info";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(90, 350);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(172, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Personal Info (not implemented yet)";
            // 
            // listViewAccountInfo
            // 
            this.listViewAccountInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewAccountInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderProperty,
            this.columnHeaderValue});
            this.listViewAccountInfo.ContextMenuStrip = this.contextMenuStripAccountProperty;
            this.listViewAccountInfo.FullRowSelect = true;
            this.listViewAccountInfo.GridLines = true;
            this.listViewAccountInfo.Location = new System.Drawing.Point(3, 3);
            this.listViewAccountInfo.MultiSelect = false;
            this.listViewAccountInfo.Name = "listViewAccountInfo";
            this.listViewAccountInfo.Size = new System.Drawing.Size(417, 221);
            this.listViewAccountInfo.TabIndex = 0;
            this.listViewAccountInfo.UseCompatibleStateImageBehavior = false;
            this.listViewAccountInfo.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderProperty
            // 
            this.columnHeaderProperty.Text = "Property";
            this.columnHeaderProperty.Width = 136;
            // 
            // columnHeaderValue
            // 
            this.columnHeaderValue.Text = "Value";
            this.columnHeaderValue.Width = 273;
            // 
            // tabPageEmpty
            // 
            this.tabPageEmpty.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageEmpty.Controls.Add(this.label1);
            this.tabPageEmpty.Location = new System.Drawing.Point(4, 22);
            this.tabPageEmpty.Name = "tabPageEmpty";
            this.tabPageEmpty.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEmpty.Size = new System.Drawing.Size(421, 560);
            this.tabPageEmpty.TabIndex = 2;
            this.tabPageEmpty.Text = "Empty";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(156, 272);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a node at the left";
            // 
            // tabPageViewOrders
            // 
            this.tabPageViewOrders.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageViewOrders.Controls.Add(this.buttonAddNewViewOrder);
            this.tabPageViewOrders.Controls.Add(this.ListViewOrders);
            this.tabPageViewOrders.Location = new System.Drawing.Point(4, 22);
            this.tabPageViewOrders.Name = "tabPageViewOrders";
            this.tabPageViewOrders.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageViewOrders.Size = new System.Drawing.Size(421, 560);
            this.tabPageViewOrders.TabIndex = 0;
            this.tabPageViewOrders.Text = "ViewOrders";
            // 
            // buttonAddNewViewOrder
            // 
            this.buttonAddNewViewOrder.Location = new System.Drawing.Point(138, 550);
            this.buttonAddNewViewOrder.Name = "buttonAddNewViewOrder";
            this.buttonAddNewViewOrder.Size = new System.Drawing.Size(149, 25);
            this.buttonAddNewViewOrder.TabIndex = 1;
            this.buttonAddNewViewOrder.Text = "Add New ViewOrder...";
            this.buttonAddNewViewOrder.UseVisualStyleBackColor = true;
            this.buttonAddNewViewOrder.Click += new System.EventHandler(this.buttonAddNewViewOrder_Click);
            // 
            // ListViewOrders
            // 
            this.ListViewOrders.AllowDrop = true;
            this.ListViewOrders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.aptNum,
            this.buildingAdd,
            this.PropertyType,
            this.ExpiresOn,
            this.More});
            this.ListViewOrders.ContextMenuStrip = this.contextMenuStripViewOrder;
            this.ListViewOrders.FullRowSelect = true;
            this.ListViewOrders.GridLines = true;
            this.ListViewOrders.HideSelection = false;
            this.ListViewOrders.Location = new System.Drawing.Point(0, 5);
            this.ListViewOrders.MultiSelect = false;
            this.ListViewOrders.Name = "ListViewOrders";
            this.ListViewOrders.ShowItemToolTips = true;
            this.ListViewOrders.Size = new System.Drawing.Size(428, 532);
            this.ListViewOrders.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ListViewOrders.TabIndex = 3;
            this.ListViewOrders.UseCompatibleStateImageBehavior = false;
            this.ListViewOrders.View = System.Windows.Forms.View.Details;
            this.ListViewOrders.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListViewOrders_ColumnClick);
            this.ListViewOrders.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewOrders_DragOver);
            this.ListViewOrders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListViewOrders_MouseDoubleClick);
            this.ListViewOrders.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewOrders_MouseDown);
            // 
            // aptNum
            // 
            this.aptNum.Text = "Suite";
            this.aptNum.Width = 38;
            // 
            // buildingAdd
            // 
            this.buildingAdd.Text = "Building Address";
            this.buildingAdd.Width = 173;
            // 
            // PropertyType
            // 
            this.PropertyType.Text = "Type";
            this.PropertyType.Width = 36;
            // 
            // ExpiresOn
            // 
            this.ExpiresOn.Text = "Expires On";
            this.ExpiresOn.Width = 80;
            // 
            // More
            // 
            this.More.Text = "MLS";
            this.More.Width = 89;
            // 
            // tabPageBanners
            // 
            this.tabPageBanners.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageBanners.Controls.Add(this.label3);
            this.tabPageBanners.Location = new System.Drawing.Point(4, 22);
            this.tabPageBanners.Name = "tabPageBanners";
            this.tabPageBanners.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBanners.Size = new System.Drawing.Size(421, 560);
            this.tabPageBanners.TabIndex = 3;
            this.tabPageBanners.Text = "Banners";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(162, 243);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Not Supported yet";
            // 
            // tabPageLogs
            // 
            this.tabPageLogs.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageLogs.Controls.Add(this.listViewLogs);
            this.tabPageLogs.Location = new System.Drawing.Point(4, 22);
            this.tabPageLogs.Name = "tabPageLogs";
            this.tabPageLogs.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLogs.Size = new System.Drawing.Size(421, 560);
            this.tabPageLogs.TabIndex = 1;
            this.tabPageLogs.Text = "Logs";
            // 
            // listViewLogs
            // 
            this.listViewLogs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.listViewLogs.ContextMenuStrip = this.contextMenuStripLogs;
            this.listViewLogs.FullRowSelect = true;
            this.listViewLogs.GridLines = true;
            this.listViewLogs.HideSelection = false;
            this.listViewLogs.Location = new System.Drawing.Point(0, 5);
            this.listViewLogs.MultiSelect = false;
            this.listViewLogs.Name = "listViewLogs";
            this.listViewLogs.Size = new System.Drawing.Size(428, 574);
            this.listViewLogs.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewLogs.TabIndex = 4;
            this.listViewLogs.UseCompatibleStateImageBehavior = false;
            this.listViewLogs.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Created";
            this.columnHeader1.Width = 142;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "S";
            this.columnHeader2.Width = 30;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "payRefId";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Oper";
            this.columnHeader4.Width = 43;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Amnt";
            this.columnHeader5.Width = 40;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Trgt";
            this.columnHeader6.Width = 48;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "targetId";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 633);
            this.Controls.Add(this.comboBoxEstateDeveloper);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.labelStreetName);
            this.Controls.Add(this.tabControlAccountProperty);
            this.Controls.Add(this.treeViewAccounts);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.AddAccount);
            this.Controls.Add(this.pnlStartupShutdown);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Main Form";
            this.contextMenuStripViewOrder.ResumeLayout(false);
            this.pnlStartupShutdown.ResumeLayout(false);
            this.pnlStartupShutdown.PerformLayout();
            this.contextMenuStripUserAccount.ResumeLayout(false);
            this.contextMenuStripLogs.ResumeLayout(false);
            this.contextMenuStripAccountProperty.ResumeLayout(false);
            this.tabControlAccountProperty.ResumeLayout(false);
            this.tabPageInfo.ResumeLayout(false);
            this.tabPageInfo.PerformLayout();
            this.tabPageEmpty.ResumeLayout(false);
            this.tabPageEmpty.PerformLayout();
            this.tabPageViewOrders.ResumeLayout(false);
            this.tabPageBanners.ResumeLayout(false);
            this.tabPageBanners.PerformLayout();
            this.tabPageLogs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AddAccount;
        private System.Windows.Forms.ListView ListViewOrders;
        private System.Windows.Forms.ColumnHeader PropertyType;
        private System.Windows.Forms.ColumnHeader ExpiresOn;
        private System.Windows.Forms.Button buttonAddNewViewOrder;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Panel pnlStartupShutdown;
        private System.Windows.Forms.Label lblStartupShutdown;
        private System.Windows.Forms.Timer tmrStartup;
        private System.Windows.Forms.ColumnHeader aptNum;
        private System.Windows.Forms.TreeView treeViewAccounts;
        private System.Windows.Forms.TabPage tabPageEmpty;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPageViewOrders;
        private System.Windows.Forms.TabPage tabPageLogs;
        private System.Windows.Forms.TabPage tabPageBanners;
        private WizardPages tabControlAccountProperty;
        private System.Windows.Forms.TabPage tabPageInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView listViewLogs;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripViewOrder;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEnableViewOrder;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExtend;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelete;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader More;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripLogs;
        private System.Windows.Forms.ToolStripMenuItem showDetailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemChangeOrderView;
        private System.Windows.Forms.ToolStripMenuItem LoadTheLinkToolStripMenuItem;
        private System.Windows.Forms.ListView listViewAccountInfo;
        private System.Windows.Forms.ColumnHeader columnHeaderProperty;
        private System.Windows.Forms.ColumnHeader columnHeaderValue;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAccountProperty;
        private System.Windows.Forms.ToolStripMenuItem changePropertyValueToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader buildingAdd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem editNotesForThisItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMLSURLForThisItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem composePromoEmailToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label labelStreetName;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripUserAccount;
        private System.Windows.Forms.ToolStripMenuItem deleteThisAccountToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxEstateDeveloper;
    }
}

