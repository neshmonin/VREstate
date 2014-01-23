using System;
using System.Windows.Forms;
using Vre.Server;

namespace UniversalIdCodec
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			try
			{
				switch (UniversalId.TypeInUrlId(textBox1.Text))
				{
					case UniversalId.IdType.ReverseRequest:
						textBox2.Text = UniversalId.ExtractAsGuid(textBox1.Text).ToString();
						rbRR.Checked = true;
						break;

					case UniversalId.IdType.ViewOrder:
						textBox2.Text = UniversalId.ExtractAsGuid(textBox1.Text).ToString();
						rbVO.Checked = true;
						break;

					default:
						textBox2.Text = UniversalId.ExtractAsString(textBox1.Text);
						rbUnk.Checked = true;
						break;
				}
			}
			catch
			{
				textBox2.Text = "?";
				rbUnk.Checked = true;
			}
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
