using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Vre.Server.Messaging
{
    internal class EmailUserMessaging : IUserMessaging
    {
        private Dictionary<Sender, NetworkCredential> _credentials;
        
        public EmailUserMessaging()
        {
            System.Security.Cryptography.RSACryptoServiceProvider.UseMachineKeyStore = true;
            System.Security.Cryptography.DSACryptoServiceProvider.UseMachineKeyStore = true;

			// TODO: Static configuration
            _credentials = new Dictionary<Sender, NetworkCredential>();
            _credentials.Add(Sender.Server, new NetworkCredential(
                Configuration.Messaging.Email.Credentials.Server.Login.Value,
                Configuration.Messaging.Email.Credentials.Server.Password.Value));
            _credentials.Add(Sender.ECommerce, new NetworkCredential(
				Configuration.Messaging.Email.Credentials.ECommerce.Login.Value,
				Configuration.Messaging.Email.Credentials.ECommerce.Password.Value));
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
            if (string.IsNullOrWhiteSpace(Configuration.Messaging.Email.SmtpServerHost.Value))
                throw new ApplicationException("SMTP service parameters are not defined in configuration; cannot send emails.");

            NetworkCredential nc = _credentials[sender];
            if (string.IsNullOrWhiteSpace(nc.UserName) || string.IsNullOrWhiteSpace(nc.Password))
                throw new ApplicationException("Sender credentials for " + sender.ToString() + " are not defined in configuration; cannot send emails.");

            try
            {
				using (SmtpClient client = new SmtpClient(
					Configuration.Messaging.Email.SmtpServerHost.Value,
					Configuration.Messaging.Email.SmtpServerPort.Value))
                {
                    client.Credentials = nc;
					client.EnableSsl = Configuration.Messaging.Email.SmtpServerUseSsl.Value;
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