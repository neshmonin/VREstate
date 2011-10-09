using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
	[Serializable]
	public partial class Option : UpdateableBase
	{
        public virtual string Description { get; set; }
        //public IList<Price> Prices { get; private set; }
        //public User Provider { get; set; }
        public virtual DateTime? CutoffDay { get; set; }
        //public virtual int ProviderID { get; private set; }
        //public virtual int BuildingID { get; private set; }
        public virtual Building Building { get; protected set; }
        public virtual User Provider { get; protected set; }
        public virtual OptionType OpType { get; private set; }
        public virtual IList<Price> Prices { get; private set; }

        private Option() { }

		public Option(Building building, User provider, string description, OptionType type)
            : base()
		{
            Created = DateTime.UtcNow;
            Updated = Created;
            Deleted = false;
            Description = description;
            Provider = provider;
            Building = building;
            OpType = type;
            //TypeID = typeId;
		}
    }
}
