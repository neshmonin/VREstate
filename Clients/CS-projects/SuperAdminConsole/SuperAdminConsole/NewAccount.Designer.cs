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
            this.tabControAccountTypes.SuspendLayout();
            this.tabPageNewCustomer.SuspendLayout();
            this.tabPageNewDevAdmin.SuspendLayout();
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
            this.textBoxEmail.Size = new System.Drawing.Size(264, 20);
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
            this.tabControAccountTypes.Controls.Add(this.tabPageNewCustomer);
            this.tabControAccountTypes.Controls.Add(this.tabPageNewDevAdmin);
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
            this.tabPageNewCustomer.Text = "New Customer";
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
            this.label1.Location = new System.Drawing.Point(295, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Nickname";
            // 
            // textBoxNickname
            // 
            this.textBoxNickname.Location = new System.Drawing.Point(298, 159);
            this.textBoxNickname.Name = "textBoxNickname";
            this.textBoxNickname.Size = new System.Drawing.Size(161, 20);
            this.textBoxNickname.TabIndex = 5;
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
    }
}