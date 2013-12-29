namespace SuperAdminConsole
{
    partial class NewAccount
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
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxEmail = new System.Windows.Forms.TextBox();
            this.buttonSendVerify = new System.Windows.Forms.Button();
            this.tabControAccountTypes = new System.Windows.Forms.TabControl();
            this.tabPageNewCustomer = new System.Windows.Forms.TabPage();
            this.tabPageNewDevAdmin = new System.Windows.Forms.TabPage();
            this.buttonCreateDevAdmin = new System.Windows.Forms.Button();
            this.textBoxPassword2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxLoginName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxDeveloper = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxNickname = new System.Windows.Forms.TextBox();
            this.tabPageNewBrokerage = new System.Windows.Forms.TabPage();
            this.tabPageNewAgent = new System.Windows.Forms.TabPage();
            this.comboBoxCountry = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxPostal = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxProvince = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxCity = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxStreet = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxPhone = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxWebsite = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonCreateBrokerage = new System.Windows.Forms.Button();
            this.buttonCreateAgent = new System.Windows.Forms.Button();
            this.textBoxPWDconfirm = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxPWD = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxUID = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.comboBoxBrokerages = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tabControAccountTypes.SuspendLayout();
            this.tabPageNewCustomer.SuspendLayout();
            this.tabPageNewDevAdmin.SuspendLayout();
            this.tabPageNewBrokerage.SuspendLayout();
            this.tabPageNewAgent.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Email";
            // 
            // textBoxEmail
            // 
            this.textBoxEmail.Location = new System.Drawing.Point(14, 159);
            this.textBoxEmail.Name = "textBoxEmail";
            this.textBoxEmail.Size = new System.Drawing.Size(228, 20);
            this.textBoxEmail.TabIndex = 4;
            this.textBoxEmail.TextChanged += new System.EventHandler(this.onTextChanged);
            // 
            // buttonSendVerify
            // 
            this.buttonSendVerify.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSendVerify.Location = new System.Drawing.Point(124, 44);
            this.buttonSendVerify.Name = "buttonSendVerify";
            this.buttonSendVerify.Size = new System.Drawing.Size(232, 27);
            this.buttonSendVerify.TabIndex = 4;
            this.buttonSendVerify.Text = "Send Verification Messaje";
            this.buttonSendVerify.UseVisualStyleBackColor = true;
            this.buttonSendVerify.Click += new System.EventHandler(this.buttonSendVerify_Click);
            // 
            // tabControAccountTypes
            // 
            this.tabControAccountTypes.Controls.Add(this.tabPageNewAgent);
            this.tabControAccountTypes.Controls.Add(this.tabPageNewDevAdmin);
            this.tabControAccountTypes.Controls.Add(this.tabPageNewBrokerage);
            this.tabControAccountTypes.Controls.Add(this.tabPageNewCustomer);
            this.tabControAccountTypes.Location = new System.Drawing.Point(1, 3);
            this.tabControAccountTypes.Name = "tabControAccountTypes";
            this.tabControAccountTypes.SelectedIndex = 0;
            this.tabControAccountTypes.Size = new System.Drawing.Size(472, 135);
            this.tabControAccountTypes.TabIndex = 5;
            this.tabControAccountTypes.SelectedIndexChanged += new System.EventHandler(this.onTextChanged);
            // 
            // tabPageNewCustomer
            // 
            this.tabPageNewCustomer.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageNewCustomer.Controls.Add(this.buttonSendVerify);
            this.tabPageNewCustomer.Location = new System.Drawing.Point(4, 22);
            this.tabPageNewCustomer.Name = "tabPageNewCustomer";
            this.tabPageNewCustomer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNewCustomer.Size = new System.Drawing.Size(464, 109);
            this.tabPageNewCustomer.TabIndex = 0;
            this.tabPageNewCustomer.Text = "New Selling Agent";
            // 
            // tabPageNewDevAdmin
            // 
            this.tabPageNewDevAdmin.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageNewDevAdmin.Controls.Add(this.buttonCreateDevAdmin);
            this.tabPageNewDevAdmin.Controls.Add(this.textBoxPassword2);
            this.tabPageNewDevAdmin.Controls.Add(this.label5);
            this.tabPageNewDevAdmin.Controls.Add(this.textBoxPassword);
            this.tabPageNewDevAdmin.Controls.Add(this.label4);
            this.tabPageNewDevAdmin.Controls.Add(this.textBoxLoginName);
            this.tabPageNewDevAdmin.Controls.Add(this.label3);
            this.tabPageNewDevAdmin.Controls.Add(this.comboBoxDeveloper);
            this.tabPageNewDevAdmin.Location = new System.Drawing.Point(4, 22);
            this.tabPageNewDevAdmin.Name = "tabPageNewDevAdmin";
            this.tabPageNewDevAdmin.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNewDevAdmin.Size = new System.Drawing.Size(464, 109);
            this.tabPageNewDevAdmin.TabIndex = 1;
            this.tabPageNewDevAdmin.Text = "New DeveAdmin";
            // 
            // buttonCreateDevAdmin
            // 
            this.buttonCreateDevAdmin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonCreateDevAdmin.Location = new System.Drawing.Point(143, 70);
            this.buttonCreateDevAdmin.Name = "buttonCreateDevAdmin";
            this.buttonCreateDevAdmin.Size = new System.Drawing.Size(202, 31);
            this.buttonCreateDevAdmin.TabIndex = 11;
            this.buttonCreateDevAdmin.Text = "Create";
            this.buttonCreateDevAdmin.UseVisualStyleBackColor = true;
            this.buttonCreateDevAdmin.Click += new System.EventHandler(this.buttonCreateDevAdmin_Click);
            // 
            // textBoxPassword2
            // 
            this.textBoxPassword2.Location = new System.Drawing.Point(308, 39);
            this.textBoxPassword2.Name = "textBoxPassword2";
            this.textBoxPassword2.Size = new System.Drawing.Size(144, 20);
            this.textBoxPassword2.TabIndex = 3;
            this.textBoxPassword2.TextChanged += new System.EventHandler(this.onTextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(241, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Type Again";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(82, 39);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(144, 20);
            this.textBoxPassword.TabIndex = 2;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.onTextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Password";
            // 
            // textBoxLoginName
            // 
            this.textBoxLoginName.Location = new System.Drawing.Point(82, 10);
            this.textBoxLoginName.Name = "textBoxLoginName";
            this.textBoxLoginName.Size = new System.Drawing.Size(144, 20);
            this.textBoxLoginName.TabIndex = 1;
            this.textBoxLoginName.TextChanged += new System.EventHandler(this.onTextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Login Name";
            // 
            // comboBoxDeveloper
            // 
            this.comboBoxDeveloper.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDeveloper.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxDeveloper.FormattingEnabled = true;
            this.comboBoxDeveloper.Location = new System.Drawing.Point(308, 10);
            this.comboBoxDeveloper.Name = "comboBoxDeveloper";
            this.comboBoxDeveloper.Size = new System.Drawing.Size(147, 21);
            this.comboBoxDeveloper.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(245, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Nickname";
            // 
            // textBoxNickname
            // 
            this.textBoxNickname.Location = new System.Drawing.Point(248, 159);
            this.textBoxNickname.Name = "textBoxNickname";
            this.textBoxNickname.Size = new System.Drawing.Size(211, 20);
            this.textBoxNickname.TabIndex = 5;
            // 
            // tabPageNewBrokerage
            // 
            this.tabPageNewBrokerage.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageNewBrokerage.Controls.Add(this.buttonCreateBrokerage);
            this.tabPageNewBrokerage.Controls.Add(this.textBoxWebsite);
            this.tabPageNewBrokerage.Controls.Add(this.label7);
            this.tabPageNewBrokerage.Controls.Add(this.textBoxPhone);
            this.tabPageNewBrokerage.Controls.Add(this.label6);
            this.tabPageNewBrokerage.Controls.Add(this.comboBoxCountry);
            this.tabPageNewBrokerage.Controls.Add(this.label12);
            this.tabPageNewBrokerage.Controls.Add(this.textBoxPostal);
            this.tabPageNewBrokerage.Controls.Add(this.label11);
            this.tabPageNewBrokerage.Controls.Add(this.textBoxProvince);
            this.tabPageNewBrokerage.Controls.Add(this.label10);
            this.tabPageNewBrokerage.Controls.Add(this.textBoxCity);
            this.tabPageNewBrokerage.Controls.Add(this.label9);
            this.tabPageNewBrokerage.Controls.Add(this.textBoxStreet);
            this.tabPageNewBrokerage.Controls.Add(this.label8);
            this.tabPageNewBrokerage.Location = new System.Drawing.Point(4, 22);
            this.tabPageNewBrokerage.Name = "tabPageNewBrokerage";
            this.tabPageNewBrokerage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNewBrokerage.Size = new System.Drawing.Size(464, 109);
            this.tabPageNewBrokerage.TabIndex = 2;
            this.tabPageNewBrokerage.Text = "New Brokerage";
            // 
            // tabPageNewAgent
            // 
            this.tabPageNewAgent.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageNewAgent.Controls.Add(this.label16);
            this.tabPageNewAgent.Controls.Add(this.textBoxPWDconfirm);
            this.tabPageNewAgent.Controls.Add(this.label13);
            this.tabPageNewAgent.Controls.Add(this.textBoxPWD);
            this.tabPageNewAgent.Controls.Add(this.label14);
            this.tabPageNewAgent.Controls.Add(this.textBoxUID);
            this.tabPageNewAgent.Controls.Add(this.label15);
            this.tabPageNewAgent.Controls.Add(this.comboBoxBrokerages);
            this.tabPageNewAgent.Controls.Add(this.buttonCreateAgent);
            this.tabPageNewAgent.Location = new System.Drawing.Point(4, 22);
            this.tabPageNewAgent.Name = "tabPageNewAgent";
            this.tabPageNewAgent.Size = new System.Drawing.Size(464, 109);
            this.tabPageNewAgent.TabIndex = 3;
            this.tabPageNewAgent.Text = "New Agent";
            // 
            // comboBoxCountry
            // 
            this.comboBoxCountry.FormattingEnabled = true;
            this.comboBoxCountry.Items.AddRange(new object[] {
            "CA",
            "US"});
            this.comboBoxCountry.Location = new System.Drawing.Point(225, 58);
            this.comboBoxCountry.Name = "comboBoxCountry";
            this.comboBoxCountry.Size = new System.Drawing.Size(80, 21);
            this.comboBoxCountry.TabIndex = 38;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(173, 62);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(43, 13);
            this.label12.TabIndex = 35;
            this.label12.Text = "Country";
            // 
            // textBoxPostal
            // 
            this.textBoxPostal.Location = new System.Drawing.Point(72, 59);
            this.textBoxPostal.Name = "textBoxPostal";
            this.textBoxPostal.Size = new System.Drawing.Size(60, 20);
            this.textBoxPostal.TabIndex = 34;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 62);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(36, 13);
            this.label11.TabIndex = 33;
            this.label11.Text = "Postal";
            // 
            // textBoxProvince
            // 
            this.textBoxProvince.Location = new System.Drawing.Point(276, 33);
            this.textBoxProvince.Name = "textBoxProvince";
            this.textBoxProvince.Size = new System.Drawing.Size(29, 20);
            this.textBoxProvince.TabIndex = 32;
            this.textBoxProvince.Text = "ON";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(224, 36);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 13);
            this.label10.TabIndex = 31;
            this.label10.Text = "Province";
            // 
            // textBoxCity
            // 
            this.textBoxCity.Location = new System.Drawing.Point(72, 33);
            this.textBoxCity.Name = "textBoxCity";
            this.textBoxCity.Size = new System.Drawing.Size(135, 20);
            this.textBoxCity.TabIndex = 30;
            this.textBoxCity.Text = "Toronto";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(24, 13);
            this.label9.TabIndex = 29;
            this.label9.Text = "City";
            // 
            // textBoxStreet
            // 
            this.textBoxStreet.Location = new System.Drawing.Point(72, 6);
            this.textBoxStreet.Name = "textBoxStreet";
            this.textBoxStreet.Size = new System.Drawing.Size(233, 20);
            this.textBoxStreet.TabIndex = 28;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "No, Street";
            // 
            // textBoxPhone
            // 
            this.textBoxPhone.Location = new System.Drawing.Point(72, 85);
            this.textBoxPhone.Name = "textBoxPhone";
            this.textBoxPhone.Size = new System.Drawing.Size(91, 20);
            this.textBoxPhone.TabIndex = 40;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 39;
            this.label6.Text = "Phone #";
            // 
            // textBoxWebsite
            // 
            this.textBoxWebsite.Location = new System.Drawing.Point(225, 85);
            this.textBoxWebsite.Name = "textBoxWebsite";
            this.textBoxWebsite.Size = new System.Drawing.Size(229, 20);
            this.textBoxWebsite.TabIndex = 42;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(173, 88);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 41;
            this.label7.Text = "Website";
            // 
            // buttonCreateBrokerage
            // 
            this.buttonCreateBrokerage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonCreateBrokerage.Location = new System.Drawing.Point(344, 6);
            this.buttonCreateBrokerage.Name = "buttonCreateBrokerage";
            this.buttonCreateBrokerage.Size = new System.Drawing.Size(110, 31);
            this.buttonCreateBrokerage.TabIndex = 43;
            this.buttonCreateBrokerage.Text = "Create";
            this.buttonCreateBrokerage.UseVisualStyleBackColor = true;
            this.buttonCreateBrokerage.Click += new System.EventHandler(this.buttonCreateBrokerage_Click);
            // 
            // buttonCreateAgent
            // 
            this.buttonCreateAgent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonCreateAgent.Location = new System.Drawing.Point(317, 71);
            this.buttonCreateAgent.Name = "buttonCreateAgent";
            this.buttonCreateAgent.Size = new System.Drawing.Size(134, 27);
            this.buttonCreateAgent.TabIndex = 5;
            this.buttonCreateAgent.Text = "Create account";
            this.buttonCreateAgent.UseVisualStyleBackColor = true;
            this.buttonCreateAgent.Click += new System.EventHandler(this.buttonCreateAgent_Click);
            // 
            // textBoxPWDconfirm
            // 
            this.textBoxPWDconfirm.Location = new System.Drawing.Point(307, 38);
            this.textBoxPWDconfirm.Name = "textBoxPWDconfirm";
            this.textBoxPWDconfirm.Size = new System.Drawing.Size(144, 20);
            this.textBoxPWDconfirm.TabIndex = 15;
            this.textBoxPWDconfirm.TextChanged += new System.EventHandler(this.textBoxPWDconfirm_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(240, 41);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 13);
            this.label13.TabIndex = 18;
            this.label13.Text = "Type Again";
            // 
            // textBoxPWD
            // 
            this.textBoxPWD.Location = new System.Drawing.Point(81, 38);
            this.textBoxPWD.Name = "textBoxPWD";
            this.textBoxPWD.Size = new System.Drawing.Size(144, 20);
            this.textBoxPWD.TabIndex = 14;
            this.textBoxPWD.TextChanged += new System.EventHandler(this.textBoxPWD_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(11, 41);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 13);
            this.label14.TabIndex = 17;
            this.label14.Text = "Password";
            // 
            // textBoxUID
            // 
            this.textBoxUID.Location = new System.Drawing.Point(81, 9);
            this.textBoxUID.Name = "textBoxUID";
            this.textBoxUID.Size = new System.Drawing.Size(144, 20);
            this.textBoxUID.TabIndex = 13;
            this.textBoxUID.TextChanged += new System.EventHandler(this.textBoxUID_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(11, 12);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(64, 13);
            this.label15.TabIndex = 16;
            this.label15.Text = "Login Name";
            // 
            // comboBoxBrokerages
            // 
            this.comboBoxBrokerages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBrokerages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxBrokerages.FormattingEnabled = true;
            this.comboBoxBrokerages.Location = new System.Drawing.Point(81, 70);
            this.comboBoxBrokerages.Name = "comboBoxBrokerages";
            this.comboBoxBrokerages.Size = new System.Drawing.Size(220, 21);
            this.comboBoxBrokerages.TabIndex = 12;
            this.comboBoxBrokerages.SelectedIndexChanged += new System.EventHandler(this.comboBoxBrokerages_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(11, 71);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(56, 13);
            this.label16.TabIndex = 19;
            this.label16.Text = "Brokerage";
            // 
            // NewAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 188);
            this.Controls.Add(this.textBoxNickname);
            this.Controls.Add(this.tabControAccountTypes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxEmail);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewAccount";
            this.Text = "Create New Account";
            this.tabControAccountTypes.ResumeLayout(false);
            this.tabPageNewCustomer.ResumeLayout(false);
            this.tabPageNewDevAdmin.ResumeLayout(false);
            this.tabPageNewDevAdmin.PerformLayout();
            this.tabPageNewBrokerage.ResumeLayout(false);
            this.tabPageNewBrokerage.PerformLayout();
            this.tabPageNewAgent.ResumeLayout(false);
            this.tabPageNewAgent.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxEmail;
        private System.Windows.Forms.Button buttonSendVerify;
        private System.Windows.Forms.TabControl tabControAccountTypes;
        private System.Windows.Forms.TabPage tabPageNewCustomer;
        private System.Windows.Forms.TabPage tabPageNewDevAdmin;
        private System.Windows.Forms.Button buttonCreateDevAdmin;
        private System.Windows.Forms.TextBox textBoxPassword2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxLoginName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxDeveloper;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxNickname;
        private System.Windows.Forms.TabPage tabPageNewAgent;
        private System.Windows.Forms.TabPage tabPageNewBrokerage;
        private System.Windows.Forms.TextBox textBoxWebsite;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxPhone;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxCountry;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxPostal;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxProvince;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxCity;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxStreet;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonCreateBrokerage;
        private System.Windows.Forms.Button buttonCreateAgent;
        private System.Windows.Forms.TextBox textBoxPWDconfirm;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxPWD;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxUID;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox comboBoxBrokerages;
        private System.Windows.Forms.Label label16;
    }
}