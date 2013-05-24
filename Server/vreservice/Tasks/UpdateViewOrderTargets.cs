using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Command;
using Vre.Server.Dao;
using Vre.Server.Mls;
using Vre.Server.RemoteService;

namespace Vre.Server.Task
{
    internal class UpdateViewOrderTargets : BaseTask
    {
        public override string Name { get { return "UpdateViewOrderTargets"; } }
	    private Dictionary<EstateDeveloper, User> _importersList = new Dictionary<EstateDeveloper, User>();

        public override void Execute(Parameters param)
        {
            IMlsInfoProvider prov;
            
			// STEP 1
			//
            // TODO: MLS provider injection point
            prov = new RetsMlsStatusProvider();

            prov.Configure(ServiceInstances.Configuration.GetValue("MLS-STATUS-TREB-Config", string.Empty));
            ServiceInstances.Logger.Info("Running MLS retrieval...");
            prov.Run();  // SLOW!!!

	        var activeItems = prov.GetCurrentActiveItems();
            CloseRemovedMlsListings(activeItems);

			// STEP 2
			//
            // TODO: MLS provider injection point
            prov = new RetsMlsInfoProvider();

            prov.Configure(ServiceInstances.Configuration.GetValue("MLS-INFO-TREB-Config", string.Empty));
            ServiceInstances.Logger.Info("Running MLS retrieval...");
            prov.Run();  // SLOW!!!

            UpdateExistingViewOrders(activeItems, prov.GetNewItems());
        }

        private void UpdateExistingViewOrders(ICollection<string> activeItems, ICollection<MlsItem> newItems)
        {
			var issues = new StringBuilder();
            int add=0, adj = 0, skp = 0, err = 0;

            ServiceInstances.Logger.Info("Got {0} items to check.", newItems.Count);

            using (var session = NHibernateHelper.GetSession())
            {
                DatabaseSettingsDao.VerifyDatabase();

	            IDictionary<Guid, string> voIds;
                using (var vodao = new ViewOrderDao(session))
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
								ImportListing(item, session, ref err, ref skp, ref add, ref issues);
							}
						}
						else
						{
							// such approach is required as we can have N View Orders for one MLS#
							foreach (var id in voIds.Keys)
							{
								if (voIds[id].Equals(item.MlsId))
								{
									voId = id;
									UpdateViewOrderInfo(item, session, voId, ref skp, ref err, ref adj);
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
            }
            ServiceInstances.Logger.Info("Update completed; {0} ViewOrders added; {1} adjusted; {2} skipped; {3} errors.",
                add, adj, skp, err);

	        if (err > 0) issues.AppendFormat("Total {0} listing processing errors.", err);

			if (issues.Length > 0)
				SendAdministrativeAlert(null, issues.ToString());
        }

		private static void UpdateViewOrderInfo(MlsItem item, ISession session, Guid voId, 
			ref int skp, ref int err, ref int adj)
	    {
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
			{
				var changed = false;

				ViewOrder vo;
				using (var vodao = new ViewOrderDao(session))
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
				using (var dao = new SuiteDao(session)) s = dao.GetById(vo.TargetObjectId);

				if (null == s)
				{
					ServiceInstances.Logger.Error("MLS#{0} Existing VOID={1} references unknown suite ID {2}",
					                                item.MlsId, vo.AutoID, vo.TargetObjectId);
					err++;
					return;
				}

				using (var manager = new SiteManager(ClientSession.MakeSystemSession(session)))
				{
					var currentPrice = s.CurrentPrice;
					var newPrice = new Money(Convert.ToDecimal(item.CurrentPrice), Currency.Cad); // TODO: Currently locked to CAD
					if (!currentPrice.HasValue || (currentPrice.Value.CompareTo(newPrice) != 0))
					//if (Math.Abs(current - item.CurrentPrice) >= 0.01)
					{
						s.CurrentPrice = newPrice;
						manager.LogNewSuitePrice(s, (float)item.CurrentPrice);

						ServiceInstances.Logger.Info("Changing suite ID {0} ({1}, {2}) price {3} -> {4}",
					                                s.AutoID, item.SuiteName, item.CompiledAddress, currentPrice, newPrice);
						changed = true;
					}
				}

				if (!string.IsNullOrEmpty(item.VTourUrl))
				{
					if (string.IsNullOrEmpty(vo.VTourUrl) || !vo.VTourUrl.Equals(item.VTourUrl))
					{
						if (!vo.VTourUrl.Contains("3dcondox.com")) // avoid replacing our tours!
						{
							var current = vo.VTourUrl;
							vo.VTourUrl = item.VTourUrl;
							using (var vodao = new ViewOrderDao(session)) vodao.Update(vo);

							ServiceInstances.Logger.Info("Changing VOID {0} tour URL from {1} to {2}",
							                                vo.AutoID, current, vo.VTourUrl);
							changed = true;
						}
						else if (string.IsNullOrEmpty(vo.InfoUrl))
						{
							vo.InfoUrl = item.VTourUrl;
							using (var vodao = new ViewOrderDao(session)) vodao.Update(vo);

							ServiceInstances.Logger.Info("Setting VOID {0} Info URL to tour URL {1}",
							                                vo.AutoID, vo.VTourUrl);
							changed = true;
						}
					}
				}

				if (!changed) return;
				adj++;
				tran.Commit();
			}
	    }

	    private void ImportListing(MlsItem item, ISession session, 
			ref int err, ref int skp, ref int add, ref StringBuilder issues)
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
			    results = AddressHelper.ParseGeographicalAddressToModel(sq, session, true);
		    }
		    catch (ArgumentException ae)
		    {
			    ServiceInstances.Logger.Error("Cannot parse address of MLS#{0} ({1}): {2}",
			                                  item.MlsId, item.CompiledAddress, ae.Message);
				issues.AppendFormat("Cannot parse address of MLS#{0} ({1}): {2}\r\n",
											  item.MlsId, item.CompiledAddress, ae.Message);
				err++;
			    return;
		    }

