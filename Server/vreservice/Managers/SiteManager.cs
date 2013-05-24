using System;
using NHibernate;
using System.Collections.Generic;
using Vre.Server.Dao;
using System.IO;
using Vre.Server.RemoteService;

namespace Vre.Server.BusinessLogic
{
    internal class SiteManager : GenericManager
    {
        public SiteManager(ClientSession clientSession) : base(clientSession) { }

        //public Building CreateBuilding(int estateDeveloperId, string buildingSchema)
        //{
        //    using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session))
        //    {
        //        Building b = CreateBuilding(estateDeveloperId);
        //        CreateSuites(b, buildingSchema);
        //        tran.Commit();
        //        return b;
        //    }
        //}

        //public Building CreateBuilding(int estateDeveloperId)
        //{
        //    using (BuildingDao bdao = new BuildingDao(_session))
        //    {
        //        Building b = new Building(estateDeveloperId);
        //        bdao.Create(b);
        //        bdao.Flush();
        //        return b;
        //    }
        //}

        public Site[] List(int developerId, bool includeDeleted)
        {
            RolePermissionCheck.CheckListSites(_session);

            using (SiteDao dao = new SiteDao(_session.DbSession))
            {
                return dao.GetByDeveloperId(developerId, includeDeleted);
            }
        }

        public void CreateSuites(Building building, string buildingSchema)
        {
            throw new NotImplementedException();
        }

        public Building[] ListBuildings(int siteId, bool includeDeleted)
        {
            RolePermissionCheck.CheckListBuildings(_session);

            using (BuildingDao dao = new BuildingDao(_session.DbSession))
            {
                return NHibernateHelper.IListToArray<Building>(dao.GetAll(siteId, includeDeleted));
            }

            //// ROLE PERMISSION LOGIC
            //// none here: everyone can get a list of these

            //using (SiteDao dao = new SiteDao(_session.DbSession))
            //{
            //    Site site = dao.GetById(siteId);
            //    if (site != null) return NHibernateHelper.IListToArray<Building>(site.Buildings);
            //    else throw new FileNotFoundException("Site does not exist.");
            //}
        }

        public Suite[] ListSuitesByBuiding(int buildingId)
        {
            // ROLE PERMISSION LOGIC
            // none here: everyone can get a list of these

            using (BuildingDao dao = new BuildingDao(_session.DbSession))
            {
                Building building = dao.GetById(buildingId);
                if (building != null) return NHibernateHelper.IListToArray<Suite>(building.Suites);
                else throw new FileNotFoundException("Building does not exist.");
            }
        }

        public Building GetBuildingById(int buildingId)
        {
            // ROLE PERMISSION LOGIC
            // none here: everyone can get a list of these

            using (BuildingDao dao = new BuildingDao(_session.DbSession))
            {
                Building building = dao.GetById(buildingId);
                if (building != null) return building;
                else throw new FileNotFoundException("Building does not exist.");
            }
        }

        public SuiteType GetSuiteTypeByName(int siteId, string suiteTypeName)
        {
            // ROLE PERMISSION LOGIC
            // none here: everyone can get a list of these

            using (SuiteTypeDao dao = new SuiteTypeDao(_session.DbSession))
            {
                SuiteType result = dao.GetBySiteAndName(siteId, suiteTypeName);
                if (result != null) return result;
                else throw new FileNotFoundException("Suite type does not exist.");
            }
        }

        public SuiteType[] ListSuiteTypes(int siteId)
        {
            // ROLE PERMISSION LOGIC
            // none here: everyone can get a list of these

            using (SiteDao dao = new SiteDao(_session.DbSession))
            {
                Site site = dao.GetById(siteId);
                if (site != null) return NHibernateHelper.IListToArray<SuiteType>(site.SuiteTypes);
                else throw new FileNotFoundException("Site does not exist.");
            }
        }

        public Suite GetSuiteById(int suiteId)
        {
            // ROLE PERMISSION LOGIC
            // none here: everyone can get this

            using (SuiteDao dao = new SuiteDao(_session.DbSession))
            {
                Suite result = dao.GetById(suiteId);
                if (result != null) return result;
                throw new FileNotFoundException("Suite does not exist.");
            }
        }

        public bool UpdateSuite(Suite suite)
        {
            RolePermissionCheck.CheckUpdateSuite(_session, suite);

            using (SuiteDao dao = new SuiteDao(_session.DbSession))
            {
                bool result = dao.SafeUpdate(suite);
                if (result)
                    ServiceInstances.Logger.Info("User ID={0} updated suite ID={1} ({2}, {3}).",
                        _session.User.AutoID, suite.AutoID,
                        suite.SuiteName, suite.Building);
                return result;
            }
        }

