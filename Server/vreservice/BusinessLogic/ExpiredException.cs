using System;

namespace Vre.Server.BusinessLogic
{
    internal class ExpiredException : Exception
    {
        public ExpiredException(string message) : base(message) { }
        public ExpiredException(string message, Exception innerException) : base(message, innerException) { }
    }
}