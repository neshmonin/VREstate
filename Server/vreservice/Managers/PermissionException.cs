using System;

namespace Vre.Server.BusinessLogic
{
    public class PermissionException : Exception
    {
        public PermissionException(string explanation) : base(explanation) { }
    }
}