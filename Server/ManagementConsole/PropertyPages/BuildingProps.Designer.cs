namespace Vre.Server.ManagementConsole
{
    partial class BuildingProps
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.cbxStatus = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpOpeningDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbAddressLine1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbAddressLine2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbCity = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbStateProvince = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbPostalCode = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbCountry = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Status:";
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.Location = new System.Drawing.Point(83, 39);
            this.tbName.MaxLength = 256;
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(259, 20);
            this.tbName.TabIndex = 1;
            // 
            // cbxStatus
            // 
            this.cbxStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxStatus.FormattingEnabled = true;
            this.cbxStatus.Location = new System.Drawing.Point(83, 67);
            this.cbxStatus.Name = "cbxStatus";
            this.cbxStatus.Size = new System.Drawing.Size(200, 21);
            this.cbxStatus.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(6, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Building properties";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Opening date:";
            // 
            // dtpOpeningDate
            // 
            this.dtpOpeningDate.Checked = false;
            this.dtpOpeningDate.Location = new System.Drawing.Point(83, 94);
            this.dtpOpeningDate.Name = "dtpOpeningDate";
            this.dtpOpeningDate.ShowCheckBox = true;
            this.dtpOpeningDate.Size = new System.Drawing.Size(200, 20);
            this.dtpOpeningDate.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tbCountry);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.tbPostalCode);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tbStateProvince);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tbCity);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.tbAddressLine2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbAddressLine1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(9, 120);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(333, 187);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Address";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Address line 1:";
            // 
            // tbAddressLine1
            // 
            this.tbAddressLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAddressLine1.Location = new System.Drawing.Point(129, 25);
            this.tbAddressLine1.MaxLength = 128;
            this.tbAddressLine1.Name = "tbAddressLine1";
            this.tbAddressLine1.Size = new System.Drawing.Size(194, 20);
            this.tbAddressLine1.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Address line 2:";
            // 
            // tbAddressLine2
            // 
            this.tbAddressLine2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAddressLine2.Location = new System.Drawing.Point(129, 51);
            this.tbAddressLine2.MaxLength = 128;
            this.tbAddressLine2.Name = "tbAddressLine2";
            this.tbAddressLine2.Size = new System.Drawing.Size(194, 20);
            this.tbAddressLine2.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Town/city:";
            // 
            // tbCity
            // 
            this.tbCity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCity.Location = new System.Drawing.Point(129, 77);
            this.tbCity.MaxLength = 64;
            this.tbCity.Name = "tbCity";
            this.tbCity.Size = new System.Drawing.Size(194, 20);
            this.tbCity.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 106);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "State/province:";
            // 
            // tbStateProvince
            // 
            this.tbStateProvince.Location = new System.Drawing.Point(129, 103);
            this.tbStateProvince.MaxLength = 8;
            this.tbStateProvince.Name = "tbStateProvince";
            this.tbStateProvince.Size = new System.Drawing.Size(69, 20);
            this.tbStateProvince.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 132);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Postal/ZIP code:";
            // 
            // tbPostalCode
            // 
            this.tbPostalCode.Location = new System.Drawing.Point(129, 129);
            this.tbPostalCode.MaxLength = 10;
            this.tbPostalCode.Name = "tbPostalCode";
            this.tbPostalCode.Size = new System.Drawing.Size(69, 20);
            this.tbPostalCode.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 158);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Country:";
            // 
            // tbCountry
            // 
            this.tbCountry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCountry.Location = new System.Drawing.Point(129, 155);
            this.tbCountry.MaxLength = 128;
            this.tbCountry.Name = "tbCountry";
            this.tbCountry.Size = new System.Drawing.Size(194, 20);
            this.tbCountry.TabIndex = 2;
            // 
            // BuildingProps
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dtpOpeningDate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbxStatus);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(350, 316);
            this.Name = "BuildingProps";
            this.Size = new System.Drawing.Size(350, 316);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.ComboBox cbxStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpOpeningDate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbAddressLine2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbAddressLine1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbCountry;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbPostalCode;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbStateProvince;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbCity;
        private System.Windows.Forms.Label label7;
    }
}
