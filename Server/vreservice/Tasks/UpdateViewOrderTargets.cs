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
			StringBuilder report = new StringBuilder();
			string salesNotifications;
			string issues;
            
			// STEP 1
			//
            // TODO: MLS provider injection point
            prov = new RetsMlsStatusProvider();

            prov.Configure(Configuration.Mls.Treb.Status.ConfigString.Value);
            ServiceInstances.Logger.Info("Running MLS retrieval...");
            prov.Run();  // SLOW!!!
			issues = prov.Parse();
			if (issues.Length > 0) report.AppendFormat("\r\nMLS Status Retrieval problems:\r\n{0}", issues);

	        var activeItems = prov.GetCurrentActiveItems();
            CloseRemovedMlsListings(activeItems);

			// STEP 2
			//
            // TODO: MLS provider injection point
            prov = new RetsMlsInfoProvider();

            prov.Configure(Configuration.Mls.Treb.Info.ConfigString.Value);
            ServiceInstances.Logger.Info("Running MLS retrieval...");
            prov.Run();  // SLOW!!!
			issues = prov.Parse();
			if (issues.Length > 0) report.AppendFormat("\r\nMLS Info Retrieval problems:\r\n{0}", issues);

			using (var session = ClientSession.MakeSystemSession())
			{
				session.Resume();
				DatabaseSettingsDao.VerifyDatabase();
				using (var manager = new SiteManager(session))
					issues = manager.ImportUpdateExistingViewOrders(activeItems, prov.GetNewItems(), false, out salesNotifications);
			}
			if (issues.Length > 0) report.AppendFormat("\r\nMLS Import problems:\r\n{0}", issues);

			if (report.Length > 0)
				SendAdministrativeAlert(null, report.ToString());

			if (!string.IsNullOrEmpty(salesNotifications))
				SendSalesAlert("MLS Importing Issues", salesNotifications);
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