using System;
using System.Collections.Generic;

namespace Vre.Server.Messaging
{
    internal enum Sender { None, Server, ECommerce }

	internal class Message
	{
		public Sender From { get; private set; }
		public Sender ReplyTo { get; set; }
		public IEnumerable<string> Recipients { get { return _recipients; } }
		public string Subject { get; private set; }
		public IEnumerable<KeyValuePair<string, string>> Body { get { return _body; } }

		private List<string> _recipients = new List<string>();
		private Dictionary<string, string> _body = new Dictionary<string,string>();

		public Message(string recipient, string subject, string message)
			: this(Sender.Server, recipient, subject, message) { }

		public Message(Sender from, string recipient, string subject, string message)
		{
			From = from;
			ReplyTo = Sender.None;
			Subject = subject;
			_recipients.Add(recipient);
			_body.Add(string.Empty, message);
		}

		public Message(IEnumerable<string> recipients, string subject, string message)
			: this(Sender.Server, recipients, subject, message) { }

		public Message(Sender from, IEnumerable<string> recipients, string subject, string message)
		{
			From = from;
			ReplyTo = Sender.None;
			Subject = subject;
			_recipients.AddRange(recipients);
			_body.Add(string.Empty, message);
		}

		public void SetDefaultBody(string body)
		{
			_body[string.Empty] = body;
		}

		public void SetAlternativeHtmlBody(string body)
		{
			_body["text/html"] = body;
		}

		public void SetAlternativeBody(string mimeType, string body)
		{
			_body[mimeType] = body;
		}
	}

    internal interface IUserMessaging : IDisposable
    {
		void Send(Message message);
    }
}