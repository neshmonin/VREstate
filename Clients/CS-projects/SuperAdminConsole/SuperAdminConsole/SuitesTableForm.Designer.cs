namespace SuperAdminConsole
{
    partial class SuitesTableForm
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
            this.labelFloorNo = new System.Windows.Forms.Label();
            this.labelUnits = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelFloorNo
            // 
            this.labelFloorNo.AutoSize = true;
            this.labelFloorNo.Location = new System.Drawing.Point(11, 9);
            this.labelFloorNo.Name = "labelFloorNo";
            this.labelFloorNo.Size = new System.Drawing.Size(40, 13);
            this.labelFloorNo.TabIndex = 0;
            this.labelFloorNo.Text = "Floor #";
            // 
            // labelUnits
            // 
            this.labelUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUnits.AutoSize = true;
            this.labelUnits.Location = new System.Drawing.Point(68, 8);
            this.labelUnits.Name = "labelUnits";
            this.labelUnits.Size = new System.Drawing.Size(71, 13);
            this.labelUnits.TabIndex = 1;
            this.labelUnits.Text = "Unit Numbers";
            this.labelUnits.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // SuitesTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(238, 29);
            this.Controls.Add(this.labelUnits);
            this.Controls.Add(this.labelFloorNo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SuitesTableForm";
            this.Text = "Suites";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelFloorNo;
        private System.Windows.Forms.Label labelUnits;

    }
}