			var procesedEds = new List<EstateDeveloper>();
			foreach (var result in results)
			{
				var suite = result as Suite;

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

				User importer = GetImportOwnerUser(ed, session, ref issues);
				if (null == importer) continue;

				var vo = new ViewOrder(importer.AutoID,
					ViewOrder.ViewOrderProduct.PublicListing, ViewOrder.ViewOrderOptions.ExternalTour,
					item.MlsId, ViewOrder.SubjectType.Suite, result.AutoID, item.VTourUrl,
					DateTime.UtcNow.AddDays(365))
					{
						Imported = true,
						InfoUrl = string.Format("http://realtor.ca/Disclaimer.aspx?Mode=5&id={0}", item.MlsId)
					};
				// todo: what's default listing lifetime?!

				using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
				{
					using (var dao = new ViewOrderDao(session)) dao.Create(vo);

					suite.CurrentPrice = new Money(Convert.ToDecimal(item.CurrentPrice), Currency.Cad); // TODO: Currently locked to CAD
					using (var manager = new SiteManager(ClientSession.MakeSystemSession(session)))
					{
						manager.LogNewSuitePrice(suite, (float)item.CurrentPrice);
					}

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
					using (var dao = new SuiteDao(session)) dao.SafeUpdate(suite);

					tran.Commit();
					add++;
				}
				ServiceInstances.Logger.Info("Imported MLS#{0} for {1} ({2}); VOID={3}",
					item.MlsId, ed.Name, item.CompiledAddress, vo.AutoID);
			}

		    if (0 == procesedEds.Count)
		    {
			    ServiceInstances.Logger.Info("MLS#{0} Lists an unknown address: {1}; shall not add View Order.",
			                                 item.MlsId, item.CompiledAddress);
			    skp++; // not a known address
		    }
	    }

