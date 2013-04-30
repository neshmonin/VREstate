using System;
using Vre.Server.Command;

namespace Vre.Server.Task
{
    internal abstract class BaseTask : ITask
    {
        public abstract string Name { get; }
        public abstract void Execute(Parameters param);

        protected void SendAdministrativeAlert(string summary, string text)
        {
            try
            {
                var recipient = ServiceInstances.Configuration.GetValue("AdminMessageRecipients", "admin@3dcondox.com");

                ServiceInstances.EmailSender.Send(recipient,
                    (summary != null) ? string.Format("Task {0}: {1}", Name, summary) : string.Format("Task {0} alert", Name),
                    text);
            }
            catch (Exception e)
            {
                ServiceInstances.Logger.Error("Administrative alert was not send.\r\nTask: {0}\r\nSummary: {1}\r\nText: {2}\r\nError: {3}",
                    Name, summary ?? "<N/A>", text, e);
            }
        }
    }
}