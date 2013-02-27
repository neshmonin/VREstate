using System;
using System.Collections.Generic;

namespace Vre.Server.Messaging
{
    internal enum Sender { Server, ECommerce }
    internal interface IUserMessaging : IDisposable
    {
        void Send(Sender sender, string recipient, string subject, string message);
        void Send(string recipient, string subject, string message);
        void Send(Sender sender, IEnumerable<string> recipients, string subject, string message);
        void Send(IEnumerable<string> recipients, string subject, string message);
    }
}