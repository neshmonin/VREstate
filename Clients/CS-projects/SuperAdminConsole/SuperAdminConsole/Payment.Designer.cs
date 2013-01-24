namespace SuperAdminConsole
{
    partial class Payment
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
            this.tabControlCards = new System.Windows.Forms.TabControl();
            this.tabCreditCards = new System.Windows.Forms.TabPage();
            this.groupBoxBillingAddress = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBoxCountry = new System.Windows.Forms.ComboBox();
            this.textBoxStreet2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxPostal = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxProvince = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxCity = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxStreet = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.groupBoxCardInfo = new System.Windows.Forms.GroupBox();
            this.textBoxLastName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.textBoxFirstName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownYY = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMM = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxCvv2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCardNo = new System.Windows.Forms.TextBox();
            this.radioButtonMasterCard = new System.Windows.Forms.RadioButton();
            this.radioButtonVISA = new System.Windows.Forms.RadioButton();
            this.buttonPayNow = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxPoNum = new System.Windows.Forms.TextBox();
            this.textBoxInvNum = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxAmount = new System.Windows.Forms.TextBox();
            this.comboBoxCurrency = new System.Windows.Forms.ComboBox();
            this.tabControlCards.SuspendLayout();
            this.tabCreditCards.SuspendLayout();
            this.groupBoxBillingAddress.SuspendLayout();
            this.groupBoxCardInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMM)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlCards
            // 
            this.tabControlCards.Controls.Add(this.tabCreditCards);
            this.tabControlCards.Location = new System.Drawing.Point(2, 38);
            this.tabControlCards.Name = "tabControlCards";
            this.tabControlCards.SelectedIndex = 0;
            this.tabControlCards.Size = new System.Drawing.Size(415, 320);
            this.tabControlCards.TabIndex = 2;
            // 
            // tabCreditCards
            // 
            this.tabCreditCards.BackColor = System.Drawing.SystemColors.Control;
            this.tabCreditCards.Controls.Add(this.groupBoxBillingAddress);
            this.tabCreditCards.Controls.Add(this.groupBoxCardInfo);
            this.tabCreditCards.Controls.Add(this.radioButtonMasterCard);
            this.tabCreditCards.Controls.Add(this.radioButtonVISA);
            this.tabCreditCards.Location = new System.Drawing.Point(4, 22);
            this.tabCreditCards.Name = "tabCreditCards";
            this.tabCreditCards.Padding = new System.Windows.Forms.Padding(3);
            this.tabCreditCards.Size = new System.Drawing.Size(407, 294);
            this.tabCreditCards.TabIndex = 0;
            this.tabCreditCards.Text = "Credit Cards";
            // 
            // groupBoxBillingAddress
            // 
            this.groupBoxBillingAddress.Controls.Add(this.label16);
            this.groupBoxBillingAddress.Controls.Add(this.comboBoxCountry);
            this.groupBoxBillingAddress.Controls.Add(this.textBoxStreet2);
            this.groupBoxBillingAddress.Controls.Add(this.label7);
            this.groupBoxBillingAddress.Controls.Add(this.label12);
            this.groupBoxBillingAddress.Controls.Add(this.textBoxPostal);
            this.groupBoxBillingAddress.Controls.Add(this.label11);
            this.groupBoxBillingAddress.Controls.Add(this.textBoxProvince);
            this.groupBoxBillingAddress.Controls.Add(this.label10);
            this.groupBoxBillingAddress.Controls.Add(this.textBoxCity);
            this.groupBoxBillingAddress.Controls.Add(this.label9);
            this.groupBoxBillingAddress.Controls.Add(this.textBoxStreet);
            this.groupBoxBillingAddress.Controls.Add(this.label8);
            this.groupBoxBillingAddress.Controls.Add(this.textBoxName);
            this.groupBoxBillingAddress.Location = new System.Drawing.Point(34, 133);
            this.groupBoxBillingAddress.Name = "groupBoxBillingAddress";
            this.groupBoxBillingAddress.Size = new System.Drawing.Size(338, 150);
            this.groupBoxBillingAddress.TabIndex = 32;
            this.groupBoxBillingAddress.TabStop = false;
            this.groupBoxBillingAddress.Text = "Billing Address";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(19, 19);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(54, 13);
            this.label16.TabIndex = 27;
            this.label16.Text = "Full Name";
            // 
            // comboBoxCountry
            // 
            this.comboBoxCountry.FormattingEnabled = true;
            this.comboBoxCountry.Items.AddRange(new object[] {
            "CA",
            "US"});
            this.comboBoxCountry.Location = new System.Drawing.Point(236, 118);
            this.comboBoxCountry.Name = "comboBoxCountry";
            this.comboBoxCountry.Size = new System.Drawing.Size(80, 21);
            this.comboBoxCountry.TabIndex = 26;
            // 
            // textBoxStreet2
            // 
            this.textBoxStreet2.Location = new System.Drawing.Point(83, 68);
            this.textBoxStreet2.Name = "textBoxStreet2";
            this.textBoxStreet2.Size = new System.Drawing.Size(233, 20);
            this.textBoxStreet2.TabIndex = 25;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 71);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "Street 2";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(184, 122);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(43, 13);
            this.label12.TabIndex = 22;
            this.label12.Text = "Country";
            // 
            // textBoxPostal
            // 
            this.textBoxPostal.Location = new System.Drawing.Point(83, 119);
            this.textBoxPostal.Name = "textBoxPostal";
            this.textBoxPostal.Size = new System.Drawing.Size(60, 20);
            this.textBoxPostal.TabIndex = 21;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 122);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(36, 13);
            this.label11.TabIndex = 20;
            this.label11.Text = "Postal";
            // 
            // textBoxProvince
            // 
            this.textBoxProvince.Location = new System.Drawing.Point(287, 93);
            this.textBoxProvince.Name = "textBoxProvince";
            this.textBoxProvince.Size = new System.Drawing.Size(29, 20);
            this.textBoxProvince.TabIndex = 19;
            this.textBoxProvince.Text = "ON";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(235, 96);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "Province";
            // 
            // textBoxCity
            // 
            this.textBoxCity.Location = new System.Drawing.Point(83, 93);
            this.textBoxCity.Name = "textBoxCity";
            this.textBoxCity.Size = new System.Drawing.Size(135, 20);
            this.textBoxCity.TabIndex = 17;
            this.textBoxCity.Text = "Toronto";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 96);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(24, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "City";
            // 
            // textBoxStreet
            // 
            this.textBoxStreet.Location = new System.Drawing.Point(83, 42);
            this.textBoxStreet.Name = "textBoxStreet";
            this.textBoxStreet.Size = new System.Drawing.Size(233, 20);
            this.textBoxStreet.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "No, Street";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(83, 16);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(233, 20);
            this.textBoxName.TabIndex = 11;
            // 
            // groupBoxCardInfo
            // 
            this.groupBoxCardInfo.Controls.Add(this.textBoxLastName);
            this.groupBoxCardInfo.Controls.Add(this.label17);
            this.groupBoxCardInfo.Controls.Add(this.textBoxFirstName);
            this.groupBoxCardInfo.Controls.Add(this.label6);
            this.groupBoxCardInfo.Controls.Add(this.numericUpDownYY);
            this.groupBoxCardInfo.Controls.Add(this.numericUpDownMM);
            this.groupBoxCardInfo.Controls.Add(this.label5);
            this.groupBoxCardInfo.Controls.Add(this.label4);
            this.groupBoxCardInfo.Controls.Add(this.label3);
            this.groupBoxCardInfo.Controls.Add(this.textBoxCvv2);
            this.groupBoxCardInfo.Controls.Add(this.label2);
            this.groupBoxCardInfo.Controls.Add(this.textBoxCardNo);
            this.groupBoxCardInfo.Location = new System.Drawing.Point(20, 25);
            this.groupBoxCardInfo.Name = "groupBoxCardInfo";
            this.groupBoxCardInfo.Size = new System.Drawing.Size(362, 102);
            this.groupBoxCardInfo.TabIndex = 31;
            this.groupBoxCardInfo.TabStop = false;
            // 
            // textBoxLastName
            // 
            this.textBoxLastName.Location = new System.Drawing.Point(187, 71);
            this.textBoxLastName.Name = "textBoxLastName";
            this.textBoxLastName.Size = new System.Drawing.Size(165, 20);
            this.textBoxLastName.TabIndex = 30;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(187, 55);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(58, 13);
            this.label17.TabIndex = 29;
            this.label17.Text = "Last Name";
            // 
            // textBoxFirstName
            // 
            this.textBoxFirstName.Location = new System.Drawing.Point(11, 71);
            this.textBoxFirstName.Name = "textBoxFirstName";
            this.textBoxFirstName.Size = new System.Drawing.Size(165, 20);
            this.textBoxFirstName.TabIndex = 28;
            this.textBoxFirstName.Tag = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "First Name";
            // 
            // numericUpDownYY
            // 
            this.numericUpDownYY.Location = new System.Drawing.Point(306, 29);
            this.numericUpDownYY.Maximum = new decimal(new int[] {
            2022,
            0,
            0,
            0});
            this.numericUpDownYY.Minimum = new decimal(new int[] {
            2012,
            0,
            0,
            0});
            this.numericUpDownYY.Name = "numericUpDownYY";
            this.numericUpDownYY.Size = new System.Drawing.Size(46, 20);
            this.numericUpDownYY.TabIndex = 9;
            this.numericUpDownYY.Value = new decimal(new int[] {
            2017,
            0,
            0,
            0});
            // 
            // numericUpDownMM
            // 
            this.numericUpDownMM.Location = new System.Drawing.Point(264, 30);
            this.numericUpDownMM.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDownMM.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMM.Name = "numericUpDownMM";
            this.numericUpDownMM.Size = new System.Drawing.Size(36, 20);
            this.numericUpDownMM.TabIndex = 8;
            this.numericUpDownMM.Value = new decimal(new int[] {
            11,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(304, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "YYYY";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(263, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "MM";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(147, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Cvv2";
            // 
            // textBoxCvv2
            // 
            this.textBoxCvv2.Location = new System.Drawing.Point(147, 30);
            this.textBoxCvv2.Name = "textBoxCvv2";
            this.textBoxCvv2.Size = new System.Drawing.Size(32, 20);
            this.textBoxCvv2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Card Number";
            // 
            // textBoxCardNo
            // 
            this.textBoxCardNo.Location = new System.Drawing.Point(11, 30);
            this.textBoxCardNo.Name = "textBoxCardNo";
            this.textBoxCardNo.Size = new System.Drawing.Size(126, 20);
            this.textBoxCardNo.TabIndex = 2;
            this.textBoxCardNo.Tag = "";
            // 
            // radioButtonMasterCard
            // 
            this.radioButtonMasterCard.AutoSize = true;
            this.radioButtonMasterCard.Location = new System.Drawing.Point(195, 9);
            this.radioButtonMasterCard.Name = "radioButtonMasterCard";
            this.radioButtonMasterCard.Size = new System.Drawing.Size(79, 17);
            this.radioButtonMasterCard.TabIndex = 1;
            this.radioButtonMasterCard.TabStop = true;
            this.radioButtonMasterCard.Text = "MasterCard";
            this.radioButtonMasterCard.UseVisualStyleBackColor = true;
            this.radioButtonMasterCard.CheckedChanged += new System.EventHandler(this.radioButtonMasterCard_CheckedChanged);
            // 
            // radioButtonVISA
            // 
            this.radioButtonVISA.AutoSize = true;
            this.radioButtonVISA.Location = new System.Drawing.Point(123, 9);
            this.radioButtonVISA.Name = "radioButtonVISA";
            this.radioButtonVISA.Size = new System.Drawing.Size(49, 17);
            this.radioButtonVISA.TabIndex = 0;
            this.radioButtonVISA.TabStop = true;
            this.radioButtonVISA.Text = "VISA";
            this.radioButtonVISA.UseVisualStyleBackColor = true;
            this.radioButtonVISA.CheckedChanged += new System.EventHandler(this.radioButtonVISA_CheckedChanged);
            // 
            // buttonPayNow
            // 
            this.buttonPayNow.Location = new System.Drawing.Point(18, 369);
            this.buttonPayNow.Name = "buttonPayNow";
            this.buttonPayNow.Size = new System.Drawing.Size(145, 23);
            this.buttonPayNow.TabIndex = 3;
            this.buttonPayNow.Text = "Pay Now";
            this.buttonPayNow.UseVisualStyleBackColor = true;
            this.buttonPayNow.Click += new System.EventHandler(this.buttonPayNow_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 15);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(43, 13);
            this.label13.TabIndex = 4;
            this.label13.Text = "Order #";
            // 
            // textBoxPoNum
            // 
            this.textBoxPoNum.Location = new System.Drawing.Point(69, 12);
            this.textBoxPoNum.Name = "textBoxPoNum";
            this.textBoxPoNum.ReadOnly = true;
            this.textBoxPoNum.Size = new System.Drawing.Size(180, 20);
            this.textBoxPoNum.TabIndex = 5;
            // 
            // textBoxInvNum
            // 
            this.textBoxInvNum.Location = new System.Drawing.Point(318, 12);
            this.textBoxInvNum.Name = "textBoxInvNum";
            this.textBoxInvNum.ReadOnly = true;
            this.textBoxInvNum.Size = new System.Drawing.Size(85, 20);
            this.textBoxInvNum.TabIndex = 7;
            this.textBoxInvNum.Text = "L000001";
            this.textBoxInvNum.Visible = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(262, 17);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 13);
            this.label14.TabIndex = 6;
            this.label14.Text = "Invoice #";
            this.label14.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(223, 374);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(43, 13);
            this.label15.TabIndex = 24;
            this.label15.Text = "Amount";
            // 
            // textBoxAmount
            // 
            this.textBoxAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBoxAmount.Location = new System.Drawing.Point(271, 371);
            this.textBoxAmount.Name = "textBoxAmount";
            this.textBoxAmount.Size = new System.Drawing.Size(68, 20);
            this.textBoxAmount.TabIndex = 25;
            this.textBoxAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // comboBoxCurrency
            // 
            this.comboBoxCurrency.FormattingEnabled = true;
            this.comboBoxCurrency.Items.AddRange(new object[] {
            "CAD",
            "USD"});
            this.comboBoxCurrency.Location = new System.Drawing.Point(345, 371);
            this.comboBoxCurrency.Name = "comboBoxCurrency";
            this.comboBoxCurrency.Size = new System.Drawing.Size(58, 21);
            this.comboBoxCurrency.TabIndex = 26;
            // 
            // Payment
            // 
            this.AcceptButton = this.buttonPayNow;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 401);
            this.Controls.Add(this.comboBoxCurrency);
            this.Controls.Add(this.textBoxAmount);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.textBoxInvNum);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBoxPoNum);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.buttonPayNow);
            this.Controls.Add(this.tabControlCards);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Payment";
            this.Text = "Payment";
            this.tabControlCards.ResumeLayout(false);
            this.tabCreditCards.ResumeLayout(false);
            this.tabCreditCards.PerformLayout();
            this.groupBoxBillingAddress.ResumeLayout(false);
            this.groupBoxBillingAddress.PerformLayout();
            this.groupBoxCardInfo.ResumeLayout(false);
            this.groupBoxCardInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMM)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlCards;
        private System.Windows.Forms.TabPage tabCreditCards;
        private System.Windows.Forms.NumericUpDown numericUpDownYY;
        private System.Windows.Forms.NumericUpDown numericUpDownMM;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxCvv2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCardNo;
        private System.Windows.Forms.RadioButton radioButtonMasterCard;
        private System.Windows.Forms.RadioButton radioButtonVISA;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxPostal;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxProvince;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxCity;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxStreet;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonPayNow;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxPoNum;
        private System.Windows.Forms.TextBox textBoxInvNum;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxAmount;
        private System.Windows.Forms.TextBox textBoxStreet2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxCurrency;
        private System.Windows.Forms.ComboBox comboBoxCountry;
        private System.Windows.Forms.GroupBox groupBoxBillingAddress;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBoxCardInfo;
        private System.Windows.Forms.TextBox textBoxLastName;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBoxFirstName;
    }
}