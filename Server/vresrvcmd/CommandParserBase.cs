using System;
using System.Collections.Generic;
using System.Text;

namespace Vre.Client.CommandLine
{
    internal abstract class CommandParserBase
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public abstract void Parse(ServerProxy proxy, List<string> elements);

        public abstract void ShowHelp();

        protected static string readPassword(string prompt)
        {
            StringBuilder result = new StringBuilder();

            Console.Write(prompt);

            do
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter) break;
                result.Append(cki.KeyChar);
            } while (true);

            Console.WriteLine(string.Empty);  // newline

            return result.ToString();
        }

        protected static Dictionary<string, string> parseOptions(List<string> elements)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (string e in elements)
            {
                string[] parts = e.Split('=');
                if (parts.Length > 2) throw new ArgumentException("Invalid parameter");
                string key = parts[0];
                string val = (parts.Length == 2) ? parts[1] : null;
                if (result.ContainsKey(key)) throw new ArgumentException("Duplicate parameter");
                result.Add(key, val);
            }

            return result;
        }
    }
}