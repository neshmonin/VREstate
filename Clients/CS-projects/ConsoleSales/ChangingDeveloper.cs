using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreClasses;

namespace ConsoleSales
{
    public class ChangingDeveloper : Developer
    {
        public ChangingDeveloper(Vre.Server.BusinessLogic.EstateDeveloper estateDeveloper) : base (estateDeveloper){}
        public ChangingDeveloper(int ID, string name) : base (ID, name){ }

        protected override Site CreateSite(Vre.Server.BusinessLogic.Site site)
        {
            ChangingSite newSite = new ChangingSite(site);
            return newSite.Create(site);
        }
    }
}
