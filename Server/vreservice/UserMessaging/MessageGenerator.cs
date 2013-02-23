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

            string path = ServiceInstances.Configuration.GetValue("MessageTemplateRoot", ".");
            if (!Path.IsPathRooted(path))
                path = Path.Combine(Path.GetDirectoryName(ServiceInstances.Configuration.FilePath), path);

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
            string subject, message;
            GetMessage(referenceUser, template, out subject, out message, parameters);
            if (null == messenger) messenger = ServiceInstances.EmailSender;
            messenger.Send(Sender.ECommerce, recipient, subject, message);
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
            foreach (string file in  // TODO: Localization extension point here: scan for different locales
                Directory.GetFiles(Path.Combine(rootPath, "en"), "*.*", SearchOption.TopDirectoryOnly))
            {
                if (!Path.GetExtension(file).Equals(".txt", StringComparison.InvariantCultureIgnoreCase)) continue;

                string name = Path.GetFileNameWithoutExtension(file);
                if (name.Equals("strings", StringComparison.InvariantCultureIgnoreCase))
                {
                    processStrings(file);
                    continue;
                }

                string value;
                using (StreamReader sr = File.OpenText(file))
                    value = sr.ReadToEnd();

                _messageTemplates.Add(name.ToUpperInvariant(), processTemplate(value));
            }

            //// TODO: Hardcoded!
            //// Non-localized text templates
            //_messageTemplates.Add("TXT_ACCOUNT_CREATED", "Account has been successfully created.");
            //_messageTemplates.Add("TXT_PASSWORD_UPDATED", "Password has been successfully updated.");
            //_messageTemplates.Add("TXT_LOGIN_CHANGED", "Login has been successfully changed.");
            //_messageTemplates.Add("TXT_VIEWORDER_PROLONGED", "Your order has been successfully prolonged.");
            //_messageTemplates.Add("TXT_VIEWORDER_DELETED", "Your order has been successfully cancelled.");
            //_messageTemplates.Add("ENUM_ViewOrderProduct_PrivateListing", "Private Listing");
            //_messageTemplates.Add("ENUM_ViewOrderProduct_PublicListing", "Public Listing");
            //_messageTemplates.Add("ENUM_ViewOrderProduct_Building3DLayout", "3D Layout");
            //_messageTemplates.Add("ENUM_ViewOrderOptions_FloorPlan", "Floor Plan");
            //_messageTemplates.Add("ENUM_ViewOrderOptions_ExternalTour", "External Tour");
            //_messageTemplates.Add("ENUM_ViewOrderOptions_VirtualTour3D", "Virtual 3D Tour");
        }

        private void processStrings(string fileName)
        {
            using (StreamReader sr = File.OpenText(fileName))
            {
                while (!sr.EndOfStream)
                {
                    try
                    {
                        string[] elements = CsvUtilities.Split(sr.ReadLine());
                        if (elements.Length != 2) continue;
                        _messageTemplates.Add(elements[0], elements[1]);
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
            string result;

            // TODO: Extension point here: add to make use of user's locale if relatedUser is non-null
            if (!_messageTemplates.TryGetValue(name, out result))
                throw new ApplicationException("Template name is unknown: " + name);

            return result;
        }
    }
}