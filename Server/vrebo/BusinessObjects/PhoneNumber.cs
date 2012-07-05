using System.Diagnostics;

namespace Vre.Server.BusinessLogic
{
    public class PhoneNumber
    {
        public enum Category
        {
            Home,
            Work,
            Mobile,
            Other
        }

        public Category PhoneCategory { get; private set; }

        public string Number { get; private set; }
        public string Extension { get; private set; }

        public PhoneNumber(Category phoneCategory, string number, string extension)
        {
            string result;

            result = string.Empty;
            foreach (char c in number) if (char.IsDigit(c)) result += c;
            Number = result;

            result = string.Empty;
            foreach (char c in extension) if (char.IsDigit(c)) result += c;
            Extension = result;

            PhoneCategory = phoneCategory;
        }

        public PhoneNumber(string encoded)
        {
            Debug.Assert((encoded != null) && (encoded.Length > 2), "Phone number format is invalid!");

            char type = encoded[0];
            if ('h' == type) PhoneCategory = Category.Home;
            else if ('m' == type) PhoneCategory = Category.Mobile;
            else if ('w' == type) PhoneCategory = Category.Work;
            else PhoneCategory = Category.Other;

            int pos = encoded.IndexOf('#');
            Number = encoded.Substring(2, pos - 2);
            Extension = encoded.Substring(pos + 1);
        }

        public override string ToString()
        {
            char type;
            switch (PhoneCategory)
            {
                case Category.Home: type = 'h'; break;
                case Category.Mobile: type = 'm'; break;
                case Category.Work: type = 'w'; break;
                default: type = 'o'; break;
            }
            return string.Format("{0};{1}#{2}", type, Number, Extension);
        }
    }
}
