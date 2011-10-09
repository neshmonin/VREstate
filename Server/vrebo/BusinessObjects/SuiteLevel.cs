using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class SuiteLevel : UpdateableBase
    {
        public virtual SuiteType SuiteType { get; protected set; }
        public virtual int Order { get; set; }
        public virtual string Name { get; set; }
        public virtual string Model { get; set; }
        public virtual IList<Room> Rooms { get; protected set; }

        protected SuiteLevel() { }
        public SuiteLevel(SuiteType suiteType, string name) : base()
        {
            InitializeNew();
            SuiteType = suiteType;
            Name = name;
            Order = 0;
            Model = null;
            Rooms = new List<Room>();
        }
    }
}
