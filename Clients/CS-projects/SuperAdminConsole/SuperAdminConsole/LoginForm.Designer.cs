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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.checkBoxIsSuperadmin = new System.Windows.Forms.CheckBox();
            this.radioButtonMainServer = new System.Windows.Forms.RadioButton();
            this.radioButtonAlternativeServer = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Login:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Password:";
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(99, 37);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(256, 20);
            this.tbLogin.TabIndex = 4;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(99, 70);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '#';
            this.tbPassword.Size = new System.Drawing.Size(256, 20);
            this.tbPassword.TabIndex = 5;
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(190, 102);
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
            this.checkBoxIsSuperadmin.Checked = true;
            this.checkBoxIsSuperadmin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIsSuperadmin.Location = new System.Drawing.Point(17, 108);
            this.checkBoxIsSuperadmin.Name = "checkBoxIsSuperadmin";
            this.checkBoxIsSuperadmin.Size = new System.Drawing.Size(135, 17);
            this.checkBoxIsSuperadmin.TabIndex = 7;
            this.checkBoxIsSuperadmin.Text = "Login as a SuperAdmin";
            this.checkBoxIsSuperadmin.UseVisualStyleBackColor = true;
            this.checkBoxIsSuperadmin.CheckedChanged += new System.EventHandler(this.checkBoxIsSuperadmin_CheckedChanged);
            // 
            // radioButtonMainServer
            // 
            this.radioButtonMainServer.AutoSize = true;
            this.radioButtonMainServer.Checked = true;
            this.radioButtonMainServer.Location = new System.Drawing.Point(155, 11);
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
            this.radioButtonAlternativeServer.Location = new System.Drawing.Point(234, 11);
            this.radioButtonAlternativeServer.Name = "radioButtonAlternativeServer";
            this.radioButtonAlternativeServer.Size = new System.Drawing.Size(75, 17);
            this.radioButtonAlternativeServer.TabIndex = 9;
            this.radioButtonAlternativeServer.Text = "Alternative";
            this.radioButtonAlternativeServer.UseVisualStyleBackColor = true;
            // 
            // LoginForm
            // 
            this.AcceptButton = this.buttonLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 140);
            this.Controls.Add(this.radioButtonAlternativeServer);
            this.Controls.Add(this.radioButtonMainServer);
            this.Controls.Add(this.checkBoxIsSuperadmin);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbLogin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.Text = "Login";
            this.Shown += new System.EventHandler(this.LoginForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox tbLogin;
        public System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.CheckBox checkBoxIsSuperadmin;
        private System.Windows.Forms.RadioButton radioButtonMainServer;
        private System.Windows.Forms.RadioButton radioButtonAlternativeServer;
    }
}