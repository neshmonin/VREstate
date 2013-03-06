namespace SuperAdminConsole
{
    partial class LoginForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.checkBoxIsSuperadmin = new System.Windows.Forms.CheckBox();
            this.radioButtonMainServer = new System.Windows.Forms.RadioButton();
            this.radioButtonAlternativeServer = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxEstateDeveloper = new System.Windows.Forms.TextBox();
            this.labelEstateDeveloper = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Login:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Password:";
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(17, 75);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(161, 20);
            this.tbLogin.TabIndex = 4;
            this.tbLogin.TextChanged += new System.EventHandler(this.tbLogin_TextChanged);
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(17, 119);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '#';
            this.tbPassword.Size = new System.Drawing.Size(161, 20);
            this.tbPassword.TabIndex = 5;
            this.tbPassword.TextChanged += new System.EventHandler(this.tbPassword_TextChanged);
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(16, 231);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(161, 26);
            this.buttonLogin.TabIndex = 6;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // checkBoxIsSuperadmin
            // 
            this.checkBoxIsSuperadmin.AutoSize = true;
            this.checkBoxIsSuperadmin.Location = new System.Drawing.Point(17, 155);
            this.checkBoxIsSuperadmin.Name = "checkBoxIsSuperadmin";
            this.checkBoxIsSuperadmin.Size = new System.Drawing.Size(156, 17);
            this.checkBoxIsSuperadmin.TabIndex = 7;
            this.checkBoxIsSuperadmin.Text = "Login as a DeveloperAdmin";
            this.checkBoxIsSuperadmin.UseVisualStyleBackColor = true;
            this.checkBoxIsSuperadmin.CheckedChanged += new System.EventHandler(this.checkBoxIsSuperadmin_CheckedChanged);
            // 
            // radioButtonMainServer
            // 
            this.radioButtonMainServer.AutoSize = true;
            this.radioButtonMainServer.Checked = true;
            this.radioButtonMainServer.Location = new System.Drawing.Point(14, 19);
            this.radioButtonMainServer.Name = "radioButtonMainServer";
            this.radioButtonMainServer.Size = new System.Drawing.Size(48, 17);
            this.radioButtonMainServer.TabIndex = 8;
            this.radioButtonMainServer.TabStop = true;
            this.radioButtonMainServer.Text = "Main";
            this.radioButtonMainServer.UseVisualStyleBackColor = true;
            // 
            // radioButtonAlternativeServer
            // 
            this.radioButtonAlternativeServer.AutoSize = true;
            this.radioButtonAlternativeServer.Location = new System.Drawing.Point(80, 19);
            this.radioButtonAlternativeServer.Name = "radioButtonAlternativeServer";
            this.radioButtonAlternativeServer.Size = new System.Drawing.Size(75, 17);
            this.radioButtonAlternativeServer.TabIndex = 9;
            this.radioButtonAlternativeServer.Text = "Alternative";
            this.radioButtonAlternativeServer.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonMainServer);
            this.groupBox1.Controls.Add(this.radioButtonAlternativeServer);
            this.groupBox1.Location = new System.Drawing.Point(16, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(161, 47);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server";
            // 
            // textBoxEstateDeveloper
            // 
            this.textBoxEstateDeveloper.Location = new System.Drawing.Point(55, 196);
            this.textBoxEstateDeveloper.Name = "textBoxEstateDeveloper";
            this.textBoxEstateDeveloper.Size = new System.Drawing.Size(122, 20);
            this.textBoxEstateDeveloper.TabIndex = 12;
            this.textBoxEstateDeveloper.Text = "Resale";
            this.textBoxEstateDeveloper.TextChanged += new System.EventHandler(this.textBoxEstateDeveloper_TextChanged);
            // 
            // labelEstateDeveloper
            // 
            this.labelEstateDeveloper.AutoSize = true;
            this.labelEstateDeveloper.Location = new System.Drawing.Point(54, 180);
            this.labelEstateDeveloper.Name = "labelEstateDeveloper";
            this.labelEstateDeveloper.Size = new System.Drawing.Size(123, 13);
            this.labelEstateDeveloper.TabIndex = 11;
            this.labelEstateDeveloper.Text = "Estate Developer Name:";
            // 
            // LoginForm
            // 
            this.AcceptButton = this.buttonLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(193, 269);
            this.Controls.Add(this.textBoxEstateDeveloper);
            this.Controls.Add(this.labelEstateDeveloper);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxIsSuperadmin);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbLogin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.Text = "Login";
            this.Shown += new System.EventHandler(this.LoginForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox tbLogin;
        public System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.CheckBox checkBoxIsSuperadmin;
        private System.Windows.Forms.RadioButton radioButtonMainServer;
        private System.Windows.Forms.RadioButton radioButtonAlternativeServer;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.TextBox textBoxEstateDeveloper;
        private System.Windows.Forms.Label labelEstateDeveloper;
    }
}