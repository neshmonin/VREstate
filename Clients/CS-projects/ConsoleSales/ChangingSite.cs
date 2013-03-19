using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreClasses;

namespace ConsoleSales
{
    public class ChangingSite : Site
    {
        public ChangingSite(Vre.Server.BusinessLogic.Site site) : base(site) { }

        protected override Building CreateBuilding(Vre.Server.BusinessLogic.ClientData cd)
        {
            ChangingBuilding newBuilding = new ChangingBuilding(new Vre.Server.BusinessLogic.Building(cd));
            return newBuilding.Create(new Vre.Server.BusinessLogic.Building(cd));
        }
    }
}
