using System.Collections.Generic;
using System.IO;
using System;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class Message
    {
        public enum ReferenceType : int
        {
            OptionCutOffDay
        }

        protected int _AutoID;
        protected int _ReferenceID;
        protected int _ReferenceType;
        protected string _Text;

    }
}