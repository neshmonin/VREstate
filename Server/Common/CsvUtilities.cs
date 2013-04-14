using System;
using System.Collections.Generic;
using System.Text;

namespace Vre
{
    public class CsvUtilities
    {
        private static readonly char[] _csvEscapingChars = new char[] { ',', '\"' };
        
        public static string ToString<T>(IEnumerable<T> items)
        {
            StringBuilder result = new StringBuilder();
            bool comma = false;
            foreach (T item in items)
            {
                if (comma) result.Append(',');
                else comma = true;
                
                string value = item.ToString();

                if (value.IndexOfAny(_csvEscapingChars) >= 0)
                {
                    result.Append('\"');
                    result.Append(value.Replace("\"", "\"\""));
                    result.Append('\"');
                }
                else result.Append(value);
            }
            return result.ToString();
        }

        /// <summary>
        /// MULTILINE ELEMENTS ARE NOT SUPPORTED
        /// </summary>
        public static string[] Split(string line)
        {
            int cnt = 1, idx, idx0;
            bool escape = false, escaped = false;

            for (idx = 0; idx < line.Length; idx++)
                if ('\"' == line[idx]) escape = !escape;
                else if ((',' == line[idx]) && !escape) cnt++;
            if (escape) throw new ArgumentException("The text is not a valid CSV");

            string[] result = new string[cnt];
            cnt = 0; idx = 0; idx0 = 0;
            while (idx < line.Length)
            {
                if ('\"' == line[idx])
                {
                    escape = !escape;
                    escaped = true;
                }
                else if ((',' == line[idx]) && !escape)
                {
                    if (escaped) result[cnt] = line.Substring(idx0 + 1, idx - idx0 - 2).Replace("\"\"", "\"");
                    else result[cnt] = line.Substring(idx0, idx - idx0);
                    idx0 = idx + 1;
                    escaped = false;
                    cnt++;
                }

                idx++;
            }

            if (escaped) result[cnt] = line.Substring(idx0 + 1, idx - idx0 - 2);
            else result[cnt] = line.Substring(idx0, idx - idx0);

            return result;
        }
    }
}