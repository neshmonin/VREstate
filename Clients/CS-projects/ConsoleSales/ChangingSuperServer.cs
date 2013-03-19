using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreClasses;

namespace ConsoleSales
{
    public class ChangingSuperServer : SuperServer
    {
        protected ChangingSuperServer() { }

        public new static SuperServer Create(string serverEndPoint, 
                                                int serverRequestTimeoutSec)
        {
            if (SuperServer.Create(serverEndPoint, serverRequestTimeoutSec) != null)
                return new ChangingSuperServer();

            return null;
        }

        protected override Developer CreateDeveloper(int ID, String name)
        {
            return new ChangingDeveloper(ID, name);
        }

        protected override Developer CreateDeveloper(Vre.Server.BusinessLogic.EstateDeveloper developer)
        {
            return new ChangingDeveloper(developer);
        }
    }
}