		private User GetImportOwnerUser(EstateDeveloper ownerEd, ISession session, ref StringBuilder issues)
		{
			User result;

			if (!_importersList.TryGetValue(ownerEd, out result))
			{
				var matchNName = ServiceInstances.Configuration.GetValue("MlsImportUserNickNameOwner", "mlsImport");

				IList<User> ownerUsers;
				using (var dao = new UserDao(session))
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

		private static User GetImportOwnerUser(ISession session, ref StringBuilder issues)
	    {
		    User result = null;
		    EstateDeveloper ownerEd;

		    var matchEdName = ServiceInstances.Configuration.GetValue("MlsImportEstateDeveloperOwner", "Resale");
		    using (var dao = new EstateDeveloperDao(session))
			    ownerEd = dao.GetById(matchEdName);

		    if (ownerEd != null)
		    {
			    var matchNName = ServiceInstances.Configuration.GetValue("MlsImportUserNuckNameOwner", "mlsImport");
			    IList<User> ownerUsers;
			    using (var dao = new UserDao(session))
				    ownerUsers = dao.ListUsers(User.Role.DeveloperAdmin, ownerEd.AutoID, null, false);

			    foreach (var user in ownerUsers)
				    if (matchNName.Equals(user.NickName))
				    {
					    result = user;
					    break;
				    }

			    if (null == result)
			    {
				    ServiceInstances.Logger.Error("MLS Import User Nickname ({0}) not found; auto-import disabled.", matchNName);
					issues.AppendFormat("MLS Import User Nickname ({0}) not found; auto-import disabled.\r\n", matchNName);
			    }
			    else
			    {
					ServiceInstances.Logger.Info("MLS Import using user {0}", result);
				}
		    }
		    else
		    {
			    ServiceInstances.Logger.Error("MLS Import Estate Developer ({0}) not found; auto-import disabled.", matchEdName);
				issues.AppendFormat("MLS Import Estate Developer ({0}) not found; auto-import disabled.\r\n", matchEdName);
		    }

		    return result;
	    }

	    private static void CloseRemovedMlsListings(ICollection<string> activeItems)
        {
            int adj = 0, skp = 0, err = 0;

            ServiceInstances.Logger.Info("Got {0} active items.", activeItems.Count);

            using (var session = NHibernateHelper.GetSession())
            {
                DatabaseSettingsDao.VerifyDatabase();

                IList<KeyValuePair<Guid, string>> voIds;
                using (var vodao = new ViewOrderDao(session))
                    // TODO: MLS provider filter injection point
                    voIds = vodao.GetAllActiveIdsAndMlsId();

                foreach (var voInfo in voIds)
                {
	                if (activeItems.Contains(voInfo.Value))
	                {
		                skp++;
		                continue;
	                }
	                try
	                {
		                using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
		                {
			                ViewOrder vo;
			                using (var vodao = new ViewOrderDao(session))
				                vo = vodao.GetById(voInfo.Key);

			                ServiceInstances.Logger.Info("VOID={0} ({1}) target listing appears removed; disposing off ViewOrder.",
			                                             voInfo.Key, NotifyExpiringViewOrders.GetSubjectAddress(session, vo));

			                vo.MarkDeleted();
			                session.Update(vo);

			                ResetViewOrderTargetState(vo, session);

			                using (var dao = new ReverseRequestDao(session))
			                {
				                var rr = dao.GetBySubjectAndType(vo.AutoID.ToString(), ReverseRequest.RequestType.ViewOrderControl);
				                if (rr != null) dao.Delete(rr);
			                }

			                adj++;
			                tran.Commit();
		                }
	                }
	                catch (Exception ex)
	                {
		                ServiceInstances.Logger.Error("VOID={0} processing error: {1}",
		                                              voInfo.Key, ex);
		                err++;
	                }
                }
            }
            ServiceInstances.Logger.Info("Removal completed; {0} ViewOrders closed (deleted); {1} skipped; {2} errors.",
                adj, skp, err);
        }

        private static void ResetViewOrderTargetState(ViewOrder viewOrder, ISession session)
        {
            var result = false;

            switch (viewOrder.TargetObjectType)
            {
                case ViewOrder.SubjectType.Suite:
                    using (var dao = new SuiteDao(session))
                    {
                        var s = dao.GetById(viewOrder.TargetObjectId);
                        switch (viewOrder.Product)
                        {
                            case ViewOrder.ViewOrderProduct.PublicListing:
                                if (s.Status == Suite.SalesStatus.ResaleAvailable)
                                {
                                    s.Status = Suite.SalesStatus.Sold;
                                    result = true;
                                }
                                break;

                            case ViewOrder.ViewOrderProduct.PrivateListing:
                                // NO-OP
                                break;
                        }
                        if (result) dao.SafeUpdate(s);
                    }
                    break;

                case ViewOrder.SubjectType.Building:
                    // TODO: ???
                    break;
            }
        }

    }
}