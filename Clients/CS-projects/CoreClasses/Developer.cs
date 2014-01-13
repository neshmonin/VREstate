using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.IO;

namespace CoreClasses
{
    public class Developer
    {
        private Vre.Server.BusinessLogic.EstateDeveloper _estateDeveloper;

        private List<Site> m_sites = new List<Site>();

        public string Name { get { return _estateDeveloper.Name; } }
        public int ID { get { return _estateDeveloper.AutoID; } }

        public List<Site> Sites { get { return m_sites; } }

        //http://server.3dcondox.com:8082/vre/data/site?ed=<estate developer id>&sid=<SID> -список всех площадок по застройщику
        //http://server.3dcondox.com:8082/vre/data/building?site=<site id>&sid=<SID> -список домов по одной площадке
        public Developer(Vre.Server.BusinessLogic.EstateDeveloper estateDeveloper)
        {
            _estateDeveloper = estateDeveloper;
        }

        public Developer(int ID, string name)
        {
            Vre.Server.BusinessLogic.ClientData developerData = new Vre.Server.BusinessLogic.ClientData();

            developerData.Add("id", ID);  // informational only
            developerData.Add("deleted", false);  // informational only

            developerData.Add("name", name);  // informational only
            developerData.Add("configuration", Vre.Server.BusinessLogic.EstateDeveloper.Configuration.Online);  // informational only

            _estateDeveloper = new Vre.Server.BusinessLogic.EstateDeveloper(developerData);
        }

        public bool PopulateSiteList()
        {
            ServerResponse resp = ServerProxy.MakeDataRequest(ServerProxy.RequestType.Get, "site",
                "ed=" + _estateDeveloper.AutoID, null);
            if (HttpStatusCode.OK != resp.ResponseCode)
                return false;

            m_sites.Clear();

            Vre.Server.BusinessLogic.ClientData siteList = resp.Data;

            foreach (Vre.Server.BusinessLogic.ClientData cd in siteList.GetNextLevelDataArray("sites"))
            {
                //Vre.Server.BusinessLogic.Site _site = new Vre.Server.BusinessLogic.Site(_estateDeveloper, cd);
                Vre.Server.BusinessLogic.Site _site = new Vre.Server.BusinessLogic.Site(cd, _estateDeveloper);
                Site site = CreateSite(_site);
                m_sites.Add(site);
            }

            return true;
        }

        protected virtual Site CreateSite(Vre.Server.BusinessLogic.Site site)
        {
            Site newSite = new Site(site);
            return newSite.Create(site);
        }

        public bool SiteExists(Site site)
        {
            return m_sites.Exists(delegate(Site match)
            {
                return match.Name == site.Name;
            });
        }

        public Site Find(string name)
        {
            return m_sites.Find(delegate(Site match)
            {
                return match.Name == name;
            });
        }

        public bool AddSite(Site newSite)
        {
            if (SiteExists(newSite))
                return false;

            m_sites.Add(newSite);
            return true;
        }

        public bool RemoveSiteByName(string name)
        {
            Site oldSite = Find(name);
            if (oldSite == null)
                return false;

            return m_sites.Remove(oldSite);
        }

        public bool RemoveSite(Site site)
        {
            return m_sites.Remove(site);
        }

        public override string ToString()
        {
            return _estateDeveloper.ToString();
        }
    }
}