        public bool UpdateBuilding(Building building)
        {
            RolePermissionCheck.CheckUpdateBuilding(_session, building);

            using (BuildingDao dao = new BuildingDao(_session.DbSession))
            {
                bool result = dao.SafeUpdate(building);
                if (result)
                    ServiceInstances.Logger.Info("User ID={0} updated building ID={1} ({2}).",
                        _session.User.AutoID, building.AutoID, building);
                return result;
            }
        }

		//public float GetCurrentSuitePrice(Suite suite)
		//{
		//    Price p = null;
                        
		//    using (OptionTypeDao dao = new OptionTypeDao(_session.DbSession))
		//    {
		//        foreach (Option opt in suite.OptionsPossible)
		//        {
		//            if (dao.IsSuiteOption(opt.OpType))
		//            {
		//                if (opt.Prices.Count > 0) p = opt.Prices[0];  // sorted by time [newest first] already
		//                break;
		//            }
		//        }
		//    }

		//    if (p != null) return p.PricePerUnitForBuyer;
		//    return -1.0f;
		//}

		//public static float GetCurrentSuitePriceF(Suite suite)
		//{
		//    Price p = null;

		//    foreach (Option opt in suite.OptionsPossible)
		//    {
		//        if (OptionTypeDao.IsSuiteOptionF(opt.OpType))
		//        {
		//            if (opt.Prices.Count > 0) p = opt.Prices[0];  // sorted by time [newest first] already
		//            break;
		//        }
		//    }

		//    if (p != null) return p.PricePerUnitForBuyer;
		//    return -1.0f;
		//}

		//public bool SetSuitePrice(Suite suite, float price)
		//{
		//    RolePermissionCheck.CheckUpdateSuite(_session, suite);

		//    bool result = false;

		//    using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
		//    {
		//        using (OptionTypeDao dao = new OptionTypeDao(_session.DbSession))
		//        {
		//            // search for a "Suite" option first
		//            foreach (Option opt in suite.OptionsPossible)
		//            {
		//                if (dao.IsSuiteOption(opt.OpType))
		//                {
		//                    emitNewPrice(opt, price);
		//                    ServiceInstances.EntityUpdateTracker.NotifyModified(new Suite[] {suite});

		//                    result = true;
		//                    break;
		//                }
		//            }
		//            if (!result) // first-time price set; create new option
		//            {
		//                // TODO: Option's owner is requesting user; should be someone else?!
		//                Option opt = new Option(suite.Building, _session.User, "Suite", dao.GetSuiteOption());
		//                using (OptionDao odao = new OptionDao(_session.DbSession)) odao.Create(opt);

		//                emitNewPrice(opt, price);

		//                suite.OptionsPossible.Add(opt);

		//                using (SuiteDao sdao = new SuiteDao(_session.DbSession))
		//                    result = sdao.SafeUpdate(suite);
		//            }
		//        }

		//        suite.CurrentPrice = new Money(Convert.ToDecimal(price), Currency.Cad);
		//        using (var dao = new SuiteDao(_session.DbSession)) dao.SafeUpdate(suite);

		//        tran.Commit();
		//    }

		//    return result;
		//}

		public bool LogNewSuitePrice(Suite suite, float price)
		{
			RolePermissionCheck.CheckUpdateSuite(_session, suite);

			bool result = false;

			using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
			{
				using (OptionTypeDao dao = new OptionTypeDao(_session.DbSession))
				{
					// search for a "Suite" option first
					foreach (Option opt in suite.OptionsPossible)
					{
						if (dao.IsSuiteOption(opt.OpType))
						{
							emitNewPrice(opt, price);

							result = true;
							break;
						}
					}
					if (!result) // first-time price set; create new option
					{
						// TODO: Option's owner is requesting user; should be someone else?!
						Option opt = new Option(suite.Building, _session.User, "Suite", dao.GetSuiteOption());
						using (OptionDao odao = new OptionDao(_session.DbSession)) odao.Create(opt);

						emitNewPrice(opt, price);

						suite.OptionsPossible.Add(opt);

						using (SuiteDao sdao = new SuiteDao(_session.DbSession))
							result = sdao.SafeUpdate(suite);
					}
				}

				tran.Commit();
			}

			return result;
		}

		private void emitNewPrice(Option opt, float price)
        {
            Price p = new Price(opt);
            p.NumberOfUnits = 1;
            p.PricePerUnitForBuyer = price;
            p.StartingDate = DateTime.UtcNow;
            p.UnitName = "CAD";

            _session.DbSession.Save(p);  // no need for special DAO; this is single-use saveable object!!!
        }
    }
}