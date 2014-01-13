using System.Collections.Generic;
using System.Text;

namespace Vre
{
    public class CsvUtilities
    {
        private static readonly char[] _csvEscapingChars = new char[] { ',', '\"', '\r', '\n' };
        
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

		public static string ToCsv<T>(IEnumerable<T> elements)
		{
			//double dummy;
			var result = new StringBuilder();
			var separator = false;
			foreach (var e in elements)
			{
				if (separator) result.Append(',');

				var csvAble = e.ToString();

				var quote = csvAble.Contains("\"");
				//var mask = csvAble.StartsWith("=") || double.TryParse(csvAble, out dummy);  // Excel issue
				var multiline = csvAble.Contains("\r\n") || csvAble.Contains("\r") || csvAble.Contains("\n");
				var escape = quote || multiline || csvAble.Contains(",");

				if (multiline) csvAble = csvAble.Replace("\r\n", "\n");
				if (quote) csvAble = csvAble.Replace("\"", "\"\"");

				if (escape) result.Append('\"');
				//if (mask) result.Append('\'');
				result.Append(csvAble);
				if (escape) result.Append('\"');

				separator = true;
			}
			return result.ToString();
		}

        /// <summary>
        /// Returns null in case of incomplete multiline argument.
        /// </summary>
        public static string[] Split(string line)
        {
            int cnt = 1;
            bool escape = false, escaped = false;

            foreach (var c in line)
                if ('\"' == c) escape = !escape;
                else if ((',' == c) && !escape) cnt++;

	        if (escape) return null;  // multiline

            var result = new string[cnt];
			int idx = 0, idx0 = 0;
            cnt = 0; 
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

			if (escaped) result[cnt] = line.Substring(idx0 + 1, idx - idx0 - 2).Replace("\"\"", "\"");
            else result[cnt] = line.Substring(idx0, idx - idx0);

            return result;
        }
    }
}