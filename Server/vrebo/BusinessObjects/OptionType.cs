using System;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public class OptionType
    {
        public const string SuiteOptionDescription = "Suite";

        public virtual int AutoID { get; protected set; }
        public virtual string Description { get; protected set; }

        protected OptionType() { }

        public OptionType(string description)
        {
            Description = description;
        }
    }
}