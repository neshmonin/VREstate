
using System;
using System.Windows.Forms;
namespace Vre.Server.ManagementConsole
{
    internal class PropertyPageHelper
    {
        public static bool NullableStringsAreEqual(string text1, string text2)
        {
            if (string.IsNullOrEmpty(text1))
            {
                if (string.IsNullOrEmpty(text2)) return true;
                else return false;
            }
            return text1.Equals(text2);
        }

        public static bool CompareDTP2NDT(DateTimePicker control, DateTime? value)
        {
            if (control.Checked == value.HasValue)
            {
                if (control.Checked) return control.Value.Equals(value.Value);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void NullableDateTimeToControl(DateTime? value, DateTimePicker control)
        {
            if (value.HasValue)
            {
                control.Value = value.Value;
                control.Checked = true;
            }
            else
            {
                control.Checked = false;
            }
        }

        public static DateTime? ControlToNullableDateTime(DateTimePicker control)
        {
            if (control.Checked) return control.Value;
            else return null;
        }
    }
}