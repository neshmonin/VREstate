
using System.Threading;
using Vre.Server.RemoteService;
using Vre.Server.Dao;
using Vre.Server.BusinessLogic;
using System.Collections.Generic;
using System;
namespace Vre.Server.Testing
{
    public class RandomUpdater
    {
        public static void Start()
        {
            new Thread(thread) { IsBackground = true, Name = "Testing.RandomUpdater" }.Start();
        }

        private static void thread()
        {
            Random rnd = new Random();

            while (true)
            {
                Thread.Sleep(10000);

                using (ClientSession session = ClientSession.MakeSystemSession())
                {
                    session.Resume();

                    EstateDeveloper developer;
                    Site site;
                    Building building;
                    Suite suite;
                    using (EstateDeveloperDao eddao = new EstateDeveloperDao(session.DbSession))
                    {
                        IList<EstateDeveloper> list = eddao.GetAll();
                        developer = list[rnd.Next(list.Count)];
                        site = developer.Sites[rnd.Next(developer.Sites.Count)];
                        building = site.Buildings[rnd.Next(site.Buildings.Count)];
                        suite = building.Suites[rnd.Next(building.Suites.Count)];
                    }

                    switch (rnd.Next(4))
                    {
                        case 0:
                            developer.MarkUpdated();
                            using (EstateDeveloperDao seddao = new EstateDeveloperDao(session.DbSession))
                                seddao.SafeUpdate(developer);
                            break;

                        case 1:
                            site.MarkUpdated();
                            using (SiteDao ssdao = new SiteDao(session.DbSession))
                                ssdao.SafeUpdate(site);
                            break;

                        case 2:
                            building.MarkUpdated();
                            using (BuildingDao sbdao = new BuildingDao(session.DbSession))
                                sbdao.SafeUpdate(building);
                            break;

                        case 3:
                            suite.MarkUpdated();
                            using (SuiteDao ssudao = new SuiteDao(session.DbSession))
                                ssudao.SafeUpdate(suite);
                            break;
                    }
                }
            }
        }
    }
}