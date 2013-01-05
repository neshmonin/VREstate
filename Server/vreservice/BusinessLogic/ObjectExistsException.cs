using System;

namespace Vre.Server.BusinessLogic
{
    internal class ObjectExistsException : Exception
    {
        public ObjectExistsException(string message) : base(message) { }
        public ObjectExistsException(string message, Exception innerException) : base(message, innerException) { }
    }
}