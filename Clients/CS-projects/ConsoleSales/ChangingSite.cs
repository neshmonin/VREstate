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
            Vre.Server.BusinessLogic.Building vre_building = new Vre.Server.BusinessLogic.Building(cd, _site);
            ChangingBuilding newBuilding = new ChangingBuilding(vre_building);
            return newBuilding.Create(vre_building);
        }
    }
}
