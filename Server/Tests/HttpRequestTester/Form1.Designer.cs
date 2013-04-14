namespace HttpRequestTester
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxTarget = new System.Windows.Forms.ComboBox();
            this.cbxService = new System.Windows.Forms.ComboBox();
            this.cbxRequestType = new System.Windows.Forms.ComboBox();
            this.cbxBodyType = new System.Windows.Forms.ComboBox();
            this.tbBodyText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBodyBrowse = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbRespCookies = new System.Windows.Forms.TextBox();
            this.tbRespProperties = new System.Windows.Forms.TextBox();
            this.tbRespHeaders = new System.Windows.Forms.TextBox();
            this.tbResponseText = new System.Windows.Forms.TextBox();
            this.lblRespCookies = new System.Windows.Forms.Label();
            this.lblProperties = new System.Windows.Forms.Label();
            this.lblHeaders = new System.Windows.Forms.Label();
            this.lblResponseContentType = new System.Windows.Forms.Label();
            this.lblHttpResponseCode = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.tbQuery = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.tbSID = new System.Windows.Forms.TextBox();
            this.btn3DCX = new System.Windows.Forms.Button();
            this.mnu3DCX = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miWebLogin = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.mnu3DCX.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Body type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Target:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Service:";
            // 
            // cbxTarget
            // 
            this.cbxTarget.FormattingEnabled = true;
            this.cbxTarget.Items.AddRange(new object[] {
            "http://localhost:8026/vre",
            "http://ref.3dcondox.com",
            "https://vrt.3dcondox.com/vre",
            "https://vrt.3dcondox.com"});
            this.cbxTarget.Location = new System.Drawing.Point(93, 6);
            this.cbxTarget.Name = "cbxTarget";
            this.cbxTarget.Size = new System.Drawing.Size(151, 21);
            this.cbxTarget.TabIndex = 0;
            this.cbxTarget.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.selectorControl_KeyPress);
            // 
            // cbxService
            // 
            this.cbxService.FormattingEnabled = true;
            this.cbxService.Items.AddRange(new object[] {
            "",
            "program",
            "data/view",
            "data/viewOrder",
            "data/user",
            "data/ft",
            "data/ed",
            "data/site",
            "data/suitetype",
            "data/building",
            "data/suite",
            "gen",
            "go",
            "button",
            "start"});
            this.cbxService.Location = new System.Drawing.Point(93, 33);
            this.cbxService.Name = "cbxService";
            this.cbxService.Size = new System.Drawing.Size(151, 21);
            this.cbxService.TabIndex = 1;
            this.cbxService.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.selectorControl_KeyPress);
            // 
            // cbxRequestType
            // 
            this.cbxRequestType.FormattingEnabled = true;
            this.cbxRequestType.Items.AddRange(new object[] {
            "GET",
            "POST",
            "PUT",
            "DELETE"});
            this.cbxRequestType.Location = new System.Drawing.Point(93, 60);
            this.cbxRequestType.Name = "cbxRequestType";
            this.cbxRequestType.Size = new System.Drawing.Size(151, 21);
            this.cbxRequestType.TabIndex = 2;
            this.cbxRequestType.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.selectorControl_KeyPress);
            // 
            // cbxBodyType
            // 
            this.cbxBodyType.FormattingEnabled = true;
            this.cbxBodyType.Items.AddRange(new object[] {
            "",
            "application/json",
            "text/html",
            "text/plain",
            "text/xml",
            "image/gif",
            "image/jpeg",
            "image/png"});
            this.cbxBodyType.Location = new System.Drawing.Point(93, 87);
            this.cbxBodyType.Name = "cbxBodyType";
            this.cbxBodyType.Size = new System.Drawing.Size(250, 21);
            this.cbxBodyType.TabIndex = 4;
            this.cbxBodyType.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.selectorControl_KeyPress);
            // 
            // tbBodyText
            // 
            this.tbBodyText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBodyText.Location = new System.Drawing.Point(93, 140);
            this.tbBodyText.Multiline = true;
            this.tbBodyText.Name = "tbBodyText";
            this.tbBodyText.Size = new System.Drawing.Size(454, 113);
            this.tbBodyText.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Query:";
            // 
            // btnBodyBrowse
            // 
            this.btnBodyBrowse.Location = new System.Drawing.Point(12, 159);
            this.btnBodyBrowse.Name = "btnBodyBrowse";
            this.btnBodyBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBodyBrowse.TabIndex = 8;
            this.btnBodyBrowse.Text = "Browse";
            this.btnBodyBrowse.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.tbRespCookies);
            this.panel1.Controls.Add(this.tbRespProperties);
            this.panel1.Controls.Add(this.tbRespHeaders);
            this.panel1.Controls.Add(this.tbResponseText);
            this.panel1.Controls.Add(this.lblRespCookies);
            this.panel1.Controls.Add(this.lblProperties);
            this.panel1.Controls.Add(this.lblHeaders);
            this.panel1.Controls.Add(this.lblResponseContentType);
            this.panel1.Controls.Add(this.lblHttpResponseCode);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Location = new System.Drawing.Point(12, 259);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(535, 189);
            this.panel1.TabIndex = 4;
            // 
            // tbRespCookies
            // 
            this.tbRespCookies.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRespCookies.Location = new System.Drawing.Point(79, 49);
            this.tbRespCookies.Multiline = true;
            this.tbRespCookies.Name = "tbRespCookies";
            this.tbRespCookies.ReadOnly = true;
            this.tbRespCookies.Size = new System.Drawing.Size(449, 20);
            this.tbRespCookies.TabIndex = 2;
            this.tbRespCookies.Visible = false;
            this.tbRespCookies.MouseClick += new System.Windows.Forms.MouseEventHandler(this.popupTextBox_MouseClick);
            this.tbRespCookies.Leave += new System.EventHandler(this.popupTextBox_Leave);
            // 
            // tbRespProperties
            // 
            this.tbRespProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRespProperties.Location = new System.Drawing.Point(79, 36);
            this.tbRespProperties.Multiline = true;
            this.tbRespProperties.Name = "tbRespProperties";
            this.tbRespProperties.ReadOnly = true;
            this.tbRespProperties.Size = new System.Drawing.Size(449, 20);
            this.tbRespProperties.TabIndex = 2;
            this.tbRespProperties.Visible = false;
            this.tbRespProperties.MouseClick += new System.Windows.Forms.MouseEventHandler(this.popupTextBox_MouseClick);
            this.tbRespProperties.Leave += new System.EventHandler(this.popupTextBox_Leave);
            // 
            // tbRespHeaders
            // 
            this.tbRespHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRespHeaders.Location = new System.Drawing.Point(79, 23);
            this.tbRespHeaders.Multiline = true;
            this.tbRespHeaders.Name = "tbRespHeaders";
            this.tbRespHeaders.ReadOnly = true;
            this.tbRespHeaders.Size = new System.Drawing.Size(449, 20);
            this.tbRespHeaders.TabIndex = 2;
            this.tbRespHeaders.Visible = false;
            this.tbRespHeaders.MouseClick += new System.Windows.Forms.MouseEventHandler(this.popupTextBox_MouseClick);
            this.tbRespHeaders.Leave += new System.EventHandler(this.popupTextBox_Leave);
            // 
            // tbResponseText
            // 
            this.tbResponseText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResponseText.Location = new System.Drawing.Point(79, 76);
            this.tbResponseText.Multiline = true;
            this.tbResponseText.Name = "tbResponseText";
            this.tbResponseText.ReadOnly = true;
            this.tbResponseText.Size = new System.Drawing.Size(449, 106);
            this.tbResponseText.TabIndex = 1;
            // 
            // lblRespCookies
            // 
            this.lblRespCookies.AutoSize = true;
            this.lblRespCookies.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRespCookies.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRespCookies.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblRespCookies.Location = new System.Drawing.Point(76, 52);
            this.lblRespCookies.Name = "lblRespCookies";
            this.lblRespCookies.Size = new System.Drawing.Size(27, 13);
            this.lblRespCookies.TabIndex = 0;
            this.lblRespCookies.Text = "N/A";
            this.lblRespCookies.Click += new System.EventHandler(this.lblProperties_Click);
            this.lblRespCookies.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblRespCookies_MouseClick);
            // 
            // lblProperties
            // 
            this.lblProperties.AutoSize = true;
            this.lblProperties.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblProperties.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblProperties.Location = new System.Drawing.Point(76, 39);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(27, 13);
            this.lblProperties.TabIndex = 0;
            this.lblProperties.Text = "N/A";
            this.lblProperties.Click += new System.EventHandler(this.lblProperties_Click);
            // 
            // lblHeaders
            // 
            this.lblHeaders.AutoSize = true;
            this.lblHeaders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblHeaders.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblHeaders.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblHeaders.Location = new System.Drawing.Point(76, 26);
            this.lblHeaders.Name = "lblHeaders";
            this.lblHeaders.Size = new System.Drawing.Size(27, 13);
            this.lblHeaders.TabIndex = 0;
            this.lblHeaders.Text = "N/A";
            this.lblHeaders.Click += new System.EventHandler(this.lblHeaders_Click);
            // 
            // lblResponseContentType
            // 
            this.lblResponseContentType.AutoSize = true;
            this.lblResponseContentType.Location = new System.Drawing.Point(76, 13);
            this.lblResponseContentType.Name = "lblResponseContentType";
            this.lblResponseContentType.Size = new System.Drawing.Size(27, 13);
            this.lblResponseContentType.TabIndex = 0;
            this.lblResponseContentType.Text = "N/A";
            // 
            // lblHttpResponseCode
            // 
            this.lblHttpResponseCode.AutoSize = true;
            this.lblHttpResponseCode.Location = new System.Drawing.Point(76, 0);
            this.lblHttpResponseCode.Name = "lblHttpResponseCode";
            this.lblHttpResponseCode.Size = new System.Drawing.Size(27, 13);
            this.lblHttpResponseCode.TabIndex = 0;
            this.lblHttpResponseCode.Text = "N/A";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 52);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Cookies:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 39);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Properties:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(50, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Headers:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Content type:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Code:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Response:";
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.Location = new System.Drawing.Point(472, 112);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 23);
            this.btnQuery.TabIndex = 6;
            this.btnQuery.Text = "Query";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // tbQuery
            // 
            this.tbQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbQuery.Location = new System.Drawing.Point(93, 114);
            this.tbQuery.Name = "tbQuery";
            this.tbQuery.Size = new System.Drawing.Size(373, 20);
            this.tbQuery.TabIndex = 5;
            this.tbQuery.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.selectorControl_KeyPress);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 143);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Body text:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(250, 64);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(28, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "SID:";
            // 
            // tbSID
            // 
            this.tbSID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSID.Location = new System.Drawing.Point(284, 61);
            this.tbSID.Name = "tbSID";
            this.tbSID.Size = new System.Drawing.Size(263, 20);
            this.tbSID.TabIndex = 3;
            this.tbSID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.selectorControl_KeyPress);
            // 
            // btn3DCX
            // 
            this.btn3DCX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn3DCX.Location = new System.Drawing.Point(472, 4);
            this.btn3DCX.Name = "btn3DCX";
            this.btn3DCX.Size = new System.Drawing.Size(75, 23);
            this.btn3DCX.TabIndex = 9;
            this.btn3DCX.Text = "3DCX >>>";
            this.btn3DCX.UseVisualStyleBackColor = true;
            this.btn3DCX.Click += new System.EventHandler(this.btn3DCX_Click);
            // 
            // mnu3DCX
            // 
            this.mnu3DCX.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miWebLogin});
            this.mnu3DCX.Name = "mnu3DCX";
            this.mnu3DCX.Size = new System.Drawing.Size(180, 48);
            // 
            // miWebLogin
            // 
            this.miWebLogin.Name = "miWebLogin";
            this.miWebLogin.Size = new System.Drawing.Size(179, 22);
            this.miWebLogin.Text = "Login as Web Client";
            this.miWebLogin.Click += new System.EventHandler(this.miWebLogin_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 460);
            this.Controls.Add(this.btn3DCX);
            this.Controls.Add(this.tbSID);
            this.Controls.Add(this.tbQuery);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnBodyBrowse);
            this.Controls.Add(this.tbBodyText);
            this.Controls.Add(this.cbxBodyType);
            this.Controls.Add(this.cbxRequestType);
            this.Controls.Add(this.cbxService);
            this.Controls.Add(this.cbxTarget);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "HTTP Query Tester";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.mnu3DCX.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxTarget;
        private System.Windows.Forms.ComboBox cbxService;
        private System.Windows.Forms.ComboBox cbxRequestType;
        private System.Windows.Forms.ComboBox cbxBodyType;
        private System.Windows.Forms.TextBox tbBodyText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBodyBrowse;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbResponseText;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Label lblHttpResponseCode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblResponseContentType;
        private System.Windows.Forms.Label lblProperties;
        private System.Windows.Forms.Label lblHeaders;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbRespProperties;
        private System.Windows.Forms.TextBox tbRespHeaders;
        private System.Windows.Forms.TextBox tbRespCookies;
        private System.Windows.Forms.Label lblRespCookies;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbQuery;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbSID;
        private System.Windows.Forms.Button btn3DCX;
        private System.Windows.Forms.ContextMenuStrip mnu3DCX;
        private System.Windows.Forms.ToolStripMenuItem miWebLogin;
    }
}

