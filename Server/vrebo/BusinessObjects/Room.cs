using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class Room : UpdateableBase
    {
        public virtual SuiteLevel Level { get; protected set; }

        public virtual ValueWithUM FloorArea { get; protected set; }
        public virtual ValueWithUM WallsArea { get; protected set; }
        public virtual ValueWithUM PlinthLength { get; protected set; }

        protected virtual string floorArea { get { return FloorArea.AsRaw; } set { FloorArea = new ValueWithUM(value); } }
        protected virtual string wallsArea { get { return WallsArea.AsRaw; } set { WallsArea = new ValueWithUM(value); } }
        protected virtual string plinthLength { get { return PlinthLength.AsRaw; } set { PlinthLength = new ValueWithUM(value); } }

        public virtual ViewPoint Location { get; protected set; }
        public virtual RoomCategory Category { get; protected set; }

        protected Room() { }
        public Room(SuiteLevel level, RoomCategory category): base()
        {
            InitializeNew();
            Level = level;
            Category = category;
            FloorArea = ValueWithUM.EmptyArea;
            WallsArea = ValueWithUM.EmptyArea;
            PlinthLength = ValueWithUM.EmptyLinear;
            Location = ViewPoint.Empty;
        }
    }
}
