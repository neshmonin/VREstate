using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public class RoomCategory
    {
        public virtual int AutoID { get; protected set; }
        public virtual string Name { get; protected set; }

        protected RoomCategory() { }
        public RoomCategory(string name) { Name = name; }
    }
}
