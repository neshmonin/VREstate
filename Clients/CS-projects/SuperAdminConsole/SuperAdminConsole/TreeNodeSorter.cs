using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperAdminConsole
{
    public class TreeNodeSorter : IComparer
    {
        // Compare the length of the strings, or the strings 
        // themselves, if they are the same length. 
        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;

            string stringX;
            string stringY;

            if (tx.Parent == null && ty.Parent == null)
            {
                if (tx.Text == "SELF (superAdmin)")
                    return -1;
                else if (ty.Text == "SELF (superAdmin)")
                    return 1;

                stringX = tx.Text.Substring(tx.Text.IndexOf(' '));
                stringY = ty.Text.Substring(ty.Text.IndexOf(' '));
            }
            else
            {
                stringX = ty.Text;
                stringY = tx.Text;
            }

            // If they are the same length, call Compare. 
            return string.Compare(stringX, stringY);
        }
    }
}
