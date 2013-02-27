using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Vre.Server.Messaging
{
    internal class EmailUserMessaging : IUserMessaging
    {
        private string _smtpServerHost;
        private int _smtpServerPort;
        private bool _smtpUseSsl;
        private Dictionary<Sender, NetworkCredential> _credentials;
        
        public EmailUserMessaging()
        {
            _smtpServerHost = ServiceInstances.Configuration.GetValue("SmtpServerHost", string.Empty);
            _smtpServerPort = ServiceInstances.Configuration.GetValue("SmtpServerPort", 25);
            _smtpUseSsl = ServiceInstances.Configuration.GetValue("SmtpServerUseSsl", false);

            System.Security.Cryptography.RSACryptoServiceProvider.UseMachineKeyStore = true;
            System.Security.Cryptography.DSACryptoServiceProvider.UseMachineKeyStore = true;

            _credentials = new Dictionary<Sender, NetworkCredential>();
            _credentials.Add(Sender.Server, new NetworkCredential(
                ServiceInstances.Configuration.GetValue("SmtpServerLogin", string.Empty),
                ServiceInstances.Configuration.GetValue("SmtpServerPassword", string.Empty)));
            _credentials.Add(Sender.ECommerce, new NetworkCredential(
                ServiceInstances.Configuration.GetValue("SmtpECommerceLogin", string.Empty),
                ServiceInstances.Configuration.GetValue("SmtpECommercePassword", string.Empty)));
        }

        public void Dispose() { }

        public void Send(IEnumerable<string> recipients, string subject, string message)
        {
            Send(Sender.Server, recipients, subject, message);
        }

        public void Send(Sender sender, System.Collections.Generic.IEnumerable<string> recipients, string subject, string message)
        {
            StringBuilder recipient = new StringBuilder();
            bool delimiter = false;
            foreach (string r in recipients)
            {
                if (delimiter) recipient.Append(',');
                recipient.Append(r);
                delimiter = true;
            }
            Send(sender, recipient.ToString(), subject, message);
        }

        public void Send(string recipient, string subject, string message)
        {
            Send(Sender.Server, recipient, subject, message);
        }

        public void Send(Sender sender, string recipient, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(_smtpServerHost))
                throw new ApplicationException("SMTP service parameters are not defined in configuration; cannot send emails.");

            NetworkCredential nc = _credentials[sender];
            if (string.IsNullOrWhiteSpace(nc.UserName) || string.IsNullOrWhiteSpace(nc.Password))
                throw new ApplicationException("Sender credentials for " + sender.ToString() + " are not defined in configuration; cannot send emails.");

            try
            {
                using (SmtpClient client = new SmtpClient(_smtpServerHost, _smtpServerPort))
                {
                    client.Credentials = nc;
                    client.EnableSsl = _smtpUseSsl;
                    client.Send(nc.UserName, recipient, subject, message);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Cannot send email.", ex);
            }
        }
    }
}