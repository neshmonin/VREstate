﻿namespace SuperAdminConsole
{
    partial class SetViewOrder
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonPrev = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.tabControlSteps = new SuperAdminConsole.WizardPages();
            this.tabPageCheckAddress = new System.Windows.Forms.TabPage();
            this.tabControlAddressPicking = new System.Windows.Forms.TabControl();
            this.tabPagePick = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxCity = new System.Windows.Forms.ComboBox();
            this.listViewAddresses = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageTypeIn = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxCountry = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonCheckAddress = new System.Windows.Forms.Button();
            this.textPostalCode = new System.Windows.Forms.TextBox();
            this.textBuildingNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textProvince = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textCity = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textUnitNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textStreet = new System.Windows.Forms.TextBox();
            this.tabPageViewOrderOptions = new System.Windows.Forms.TabPage();
            this.textBoxNote = new System.Windows.Forms.RichTextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.buttonPayment = new System.Windows.Forms.Button();
            this.groupBoxListingOptions = new System.Windows.Forms.GroupBox();
            this.textBoxMLS = new System.Windows.Forms.RichTextBox();
            this.textBoxMlsUrl = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.labelPercent = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelTax = new System.Windows.Forms.Label();
            this.numericUpDownTax = new System.Windows.Forms.NumericUpDown();
            this.textBoxTotal = new System.Windows.Forms.TextBox();
            this.numericUpDownPrice = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownDaysValid = new System.Windows.Forms.NumericUpDown();
            this.labelDays = new System.Windows.Forms.Label();
            this.labelValidFor = new System.Windows.Forms.Label();
            this.labelPrice = new System.Windows.Forms.Label();
            this.buttonCheckLink = new System.Windows.Forms.Button();
            this.textExternalLink = new System.Windows.Forms.TextBox();
            this.radioButtonExternalLink = new System.Windows.Forms.RadioButton();
            this.radioButtonFloorplan = new System.Windows.Forms.RadioButton();
            this.tabPageGenerateViewOrder = new System.Windows.Forms.TabPage();
            this.textBox300Chars = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonSendEmailk = new System.Windows.Forms.Button();
            this.buttonGenerateURL = new System.Windows.Forms.Button();
            this.textBoxViewOrderURL = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxStreetName = new System.Windows.Forms.TextBox();
            this.tabControlSteps.SuspendLayout();
            this.tabPageCheckAddress.SuspendLayout();
            this.tabControlAddressPicking.SuspendLayout();
            this.tabPagePick.SuspendLayout();
            this.tabPageTypeIn.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageViewOrderOptions.SuspendLayout();
            this.groupBoxListingOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDaysValid)).BeginInit();
            this.tabPageGenerateViewOrder.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(10, 345);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(77, 23);
            this.buttonCancel.TabIndex = 18;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonPrev
            // 
            this.buttonPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPrev.Enabled = false;
            this.buttonPrev.Location = new System.Drawing.Point(208, 344);
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.Size = new System.Drawing.Size(70, 24);
            this.buttonPrev.TabIndex = 19;
            this.buttonPrev.Text = "< Prev";
            this.buttonPrev.UseVisualStyleBackColor = true;
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNext.Location = new System.Drawing.Point(285, 344);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(70, 24);
            this.buttonNext.TabIndex = 20;
            this.buttonNext.Text = "Next >";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonFinish
            // 
            this.buttonFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFinish.Enabled = false;
            this.buttonFinish.Location = new System.Drawing.Point(420, 344);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(70, 24);
            this.buttonFinish.TabIndex = 21;
            this.buttonFinish.Text = "Finish";
            this.buttonFinish.UseVisualStyleBackColor = true;
            this.buttonFinish.Click += new System.EventHandler(this.buttonFinish_Click);
            // 
            // tabControlSteps
            // 
            this.tabControlSteps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlSteps.Controls.Add(this.tabPageCheckAddress);
            this.tabControlSteps.Controls.Add(this.tabPageViewOrderOptions);
            this.tabControlSteps.Controls.Add(this.tabPageGenerateViewOrder);
            this.tabControlSteps.Location = new System.Drawing.Point(-3, 0);
            this.tabControlSteps.Name = "tabControlSteps";
            this.tabControlSteps.SelectedIndex = 0;
            this.tabControlSteps.Size = new System.Drawing.Size(509, 354);
            this.tabControlSteps.TabIndex = 17;
            // 
            // tabPageCheckAddress
            // 
            this.tabPageCheckAddress.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageCheckAddress.Controls.Add(this.tabControlAddressPicking);
            this.tabPageCheckAddress.Location = new System.Drawing.Point(4, 22);
            this.tabPageCheckAddress.Name = "tabPageCheckAddress";
            this.tabPageCheckAddress.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCheckAddress.Size = new System.Drawing.Size(501, 328);
            this.tabPageCheckAddress.TabIndex = 0;
            this.tabPageCheckAddress.Text = "Address";
            // 
            // tabControlAddressPicking
            // 
            this.tabControlAddressPicking.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlAddressPicking.Controls.Add(this.tabPagePick);
            this.tabControlAddressPicking.Controls.Add(this.tabPageTypeIn);
            this.tabControlAddressPicking.Location = new System.Drawing.Point(6, 3);
            this.tabControlAddressPicking.Name = "tabControlAddressPicking";
            this.tabControlAddressPicking.SelectedIndex = 0;
            this.tabControlAddressPicking.Size = new System.Drawing.Size(494, 327);
            this.tabControlAddressPicking.TabIndex = 1;
            this.tabControlAddressPicking.SelectedIndexChanged += new System.EventHandler(this.tabControlAddressPicking_SelectedTabChanged);
            // 
            // tabPagePick
            // 
            this.tabPagePick.BackColor = System.Drawing.SystemColors.Control;
            this.tabPagePick.Controls.Add(this.textBoxStreetName);
            this.tabPagePick.Controls.Add(this.label13);
            this.tabPagePick.Controls.Add(this.label7);
            this.tabPagePick.Controls.Add(this.comboBoxCity);
            this.tabPagePick.Controls.Add(this.listViewAddresses);
            this.tabPagePick.Location = new System.Drawing.Point(4, 22);
            this.tabPagePick.Name = "tabPagePick";
            this.tabPagePick.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePick.Size = new System.Drawing.Size(486, 301);
            this.tabPagePick.TabIndex = 1;
            this.tabPagePick.Text = "From List";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "City:";
            // 
            // comboBoxCity
            // 
            this.comboBoxCity.AutoCompleteCustomSource.AddRange(new string[] {
            "Toronto",
            "Mississauga",
            "Brampton",
            "Caledon",
            "Oakville",
            "Burlington",
            "Milton",
            "Halton Hills",
            "Vaughan",
            "Markham",
            "Richmond Hill",
            "Aurora",
            "Newmarket",
            "King",
            "Pickering",
            "Ajax",
            "Withby",
            "Oshawa",
            "Clarington",
            "Orlando"});
            this.comboBoxCity.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxCity.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.comboBoxCity.FormattingEnabled = true;
            this.comboBoxCity.Items.AddRange(new object[] {
            "Toronto",
            "Mississauga",
            "Brampton",
            "Caledon",
            "Oakville",
            "Burlington",
            "Milton",
            "Halton Hills",
            "Vaughan",
            "Markham",
            "Richmond Hill",
            "Aurora",
            "Newmarket",
            "King",
            "Pickering",
            "Ajax",
            "Withby",
            "Oshawa",
            "Clarington",
            "Orlando",
            "- empty addresses -"});
            this.comboBoxCity.Location = new System.Drawing.Point(41, 6);
            this.comboBoxCity.Name = "comboBoxCity";
            this.comboBoxCity.Size = new System.Drawing.Size(149, 21);
            this.comboBoxCity.TabIndex = 1;
            this.comboBoxCity.SelectedIndexChanged += new System.EventHandler(this.comboBoxCity_SelectedIndexChanged);
            // 
            // listViewAddresses
            // 
            this.listViewAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewAddresses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader3,
            this.columnHeader1,
            this.columnHeader5});
            this.listViewAddresses.FullRowSelect = true;
            this.listViewAddresses.GridLines = true;
            this.listViewAddresses.HideSelection = false;
            this.listViewAddresses.LabelEdit = true;
            this.listViewAddresses.Location = new System.Drawing.Point(5, 31);
            this.listViewAddresses.MultiSelect = false;
            this.listViewAddresses.Name = "listViewAddresses";
            this.listViewAddresses.Size = new System.Drawing.Size(473, 260);
            this.listViewAddresses.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listViewAddresses.TabIndex = 0;
            this.listViewAddresses.UseCompatibleStateImageBehavior = false;
            this.listViewAddresses.View = System.Windows.Forms.View.Details;
            this.listViewAddresses.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewAddresses_ColumnClick);
            this.listViewAddresses.SelectedIndexChanged += new System.EventHandler(this.listViewAddresses_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 99;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "No";
            this.columnHeader4.Width = 41;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Street";
            this.columnHeader3.Width = 206;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "City";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Postal";
            this.columnHeader5.Width = 63;
            // 
            // tabPageTypeIn
            // 
            this.tabPageTypeIn.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageTypeIn.Controls.Add(this.groupBox1);
            this.tabPageTypeIn.Location = new System.Drawing.Point(4, 22);
            this.tabPageTypeIn.Name = "tabPageTypeIn";
            this.tabPageTypeIn.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTypeIn.Size = new System.Drawing.Size(486, 301);
            this.tabPageTypeIn.TabIndex = 0;
            this.tabPageTypeIn.Text = "Type In";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.comboBoxCountry);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.buttonCheckAddress);
            this.groupBox1.Controls.Add(this.textPostalCode);
            this.groupBox1.Controls.Add(this.textBuildingNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textProvince);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textCity);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textUnitNo);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textStreet);
            this.groupBox1.Location = new System.Drawing.Point(5, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(478, 149);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Address";
            // 
            // comboBoxCountry
            // 
            this.comboBoxCountry.Items.AddRange(new object[] {
            "Canada",
            "USA"});
            this.comboBoxCountry.Location = new System.Drawing.Point(401, 70);
            this.comboBoxCountry.Name = "comboBoxCountry";
            this.comboBoxCountry.Size = new System.Drawing.Size(67, 21);
            this.comboBoxCountry.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(400, 54);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Country";
            // 
            // buttonCheckAddress
            // 
            this.buttonCheckAddress.Location = new System.Drawing.Point(119, 107);
            this.buttonCheckAddress.Name = "buttonCheckAddress";
            this.buttonCheckAddress.Size = new System.Drawing.Size(252, 27);
            this.buttonCheckAddress.TabIndex = 7;
            this.buttonCheckAddress.Text = "Check if This Address is Available";
            this.buttonCheckAddress.UseVisualStyleBackColor = true;
            this.buttonCheckAddress.Click += new System.EventHandler(this.buttonCheckAddress_Click);
            // 
            // textPostalCode
            // 
            this.textPostalCode.Location = new System.Drawing.Point(263, 70);
            this.textPostalCode.Name = "textPostalCode";
            this.textPostalCode.Size = new System.Drawing.Size(67, 20);
            this.textPostalCode.TabIndex = 6;
            this.textPostalCode.TextChanged += new System.EventHandler(this.textPostalCode_TextChanged);
            // 
            // textBuildingNo
            // 
            this.textBuildingNo.Location = new System.Drawing.Point(11, 33);
            this.textBuildingNo.Name = "textBuildingNo";
            this.textBuildingNo.Size = new System.Drawing.Size(67, 20);
            this.textBuildingNo.TabIndex = 1;
            this.textBuildingNo.Text = "650";
            this.textBuildingNo.TextChanged += new System.EventHandler(this.textBuildingNo_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(262, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Postal Code*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Building #*";
            // 
            // textProvince
            // 
            this.textProvince.Location = new System.Drawing.Point(201, 70);
            this.textProvince.Name = "textProvince";
            this.textProvince.Size = new System.Drawing.Size(51, 20);
            this.textProvince.TabIndex = 5;
            this.textProvince.Text = "ON";
            this.textProvince.TextChanged += new System.EventHandler(this.textProvince_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(201, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Province";
            // 
            // textCity
            // 
            this.textCity.Location = new System.Drawing.Point(11, 70);
            this.textCity.Name = "textCity";
            this.textCity.Size = new System.Drawing.Size(175, 20);
            this.textCity.TabIndex = 4;
            this.textCity.Text = "Toronto";
            this.textCity.TextChanged += new System.EventHandler(this.textCity_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "City";
            // 
            // textUnitNo
            // 
            this.textUnitNo.Location = new System.Drawing.Point(401, 33);
            this.textUnitNo.Name = "textUnitNo";
            this.textUnitNo.Size = new System.Drawing.Size(67, 20);
            this.textUnitNo.TabIndex = 3;
            this.textUnitNo.Text = "0801";
            this.textUnitNo.TextChanged += new System.EventHandler(this.textUnitNo_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(400, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Unit #";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(91, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Street";
            // 
            // textStreet
            // 
            this.textStreet.Location = new System.Drawing.Point(93, 33);
            this.textStreet.Name = "textStreet";
            this.textStreet.Size = new System.Drawing.Size(290, 20);
            this.textStreet.TabIndex = 2;
            this.textStreet.Text = "Queens";
            this.textStreet.TextChanged += new System.EventHandler(this.textStreet_TextChanged);
            // 
            // tabPageViewOrderOptions
            // 
            this.tabPageViewOrderOptions.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageViewOrderOptions.Controls.Add(this.textBoxNote);
            this.tabPageViewOrderOptions.Controls.Add(this.label11);
            this.tabPageViewOrderOptions.Controls.Add(this.buttonPayment);
            this.tabPageViewOrderOptions.Controls.Add(this.groupBoxListingOptions);
            this.tabPageViewOrderOptions.Location = new System.Drawing.Point(4, 22);
            this.tabPageViewOrderOptions.Name = "tabPageViewOrderOptions";
            this.tabPageViewOrderOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageViewOrderOptions.Size = new System.Drawing.Size(501, 328);
            this.tabPageViewOrderOptions.TabIndex = 1;
            this.tabPageViewOrderOptions.Text = "Options";
            // 
            // textBoxNote
            // 
            this.textBoxNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxNote.EnableAutoDragDrop = true;
            this.textBoxNote.Location = new System.Drawing.Point(8, 211);
            this.textBoxNote.Name = "textBoxNote";
            this.textBoxNote.Size = new System.Drawing.Size(477, 72);
            this.textBoxNote.TabIndex = 4;
            this.textBoxNote.Text = "";
            this.textBoxNote.TextChanged += new System.EventHandler(this.textBoxNote_TextChanged_1);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(11, 194);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(139, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Additional Notes (or a URL):";
            // 
            // buttonPayment
            // 
            this.buttonPayment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPayment.Location = new System.Drawing.Point(175, 288);
            this.buttonPayment.Name = "buttonPayment";
            this.buttonPayment.Size = new System.Drawing.Size(172, 23);
            this.buttonPayment.TabIndex = 2;
            this.buttonPayment.Text = "Payment...";
            this.buttonPayment.UseVisualStyleBackColor = true;
            this.buttonPayment.Click += new System.EventHandler(this.buttonPayment_Click);
            // 
            // groupBoxListingOptions
            // 
            this.groupBoxListingOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxListingOptions.Controls.Add(this.textBoxMLS);
            this.groupBoxListingOptions.Controls.Add(this.textBoxMlsUrl);
            this.groupBoxListingOptions.Controls.Add(this.label12);
            this.groupBoxListingOptions.Controls.Add(this.labelPercent);
            this.groupBoxListingOptions.Controls.Add(this.label9);
            this.groupBoxListingOptions.Controls.Add(this.labelTax);
            this.groupBoxListingOptions.Controls.Add(this.numericUpDownTax);
            this.groupBoxListingOptions.Controls.Add(this.textBoxTotal);
            this.groupBoxListingOptions.Controls.Add(this.numericUpDownPrice);
            this.groupBoxListingOptions.Controls.Add(this.numericUpDownDaysValid);
            this.groupBoxListingOptions.Controls.Add(this.labelDays);
            this.groupBoxListingOptions.Controls.Add(this.labelValidFor);
            this.groupBoxListingOptions.Controls.Add(this.labelPrice);
            this.groupBoxListingOptions.Controls.Add(this.buttonCheckLink);
            this.groupBoxListingOptions.Controls.Add(this.textExternalLink);
            this.groupBoxListingOptions.Controls.Add(this.radioButtonExternalLink);
            this.groupBoxListingOptions.Controls.Add(this.radioButtonFloorplan);
            this.groupBoxListingOptions.Location = new System.Drawing.Point(9, 6);
            this.groupBoxListingOptions.Name = "groupBoxListingOptions";
            this.groupBoxListingOptions.Size = new System.Drawing.Size(477, 185);
            this.groupBoxListingOptions.TabIndex = 1;
            this.groupBoxListingOptions.TabStop = false;
            this.groupBoxListingOptions.Text = "Listing Options";
            // 
            // textBoxMLS
            // 
            this.textBoxMLS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMLS.EnableAutoDragDrop = true;
            this.textBoxMLS.Location = new System.Drawing.Point(371, 13);
            this.textBoxMLS.Multiline = false;
            this.textBoxMLS.Name = "textBoxMLS";
            this.textBoxMLS.Size = new System.Drawing.Size(100, 21);
            this.textBoxMLS.TabIndex = 24;
            this.textBoxMLS.Text = "";
            this.textBoxMLS.TextChanged += new System.EventHandler(this.textBoxMLS_TextChanged_1);
            // 
            // textBoxMlsUrl
            // 
            this.textBoxMlsUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMlsUrl.Location = new System.Drawing.Point(62, 156);
            this.textBoxMlsUrl.Name = "textBoxMlsUrl";
            this.textBoxMlsUrl.Size = new System.Drawing.Size(405, 20);
            this.textBoxMlsUrl.TabIndex = 5;
            this.textBoxMlsUrl.TextChanged += new System.EventHandler(this.textBoxMlsUrl_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 161);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "MLS URL";
            // 
            // labelPercent
            // 
            this.labelPercent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPercent.AutoSize = true;
            this.labelPercent.Location = new System.Drawing.Point(379, 124);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.Size = new System.Drawing.Size(24, 13);
            this.labelPercent.TabIndex = 23;
            this.labelPercent.Text = "% =";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(333, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "MLS#";
            // 
            // labelTax
            // 
            this.labelTax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTax.AutoSize = true;
            this.labelTax.Location = new System.Drawing.Point(313, 124);
            this.labelTax.Name = "labelTax";
            this.labelTax.Size = new System.Drawing.Size(31, 13);
            this.labelTax.TabIndex = 22;
            this.labelTax.Text = "+Tax";
            // 
            // numericUpDownTax
            // 
            this.numericUpDownTax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownTax.Location = new System.Drawing.Point(344, 121);
            this.numericUpDownTax.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.numericUpDownTax.Name = "numericUpDownTax";
            this.numericUpDownTax.Size = new System.Drawing.Size(36, 20);
            this.numericUpDownTax.TabIndex = 21;
            this.numericUpDownTax.Value = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.numericUpDownTax.ValueChanged += new System.EventHandler(this.numericUpDownTax_ValueChanged);
            // 
            // textBoxTotal
            // 
            this.textBoxTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxTotal.ForeColor = System.Drawing.Color.Black;
            this.textBoxTotal.Location = new System.Drawing.Point(407, 122);
            this.textBoxTotal.Name = "textBoxTotal";
            this.textBoxTotal.ReadOnly = true;
            this.textBoxTotal.Size = new System.Drawing.Size(52, 20);
            this.textBoxTotal.TabIndex = 20;
            this.textBoxTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // numericUpDownPrice
            // 
            this.numericUpDownPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownPrice.DecimalPlaces = 2;
            this.numericUpDownPrice.Location = new System.Drawing.Point(244, 121);
            this.numericUpDownPrice.Name = "numericUpDownPrice";
            this.numericUpDownPrice.Size = new System.Drawing.Size(67, 20);
            this.numericUpDownPrice.TabIndex = 18;
            this.numericUpDownPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownPrice.ValueChanged += new System.EventHandler(this.numericUpDownPrice_ValueChanged);
            // 
            // numericUpDownDaysValid
            // 
            this.numericUpDownDaysValid.Location = new System.Drawing.Point(62, 121);
            this.numericUpDownDaysValid.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.numericUpDownDaysValid.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDaysValid.Name = "numericUpDownDaysValid";
            this.numericUpDownDaysValid.Size = new System.Drawing.Size(43, 20);
            this.numericUpDownDaysValid.TabIndex = 17;
            this.numericUpDownDaysValid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownDaysValid.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.numericUpDownDaysValid.ValueChanged += new System.EventHandler(this.numericUpDownDaysValid_ValueChanged);
            // 
            // labelDays
            // 
            this.labelDays.AutoSize = true;
            this.labelDays.Location = new System.Drawing.Point(108, 125);
            this.labelDays.Name = "labelDays";
            this.labelDays.Size = new System.Drawing.Size(29, 13);
            this.labelDays.TabIndex = 16;
            this.labelDays.Text = "days";
            // 
            // labelValidFor
            // 
            this.labelValidFor.AutoSize = true;
            this.labelValidFor.Location = new System.Drawing.Point(13, 125);
            this.labelValidFor.Name = "labelValidFor";
            this.labelValidFor.Size = new System.Drawing.Size(45, 13);
            this.labelValidFor.TabIndex = 15;
            this.labelValidFor.Text = "Valid for";
            // 
            // labelPrice
            // 
            this.labelPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPrice.AutoSize = true;
            this.labelPrice.Location = new System.Drawing.Point(194, 124);
            this.labelPrice.Name = "labelPrice";
            this.labelPrice.Size = new System.Drawing.Size(49, 13);
            this.labelPrice.TabIndex = 13;
            this.labelPrice.Text = "Price ($):";
            // 
            // buttonCheckLink
            // 
            this.buttonCheckLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCheckLink.Enabled = false;
            this.buttonCheckLink.Location = new System.Drawing.Point(274, 47);
            this.buttonCheckLink.Name = "buttonCheckLink";
            this.buttonCheckLink.Size = new System.Drawing.Size(197, 22);
            this.buttonCheckLink.TabIndex = 11;
            this.buttonCheckLink.Text = "Validate this Link Now...";
            this.buttonCheckLink.UseVisualStyleBackColor = true;
            this.buttonCheckLink.Click += new System.EventHandler(this.buttonCheckLink_Click);
            // 
            // textExternalLink
            // 
            this.textExternalLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textExternalLink.Location = new System.Drawing.Point(39, 75);
            this.textExternalLink.Name = "textExternalLink";
            this.textExternalLink.Size = new System.Drawing.Size(432, 20);
            this.textExternalLink.TabIndex = 10;
            this.textExternalLink.TextChanged += new System.EventHandler(this.textExternalLink_TextChanged);
            // 
            // radioButtonExternalLink
            // 
            this.radioButtonExternalLink.AutoSize = true;
            this.radioButtonExternalLink.Location = new System.Drawing.Point(19, 54);
            this.radioButtonExternalLink.Name = "radioButtonExternalLink";
            this.radioButtonExternalLink.Size = new System.Drawing.Size(86, 17);
            this.radioButtonExternalLink.TabIndex = 9;
            this.radioButtonExternalLink.Text = "External Link";
            this.radioButtonExternalLink.UseVisualStyleBackColor = true;
            this.radioButtonExternalLink.CheckedChanged += new System.EventHandler(this.radioButtonExternalLink_CheckedChanged);
            // 
            // radioButtonFloorplan
            // 
            this.radioButtonFloorplan.AutoSize = true;
            this.radioButtonFloorplan.Checked = true;
            this.radioButtonFloorplan.Location = new System.Drawing.Point(19, 26);
            this.radioButtonFloorplan.Name = "radioButtonFloorplan";
            this.radioButtonFloorplan.Size = new System.Drawing.Size(72, 17);
            this.radioButtonFloorplan.TabIndex = 8;
            this.radioButtonFloorplan.TabStop = true;
            this.radioButtonFloorplan.Text = "Floor Plan";
            this.radioButtonFloorplan.UseVisualStyleBackColor = true;
            this.radioButtonFloorplan.CheckedChanged += new System.EventHandler(this.radioButtonFloorplan_CheckedChanged);
            // 
            // tabPageGenerateViewOrder
            // 
            this.tabPageGenerateViewOrder.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageGenerateViewOrder.Controls.Add(this.textBox300Chars);
            this.tabPageGenerateViewOrder.Controls.Add(this.label8);
            this.tabPageGenerateViewOrder.Controls.Add(this.buttonSendEmailk);
            this.tabPageGenerateViewOrder.Controls.Add(this.buttonGenerateURL);
            this.tabPageGenerateViewOrder.Controls.Add(this.textBoxViewOrderURL);
            this.tabPageGenerateViewOrder.Location = new System.Drawing.Point(4, 22);
            this.tabPageGenerateViewOrder.Name = "tabPageGenerateViewOrder";
            this.tabPageGenerateViewOrder.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGenerateViewOrder.Size = new System.Drawing.Size(501, 328);
            this.tabPageGenerateViewOrder.TabIndex = 3;
            this.tabPageGenerateViewOrder.Text = "Generate";
            // 
            // textBox300Chars
            // 
            this.textBox300Chars.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox300Chars.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox300Chars.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox300Chars.Location = new System.Drawing.Point(12, 169);
            this.textBox300Chars.Multiline = true;
            this.textBox300Chars.Name = "textBox300Chars";
            this.textBox300Chars.ReadOnly = true;
            this.textBox300Chars.Size = new System.Drawing.Size(477, 147);
            this.textBox300Chars.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 153);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(322, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = ".. or Copy-Paste this message to REALTOR email (up to 300 chars)";
            // 
            // buttonSendEmailk
            // 
            this.buttonSendEmailk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSendEmailk.Location = new System.Drawing.Point(170, 119);
            this.buttonSendEmailk.Name = "buttonSendEmailk";
            this.buttonSendEmailk.Size = new System.Drawing.Size(138, 25);
            this.buttonSendEmailk.TabIndex = 17;
            this.buttonSendEmailk.Text = "Email Customer...";
            this.buttonSendEmailk.UseVisualStyleBackColor = true;
            this.buttonSendEmailk.Click += new System.EventHandler(this.buttonSendEmailk_Click);
            // 
            // buttonGenerateURL
            // 
            this.buttonGenerateURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGenerateURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonGenerateURL.Location = new System.Drawing.Point(109, 30);
            this.buttonGenerateURL.Name = "buttonGenerateURL";
            this.buttonGenerateURL.Size = new System.Drawing.Size(243, 35);
            this.buttonGenerateURL.TabIndex = 12;
            this.buttonGenerateURL.Text = "Generate ViewOrder URL";
            this.buttonGenerateURL.UseVisualStyleBackColor = true;
            this.buttonGenerateURL.Click += new System.EventHandler(this.buttonGenerateURL_Click);
            // 
            // textBoxViewOrderURL
            // 
            this.textBoxViewOrderURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxViewOrderURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxViewOrderURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxViewOrderURL.Location = new System.Drawing.Point(12, 81);
            this.textBoxViewOrderURL.Name = "textBoxViewOrderURL";
            this.textBoxViewOrderURL.ReadOnly = true;
            this.textBoxViewOrderURL.Size = new System.Drawing.Size(477, 22);
            this.textBoxViewOrderURL.TabIndex = 14;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(208, 9);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(111, 13);
            this.label13.TabIndex = 3;
            this.label13.Text = "Type Street name in...";
            // 
            // textBoxStreetName
            // 
            this.textBoxStreetName.Location = new System.Drawing.Point(326, 6);
            this.textBoxStreetName.Name = "textBoxStreetName";
            this.textBoxStreetName.Size = new System.Drawing.Size(152, 20);
            this.textBoxStreetName.TabIndex = 4;
            this.textBoxStreetName.TextChanged += new System.EventHandler(this.textBoxStreetName_TextChanged);
            // 
            // SetViewOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(502, 376);
            this.ControlBox = false;
            this.Controls.Add(this.buttonFinish);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonPrev);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControlSteps);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetViewOrder";
            this.Text = "ViewOrder Dialog";
            this.Load += new System.EventHandler(this.NewViewOrder_Load);
            this.tabControlSteps.ResumeLayout(false);
            this.tabPageCheckAddress.ResumeLayout(false);
            this.tabControlAddressPicking.ResumeLayout(false);
            this.tabPagePick.ResumeLayout(false);
            this.tabPagePick.PerformLayout();
            this.tabPageTypeIn.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageViewOrderOptions.ResumeLayout(false);
            this.tabPageViewOrderOptions.PerformLayout();
            this.groupBoxListingOptions.ResumeLayout(false);
            this.groupBoxListingOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDaysValid)).EndInit();
            this.tabPageGenerateViewOrder.ResumeLayout(false);
            this.tabPageGenerateViewOrder.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonPrev;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.TabPage tabPageGenerateViewOrder;
        private System.Windows.Forms.Button buttonSendEmailk;
        private System.Windows.Forms.Button buttonGenerateURL;
        private System.Windows.Forms.TextBox textBoxViewOrderURL;
        private System.Windows.Forms.TabPage tabPageViewOrderOptions;
        private System.Windows.Forms.Button buttonPayment;
        private System.Windows.Forms.GroupBox groupBoxListingOptions;
        private System.Windows.Forms.Label labelPercent;
        private System.Windows.Forms.Label labelTax;
        private System.Windows.Forms.NumericUpDown numericUpDownTax;
        private System.Windows.Forms.TextBox textBoxTotal;
        private System.Windows.Forms.NumericUpDown numericUpDownPrice;
        private System.Windows.Forms.NumericUpDown numericUpDownDaysValid;
        private System.Windows.Forms.Label labelDays;
        private System.Windows.Forms.Label labelValidFor;
        private System.Windows.Forms.Label labelPrice;
        private System.Windows.Forms.Button buttonCheckLink;
        private System.Windows.Forms.TextBox textExternalLink;
        private System.Windows.Forms.RadioButton radioButtonExternalLink;
        private System.Windows.Forms.RadioButton radioButtonFloorplan;
        private System.Windows.Forms.TabPage tabPageCheckAddress;
        private System.Windows.Forms.TabControl tabControlAddressPicking;
        private System.Windows.Forms.TabPage tabPagePick;
        private System.Windows.Forms.ListView listViewAddresses;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.TabPage tabPageTypeIn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxCountry;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonCheckAddress;
        private System.Windows.Forms.TextBox textPostalCode;
        private System.Windows.Forms.TextBox textBuildingNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textProvince;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textCity;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textUnitNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textStreet;
        private WizardPages tabControlSteps;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxCity;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox300Chars;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxMlsUrl;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RichTextBox textBoxNote;
        private System.Windows.Forms.RichTextBox textBoxMLS;
        private System.Windows.Forms.TextBox textBoxStreetName;
        private System.Windows.Forms.Label label13;
    }
}