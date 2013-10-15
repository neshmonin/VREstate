using System;
using NHibernate;
using System.Collections.Generic;
using Vre.Server.Dao;
using System.IO;
using Vre.Server.RemoteService;
using Vre.Server.Mls;
using System.Text;
using System.Linq;

namespace Vre.Server.BusinessLogic
{
    internal class SiteManager : GenericManager
	{
		private Dictionary<EstateDeveloper, User> _importersList = new Dictionary<EstateDeveloper, User>();

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

		public string ImportUpdateExistingViewOrders(ICollection<string> activeItems, ICollection<MlsItem> newItems, 
			bool importOnly)
		{
			var issues = new StringBuilder();
			int add = 0, adj = 0, skp = 0, err = 0;

			ServiceInstances.Logger.Info("Got {0} items to check.", newItems.Count);

			IDictionary<Guid, string> voIds;
			using (var vodao = new ViewOrderDao(_session.DbSession))
				// TODO: MLS provider filter injection point
				voIds = vodao.GetAllActiveIdsAndMlsIdV2();

			foreach (var item in newItems)
			{
				var voId = Guid.Empty;

				try
				{
					if (!voIds.Values.Contains(item.MlsId))
					{
						if (string.IsNullOrEmpty(item.StreetNumber))
						{
							ServiceInstances.Logger.Warn("MLS#{0} lists incomplete address ({1}); not processed.",
								item.MlsId, item.CompiledAddress);
							issues.AppendFormat("MLS#{0} lists incomplete address ({1}); not processed.\r\n",
								item.MlsId, item.CompiledAddress);
							skp++;
						}
						else
						{
							IEnumerable<UpdateableBase> results = FindMlsItemTarget(item, issues);
							if (null == results)
								err++;
							else
								ImportListing(item, results, ref err, ref skp, ref add, ref issues);
						}
					}
					else if (!importOnly)
					{
						// such approach is required as we can have N View Orders for one MLS#
						foreach (var id in voIds.Keys)
						{
							if (voIds[id].Equals(item.MlsId))
							{
								voId = id;
								UpdateViewOrderInfo(item, voId, ref skp, ref err, ref adj);
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (voId.Equals(Guid.Empty))
						ServiceInstances.Logger.Error("MLS#{0} processing error: {1}",
														item.MlsId, ex);
					else
						ServiceInstances.Logger.Error("MLS#{0} (VOID={1}) processing error: {2}",
														item.MlsId, voId, ex);
					err++;
				}
			}
			ServiceInstances.Logger.Info("Update completed; {0} ViewOrders added; {1} adjusted; {2} skipped; {3} errors.",
				add, adj, skp, err);

			if (err > 0) issues.AppendFormat("Total {0} listing processing errors.", err);

			return issues.ToString();
		}

		public string RetroImportExistingViewOrders(ICollection<MlsItem> newItems, int buildingId,
			ref int add, ref int err)
		{
			var issues = new StringBuilder();
			int skp = 0;

			IDictionary<Guid, string> voIds;
			using (var vodao = new ViewOrderDao(_session.DbSession))
				// TODO: MLS provider filter injection point
				voIds = vodao.GetAllActiveIdsAndMlsIdV2();

			foreach (var item in newItems)
			{
				var voId = Guid.Empty;

				try
				{
					if (!voIds.Values.Contains(item.MlsId))
					{
						if (!string.IsNullOrEmpty(item.StreetNumber))
						{
							IEnumerable<UpdateableBase> results = FindMlsItemTarget(item, issues);
							if (null == results)
							{
								err++;
							}
							else
							{
								List<UpdateableBase> targets = results.ToList();

								// filter targets against our building filter argument
								//
								for (int idx = targets.Count - 1; idx >= 0; idx--)
								{
									Suite s = targets[idx] as Suite;
									if (s != null)
									{
										//if (!buildingFilter.Contains(s.Building.AutoID))
										if (s.Building.AutoID != buildingId)
											targets.RemoveAt(idx);
									}
									else
									{
										Building b = targets[idx] as Building;
										if (b != null)
										{
											//if (!buildingFilter.Contains(b.AutoID))
											if (b.AutoID != buildingId)
												targets.RemoveAt(idx);
										}
										else
										{
											targets.RemoveAt(idx);
										}
									}
								}

								ImportListing(item, targets, ref err, ref skp, ref add, ref issues);
							}
						}
					}
					else
					{
						// check if MLS info needs to be imported
						ImportUpdateMlsInfo(item, true);
					}
				}
				catch (Exception ex)
				{
					if (voId.Equals(Guid.Empty))
						issues.AppendFormat("MLS#{0} processing error: {1}",
														item.MlsId, ex);
					else
						issues.AppendFormat("MLS#{0} (VOID={1}) processing error: {2}",
														item.MlsId, voId, ex);
					err++;
				}
			}

			if (err > 0) issues.AppendFormat("Total {0} listing processing errors.", err);

			return issues.ToString();
		}

		public string RetroImportExistingViewOrders(
			ICollection<MlsItem> newItems, 
			ref IDictionary<Guid, string> voIds,
			ref int add, ref int err)
		{
			var issues = new StringBuilder();
			int skp = 0;

			foreach (var item in newItems)
			{
				var voId = Guid.Empty;

				try
				{
					if (!voIds.Values.Contains(item.MlsId))
					{
						if (!string.IsNullOrEmpty(item.StreetNumber))
						{
							IEnumerable<UpdateableBase> results = FindMlsItemTarget(item, issues);
							if (null == results)
							{
								err++;
							}
							else
							{
								foreach (var id in ImportListing(
									item, results, ref err, ref skp, ref add, ref issues))
									voIds.Add(new KeyValuePair<Guid, string>(id, item.MlsId));
							}
						}
					}
					else
					{
						// check if MLS info needs to be imported
						ImportUpdateMlsInfo(item, true);
					}
				}
				catch (Exception ex)
				{
					if (voId.Equals(Guid.Empty))
						issues.AppendFormat("MLS#{0} processing error: {1}",
														item.MlsId, ex);
					else
						issues.AppendFormat("MLS#{0} (VOID={1}) processing error: {2}",
														item.MlsId, voId, ex);
					err++;
				}
			}

			if (err > 0) issues.AppendFormat("Total {0} listing processing errors.", err);

			return issues.ToString();
		}

		private void UpdateViewOrderInfo(MlsItem item, Guid voId,
			ref int skp, ref int err, ref int adj)
		{
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
			{
				var changed = false;

				ViewOrder vo;
				using (var vodao = new ViewOrderDao(_session.DbSession))
					vo = vodao.GetById(voId);

				if ((null == vo) || (vo.TargetObjectType != ViewOrder.SubjectType.Suite))
				{
					ServiceInstances.Logger.Warn("MLS#{0} Existing VOID={1} references a non-suite; skipped.",
						item.MlsId, voId);
					skp++;
					// TODO: add support for buildings
					return;
				}

				Suite s;
				using (var dao = new SuiteDao(_session.DbSession)) s = dao.GetById(vo.TargetObjectId);

				if (null == s)
				{
					ServiceInstances.Logger.Error("MLS#{0} Existing VOID={1} references unknown suite ID {2}",
													item.MlsId, vo.AutoID, vo.TargetObjectId);
					err++;
					return;
				}

				var currentPrice = s.CurrentPrice;
				var newPrice = new Money(Convert.ToDecimal(item.CurrentPrice), Currency.Cad); // TODO: Currently locked to CAD
				if (!currentPrice.HasValue || (currentPrice.Value.CompareTo(newPrice) != 0))
				//if (Math.Abs(current - item.CurrentPrice) >= 0.01)
				{
					s.CurrentPrice = newPrice;
					LogNewSuitePrice(s, (float)item.CurrentPrice);

					ServiceInstances.Logger.Info("Changing suite ID {0} ({1}, {2}) price {3} -> {4}",
												s.AutoID, item.SuiteName, item.CompiledAddress, currentPrice, newPrice);
					changed = true;
				}

				// TODO: Take special care of manually created ViewOrders (e.g. BuyingAgent's orders)?

				if (!string.IsNullOrEmpty(item.VTourUrl))
				{
					if (string.IsNullOrEmpty(vo.VTourUrl) || !vo.VTourUrl.Equals(item.VTourUrl))
					{
						if (!vo.VTourUrl.Contains("3dcondox.com")) // avoid replacing our tours!
							// TODO: hard-coded domain name
						{
							var current = vo.VTourUrl;
							vo.VTourUrl = item.VTourUrl;
							using (var vodao = new ViewOrderDao(_session.DbSession)) vodao.Update(vo);

							ServiceInstances.Logger.Info("Changing VOID {0} tour URL from {1} to {2}",
															vo.AutoID, current, vo.VTourUrl);
							changed = true;
						}
						else if (string.IsNullOrEmpty(vo.InfoUrl))
						{
							vo.InfoUrl = item.VTourUrl;
							using (var vodao = new ViewOrderDao(_session.DbSession)) vodao.Update(vo);

							ServiceInstances.Logger.Info("Setting VOID {0} Info URL to tour URL {1}",
															vo.AutoID, vo.VTourUrl);
							changed = true;
						}
					}
				}

				// Update extra information
				ImportUpdateMlsInfo(item, false);
				
				if (!changed) return;
				adj++;
				tran.Commit();
			}
		}

		private IList<Guid> ImportListing(MlsItem item, IEnumerable<UpdateableBase> targets,
			ref int err, ref int skp, ref int add, ref StringBuilder issues)
		{
			var procesedEds = new List<EstateDeveloper>();
			var result = new List<Guid>();
			foreach (var target in targets)
			{
				var suite = target as Suite;

				if (null == suite)
				{
					ServiceInstances.Logger.Warn("MLS#{0} Lists a non-suite; shall not add View Order.",
												 item.MlsId);
					skp++; // not a known address
					continue;
				}

				var ed = suite.Building.ConstructionSite.Developer;

				if (procesedEds.Contains(ed))
				{
					ServiceInstances.Logger.Error(
						"MLS#{0} Lists address {1} which resolves to more than one suite belonging to a single Estate Developer.",
						item.MlsId, item.CompiledAddress);
					issues.AppendFormat(
						"MLS#{0} Lists address {1} which resolves to more than one suite belonging to a single Estate Developer.\r\n",
						item.MlsId, item.CompiledAddress);
					skp++; // not a known address
					continue;
				}
				procesedEds.Add(ed);

				User importer = GetImportOwnerUser(ed, ref issues);
				if (null == importer) continue;

				var vo = new ViewOrder(importer.AutoID,
					ViewOrder.ViewOrderProduct.PublicListing, ViewOrder.ViewOrderOptions.ExternalTour,
					item.MlsId, ViewOrder.SubjectType.Suite, target.AutoID, item.VTourUrl,
					DateTime.UtcNow.AddDays(365))
				{
					Imported = true,
					InfoUrl = string.Format("http://realtor.ca/Disclaimer.aspx?Mode=5&id={0}", item.MlsId)
				};
				// todo: what's default listing lifetime?!

				// Import extra information
				ImportUpdateMlsInfo(item, false);

				using (var tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
				{
					using (var dao = new ViewOrderDao(_session.DbSession)) dao.Create(vo);

					suite.CurrentPrice = new Money(Convert.ToDecimal(item.CurrentPrice), Currency.Cad); // TODO: Currently locked to CAD
					LogNewSuitePrice(suite, (float)item.CurrentPrice);
					
					switch (item.SaleLeaseState)
					{
						case MlsItem.SaleLease.Lease:
							suite.Status = Suite.SalesStatus.AvailableRent;
							break;

						case MlsItem.SaleLease.Sale:
							suite.Status = Suite.SalesStatus.ResaleAvailable;
							break;

						default:
							ServiceInstances.Logger.Error("MLS#{0} listed unknown sale status; suite status not changed!", item.MlsId);
							issues.AppendFormat("MLS#{0} listed unknown sale status; suite status not changed!\r\n", item.MlsId);
							break;
					}
					using (var dao = new SuiteDao(_session.DbSession)) dao.SafeUpdate(suite);

					tran.Commit();
					add++;
				}
				ServiceInstances.Logger.Info("Imported MLS#{0} for {1} ({2}); VOID={3}",
					item.MlsId, ed.Name, item.CompiledAddress, vo.AutoID);

				result.Add(vo.AutoID);
			}

			if (0 == procesedEds.Count)
			{
				ServiceInstances.Logger.Info("MLS#{0} Lists an unknown address: {1}; shall not add View Order.",
											 item.MlsId, item.CompiledAddress);
				skp++; // not a known address
			}

			return result;
		}

		private IEnumerable<UpdateableBase> FindMlsItemTarget(MlsItem item, StringBuilder issues)
		{
			var sq = new ServiceQuery();
			if (!string.IsNullOrEmpty(item.PostalCode)) sq.Add("ad_po", item.PostalCode);
			if (!string.IsNullOrEmpty(item.StateProvince)) sq.Add("ad_stpr", item.StateProvince);
			if (!string.IsNullOrEmpty(item.Municipality)) sq.Add("ad_mu", item.Municipality);
			if (!string.IsNullOrEmpty(item.StreetName)) sq.Add("ad_stn", item.StreetName);
			if (!string.IsNullOrEmpty(item.StreetType)) sq.Add("ad_stt", item.StreetType.ToUpperInvariant());
			if (!string.IsNullOrEmpty(item.StreetDirection)) sq.Add("ad_std", item.StreetDirection.ToLowerInvariant());
			if (!string.IsNullOrEmpty(item.StreetNumber)) sq.Add("ad_bn", item.StreetNumber);
			if (!string.IsNullOrEmpty(item.SuiteName)) sq.Add("ad_ibn", item.SuiteName);

			IEnumerable<UpdateableBase> results;
			try
			{
				ServiceInstances.Logger.Debug("MLS Item to parse: id=<{0}>, pc=<{1}>, sp=<{2}>, m=<{3}>, "
					+ "sn=<{4}>, st=<{5}>, sd=<{6}>, bn=<{7}>, stn=<{8}>, addr=<{9}>",
					item.MlsId,
					item.PostalCode, item.StateProvince, item.Municipality,
					item.StreetName, item.StreetType, item.StreetDirection,
					item.StreetNumber, 
					item.SuiteName,
					item.CompiledAddress);

				results = AddressHelper.ParseGeographicalAddressToModel(sq, _session.DbSession, true);
			}
			catch (ArgumentException ae)
			{
				//ServiceInstances.Logger.Error("Cannot parse address of MLS#{0} ({1}): {2}",
				//                              item.MlsId, item.CompiledAddress, ae.Message);
				issues.AppendFormat("Cannot parse address of MLS#{0} ({1}): {2}\r\n",
											  item.MlsId, item.CompiledAddress, ae.Message);
				results = null;
			}
			return results;
		}

		private User GetImportOwnerUser(EstateDeveloper ownerEd, ref StringBuilder issues)
		{
			User result;

			if (!_importersList.TryGetValue(ownerEd, out result))
			{
				var matchNName = Configuration.Mls.ImportUserNickNameOwner.Value;

				IList<User> ownerUsers;
				using (var dao = new UserDao(_session.DbSession))
					ownerUsers = dao.ListUsers(User.Role.DeveloperAdmin, ownerEd.AutoID, null, false);

				foreach (var user in ownerUsers)
					if (matchNName.Equals(user.NickName))
					{
						result = user;
						break;
					}

				if (null == result)
				{
					ServiceInstances.Logger.Error(
						"MLS Import User Nickname ({0}) not found in {1}; auto-import disabled for this Estate Developer.",
						matchNName, ownerEd.Name);
					issues.AppendFormat(
						"MLS Import User Nickname ({0}) not found in {1}; auto-import disabled for this Estate Developer.\r\n",
						matchNName, ownerEd.Name);
				}
				else
				{
					ServiceInstances.Logger.Info("MLS Import using user {0} for {1} Estate Developer.", result, ownerEd.Name);
				}
				_importersList.Add(ownerEd, result);  // make sure NULL is added to list too so that we get the error once.
			}

			return result;
		}

		private void ImportUpdateMlsInfo(MlsItem item, bool importOnly)
		{
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(_session))
			{
				MlsInfo info;
				using (var dao = new MlsInfoDao(_session.DbSession))
				{
					info = dao.GetByMlsNum(item.MlsId);

					if (info != null)
					{
						if (!importOnly)
						{
							info.Update(item);
							if (info.Deleted) info.Undelete();
							dao.Update(info);
							ServiceInstances.Logger.Info("Updated MLS info for {0}", item.MlsId);
						}
					}
					else
					{
						info = new MlsInfo(item);
						dao.Create(info);
						ServiceInstances.Logger.Info("Added MLS info for {0}", item.MlsId);
					}
				}
				tran.Commit();
				_session.DbSession.Flush();  // required to ensure "info" gets flushed before it is GC-ed
			}
		}
	}
}