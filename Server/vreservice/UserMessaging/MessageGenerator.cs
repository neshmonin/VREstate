using System;
using System.Collections.Generic;
using System.IO;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Messaging
{
    internal class MessageGenerator
    {
        private Dictionary<string, string> _messageTemplates;

        public MessageGenerator()
        {
            _messageTemplates = new Dictionary<string, string>();

			Configuration.OnModified += new EventHandler((o, e) => setup());
			setup();
        }

		private void setup()
		{
			string path = Configuration.Messaging.MessageTemplateRoot.Value;
			if (!Path.IsPathRooted(path))
				path = Path.Combine(Configuration.ConfigurationFilesPath, path);

			scanTemplates(path);
		}

        public void SendMessage(IUserMessaging messenger, User user, string templateName, params object[] parameters)
        {
            sendMessageInt(messenger, user, user.PrimaryEmailAddress, templateName, parameters);
            ServiceInstances.Logger.Info("{0} message sent to {1} ({2})", templateName, user.PrimaryEmailAddress, user.AutoID);
        }

        public void SendMessage(IUserMessaging messenger, User user, string recipient, string templateName, params object[] parameters)
        {
            sendMessageInt(messenger, user, recipient, templateName, parameters);
            ServiceInstances.Logger.Info("{0} message sent to {1} ({2})",
                templateName, recipient, (user != null) ? user.AutoID : -1);
        }

        public void SendMessage(IUserMessaging messenger, string recipient, string templateName, params object[] parameters)
        {
            sendMessageInt(messenger, null, recipient, templateName, parameters);
            ServiceInstances.Logger.Info("{0} message sent to {1}", templateName, recipient);
        }

        private void sendMessageInt(IUserMessaging messenger, User referenceUser,
            string recipient, string template, params object[] parameters)
        {
            var message = GetMessage(referenceUser, template, recipient, parameters);
            if (null == messenger) messenger = ServiceInstances.EmailSender;
			message.ReplyTo = Sender.ECommerce;  // TODO: Hard-coded destination
            messenger.Send(message);
        }

        public Message GetMessage(User referenceUser, string templateName, string recipient,
            params object[] parameters)
        {
			string subject, body;
			GetMessage(referenceUser, templateName, out subject, out body, parameters);

			var result = new Message(recipient, subject, body);

			var template = getTemplate(templateName + "_HTML", referenceUser, false);
			if (template != null)
				result.SetAlternativeHtmlBody(formatMessage(referenceUser, template, parameters));

			return result;
        }

		public void GetMessage(User referenceUser, string templateName,
			out string subject, out string message, params object[] parameters)
		{
			string template = getTemplate(templateName, referenceUser);

			// Process placeholders
			//
			message = formatMessage(referenceUser, template, parameters);

			// Detach subject from body: subject is first line
			//
			int pos1 = message.IndexOf('\r');
			int pos2 = pos1;
			if ((pos1 < 0) || (pos1 >= message.Length)) { pos1 = message.IndexOf('\n'); pos2 = pos1; }
			else if ((pos2 < (message.Length - 1)) && (message[pos2 + 1] == '\n')) { pos2++; }

			subject = VersionGen.ProductName;  // default, should never appear though! :)

			if ((pos1 >= 0) && (pos1 < message.Length))
			{
				subject = message.Substring(0, pos1);
				message = message.Substring(pos2 + 1);
			}
		}
		
		public string GetMessage(User referenceUser, string templateName,
            params object[] parameters)
        {
            string template = getTemplate(templateName, referenceUser);

            // Process placeholders
            //
            return formatMessage(referenceUser, template, parameters);
        }

        public string GetMessage(User referenceUser, string templateName)
        {
            return getTemplate(templateName, referenceUser);
        }

        private string formatMessage(User referenceUser, string template, params object[] parameters)
        {
            int idx = parameters.Length;
            object[] procd = new object[idx];

            for (idx--; idx >= 0; idx--)
            {
                object p = parameters[idx];

                Type pt = p.GetType();
                if (pt.IsSubclassOf(typeof(Enum)))
                {
                    string key = "ENUM_" + pt.Name + "_" + Enum.GetName(pt, p);
                    p = getTemplate(key, referenceUser);
                }
                else if (pt.Equals(typeof(string)))
                {
                    string text = p as string;
                    if (text.StartsWith("@"))
                        p = getTemplate(text.Substring(1), referenceUser);
                }
                else if (pt.Equals(typeof(DateTime)))
                {
                    DateTime dt = (DateTime)p;
                    if (0 == dt.TimeOfDay.Ticks)
                        p = dt.ToString(getTemplate("DATE", referenceUser));
                    else
                        p = dt.ToString(getTemplate("DATETIME", referenceUser));
                }
                else if (pt.Equals(typeof(TimeSpan)))
                {
                    TimeSpan ts = (TimeSpan)p;
                    p = ts.ToString(getTemplate("TIME", referenceUser));
                }

                procd[idx] = p;
            }

            return string.Format(template, procd);
        }

        private void scanTemplates(string rootPath)
        {
			var templates = new Dictionary<string, string>();

            foreach (string file in  // TODO: Localization extension point here: scan for different locales
                Directory.GetFiles(Path.Combine(rootPath, "en"), "*.*", SearchOption.TopDirectoryOnly))
            {
                if (!Path.GetExtension(file).Equals(".txt", StringComparison.InvariantCultureIgnoreCase)) continue;

                string name = Path.GetFileNameWithoutExtension(file);
                if (name.Equals("strings", StringComparison.InvariantCultureIgnoreCase))
                {
                    processStrings(file, ref templates);
                    continue;
                }

                string value;
                using (StreamReader sr = File.OpenText(file))
                    value = sr.ReadToEnd();

				templates.Add(name.ToUpperInvariant(), processTemplate(value));
            }

			_messageTemplates = templates;
        }

        private static void processStrings(string fileName, ref Dictionary<string, string> templates)
        {
            using (StreamReader sr = File.OpenText(fileName))
            {
                while (!sr.EndOfStream)
                {
                    try
                    {
                        string[] elements = CsvUtilities.Split(sr.ReadLine());
                        if (elements.Length != 2) continue;
                        templates.Add(elements[0], elements[1]);
                    }
                    catch (ArgumentException) { }
                }
            }
        }

        private static readonly char[] _formatEscape = new char[] { '{', '}' };
        private static string processTemplate(string template)
        {
            Stack<bool> decisions = new Stack<bool>();
            int pos = 0;
            do
            {
                pos = template.IndexOfAny(_formatEscape, pos);
                if (pos < 0) break;
                bool escape;
                if (template[pos] == '{')
                {
                    if (pos < (template.Length - 1)) escape = !char.IsDigit(template, pos + 1);// char.IsWhiteSpace(template, pos + 1);
                    else escape = true;

                    decisions.Push(escape);
                    if (escape) template = template.Insert(pos++, "{");
                }
                else
                {
                    if (decisions.Pop()) template = template.Insert(pos++, "}");
                }
                pos++;
            }
            while (true);
            return template;
        }

		private string getTemplate(string name, User relatedUser)
		{
			return getTemplate(name, relatedUser, true);
		}

        private string getTemplate(string name, User relatedUser, bool failIfNotExist)
        {
            string result;

            // TODO: Extension point here: add to make use of user's locale if relatedUser is non-null
			if (!_messageTemplates.TryGetValue(name, out result))
			{
				if (failIfNotExist)
					throw new ApplicationException("Template name is unknown: " + name);
				else
					result = null;
			}

            return result;
        }
    }
}