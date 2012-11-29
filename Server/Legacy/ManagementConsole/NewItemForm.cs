using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Vre.Server.ManagementConsole
{
    public partial class NewItemForm : Form
    {
        public NewItemForm()
        {
            InitializeComponent();
        }

        public void Position()
        {
            Top = Cursor.Position.Y - Height / 2;
            Left = Cursor.Position.X - Width / 2;
        }
    }
}
