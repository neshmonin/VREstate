namespace UniversalIdCodec
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.rbRR = new System.Windows.Forms.RadioButton();
			this.rbVO = new System.Windows.Forms.RadioButton();
			this.rbUnk = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Encoded:";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(84, 12);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(439, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Decoded:";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(84, 38);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(214, 20);
			this.textBox2.TabIndex = 1;
			this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// rbRR
			// 
			this.rbRR.AutoSize = true;
			this.rbRR.Location = new System.Drawing.Point(304, 39);
			this.rbRR.Name = "rbRR";
			this.rbRR.Size = new System.Drawing.Size(41, 17);
			this.rbRR.TabIndex = 2;
			this.rbRR.TabStop = true;
			this.rbRR.Text = "RR";
			this.rbRR.UseVisualStyleBackColor = true;
			// 
			// rbVO
			// 
			this.rbVO.AutoSize = true;
			this.rbVO.Location = new System.Drawing.Point(378, 39);
			this.rbVO.Name = "rbVO";
			this.rbVO.Size = new System.Drawing.Size(40, 17);
			this.rbVO.TabIndex = 2;
			this.rbVO.TabStop = true;
			this.rbVO.Text = "VO";
			this.rbVO.UseVisualStyleBackColor = true;
			// 
			// rbUnk
			// 
			this.rbUnk.AutoSize = true;
			this.rbUnk.Location = new System.Drawing.Point(452, 39);
			this.rbUnk.Name = "rbUnk";
			this.rbUnk.Size = new System.Drawing.Size(71, 17);
			this.rbUnk.TabIndex = 2;
			this.rbUnk.TabStop = true;
			this.rbUnk.Text = "Unknown";
			this.rbUnk.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(543, 75);
			this.Controls.Add(this.rbUnk);
			this.Controls.Add(this.rbVO);
			this.Controls.Add(this.rbRR);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MainForm";
			this.Text = "UniversalIdCodec - 3DCX";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.RadioButton rbRR;
		private System.Windows.Forms.RadioButton rbVO;
		private System.Windows.Forms.RadioButton rbUnk;
	}
}

