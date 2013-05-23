using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.IO;

namespace CoreClasses
{
    public class SuperServer
    {
        public const string SUPERSERVERDIR = "SuperServer";
        static Dictionary<string, Developer> developers = new Dictionary<string, Developer>();

        public static Dictionary<string, Developer> Developers
        {
            get { return SuperServer.developers; }
            set { SuperServer.developers = value; }
        }

        protected SuperServer() { }

        public static SuperServer Create(string serverEndPoint, 
                                  int serverRequestTimeoutSec)
        {
            if (!ServerProxy.Create(serverEndPoint, serverRequestTimeoutSec))
                return null;

            if (ServerProxy.Test())
                return new SuperServer();

            return null;
        }

        protected virtual Developer CreateDeveloper(int ID, String name)
        {
            return new Developer(ID, name);
        }

        //http://server.3dcondox.com:8082/vre/data/ed?ed=<DeveloperID>&sid=<SID>
        public bool Login(string login, string password, string role, string developerId)
        {
            foreach (var key in developers.Keys)
                developers.Remove(key);

            if (!ServerProxy.Login(login, password, role, developerId))
                return false;

            string name = "Default";
            int ID = 0;
            try { ID = int.Parse(developerId); } catch (System.FormatException) {}

            Developer dev = CreateDeveloper(ID, name);
            Developers.Add(name, dev);

            return true;
        }

        protected virtual Developer CreateDeveloper(Vre.Server.BusinessLogic.EstateDeveloper developer)
        {
            return new Developer(developer);
        }


        //http://server.3dcondox.com:8082/vre/data/ed?sid=<SID>
        public bool SuperadminLogin(string login, string password)
        {
            foreach (var key in developers.Keys)
                developers.Remove(key);

            if (!ServerProxy.SuperadminLogin(login, password))
                return false;

            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get, "ed", "", null);
            if (HttpStatusCode.OK != resp.ResponseCode)
                return false;

            Vre.Server.BusinessLogic.ClientData developerList = resp.Data;

            foreach (Vre.Server.BusinessLogic.ClientData cd in developerList.GetNextLevelDataArray("developers"))
            {
                Vre.Server.BusinessLogic.EstateDeveloper _developer = 
                    new Vre.Server.BusinessLogic.EstateDeveloper(cd);
                Developer dev = CreateDeveloper(_developer);
                Developers.Add(_developer.Name, dev);
            }

            return true;
        }
    }
}
