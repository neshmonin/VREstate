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

		public void Send(Message message)
		{
			if (string.IsNullOrWhiteSpace(Configuration.Messaging.Email.SmtpServerHost.Value))
				throw new ApplicationException("SMTP service parameters are not defined in configuration; cannot send emails.");

			MailMessage msg = new MailMessage();

			msg.SubjectEncoding = Encoding.UTF8;
			msg.Subject = message.Subject;

			foreach (var rcp in message.Recipients) msg.To.Add(rcp);
			if (0 == msg.To.Count)
				throw new ArgumentException("A message with subject '{0}' was attempted to be sent with no recipients provided.", msg.Subject);

			Sender sender = message.From;
			if (sender == Sender.None)
			{
				sender = Sender.Server;
				ServiceInstances.Logger.Error("A message to {0} (subject = '{1}') was sent with no sender provided; server assumed.",
					msg.To, msg.Subject);
			}
			NetworkCredential nc = _credentials[sender];
			if (string.IsNullOrWhiteSpace(nc.UserName) || string.IsNullOrWhiteSpace(nc.Password))
				throw new ApplicationException("Sender credentials for " + sender.ToString() + " are not defined in configuration; cannot send emails.");
			msg.From = new MailAddress(nc.UserName, "3D Condo Explorer");  // TODO: Hard-coded

			if (message.ReplyTo != Sender.None)
			{
				NetworkCredential rtnc = _credentials[sender];
				if (string.IsNullOrWhiteSpace(rtnc.UserName))
					ServiceInstances.Logger.Error("Message's reply-to is set to {0}, but it is not defined in configuration; field not set.",
						message.ReplyTo);
				else
					msg.ReplyToList.Add(nc.UserName);
			}

			foreach (var bb in message.Body)
			{
				if (string.IsNullOrWhiteSpace(bb.Key))
				{
					msg.BodyEncoding = Encoding.UTF8;
					msg.Body = bb.Value;
				}
				else
				{
					msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
						bb.Value, Encoding.UTF8, bb.Key));
				}
			}

			try
			{
				using (SmtpClient client = new SmtpClient(
					Configuration.Messaging.Email.SmtpServerHost.Value,
					Configuration.Messaging.Email.SmtpServerPort.Value))
				{
					client.Credentials = nc;
					client.EnableSsl = Configuration.Messaging.Email.SmtpServerUseSsl.Value;
					client.Send(msg);
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Cannot send email.", ex);
			}
		}
	}
}