using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CoreClasses;

namespace ConsoleSales
{
    public class ChangingBuilding : Building
    {
        public ChangingBuilding(Vre.Server.BusinessLogic.Building building) : base (building){}

        protected override Suite CreateSuite(Vre.Server.BusinessLogic.Client.SuiteEx suite)
        {
            return new ChangingSuite(suite);
        }
    }
}
