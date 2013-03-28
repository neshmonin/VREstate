using System;
using System.Collections.Generic;
using System.Threading;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;
using Vre.Server.RemoteService;

namespace Vre.Server.Testing
{
    public class RandomUpdater
    {
        private static int _refreshTimeSec;

        public static void Start(int refreshTimeSec)
        {
            if (refreshTimeSec < 1) return;
            _refreshTimeSec = refreshTimeSec;
            new Thread(thread) { IsBackground = true, Name = "Testing.RandomUpdater" }.Start();
        }

        private static void thread()
        {
            Random rnd = new Random();

            while (true)
            {
                Thread.Sleep(1000 * _refreshTimeSec);

                try
                {
                    using (ClientSession session = ClientSession.MakeSystemSession())
                    {
                        session.Resume();

                        EstateDeveloper developer;
                        Site site;
                        Building building;
                        Suite suite;

                        using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session))
                        {
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
                                    using (EstateDeveloperDao seddao = new EstateDeveloperDao(session.DbSession))
                                        seddao.SafeUpdate(developer);
                                    break;

                                case 1:
                                    using (SiteDao ssdao = new SiteDao(session.DbSession))
                                        ssdao.SafeUpdate(site);
                                    break;

                                case 2:
                                    using (BuildingDao sbdao = new BuildingDao(session.DbSession))
                                        sbdao.SafeUpdate(building);
                                    break;

                                case 3:
                                    using (SuiteDao ssudao = new SuiteDao(session.DbSession))
                                        ssudao.SafeUpdate(suite);
                                    break;
                            }

                            tran.Commit();
                        }
                    }
                }
                catch (Exception e)
                {
                    ServiceInstances.Logger.Error("Testing:RandomUpdater failed iteration: {0}", e);
                }
            }
        }
    }
}