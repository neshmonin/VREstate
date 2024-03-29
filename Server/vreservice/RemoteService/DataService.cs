﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.RemoteService
{
    internal class DataService
    {
		// TODO: make this variable
		const double defaultProximityQuadradiusM = 1000.0;
		
		private static IPRangeFilter _debugClientFilter = null;

        public const string ServicePathPrefix = ServicePathElement0 + "/";
        private const string ServicePathElement0 = "data";

        enum ModelObject { User, EstateDeveloper, Site, Building, Suite, 
			SuiteType, ViewOrder, View, FinancialTransaction, Inventory, 
			NamedSearchFilter, Brokerage, PricingPolicy, Billing, Invoice }

		static DataService()
		{
			configure();
			Configuration.OnModified += new EventHandler((s, e) => configure());
		}

        private static void configure()
        {
			var filter = new IPRangeFilter();
			filter.AddIncludeRanges(Configuration.Debug.DebugClientIpRangeInclude.Value);
			filter.AddExcludeRanges(Configuration.Debug.DebugClientIpRangeExclude.Value);

			_debugClientFilter = filter;
        }

        public static void ProcessGetRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;
            string strObjectId;
            bool includeDeleted;
            ChangeSubscriptionRequest csrq;

            if (!Configuration.Security.AllowSensitiveDataOverNonSecureConnection.Value
				&& !request.UserInfo.Session.TrustedConnection) 
                throw new PermissionException("Service available only over secure connection.");

            getPathElements(request.Request.Path, out mo, out objectId, out strObjectId);

			request.Response.ClientCaching = false;  // no data read requests should be cached

            csrq = retrieveChangeSubscriptionRequest(request.Request.Query);
            includeDeleted = request.Request.Query.GetParam("withdeleted", false);

            if (includeDeleted) RolePermissionCheck.CheckReadDeletedObjects(request.UserInfo.Session);

			ServiceInstances.Logger.Debug("R={0}", request.Request.Referer);

            switch (mo)
            {
                case ModelObject.EstateDeveloper:
                    if (-1 == objectId)
                    {
                        getDeveloperList(request.UserInfo.Session, request.Response, includeDeleted);
                    }
                    else
                    {
                        // TODO: get a single estate developer data
                        throw new NotImplementedException();
                    }
                    return;

                case ModelObject.Site:
                    if (-1 == objectId)
                    {
                        int estateDeveloperId = ResolveDeveloperId(request.UserInfo.Session.DbSession, request.Request.Query["ed"]);
                        if (-1 == estateDeveloperId) throw new ArgumentException("Developer ID is missing.");
                        getSiteList(request.UserInfo.Session, estateDeveloperId, request.Response, includeDeleted);
                    }
                    else
                    {
                        getBuilding(request.UserInfo.Session, objectId, request.Response, csrq);
                    }
                    return;

                case ModelObject.Building:
                    if (-1 == objectId)
                    {
                        getBuildingList(request.UserInfo.Session, request.Request.Query, request.Response, csrq, includeDeleted);
                    }
                    else
                    {
                        getBuilding(request.UserInfo.Session, objectId, request.Response, csrq);
                    }
                    return;

                case ModelObject.Suite:
                    if (-1 == objectId)
                    {
						int buildingId = request.Request.Query.GetParam("buildingid", -1);
						if (-1 == buildingId) buildingId = request.Request.Query.GetParam("building", -1);  // OBSOLETE URI
                        if (-1 == buildingId) throw new ArgumentException("Building ID is missing.");

                        Suite.SalesStatus filter;
						string filterStr = request.Request.Query.GetParam("statusfilter", string.Empty);
						if (string.IsNullOrEmpty(filterStr)) filterStr = request.Request.Query.GetParam("statusFilter", "");  // OBSOLETE URI
						// TODO: Missing 'includeDeleted' option!
                        if (Enum.TryParse<Suite.SalesStatus>(filterStr, true, out filter))
                            getSuiteList(request.UserInfo.Session, buildingId, request.Response, csrq, filter);
                        else
                            getSuiteList(request.UserInfo.Session, buildingId, request.Response, csrq, null);
                    }
                    else
                    {
                        getSuite(request.UserInfo.Session, objectId, request.Response, csrq);
                    }
                    return;

                case ModelObject.SuiteType:
                    if (-1 == objectId)
                    {                        
                        int siteId = request.Request.Query.GetParam("siteid", -1);
						if (-1 == siteId) siteId = request.Request.Query.GetParam("site", -1);  // OBSOLETE URI
                        if (-1 == siteId) throw new ArgumentException("Site ID is missing.");
						// TODO: Missing 'includeDeleted' option!
						getSuiteTypeList(request.UserInfo.Session, siteId, request.Response);
                    }
                    else
                    {
                        // TODO: get a single suite type data
                        throw new NotImplementedException();
                    }
                    return;

                case ModelObject.User:
                    if (-1 == objectId)
                    {
                        getUserList(request.UserInfo.Session, request.Request.Query, request.Response, includeDeleted);
                    }
                    else if (0 == objectId)
                    {
                        getUser(request.UserInfo.Session, request.UserInfo.Session.User.AutoID, request.Response);
                    }
                    else
                    {
                        getUser(request.UserInfo.Session, objectId, request.Response);
                    }
                    return;

                case ModelObject.ViewOrder:
                    if (null == strObjectId)
                    {
                        if (-1 == objectId)
                            getViewOrderList(request.UserInfo.Session, request.Request.Query, request.Response, includeDeleted);
                        else
                            throw new NotImplementedException();
                    }
                    else
                    {
                        getViewOrder(request.UserInfo.Session, request.Request.Query, strObjectId, request.Response);
                    }
                    return;

				case ModelObject.View:
                    getView(request, csrq);
                    return;

                case ModelObject.FinancialTransaction:
                    if (-1 == objectId)
                    {
                        getFinancialTransactionList(request.UserInfo.Session, request.Request.Query, request.Response);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    return;

                case ModelObject.Inventory:
                    if (-1 == objectId)
                    {
                        int buildingId = request.Request.Query.GetParam("buildingid", -1);
						if (-1 == buildingId) buildingId = request.Request.Query.GetParam("building", -1);  // OBSOLETE URI
						string mlsId = request.Request.Query.GetParam("mlsid", string.Empty);
						if (string.IsNullOrEmpty(mlsId)) mlsId = request.Request.Query.GetParam("mlsId", string.Empty);  // OBSOLETE URI
						if (buildingId > 0)
						{
							getInventoryList(request.UserInfo.Session, buildingId, csrq, request.Response);
						}
						else if (!string.IsNullOrEmpty(mlsId))
						{
							getInventory(request.UserInfo.Session, mlsId, csrq, request.Response);
						}
                        else throw new ArgumentException("Entity ID is missing.");
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    return;

				case ModelObject.NamedSearchFilter:
					if (-1 == objectId)
					{
						getNamedSearchFilterList(request.UserInfo.Session, request.Request.Query, request.Response, includeDeleted);
					}
					else
					{
						getNamedSearchFilter(request.UserInfo.Session, objectId, request.Response);
					}
					return;

				case ModelObject.Brokerage:
					if (-1 == objectId)
					{
						getBrokerageList(request.UserInfo.Session, request.Request.Query, request.Response, includeDeleted);
					}
					else
					{
						getBrokerage(request.UserInfo.Session, objectId, request.Response);
					}
					return;

				case ModelObject.PricingPolicy:
					if (-1 == objectId)
					{
						getPricingPolicyList(request.UserInfo.Session, request.Request.Query, request.Response, includeDeleted);
					}
					else
					{
						getPricingPolicy(request.UserInfo.Session, objectId, request.Response);
					}
					return;

				case ModelObject.Billing:
					getBilling(request.UserInfo.Session, request.Request.Query, request.Response);
					return;

				case ModelObject.Invoice:
					getInvoiceList(request.UserInfo.Session, request.Request.Query, request.Response);
					return;
			}

            throw new NotImplementedException();
        }

        public static void ProcessReplaceRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;
            string strObjectId;

            if (!request.UserInfo.Session.TrustedConnection) throw new PermissionException("Service available only over secure connection.");

            getPathElements(request.Request.Path, out mo, out objectId, out strObjectId);

            if (null == request.Request.Data) throw new ArgumentException("Object data not passed.");

            switch (mo)
            {
                case ModelObject.Building:
                    if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
                    updateBuilding(request.UserInfo.Session, objectId, request.Request.Data, request.Response);
                    return;

				case ModelObject.Suite:
                    if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
                    updateSuite(request.UserInfo.Session, objectId, request.Request.Data, request.Response);
					return;

                case ModelObject.User:
                    if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
                    updateUser(request.UserInfo.Session, objectId, request.Request.Data, request.Response);
                    return;

                case ModelObject.ViewOrder:
                    if (null == strObjectId) throw new ArgumentException("Object ID is missing.");
                    updateViewOrder(request.UserInfo.Session, request.Request.Query, strObjectId, request.Request.Data, request.Response);
                    return;

				case ModelObject.NamedSearchFilter:
					if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
					updateNamedSearchFilter(request.UserInfo.Session, objectId, request.Request.Data, request.Response);
					return;

				case ModelObject.Brokerage:
					if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
					updateBrokerage(request.UserInfo.Session, objectId, request.Request.Data, request.Response);
					return;

				case ModelObject.PricingPolicy:
					if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
					updatePricingPolicy(request.UserInfo.Session, objectId, request.Request.Data, request.Response);
					return;
			}

            throw new NotImplementedException();
        }

        public static void ProcessCreateRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;
            string strObjectId;

            if (!request.UserInfo.Session.TrustedConnection) throw new PermissionException("Service available only over secure connection.");

            getPathElements(request.Request.Path, out mo, out objectId, out strObjectId);

            if (null == request.Request.Data) throw new ArgumentException("Object data not passed.");

            switch (mo)
            {
                case ModelObject.User:
                    createUser(request.UserInfo.Session, request.Request.Data, request.Response);
                    return;

                //case ModelObject.ViewOrder:
                //    createListing(request.UserInfo.Session, request.Request.Data, request.Response);
                //    return;

				case ModelObject.NamedSearchFilter:
					createNamedSearchFilter(request.UserInfo.Session, request.Request.Data, request.Response);
                    return;

				case ModelObject.Brokerage:
					createBrokerage(request.UserInfo.Session, request.Request.Data, request.Response);
					return;

				case ModelObject.PricingPolicy:
					createPricingPolicy(request.UserInfo.Session, request.Request.Data, request.Response);
					return;
			}

            throw new NotImplementedException();
        }

        public static void ProcessDeleteRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;
            string strObjectId;

            if (!request.UserInfo.Session.TrustedConnection) throw new PermissionException("Service available only over secure connection.");

            getPathElements(request.Request.Path, out mo, out objectId, out strObjectId);

            switch (mo)
            {
                case ModelObject.User:
                    if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
                    deleteUser(request.UserInfo.Session, objectId, request.Response);
                    return;

                case ModelObject.ViewOrder:
                    if (null == strObjectId) throw new ArgumentException("Object ID is missing.");
                    deleteViewOrder(request.UserInfo.Session, strObjectId, request.Response);
                    return;

				case ModelObject.Building:
					if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
					deleteBuilding(request.UserInfo.Session, objectId, request.Response);
					return;

				case ModelObject.NamedSearchFilter:
					if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
					deleteNamedSearchFilter(request.UserInfo.Session, objectId, request.Response);
					return;

				case ModelObject.Brokerage:
					if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
					deleteBrokerage(request.UserInfo.Session, objectId, request.Response);
					return;

				case ModelObject.PricingPolicy:
					if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
					deletePricingPolicy(request.UserInfo.Session, objectId, request.Response);
					return;
			}

            throw new NotImplementedException();
        }

        private static void getPathElements(string path, out ModelObject mo, out int id, out string strId)
        {
            string[] elements = path.Split('/');
            if ((elements.Length < 2) || (elements.Length > 3)) throw new ArgumentException("Object path is invalid (0).");

            if (!elements[0].Equals(ServicePathElement0)) throw new ArgumentException("Object path is invalid (1).");

            if (elements[1].Equals("ed")) mo = ModelObject.EstateDeveloper;
            else if (elements[1].Equals("site")) mo = ModelObject.Site;
            else if (elements[1].Equals("building")) mo = ModelObject.Building;
            else if (elements[1].Equals("suite")) mo = ModelObject.Suite;
            else if (elements[1].Equals("user")) mo = ModelObject.User;
            else if (elements[1].Equals("suitetype")) mo = ModelObject.SuiteType;
            else if (elements[1].Equals("viewOrder")) mo = ModelObject.ViewOrder;  // OBSOLETE URI
			else if (elements[1].Equals("vieworder")) mo = ModelObject.ViewOrder;
			else if (elements[1].Equals("view")) mo = ModelObject.View;
            else if (elements[1].Equals("ft")) mo = ModelObject.FinancialTransaction;
            else if (elements[1].Equals("inventory")) mo = ModelObject.Inventory;
			else if (elements[1].Equals("nsf")) mo = ModelObject.NamedSearchFilter;
			else if (elements[1].Equals("brokerage")) mo = ModelObject.Brokerage;
			else if (elements[1].Equals("pp")) mo = ModelObject.PricingPolicy;
			else if (elements[1].Equals("bill")) mo = ModelObject.Billing;
			else if (elements[1].Equals("invoice")) mo = ModelObject.Invoice;
			else throw new ArgumentException("Object path is invalid (2).");

            strId = null;

            if (2 == elements.Length)
            {
                id = -1;
            }
            else
            {
                if (mo != ModelObject.ViewOrder)
                {
                    if (!int.TryParse(elements[2], out id)) throw new ArgumentException("Object path is invalid (3).");
                }
                else
                {
                    id = -1;
                    strId = elements[2];
                }
            }
        }

        #region retrieval
        private static void getDeveloperList(ClientSession session, IResponseData resp, bool includeDeleted)
        {
            ClientData[] result = null;

            //using (UserManager manager = new UserManager(session))
            using (DeveloperManager manager = new DeveloperManager(session))
            {
                EstateDeveloper[] developerList = manager.List(includeDeleted);

                int cnt = developerList.Length;
                result = new ClientData[cnt];
                for (int idx = 0; idx < cnt; idx++)
                {
                    EstateDeveloper ed = developerList[idx];
                    result[idx] = ed.GetClientData();
                }
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Add("developers", result);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getSiteList(ClientSession session, int developerId, IResponseData resp, bool includeDeleted)
        {
            ClientData[] result = null;

            using (SiteManager manager = new SiteManager(session))
            {
                Site[] siteList = manager.List(developerId, includeDeleted);

                int cnt = siteList.Length;
                result = new ClientData[cnt];
                for (int idx = 0; idx < cnt; idx++)
                {
                    Site s = siteList[idx];
                    ReferencedFileHelper.ConvertUrlsToAbsolute(s);
                    result[idx] = s.GetClientData();
                }
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Add("sites", result);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getBuildingList(ClientSession session, ServiceQuery query, IResponseData resp, 
            ChangeSubscriptionRequest csrq, bool includeDeleted)
        {
            IList<Building> toReturn = null;
            ClientData[] result = null;

            string scopeType = query.GetParam("scopetype", "site");
			if (string.IsNullOrEmpty(scopeType)) scopeType = query.GetParam("scopeType", "site");  // OBSOLETE URI

            if (scopeType.Equals("address"))
            {
                int devLock = -1;
                string edId = query["ed"];
                if (edId != null)
                {
                    devLock = ResolveDeveloperId(session.DbSession, edId);
                    if (devLock < 0) throw new ArgumentException("Estate Developer ID is invalid");
                }

                toReturn = new List<Building>();
                foreach (UpdateableBase u in AddressHelper.ParseGeographicalAddressToModel(query, session.DbSession, true))
                {
                    Building b = u as Building;
                    if ((b != null) && ((devLock < 0) || (b.ConstructionSite.Developer.AutoID.Equals(devLock)))) 
                        toReturn.Add(b);
                }
            }
            else if (scopeType.Equals("site"))
            {
                int siteId = query.GetParam("site", -1);
                if (-1 == siteId) throw new ArgumentException("Site ID is missing.");

                using (SiteManager manager = new SiteManager(session))
                    toReturn = manager.ListBuildings(siteId, includeDeleted);
            }
            else throw new NotImplementedException("Unknown scope type");

            if (toReturn != null)
            {
                int cnt = toReturn.Count;
                result = new ClientData[cnt];
                for (int idx = 0; idx < cnt; idx++)
                {
                    Building b = toReturn[idx];
                    ReferencedFileHelper.ConvertUrlsToAbsolute(b);
                    result[idx] = b.GetClientData();
                }

                if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, toReturn, new Suite[0], csrq);
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Add("buildings", result);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getBuilding(ClientSession session, int buildingId, IResponseData resp, ChangeSubscriptionRequest csrq)
        {
            ClientData result = null;

            using (SiteManager manager = new SiteManager(session))
            {
                Building building = manager.GetBuildingById(buildingId);

                ReferencedFileHelper.ConvertUrlsToAbsolute(building);
                result = building.GetClientData();

                if (csrq != ChangeSubscriptionRequest.None)
                    setChangeSubscription(session, building, csrq);
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Merge(result);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getSuiteList(ClientSession session, int buildingId, IResponseData resp, 
            ChangeSubscriptionRequest csrq, Suite.SalesStatus? statusFilter)
        {
            List<ClientData> result = null;

            using (SiteManager manager = new SiteManager(session))
            {
                Suite[] suiteList = manager.ListSuitesByBuiding(buildingId);

                int cnt = suiteList.Length;
                result = new List<ClientData>(cnt);
                for (int idx = 0; idx < cnt; idx++)
                {
                    Suite s = suiteList[idx];
                    insertSuiteIntoResult(statusFilter, result, s);
                }

                if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, suiteList, csrq);
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Add("suites", NHibernateHelper.IListToArray<ClientData>(result));
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void insertSuiteIntoResult(Suite.SalesStatus? statusFilter, List<ClientData> result, Suite s)
        {
            bool add = false;
            if (statusFilter != null)
            {
                if (s.Status.Equals(statusFilter.Value)) add = true;
            }
            else
            {
                add = true;
            }
            if (add)
            {
                result.Add(s.GetClientData());
            }
        }

        private static void getSuite(ClientSession session, int suiteId, IResponseData resp, ChangeSubscriptionRequest csrq)
        {
            ClientData result = null;

            using (var dao = new SuiteDao(session.DbSession))
            {
                var suite = dao.GetById(suiteId);

	            result = suite.GetClientData();

                if (csrq != ChangeSubscriptionRequest.None)
                    setChangeSubscription(session, suite, csrq);
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Merge(result);
            resp.ResponseCode = HttpStatusCode.OK;
        }

		//private static void getSuiteType(ClientSession session, int siteId, string name, IResponseData resp)
		//{
		//    SuiteType st;

		//    using (SiteManager manager = new SiteManager(session))
		//    {
		//        st = manager.GetSuiteTypeByName(siteId, name);
		//    }

		//    ServiceInstances.ModelCache.FillWithModelInfo(st, false);
		//    ReferencedFileHelper.ConvertUrlsToAbsolute(st);

		//    // produce output
		//    //
		//    resp.Data = st.GetClientData();
		//    resp.ResponseCode = HttpStatusCode.OK;
		//}

        private static void getSuiteTypeList(ClientSession session, int siteId, IResponseData resp)
        {
            SuiteType[] list;

            using (SiteManager manager = new SiteManager(session))
            {
                list = manager.ListSuiteTypes(siteId);
            }

            foreach (SuiteType st in list)
            {
                ReferencedFileHelper.ConvertUrlsToAbsolute(st);
            }

            // produce output
            //
            int cnt = list.Length;
            ClientData[] result = new ClientData[cnt];
            for (int idx = 0; idx < cnt; idx++) result[idx] = list[idx].GetClientData();

            resp.Data = new ClientData();
            resp.Data.Add("suiteTypes", result);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getUser(ClientSession session, int userId, IResponseData resp)
        {
            User user;
			
            using (var manager = new UserManager(session)) user = manager.Get(userId);
            
            ClientData result = user.GetClientData();

			if (RolePermissionCheck.GenericUserControlTest(session, user, 
				RolePermissionCheck.UserInfoAccessLevel.Transactional))
				result.Add("creditUnits", user.CreditUnits);

            using (IAuthentication auth = new Authentication(session.DbSession))
                fillInLoginInfo(ref result, ref user, auth);

            resp.Data = result;
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getUserList(ClientSession session, ServiceQuery query, IResponseData resp, bool includeDeleted)
        {
            ClientData[] result;

            if (query.GetParam("sellerMode", false))
            {
                User[] list = session.User.CanView.ToArray();

                int cnt = list.Length;
                result = new ClientData[cnt];
                for (int idx = 0; idx < cnt; idx++)
                    result[idx] = list[idx].GetClientData();
            }
            else
            {
                User.Role role;
                if (!Enum.TryParse<User.Role>(query.GetParam("role", "buyer"), true, out role)) role = User.Role.Buyer;
                int estateDeveloperId = ResolveDeveloperId(session.DbSession, query["ed"]);// data.GetProperty("ed", -1);
                string nameLookup = query.GetParam("namefilter", string.Empty);
				if (string.IsNullOrEmpty(nameLookup)) nameLookup = query.GetParam("nameFilter", string.Empty);// data.GetProperty("nameFilter", string.Empty);  // OBSOLETE URI
				int brokerageId = query.GetParam("brokerageid", -1);
                User[] list;

				if (User.IsEstateDeveloperTied(role) && (estateDeveloperId < 0) && session.User.EstateDeveloperID.HasValue)
					estateDeveloperId = session.User.EstateDeveloperID.Value;

                using (UserManager manager = new UserManager(session))
                    list = manager.List(role, estateDeveloperId, brokerageId, nameLookup, includeDeleted);

                // produce output
                //
                int cnt = list.Length;
                result = new ClientData[cnt];
                using (IAuthentication auth = new Authentication(session.DbSession))
                {
                    for (int idx = 0; idx < cnt; idx++)
                    {
                        result[idx] = list[idx].GetClientData();

						if (RolePermissionCheck.GenericUserControlTest(session, list[idx],
							RolePermissionCheck.UserInfoAccessLevel.Transactional))
							result[idx].Add("creditUnits", list[idx].CreditUnits);

						fillInLoginInfo(ref result[idx], ref list[idx], auth);
                    }
                }
            }

            resp.Data = new ClientData();
            resp.Data.Add("users", result);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void fillInLoginInfo(ref ClientData result, ref User user, IAuthentication auth)
        {
            LoginType lt;
            User.Role ur;
            int ed;
            string login;
            if (auth.LoginByUserId(user.AutoID, out lt, out ur, out ed, out login))
            {
                result.Add("loginType", lt);
                result.Add("login", login);
            }
        }

		private static void getNamedSearchFilterList(ClientSession session, ServiceQuery query, IResponseData resp,
			bool includeDeleted)
		{
			IList<NamedSearchFilter> toReturn = null;
			ClientData[] result = null;

			int ownerId = query.GetParam("ownerid", session.User.AutoID);
			if ((ownerId == session.User.AutoID) && query.Contains("ownerId")) ownerId = query.GetParam("ownerId", session.User.AutoID); // OBSOLETE URI

			using (var dao = new NamedSearchFilterDao(session.DbSession))
				toReturn = dao.Get(ownerId, includeDeleted);

			int cnt = toReturn.Count;
			result = new ClientData[cnt];
			for (int idx = 0; idx < cnt; idx++)
			{
				var nsf = toReturn[idx];
				RolePermissionCheck.CheckReadNamedSearchFilter(session, nsf);
				result[idx] = nsf.GetClientData();
			}

			// produce output
			//
			resp.Data = new ClientData();
			if (result != null) resp.Data.Add("filters", result);
			resp.ResponseCode = HttpStatusCode.OK;
		}

		private static void getNamedSearchFilter(ClientSession session, int filterId, IResponseData resp)
		{
			NamedSearchFilter toReturn = null;

			using (var dao = new NamedSearchFilterDao(session.DbSession))
				toReturn = dao.GetById(filterId);

			RolePermissionCheck.CheckReadNamedSearchFilter(session, toReturn);

			resp.Data = toReturn.GetClientData();
			resp.ResponseCode = HttpStatusCode.OK;
		}

		private static void getInventoryList(ClientSession session, int buildingId, ChangeSubscriptionRequest csrq, IResponseData resp)
        {
            Building b;

            using (BuildingDao dao = new BuildingDao(session.DbSession))
                b = dao.GetById(buildingId);
            if (null == b) throw new FileNotFoundException("Building does not exist");

			IList<ViewOrder> relatedVos;
			using (var dao = new ViewOrderDao(session.DbSession))
				relatedVos = dao.GetActiveByBuilding(b, true);

            List<ClientData> result = new List<ClientData>(b.Suites.Count);
            foreach (Suite s in b.Suites)
            {
                if ((s.SuiteType != null)
                    // a bit of too deep inspection to avoid unnecessary calls; maybe not required
                    //&& (!string.IsNullOrWhiteSpace(s.SuiteType.FloorPlanUrl)) && (!s.SuiteType.FloorPlanUrl.StartsWith("http://"))
                    )
                    ReferencedFileHelper.ConvertUrlsToAbsolute(s.SuiteType);

				var cd = s.GetInventoryClientData(null, false);
	
				var vo = relatedVos.FirstOrDefault(v => v.TargetObjectId == s.AutoID);
				if (vo != null) vo.GetInventoryClientData(cd, true);

				result.Add(cd);
            }

            if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, b.Suites, csrq);

            resp.Data = new ClientData();
            resp.Data.Add("inventory", result);
            resp.ResponseCode = HttpStatusCode.OK;
        }

		private static void getInventory(ClientSession session, string mlsId, ChangeSubscriptionRequest csrq, IResponseData resp)
		{
			Suite s;
			ViewOrder vo;

			using (var dao = new ViewOrderDao(session.DbSession))
				vo = dao.GetByImportedMlsId(mlsId).FirstOrDefault(v => v.TargetObjectType == ViewOrder.SubjectType.Suite);
			if (null == vo) throw new FileNotFoundException("Unknown MLS number.");

			using (var dao = new SuiteDao(session.DbSession))
				s = dao.GetById(vo.TargetObjectId);
			if (null == s) throw new FileNotFoundException("Internal error #D83C952A");

			var cd = s.GetInventoryClientData(null, false);
			vo.GetInventoryClientData(cd, true);

			cd.Add("address", AddressHelper.ConvertToReadableAddress(s.Building, s));

            ClientData[] result = new ClientData[1];
			result[0] = cd;

			if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, s, csrq);

			resp.Data = new ClientData();
			resp.Data.Add("inventory", result);
			resp.ResponseCode = HttpStatusCode.OK;
		}

		private static void getFinancialTransactionList(ClientSession session, ServiceQuery query, IResponseData resp)
        {
            ClientData[] result;

            int userId = query.GetParam("userid", -1);
			if (-1 == userId) userId = query.GetParam("userId", -1);  // OBSOLETE URI
            User user;
            FinancialTransaction[] list;

            // TODO: implement paging
            query.GetParam("pg_startidx", -1);
            query.GetParam("pg_maxcount", -1);

            using (UserDao dao = new UserDao(session.DbSession))
                user = dao.GetById(userId);

            if (null == user) throw new FileNotFoundException("User does not exist");

            RolePermissionCheck.CheckUserAccess(session, user, RolePermissionCheck.UserInfoAccessLevel.Transactional);

            using (FinancialTransactionDao dao = new FinancialTransactionDao(session.DbSession))
                list = dao.Get(FinancialTransaction.AccountType.User, userId);

            // produce output
            //
            int cnt = list.Length;
            result = new ClientData[cnt];
            for (int idx = 0; idx < cnt; idx++)
                result[idx] = list[idx].GetClientData();

            resp.Data = new ClientData();
            resp.Data.Add("transactions", result);
            resp.Data.Add("totalCount", result.Count());
            resp.ResponseCode = HttpStatusCode.OK;
        }

		private static void getViewOrder(ClientSession session, ServiceQuery query, string strObjectId, IResponseData resp)
        {
            ViewOrder viewOrder = RetrieveViewOrder(session.DbSession, strObjectId, true);

            resp.Data = convertViewOrderdata(session.DbSession, viewOrder, query.GetParam("verbose", false));

            resp.ResponseCode = HttpStatusCode.OK;
        }

        internal static ClientData convertViewOrderdata(ISession session, ViewOrder viewOrder, bool verbose)
        {
            viewOrder.ViewOrderURL = ReverseRequestService.GenerateUrl(viewOrder);
            ClientData result = viewOrder.GetClientData();

            if (verbose)
            {
				string label = retrieveViewOrderAddress(session, viewOrder);
                if (label != null) result.Add("label", label);
            }

            return result;
        }

		private static string retrieveViewOrderAddress(ISession session, ViewOrder viewOrder)
		{
			string label = null;
			switch (viewOrder.TargetObjectType)
			{
				case ViewOrder.SubjectType.Building:
					{
						Building b;
						using (BuildingDao dao = new BuildingDao(session))
							b = dao.GetById(viewOrder.TargetObjectId);
						label = AddressHelper.ConvertToReadableAddress(b, null);
					}
					break;

				case ViewOrder.SubjectType.Suite:
					{
						Suite s;
						using (SuiteDao dao = new SuiteDao(session))
							s = dao.GetById(viewOrder.TargetObjectId);
						label = AddressHelper.ConvertToReadableAddress(s.Building, s);
					}
					break;
			}
			return label;
		}

        private static void getViewOrderList(ClientSession session, ServiceQuery query, IResponseData resp, bool includeDeleted)
        {
            int userId = query.GetParam("userid", -1);
			if (-1 == userId) userId = query.GetParam("userId", -1);  // OBSOLETE URI
            User user;
            ViewOrder[] list;

            using (UserDao dao = new UserDao(session.DbSession))
                user = dao.GetById(userId);

            if (null == user) throw new FileNotFoundException("User does not exist");

            int devLock = -1;
            string edId = query["ed"];
            if (edId != null)
            {
                devLock = ResolveDeveloperId(session.DbSession, edId);
                if (devLock < 0) throw new ArgumentException("Estate Developer ID is invalid");
            }

            RolePermissionCheck.CheckUserAccess(session, user, RolePermissionCheck.UserInfoAccessLevel.Transactional);

            using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                list = dao.Get(userId, includeDeleted);

            DateTime timeLim = DateTime.MaxValue;

            if ("expired".Equals(query["status"]))
            {
                // filter expired view orders only
                timeLim = DateTime.UtcNow;
            }

            // produce output
            //
            bool verbose = query.GetParam("verbose", false);
            int cnt = list.Length;
            List<ClientData> result = new List<ClientData>(cnt);
            for (int idx = 0; idx < cnt; idx++)
            {
                ViewOrder vo = list[idx];
                if (vo.ExpiresOn < timeLim)
                {
                    if ((devLock >= 0) && (extractDeveloperFromViewOrder(session, vo).AutoID != devLock)) continue;
                    result.Add(convertViewOrderdata(session.DbSession, vo, verbose));
                }
            }

            resp.Data = new ClientData();
            resp.Data.Add("viewOrders", result.ToArray());
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getView(IServiceRequest request, ChangeSubscriptionRequest csrq)
        {
            var type = request.Request.Query.GetParam("type", "vieworder").ToLowerInvariant();

			var vs = new ViewSettings(request.Request.Query, type);

            if (type.Equals("vieworder")) getViewViewOrder(request, vs, csrq);
			else if (type.Equals("site")) getViewSite(request.UserInfo.Session, request.Request.Query, vs, csrq, request.Response);
			else if (type.Equals("building")) getViewBuilding(request.UserInfo.Session, request.Request.Query, vs, csrq, request.Response);
			else if (type.Equals("suite")) getViewSuite(request.UserInfo.Session, request.Request.Query, vs, csrq, request.Response);
			else if (type.Equals("geo")) getViewGeo(request.UserInfo.Session, request.Request.Query, vs, csrq, request.Response);
            else throw new NotImplementedException();
        }

        private static void getViewViewOrder(IServiceRequest request, ViewSettings vs, ChangeSubscriptionRequest csrq)
        {
            ViewOrder viewOrder;
            var viewOrderValid = false;
			var session = request.UserInfo.Session;

            using (var tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
            {
				viewOrder = RetrieveViewOrder(request, false);
				
                viewOrderValid = (viewOrder.Enabled && (viewOrder.ExpiresOn > DateTime.UtcNow));

                if (viewOrderValid)
                {
                    viewOrder.Touch();

                    using (var dao = new ViewOrderDao(session.DbSession))
                        dao.Update(viewOrder);

                    tran.Commit();
                }
            }

            switch (viewOrder.TargetObjectType)
            {
                case ViewOrder.SubjectType.Suite:
                    getViewSuiteViewOrder(session, viewOrder, vs, csrq, request.Response);
                    break;

                case ViewOrder.SubjectType.Building:
					getViewBuildingViewOrder(session, viewOrder, vs, csrq, request.Response);
                    break;

                default:
                    throw new NotImplementedException("7FD8B0");
            }
        }

        private static void getViewBuildingViewOrder(ClientSession session, ViewOrder viewOrder, 
			ViewSettings vs, ChangeSubscriptionRequest csrq, IResponseData resp)
        {
            IList<ViewOrder> viewOrders;
            Building building;

            using (BuildingDao dao = new BuildingDao(session.DbSession))
                building = dao.GetById(viewOrder.TargetObjectId);

            if (null == building) throw new FileNotFoundException("Unknown object listed");

            // TODO
            //if (viewOrder.Product == ViewOrder.ViewOrderProduct.PublicListing)
            //{
            //    using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
            //        viewOrders = dao.GetActiveSameBuilding(ViewOrder.ViewOrderProduct.PublicListing, suite);

            //    List<int> suiteIds = new List<int>(viewOrders.Count);
            //    foreach (ViewOrder vo in viewOrders) suiteIds.Add(vo.TargetObjectId);
            //    using (SuiteDao dao = new SuiteDao(session.DbSession))
            //        suites = dao.GetByIdList(suiteIds);
            //}
            viewOrders = new ViewOrder[1];
            viewOrders[0] = viewOrder;

            //generateViewResponse(session.DbSession, building.Suites, viewOrders, viewOrder.AutoID,
            //    resp, ViewResponseSoldPropertyLevel.Building, false);

            if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, building, csrq);

			var center = building.Location;
            generateViewResponse(session.DbSession,
				StructuresByGeoProximity(session.DbSession, center.Longitude, center.Latitude, defaultProximityQuadradiusM),
				null, new Building[] { building }, null, building.Suites, viewOrders, viewOrder.AutoID,
                resp, vs, false);
        }

        private static void getViewSuiteViewOrder(ClientSession session, ViewOrder viewOrder, 
			ViewSettings vs, ChangeSubscriptionRequest csrq, IResponseData resp)
        {
            IList<Building> buildings = null;
            IList<ViewOrder> viewOrders;
            IList<Suite> suites = null;
            Suite suite;

            using (var dao = new SuiteDao(session.DbSession))
                suite = dao.GetById(viewOrder.TargetObjectId);

            if (null == suite) throw new FileNotFoundException("Unknown object listed");

            if (viewOrder.Product == ViewOrder.ViewOrderProduct.PublicListing)
            {
                var devLock = suite.Building.ConstructionSite.Developer;
                buildings = new List<Building>();
				int cnt = 0;
                foreach (var b in BuildingsByGeoProximity(session.DbSession, suite.Building, defaultProximityQuadradiusM))
                {
                    if (b.ConstructionSite.Developer.Equals(devLock))
                        buildings.Add(b);
					cnt++;
                }

                ServiceInstances.Logger.Debug("dSPVO: got {0} live buildings (total {1} in proximity).", buildings.Count, cnt);

                using (var dao = new ViewOrderDao(session.DbSession))
                    viewOrders = dao.GetActiveInBuildings(ViewOrder.ViewOrderProduct.PublicListing, 
						buildings.ConvertTo(b => b.AutoID).ToArray(), vs.ShowImported);

                List<int> suiteIds = new List<int>(viewOrders.Count);
                foreach (var vo in viewOrders) suiteIds.Add(vo.TargetObjectId);
                using (var dao = new SuiteDao(session.DbSession))
                    suites = dao.GetByIdList(suiteIds);

	            foreach (var s in from b in buildings from s in b.Suites where vs.SuiteStatusMatches(s) select s)
		            if (!suites.Contains(s)) suites.Add(s);

	            if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, buildings, csrq);
            }
            else
            {
                viewOrders = new ViewOrder[1];
                viewOrders[0] = viewOrder;

                suites = new Suite[1];
                suites[0] = suite;

                if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, suites, csrq);
            }

            //generateViewResponse(session.DbSession, suites, viewOrders, viewOrder.AutoID,
            //    resp, ViewResponseSoldPropertyLevel.Suite, true);

			var center = suite.Building.Location;
            generateViewResponse(session.DbSession,
				StructuresByGeoProximity(session.DbSession, center.Longitude, center.Latitude, defaultProximityQuadradiusM),
                null, buildings, null, suites, viewOrders, viewOrder.AutoID,
                resp, vs, false);
        }

        private static void getViewGeo(ClientSession session, ServiceQuery query, 
			ViewSettings vs, ChangeSubscriptionRequest csrq, IResponseData resp)
        {
            string param;
            double cLon, cLat, sqRadM;

            param = query["gsq_lon"];
            if (null == param) throw new ArgumentException("Geo square center longitude missing.");
            if (!double.TryParse(param, out cLon)) throw new ArgumentException();

            param = query["gsq_lat"];
            if (null == param) throw new ArgumentException("Geo square center latitude missing.");
            if (!double.TryParse(param, out cLat)) throw new ArgumentException();

            param = query["gsq_sqrad"];
            if (null == param) throw new ArgumentException("Geo square squradius missing.");
            if (!double.TryParse(param, out sqRadM)) throw new ArgumentException();

            EstateDeveloper devLock = null;
            string voId = query["relatedvieworderid"];
			if (string.IsNullOrEmpty(voId)) voId = query["relatedVoId"];  // OBSOLETE URI
            if (voId != null)
            {
                ViewOrder vo = RetrieveViewOrder(session.DbSession, voId, false);
                if (vo.Enabled && (vo.ExpiresOn > DateTime.UtcNow))
                    devLock = extractDeveloperFromViewOrder(session, vo);
            }

            List<Building> buildings = new List<Building>();
            foreach (var b in BuildingsByGeoProximity(session.DbSession, cLon, cLat, sqRadM))
            {
                if ((null == devLock) || (b.ConstructionSite.Developer.Equals(devLock)))
                    buildings.Add(b);
            }

            var suites = (from b in buildings from s in b.Suites where vs.SuiteStatusMatches(s) select s).ToList();

	        if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, buildings, suites, csrq);

            generateViewResponse(session.DbSession,
				StructuresByGeoProximity(session.DbSession, cLon, cLat, sqRadM),
                null, buildings, null, suites, null, Guid.Empty,
                resp, vs, false);
        }

        private static void getViewSite(ClientSession session, ServiceQuery query, 
			ViewSettings vs, ChangeSubscriptionRequest csrq, IResponseData resp)
        {
            int objectId = query.GetParam("id", -1);
            if (objectId < 0) throw new ArgumentException("Object ID missing.");

            Site site;
            using (SiteDao dao = new SiteDao(session.DbSession))
                site = dao.GetById(objectId);

            if ((null == site) || site.Deleted) throw new FileNotFoundException("Unknown site");

			var suites = (from b in site.Buildings from s in b.Suites where vs.SuiteStatusMatches(s) select s).ToList();

            if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, site.Buildings, suites, csrq);

			var center = site.Location;
            generateViewResponse(session.DbSession,
				StructuresByGeoProximity(session.DbSession, center.Longitude, center.Latitude, defaultProximityQuadradiusM),
				new Site[] { site }, site.Buildings, null, suites, null, Guid.Empty, 
                resp, vs, false);
        }

        private static void getViewBuilding(ClientSession session, ServiceQuery query, 
			ViewSettings vs, ChangeSubscriptionRequest csrq, IResponseData resp)
        {
            int objectId = query.GetParam("id", -1);
            if (objectId < 0) throw new ArgumentException("Object ID missing.");

            Building building;
            using (BuildingDao dao = new BuildingDao(session.DbSession))
                building = dao.GetById(objectId);

            if ((null == building) || building.Deleted) throw new FileNotFoundException("Unknown building");

			var suites = (from s in building.Suites where vs.SuiteStatusMatches(s) select s).ToList();
			
			if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, building, csrq);

			var center = building.Location;
            generateViewResponse(session.DbSession,
				StructuresByGeoProximity(session.DbSession, center.Longitude, center.Latitude, defaultProximityQuadradiusM),
				null, new Building[] { building }, null, suites, null, Guid.Empty,
                resp, vs, true);
        }

        private static void getViewSuite(ClientSession session, ServiceQuery query, 
			ViewSettings vs, ChangeSubscriptionRequest csrq, IResponseData resp)
        {
            int objectId = query.GetParam("id", -1);
            if (objectId < 0) throw new ArgumentException("Object ID missing.");

            Suite suite;
            using (SuiteDao dao = new SuiteDao(session.DbSession))
                suite = dao.GetById(objectId);

            if ((null == suite) || suite.Deleted) throw new FileNotFoundException("Unknown suite");

            if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, suite, csrq);

			var center = suite.Building.Location;
            generateViewResponse(session.DbSession,
				StructuresByGeoProximity(session.DbSession, center.Longitude, center.Latitude, defaultProximityQuadradiusM),
				null, null, null, new Suite[] { suite }, null, Guid.Empty, resp, vs, true);
        }

        private static void generateViewResponse(ISession dbSession,
			ICollection<Suite> suites,
            IResponseData resp,
            ViewSettings vs, bool minimizeOutput)
        {
            generateViewResponse(dbSession, null, null, null, null, suites, null, Guid.Empty, resp, vs, minimizeOutput);
        }

		//private static void generateViewResponse(ISession dbSession,
		//    ICollection<Suite> suites,
		//    ICollection<ViewOrder> viewOrders,
		//    Guid primaryListingId,
		//    IResponseData resp,
		//    ViewSettings vs, bool minimizeOutput)
		//{
		//    generateViewResponse(dbSession, null, null, null, suites, viewOrders, primaryListingId, resp, vs, minimizeOutput);
		//}

        private enum ViewResponseSoldPropertyLevel 
        { 
            /// <summary>
            /// No sold property in output
            /// </summary>
            None, 
            /// <summary>
            /// Sold buildings in output; no sold suites
            /// </summary>
            Building, 
            /// <summary>
            /// All sold property in output
            /// </summary>
            Suite 
        }

        private static void generateViewResponse(ISession dbSession,
			ICollection<Structure> structures,
			ICollection<Site> sites, ICollection<Building> buildings,
			ICollection<SuiteType> suiteTypes, ICollection<Suite> suites,
			ICollection<ViewOrder> viewOrders,
            Guid primaryListingId,
            IResponseData resp,
            ViewSettings vs, bool minimizeOutput)
        {
            generateViewResponse(dbSession, structures, sites, buildings, suiteTypes, suites, viewOrders, primaryListingId, resp, vs, minimizeOutput,
                false);
        }

        private static void generateViewResponse(ISession dbSession,
			ICollection<Structure> structures,
			ICollection<Site> sites, ICollection<Building> buildings,
			ICollection<SuiteType> suiteTypes, ICollection<Suite> suites,
			ICollection<ViewOrder> viewOrders,
            Guid primaryListingId,
            IResponseData resp,
            ViewSettings vs, bool minimizeOutput,
            bool tempEventMode)
        {
            resp.Data = new ClientData();

            List<ClientData> elements;

            HashSet<SuiteType> usedSuiteTypes = new HashSet<SuiteType>();
            HashSet<Building> usedBuildings = new HashSet<Building>();
            HashSet<Site> usedSites = new HashSet<Site>();

            TempReconcileViewOrdersNow(suites, dbSession);

            elements = new List<ClientData>(suites.Count());
		    foreach (Suite s in suites)
		    {
				//if ((soldPropertyLevel == ViewResponseSoldPropertyLevel.None)
				//    || (soldPropertyLevel == ViewResponseSoldPropertyLevel.Building))
				//{
				//    if (s.Status == Suite.SalesStatus.Sold) continue;
				//}

			    ClientData cd = s.GetClientData();
			    //ClientData cd = s.GetClientData();
			    //if (maskSaleStatus) cd["status"] = "Selected";
			    elements.Add(cd);
			    usedSuiteTypes.Add(s.SuiteType);
			    usedBuildings.Add(s.Building);
		    }
	        resp.Data.Add("suites", elements.ToArray());

            if (suiteTypes != null)
            {
                elements = new List<ClientData>(suiteTypes.Count());
                foreach (SuiteType st in suiteTypes)
                {
                    if (!usedSuiteTypes.Contains(st)) continue;
                    ReferencedFileHelper.ConvertUrlsToAbsolute(st);
                    elements.Add(st.GetClientData());
                }
            }
            else
            {
                elements = new List<ClientData>(usedSuiteTypes.Count());
                foreach (SuiteType st in usedSuiteTypes)
                {
                    ReferencedFileHelper.ConvertUrlsToAbsolute(st);
                    elements.Add(st.GetClientData());
                }
            }
            resp.Data.Add("suiteTypes", elements.ToArray());

            if (buildings != null)
            {
                elements = new List<ClientData>(buildings.Count());
                foreach (Building b in buildings)
                {
                    if (!usedBuildings.Contains(b))
                    {
						//if (soldPropertyLevel == ViewResponseSoldPropertyLevel.None)  // make sure suites in output are always backed
						//// by building disregarding building status
						//{
						//    if (b.Status == Building.BuildingStatus.Sold) continue;
						//}
                        if (minimizeOutput) continue;
                    }
                    ReferencedFileHelper.ConvertUrlsToAbsolute(b);
                    ClientData cd = b.GetClientData();
                    //if (maskSaleStatus) cd["status"] = "Selected";
                    cd.Add("address", AddressHelper.ConvertToReadableAddress(b, null));
                    elements.Add(cd);
                    usedSites.Add(b.ConstructionSite);
                }
            }
            else
            {
                elements = new List<ClientData>(usedBuildings.Count());
                foreach (Building b in usedBuildings)
                {
                    ReferencedFileHelper.ConvertUrlsToAbsolute(b);
                    ClientData cd = b.GetClientData();
                    //if (maskSaleStatus) cd["status"] = "Selected";
                    cd.Add("address", AddressHelper.ConvertToReadableAddress(b, null));
                    elements.Add(cd);
                    usedSites.Add(b.ConstructionSite);
                }
            }
            resp.Data.Add("buildings", elements.ToArray());

			if (structures != null)
			{
				resp.Data.Add("structures", structures.ConvertTo((s) => s.DisplayModelUrl).ToArray());
			}

            if (!tempEventMode)
            {
                if (sites != null)
                {
                    elements = new List<ClientData>(sites.Count());
                    foreach (Site s in sites)
                    {
                        if (!usedSites.Contains(s)) continue;
                        ReferencedFileHelper.ConvertUrlsToAbsolute(s);
                        elements.Add(s.GetClientData());
                    }
                }
                else
                {
                    elements = new List<ClientData>(usedSites.Count());
                    foreach (Site s in usedSites)
                    {
                        ReferencedFileHelper.ConvertUrlsToAbsolute(s);
                        elements.Add(s.GetClientData());
                    }
                }
                resp.Data.Add("sites", elements.ToArray());

                if (viewOrders != null)
                {
                    elements = new List<ClientData>(viewOrders.Count());
                    DateTime now = DateTime.UtcNow;
                    foreach (ViewOrder vo in viewOrders)
                    {
                        var cd = new ClientData();
                        // Cannot reuse viewOrder.GetClientData() here as it exposes too much information
                        cd.Add("id", vo.AutoID);
						// TODO: Limit output in case ViewOrder is expired
                        cd.Add("targetObjectType", ClientData.ConvertProperty(vo.TargetObjectType));
                        cd.Add("targetObjectId", vo.TargetObjectId);
                        cd.Add("product", ClientData.ConvertProperty(vo.Product));
                        cd.Add("options", ClientData.ConvertProperty(vo.Options));
                        cd.Add("infoUrl", vo.InfoUrl);
                        cd.Add("vTourUrl", vo.VTourUrl);
						cd.Add("shareLink", ReverseRequestService.GenerateUrl(vo));

                        if (!vo.Enabled || (vo.ExpiresOn < now))
                        {
	                        cd.Add("reason", !vo.Enabled ? "disabled" : "expired");
	                        cd.Add("recoverUrl", 
								string.Format(Configuration.Urls.DisabledViewOrderRecover.Value, vo.AutoID.ToString("N")));
                        }

	                    elements.Add(cd);
                    }
                }
                else
                {
                    elements = new List<ClientData>(0);
                }
                resp.Data.Add("viewOrders", elements.ToArray());

				resp.Data.Add("viewSettings", vs.GetClientData());

                if (!primaryListingId.Equals(Guid.Empty)) resp.Data.Add("primaryViewOrderId", primaryListingId);
                resp.Data.Add("initialView", "");  // TODO
            }  // !tempEventMode

            resp.ResponseCode = HttpStatusCode.OK;
        }

        public static void GenerateEventDataResponse(
            ISession session, ref IResponseData response,
            ref IList<Building> buildings, ref IList<Suite> suites)
        {
            generateViewResponse(session, null, null, buildings, null, suites, null, Guid.Empty, response,
                new ViewSettings(true), true, true);
        }

		private class ViewSettings
		{
			public readonly bool ShowSold;
			public readonly bool ShowAvailable;
			public readonly bool ShowOnHold;
			public readonly bool ShowResaleAvailable;
			public readonly bool ShowRental;
			public readonly bool ShowImported;

			public ViewSettings(bool defSetting)
			{
				ShowSold = defSetting;
				ShowAvailable = defSetting;
				ShowOnHold = defSetting;
				ShowResaleAvailable = defSetting;
				ShowRental = defSetting;
				ShowImported = defSetting;
			}

			public ViewSettings(ServiceQuery query, string type)
			{
				ShowImported = false;

				if (type.Equals("vieworder"))
				{
					ShowSold = false;
					ShowAvailable = false;
					ShowOnHold = false;
					ShowResaleAvailable = true;
					ShowRental = true;
				}
				else if (type.Equals("site"))
				{
					ShowSold = false;
					ShowAvailable = true;
					ShowOnHold = true;
					ShowResaleAvailable = false;
					ShowRental = false;
				}
				else if (type.Equals("building"))
				{
					ShowSold = false;                       
					ShowAvailable = true;
					ShowOnHold = true;
					ShowResaleAvailable = false;
					ShowRental = false;
				}
				else if (type.Equals("suite"))
				{
					ShowSold = true;
					ShowAvailable = true;
					ShowOnHold = true;
					ShowResaleAvailable = true;
					ShowRental = true;
				}
				else if (type.Equals("geo"))
				{
					ShowSold = true;
					ShowAvailable = true;
					ShowOnHold = true;
					ShowResaleAvailable = true;
					ShowRental = true;
				}
				else throw new NotImplementedException();

				ShowSold = query.GetParam("showSold", ShowSold);  // OBSOLETE URI
				ShowSold = query.GetParam("showsold", ShowSold);  // OBSOLETE URI
				ShowSold = query.GetParam("sp_s", ShowSold);
				ShowAvailable = query.GetParam("sp_sa", ShowAvailable);
				ShowOnHold = query.GetParam("sp_sh", ShowOnHold);
				ShowResaleAvailable = query.GetParam("sp_ra", ShowResaleAvailable);
				ShowRental = query.GetParam("sp_rr", ShowRental);
				ShowImported = query.GetParam("includeImported", ShowImported);  // OBSOLETE URI
				ShowImported = query.GetParam("includeimported", ShowImported);  // OBSOLETE URI
				ShowImported = query.GetParam("sp_i", ShowImported);
			}

			public ClientData GetClientData()
			{
				return new ClientData
					{
						{"sp_s", ShowSold},
						{"sp_sa", ShowAvailable},
						{"sp_sh", ShowOnHold},
						{"sp_ra", ShowResaleAvailable},
						{"sp_rr", ShowRental},
						{"sp_i", ShowImported}
					};
			}

			public bool SuiteStatusMatches(Suite s)
			{
				switch (s.Status)
				{
					case Suite.SalesStatus.Available:
						return ShowAvailable;

					case Suite.SalesStatus.AvailableRent:
						return ShowRental;

					case Suite.SalesStatus.OnHold:
						return ShowOnHold;

					case Suite.SalesStatus.ResaleAvailable:
						return ShowResaleAvailable;

					case Suite.SalesStatus.Sold:
						return ShowSold;
				}
				return false;
			}
		}

		private static void getBrokerage(ClientSession session, int userId, IResponseData resp)
		{
			BrokerageInfo info;

			using (var dao = new BrokerageInfoDao(session.DbSession)) info = dao.GetById(userId, true);

			RolePermissionCheck.CheckReadBrokerage(session, info);

			ClientData result = info.GetClientData();
						
			if (User.Role.SuperAdmin == session.User.UserRole)
				result.Add("creditUnits", info.CreditUnits);

			resp.Data = result;
			resp.ResponseCode = HttpStatusCode.OK;
		}

		private static void getBrokerageList(ClientSession session, ServiceQuery query, IResponseData resp, bool includeDeleted)
		{
			ClientData[] result;
			BrokerageInfo[] list;

			using (var dao = new BrokerageInfoDao(session.DbSession)) list = dao.GetAll(includeDeleted).ToArray();

			// produce output
			//
			int cnt = list.Length;
			result = new ClientData[cnt];
			for (int idx = 0; idx < cnt; idx++)
			{
				RolePermissionCheck.CheckReadBrokerage(session, list[idx]);

				result[idx] = list[idx].GetClientData();

				if (User.Role.SuperAdmin == session.User.UserRole)
					result[idx].Add("creditUnits", list[idx].CreditUnits);
			}

			resp.Data = new ClientData();
			resp.Data.Add("brokerages", result);
			resp.ResponseCode = HttpStatusCode.OK;
		}

		private static void getPricingPolicy(ClientSession session, int userId, IResponseData resp)
		{
			PricingPolicy info;

			using (var dao = new PricingPolicyDao(session.DbSession)) info = dao.GetById(userId, true);

			RolePermissionCheck.CheckReadPricingPolicy(session, info);

			resp.Data = info.GetClientData();
			resp.ResponseCode = HttpStatusCode.OK;
		}

		private static void getPricingPolicyList(ClientSession session, ServiceQuery query, IResponseData resp, bool includeDeleted)
		{
			IEnumerable<PricingPolicy> list;

			using (var dao = new PricingPolicyDao(session.DbSession))
			{
				if (query.Contains("defaults"))
				{
					PricingPolicy.SubjectType subject;
					PricingPolicy.ServiceType service;
					if (query.GetParam("defaults", false))
					{
						if (Enum.TryParse<PricingPolicy.SubjectType>(query["subject"], true, out subject))
						{
							if (Enum.TryParse<PricingPolicy.ServiceType>(query["service"], true, out service))
								list = dao.GetDefaults(subject, service, includeDeleted);
							else
								list = dao.GetDefaults(subject, includeDeleted);
						}
						else
						{
							if (Enum.TryParse<PricingPolicy.ServiceType>(query["service"], true, out service))
								list = dao.GetDefaults(service, includeDeleted);
							else
								list = dao.GetDefaults(includeDeleted);
						}
					}
					else
					{
						if (Enum.TryParse<PricingPolicy.SubjectType>(query["subject"], true, out subject))
						{
							int id = query.GetParam("subjectid", -1);
							if (id > 0)
								list = dao.GetBySubject(subject, id, includeDeleted);
							else
								list = dao.GetBySubject(subject, includeDeleted);
						}
						else if (Enum.TryParse<PricingPolicy.ServiceType>(query["service"], true, out service))
						{
							if (Enum.TryParse<PricingPolicy.SubjectType>(query["subject"], true, out subject))
								list = dao.GetBySubjectAndService(subject, service, includeDeleted);
							else
								list = dao.GetByService(service, includeDeleted);
						}
						else
						{
							list = dao.GetNonDefaults(includeDeleted);
						}
					}
				}
				else if (query.Contains("explain"))
				{
					PricingPolicy.SubjectType subject;
					PricingPolicy.ServiceType service;
					int id = query.GetParam("subjectid", -1);
					if (id < 1)
						throw new ArgumentException("Explain requires valid subject ID");
					if (!Enum.TryParse<PricingPolicy.SubjectType>(query["subject"], true, out subject))
						throw new ArgumentException("Explain requires valid subject");
					if (!Enum.TryParse<PricingPolicy.ServiceType>(query["service"], true, out service))
						throw new ArgumentException("Explain requires valid service");

					object target = null;

					switch (subject)
					{
						case PricingPolicy.SubjectType.Brokerage:
							using (var sdao = new BrokerageInfoDao(session.DbSession))
								target = sdao.GetById(id);
							break;

						case PricingPolicy.SubjectType.Agent:
							using (var sdao = new UserDao(session.DbSession))
								target = sdao.GetById(id);
							break;
					}

					list = dao.GetFor(target, service);
				}
				else
				{
					list = dao.GetAll(includeDeleted);
				}
			}

			// produce output
			//
			var result = new List<ClientData>();
			foreach (var pp in list)
			{
				RolePermissionCheck.CheckReadPricingPolicy(session, pp);

				result.Add(pp.GetClientData());
			}

			resp.Data = new ClientData();
			resp.Data.Add("policies", result.ToArray());
			resp.ResponseCode = HttpStatusCode.OK;
		}

		private static void getBilling(ClientSession session, ServiceQuery query, IResponseData resp)
		{
			var type = query.GetParam("type", "brokerage").ToLowerInvariant();
			int objectId = query.GetParam("id", -1);
			if (objectId < 0) throw new ArgumentException("Object ID missing.");

			// TODO: Replace with proper rule checking
			if (session.User.UserRole != User.Role.SuperAdmin)
				throw new UnauthorizedAccessException();

			Vre.Server.Accounting.Invoice result = null;
			if (type.Equals("brokerage"))
			{
				using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
				{
					BrokerageInfo b;
					using (var dao = new BrokerageInfoDao(session.DbSession))
						b = dao.GetById(objectId);

					if (null == b) throw new FileNotFoundException("Brokerage does not exist");

					result = Vre.Server.Accounting.Biller.CalculateCurrentForBrokerage(session.DbSession, b);
				}
			}
			else if (type.Equals("agent"))
			{
				using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
				{
					User u;
					using (var dao = new UserDao(session.DbSession))
						u = dao.GetById(objectId);

					if (null == u) throw new FileNotFoundException("User does not exist");

					result = Vre.Server.Accounting.Biller.CalculateCurrentForAgent(session.DbSession, u);
				}
			}

			if (result != null)
			{
				resp.ResponseCode = HttpStatusCode.OK;
				resp.Data = result.GetClientData();
			}
			else
			{
				resp.ResponseCode = HttpStatusCode.NotFound;
				resp.ResponseCodeDescription = "No data generated for type/id/time provided";
			}
		}

		private static void getInvoiceList(ClientSession session, ServiceQuery query, IResponseData resp)
		{
			var type = query.GetParam("type", "brokerage").ToLowerInvariant();
			int objectId = query.GetParam("id", -1);
			if (objectId < 0) throw new ArgumentException("Object ID missing.");

			DateTime from, to;
			if (!DateTime.TryParseExact(query.GetParam("from", string.Empty), "yyyy-MM-ddTHH:mm:ss", 
				null, System.Globalization.DateTimeStyles.None, out from))
				throw new ArgumentException("'From' value is invalid");
			if (!DateTime.TryParseExact(query.GetParam("to", string.Empty), "yyyy-MM-ddTHH:mm:ss",
				null, System.Globalization.DateTimeStyles.None, out to))
				throw new ArgumentException("'To' value is invalid");

			// TODO: Replace with proper rule checking
			if (session.User.UserRole != User.Role.SuperAdmin)
				throw new UnauthorizedAccessException();

			IEnumerable<Invoice> result = null;
			if (type.Equals("brokerage"))
			{
				using (var dao = new InvoiceDao(session.DbSession))
					result = dao.Get(Invoice.SubjectType.Brokerage, objectId, from, to);
			}
			else if (type.Equals("agent"))
			{
				using (var dao = new InvoiceDao(session.DbSession))
					result = dao.Get(Invoice.SubjectType.Agent, objectId, from, to);
			}

			if (result != null)
			{
				resp.ResponseCode = HttpStatusCode.OK;
				resp.Data = new ClientData();

				List<ClientData> rl = new List<ClientData>();
				foreach (var i in result) rl.Add(i.GetClientData());
				resp.Data.Add("invoices", rl.ToArray());
			}
			else
			{
				resp.ResponseCode = HttpStatusCode.NotFound;
				resp.ResponseCodeDescription = "No data generated for type/id/time provided";
			}
		}
		#endregion

        #region update
        private static void updateBuilding(ClientSession session, int buildingId, ClientData data, IResponseData resp)
        {
            int updatedCnt = 0;
            List<int> staleIds = new List<int>();
            string error = null;
            Building building;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session))
            {
                using (SiteManager manager = new SiteManager(session))
                {
                    building = manager.GetBuildingById(buildingId);

                    if (building.UpdateFromClient(data))
                    {
                        manager.UpdateBuilding(building);
                    }

                    foreach (ClientData suiteData in data.GetNextLevelDataArray("suites"))
                    {
                        Suite suite = null;

                        int suiteId = suiteData.GetProperty("id", -1);
                        if (suiteId >= 0)
                            try
                            {
                                suite = manager.GetSuiteById(suiteId);
                            }
                            catch (FileNotFoundException)
                            {
                                // try searching by name (number)
                                string name = suiteData.GetProperty("name", string.Empty);
                                var suites = from s in building.Suites 
                                             where s.SuiteName.Equals(name)
                                             select s;
                                if (1 == suites.Count()) suite = suites.First();
								ServiceInstances.Logger.Debug("UB: Suite not found by ID ({0}); searching by name ({1}) got ID={2}",
									suiteId, name, (suite != null) ? suite.AutoID : -1);
                            }

                        if (null == suite) continue;
                        if (suite.Building.AutoID != building.AutoID)
                        {
                            throw new InvalidDataException("The suite ID=" + suite.AutoID + 
                                " does not belong to building ID=" + building.AutoID);
                        }

	                    var price = suite.CurrentPrice;
//ServiceInstances.Logger.Debug("UB#1: Suite ID={0} price={1}", suite.AutoID, price.HasValue ? price.Value.ToFullString() : "?");
	                    if (!suite.UpdateFromClient(suiteData)) continue;
//ServiceInstances.Logger.Debug("UB#2: Suite ID={0} price={1}", suite.AutoID, suite.CurrentPrice.HasValue ? suite.CurrentPrice.Value.ToFullString() : "?");
						try
	                    {
		                    if (true)//manager.UpdateSuite(suite))
		                    {
			                    updatedCnt++;

			                    if (suite.CurrentPrice.HasValue && (
									(price.HasValue && (suite.CurrentPrice.Value.CompareTo(price.Value) != 0))
									|| (!price.HasValue)
									))
				                    manager.LogNewSuitePrice(suite, (float)Convert.ToDouble(suite.CurrentPrice), suite.CurrentPrice.Value.Currency);
		                    }
		                    else
		                    {
			                    staleIds.Add(suite.AutoID);
			                    error = "At least one object is stale.";
		                    }
	                    }
	                    catch (Exception ex)
	                    {
		                    error = string.Format("Cannot update suite {0} (ID={1}): {2}", suite.SuiteName, suite.AutoID, ex.Message);
		                    ServiceInstances.Logger.Error("Cannot update suite ID={0}: {1}", suite.AutoID, Utilities.ExplodeException(ex));
		                    break;
	                    }
                    }  // foreach suite

                }  // using SiteManager

                if (null == error) tran.Commit();
                else tran.Rollback();
            }  // transaction

            // make sure building information is refreshed
            session.DbSession.Refresh(building);

            if (null == error)
            {
                resp.ResponseCode = (0 == updatedCnt) ? HttpStatusCode.NotModified : HttpStatusCode.OK;
                resp.Data = new ClientData {{"updated", updatedCnt}};
            }
            else
            {
                resp.ResponseCode = HttpStatusCode.Conflict;
                resp.ResponseCodeDescription = error;
                resp.Data = new ClientData {{"updated", 0}};
	            if (staleIds.Count > 0) resp.Data.Add("staleIds", CsvUtilities.ToString<int>(staleIds));
            }
        }

		private static void updateSuite(ClientSession session, int suiteId, ClientData data, IResponseData resp)
		{
			bool updated = false;
			bool unauthorizedChange = false;

			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
			{
				using (var manager = new SiteManager(session))
					updated = manager.UpdateSuite(suiteId, data, out unauthorizedChange);

				if (updated) tran.Commit();
				else tran.Rollback();
			}  // transaction

			setTriStateUpdateResponse(resp, updated, unauthorizedChange);
		}

		private static void setTriStateUpdateResponse(IResponseData resp, bool updated, bool unauthorizedChange)
		{
			resp.Data = new ClientData();
			if (updated)
			{
				if (unauthorizedChange)
				{
					resp.ResponseCode = HttpStatusCode.Unauthorized;
					resp.ResponseCodeDescription = "Some changes were not applied";
				}
				else
				{
					resp.ResponseCode = HttpStatusCode.OK;
				}
				resp.Data.Add("updated", 1);
			}
			else
			{
				resp.ResponseCode = HttpStatusCode.NotModified;
				resp.Data.Add("updated", 0);
			}
		}

		private static void updateUser(ClientSession session, int userId, ClientData data, IResponseData resp)
        {
            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
            {
                using (UserManager manager = new UserManager(session))
                {
                    User user = manager.Get(userId);

					var updated = user.UpdateFromClient(data);

                    // block changing brokerage for now; should be possible?!
                    //var bid = data.GetProperty("brokerageId", -1);
                    //if (bid > 0)  // try setting brokerage
                    //{
                    //    if (((user.BrokerInfo != null) && (user.BrokerInfo.AutoID != bid))
                    //        || (null == user.BrokerInfo))
                    //    {
                    //        using (var dao = new BrokerageInfoDao(session.DbSession))
                    //            user.BrokerInfo = dao.GetById(bid);
                    //        updated = true;
                    //    }
                    //}
                    //else if ((0 == bid) && (user.BrokerInfo != null))
                    //{
                    //    user.BrokerInfo = null;
                    //    updated = true;
                    //}
					
					if (updated)
                    {
                        user.MarkUpdated();
                        manager.Update(user);
                        resp.ResponseCode = HttpStatusCode.OK;
                        resp.Data = new ClientData();
                        resp.Data.Add("updated", 1);
                    }
                    else
                    {
                        resp.ResponseCode = HttpStatusCode.NotModified;
                        resp.Data = new ClientData();
                        resp.Data.Add("updated", 0);
                    }
                }

                tran.Commit();
            }
        }

		private static void updateNamedSearchFilter(ClientSession session, int filterId, ClientData data, IResponseData resp)
		{
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
			{
				using (var dao = new NamedSearchFilterDao(session.DbSession))
				{
					var nsf = dao.GetById(filterId);

					if (null == nsf) throw new FileNotFoundException();

					RolePermissionCheck.CheckUpdateNamedSearchFilter(session, nsf);

					if (nsf.UpdateFromClient(data))
					{
						nsf.MarkUpdated();
						dao.Update(nsf);
						resp.ResponseCode = HttpStatusCode.OK;
						resp.Data = new ClientData();
						resp.Data.Add("updated", 1);
					}
					else
					{
						resp.ResponseCode = HttpStatusCode.NotModified;
						resp.Data = new ClientData();
						resp.Data.Add("updated", 0);
					}
				}

				tran.Commit();
			}
		}

        private static void updateViewOrder(ClientSession session, ServiceQuery query, string strObjectId, ClientData data, IResponseData resp)
        {
            string paymentSystemRefId = query["pr"];
			bool unauthorizedChange = false;
			bool updated = false;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
            {
                ViewOrder viewOrder = RetrieveViewOrder(session.DbSession, strObjectId, true);

                User owner;
                using (UserDao dao = new UserDao(session.DbSession))
                    owner = dao.GetById(viewOrder.OwnerId);

				if (RolePermissionCheck.CheckLimitedUpdateViewOrder(session, owner))
				{
					switch (viewOrder.UpdateFromClient(data, new[] { "enabled" }))
					{
						case ViewOrder.ClientUpdateResult.Changed: updated = true; break;
						case ViewOrder.ClientUpdateResult.ChangesSkipped: updated = true; unauthorizedChange = true; break;
					}
				}
				else
				{
					RolePermissionCheck.CheckUpdateViewOrder(session, owner);
					unauthorizedChange = false;
					updated = viewOrder.UpdateFromClient(data);
				}

				if (updated)
				{
                    viewOrder.MarkUpdated();

                    using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                        dao.Update(viewOrder);

                    // Generate financial transaction
                    //
					//FinancialTransaction.TranTarget tt = FinancialTransaction.TranTarget.Suite;
					//switch (viewOrder.TargetObjectType)
					//{
					//    case ViewOrder.SubjectType.Building: tt = FinancialTransaction.TranTarget.Building; break;
					//    case ViewOrder.SubjectType.Suite: tt = FinancialTransaction.TranTarget.Suite; break;
					//}
					//FinancialTransaction ft = new FinancialTransaction(session.User.AutoID,
					//    FinancialTransaction.AccountType.User, viewOrder.OwnerId,
					//    FinancialTransaction.OperationType.Debit, 0m,
					//    FinancialTransaction.TranSubject.ViewOrder,
					//    tt, viewOrder.TargetObjectId, viewOrder.AutoID.ToString());

					//if (!string.IsNullOrWhiteSpace(paymentSystemRefId))
					//    ft.SetPaymentSystemReference(FinancialTransaction.PaymentSystemType.CondoExplorer, paymentSystemRefId);

					//using (FinancialTransactionDao dao = new FinancialTransactionDao(session.DbSession))
					//{
					//    dao.Create(ft);
					//    ft.SetAutoSystemReferenceId();
					//    dao.Update(ft);
					//}

                    ReflectViewOrderStatusInTarget(viewOrder, session.DbSession);
                    //resp.Data.Add("ref", ft.SystemRefId);
                }
                tran.Commit();
            }
			setTriStateUpdateResponse(resp, updated, unauthorizedChange);
		}
		
		private static void updateBrokerage(ClientSession session, int itemId, ClientData data, IResponseData resp)
		{
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
			{
				using (var dao = new BrokerageInfoDao(session.DbSession))
				{
					var info = dao.GetById(itemId);

					if (null == info) throw new FileNotFoundException();

					RolePermissionCheck.CheckUpdateBrokerage(session, info);

					if (info.UpdateFromClient(data))
					{
						var test = dao.GetByName(info.Name);
						if ((test != null) && !test.Equals(info))
							throw new ObjectExistsException("Brokerage with this name already exists");

						info.MarkUpdated();
						dao.SafeUpdate(info);
						resp.ResponseCode = HttpStatusCode.OK;
						resp.Data = new ClientData();
						resp.Data.Add("updated", 1);
					}
					else
					{
						resp.ResponseCode = HttpStatusCode.NotModified;
						resp.Data = new ClientData();
						resp.Data.Add("updated", 0);
					}
				}

				tran.Commit();
			}
		}

		private static void updatePricingPolicy(ClientSession session, int itemId, ClientData data, IResponseData resp)
		{
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
			{
				using (var dao = new PricingPolicyDao(session.DbSession))
				{
					var info = dao.GetById(itemId);

					if (null == info) throw new FileNotFoundException();

					RolePermissionCheck.CheckUpdatePricingPolicy(session, info);

					if (info.UpdateFromClient(data))
					{
						info.MarkUpdated();
						dao.Update(info);
						resp.ResponseCode = HttpStatusCode.OK;
						resp.Data = new ClientData();
						resp.Data.Add("updated", 1);
					}
					else
					{
						resp.ResponseCode = HttpStatusCode.NotModified;
						resp.Data = new ClientData();
						resp.Data.Add("updated", 0);
					}
				}

				tran.Commit();
			}
		}
		#endregion

        #region create
        private static void createUser(ClientSession session, ClientData data, IResponseData resp)
        {
            User.Role role = data.GetProperty<User.Role>("role", User.Role.Visitor);
            LoginType type = data.GetProperty<LoginType>("type", LoginType.Plain);
            int estateDeveloperId = ResolveDeveloperId(session.DbSession, data.GetProperty("estateDeveloperId", string.Empty));
            if (estateDeveloperId < 0) estateDeveloperId = ResolveDeveloperId(session.DbSession, data.GetProperty("ed", string.Empty));  // OBSOLETE
            string login = data.GetProperty("uid", string.Empty);
            string password = data.GetProperty("pwd", string.Empty);
            var brokerageId = data.GetProperty("brokerageId", -1);

			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
			{
				User u;
				using (UserManager manager = new UserManager(session))
				{
					manager.Create(role, estateDeveloperId >= 0 ? estateDeveloperId : brokerageId, type, login, password);
					try
					{
						// create contact info block with any added fields from inbound JSON
						u = manager.Get(type, role, estateDeveloperId, login);
						u.UpdateFromClient(data);

						if (brokerageId > 0)  // try setting brokerage
						{
							using (var dao = new BrokerageInfoDao(session.DbSession))
							u.BrokerInfo = dao.GetById(brokerageId);
						}

						manager.Update(u);

						resp.ResponseCode = HttpStatusCode.OK;
						resp.Data = new ClientData();
						resp.Data.Add("id", u.AutoID);
					}
					catch (Exception ex)
					{
						resp.ResponseCode = HttpStatusCode.Created;
						resp.ResponseCodeDescription = "Contact information was not stored.";
						ServiceInstances.Logger.Error("Contact information for created user {0}[{1}] was not saved: {2}", type, login, ex);
					}
				}
				tran.Commit();
			}
        }

		private static void createNamedSearchFilter(ClientSession session, ClientData data, IResponseData resp)
		{
			RolePermissionCheck.CheckCreateNamedSearchFilter(session);
			
			NamedSearchFilter result = new NamedSearchFilter(session.User);
			result.UpdateFromClient(data);

			using (var dao = new NamedSearchFilterDao(session.DbSession))
				dao.Create(result);

			resp.ResponseCode = HttpStatusCode.OK;
			resp.Data = new ClientData();
			resp.Data.Add("id", result.AutoID);
		}

        //private static void createListing(ClientSession session, ClientData data, IResponseData resp)
        //{



        //    User.Role role = data.GetProperty<User.Role>("role", User.Role.Visitor);
        //    LoginType type = data.GetProperty<LoginType>("type", LoginType.Plain);
        //    int estateDeveloperId = data.GetProperty("ed", -1);
        //    string login = data.GetProperty("uid", string.Empty);
        //    string password = data.GetProperty("pwd", string.Empty);

        //    using (UserManager manager = new UserManager(session))
        //    {
        //        manager.Create(role, estateDeveloperId, type, login, password);
        //        try
        //        {
        //            // create contact info block with any added fields from inbound JSON
        //            User u = manager.Get(type, role, estateDeveloperId, login);
        //            u.UpdateFromClient(data);
        //            resp.ResponseCode = HttpStatusCode.OK;
        //        }
        //        catch (Exception ex)
        //        {
        //            resp.ResponseCode = HttpStatusCode.Created;
        //            resp.ResponseCodeDescription = "Contact information was not stored.";
        //            ServiceInstances.Logger.Error("Contact information for created user {0}[{1}] was not saved: {2}", type, login, ex);
        //        }
        //    }
        //}

		private static void createBrokerage(ClientSession session, ClientData data, IResponseData resp)
		{
			RolePermissionCheck.CheckCreateBrokerage(session);

			var result = new BrokerageInfo(string.Empty);
			result.UpdateFromClient(data);

			if (string.IsNullOrEmpty(result.Name))
				throw new ArgumentException("Name field is requlred");

			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
			{
				using (var dao = new BrokerageInfoDao(session.DbSession))
				{
					if (dao.Exists(result.Name))
						throw new ObjectExistsException("Object with this name already exists");
					dao.Create(result);
				}
				tran.Commit();
			}

			resp.ResponseCode = HttpStatusCode.OK;
			resp.Data = new ClientData();
			resp.Data.Add("id", result.AutoID);
		}

		private static void createPricingPolicy(ClientSession session, ClientData data, IResponseData resp)
		{
			RolePermissionCheck.CheckCreatePricingPolicy(session);

			var result = new PricingPolicy(data);

			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
			{
				using (var dao = new PricingPolicyDao(session.DbSession))
				{
					if (dao.Exists(result.TargetObjectType, result.TargetObjectId, result.Service))
						throw new ObjectExistsException("Object with these settings already exists");
					dao.Create(result);
				}
				tran.Commit();
			}

			resp.ResponseCode = HttpStatusCode.OK;
			resp.Data = new ClientData();
			resp.Data.Add("id", result.AutoID);
		}
		#endregion

        #region delete
        private static void deleteUser(ClientSession session, int userId, IResponseData resp)
        {
            User user = null;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
            {
                using (UserManager manager = new UserManager(session))
                {
                    user = manager.Get(userId);

                    manager.Delete(user);

                    using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                    {
                        foreach (ViewOrder vo in dao.Get(user.AutoID))
                        {
                            vo.MarkDeleted();
                            dao.Update(vo);
                        }
                    }

                    resp.ResponseCode = HttpStatusCode.OK;
                    resp.Data = new ClientData();
                    resp.Data.Add("deleted", 1);
                }
                tran.Commit();
            }
        }

		private static void deleteBuilding(ClientSession session, int buildingId, IResponseData resp)
		{
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
			{
				using (var dao = new BuildingDao(session.DbSession))
				{
					var building = dao.GetById(buildingId);

					if (null == building) throw new FileNotFoundException();

					RolePermissionCheck.CheckDeleteBuilding(session, building);

					building.MarkDeleted();
					dao.SafeUpdate(building);

					resp.ResponseCode = HttpStatusCode.OK;
					resp.Data = new ClientData();
					resp.Data.Add("deleted", 1);
				}
				tran.Commit();
			}
		}

		private static void deleteNamedSearchFilter(ClientSession session, int filterId, IResponseData resp)
		{
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
			{
				using (var dao = new NamedSearchFilterDao(session.DbSession))
				{
					var nsf = dao.GetById(filterId);

					if (null == nsf) throw new FileNotFoundException();

					RolePermissionCheck.CheckDeleteNamedSearchFilter(session, nsf);

					nsf.MarkDeleted();
					dao.Update(nsf);

					resp.ResponseCode = HttpStatusCode.OK;
					resp.Data = new ClientData();
					resp.Data.Add("deleted", 1);
				}
				tran.Commit();
			}
		}

		private static void deleteViewOrder(ClientSession session, string strObjectId, IResponseData resp)
        {
            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
            {
                ViewOrder viewOrder = RetrieveViewOrder(session.DbSession, strObjectId, true);

                if (null == viewOrder) throw new FileNotFoundException("Undefined view order");

                User owner;
                using (UserDao dao = new UserDao(session.DbSession))
                    owner = dao.GetById(viewOrder.OwnerId);

                RolePermissionCheck.CheckDeleteViewOrder(session, owner);

                viewOrder.MarkDeleted();
                using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                    dao.Update(viewOrder);

                ReflectViewOrderStatusInTarget(viewOrder, session.DbSession);

                tran.Commit();
            }

            resp.ResponseCode = HttpStatusCode.OK;
            resp.Data = new ClientData();
            resp.Data.Add("deleted", 1);
        }

		private static void deleteBrokerage(ClientSession session, int itemId, IResponseData resp)
		{
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
			{
				using (var dao = new BrokerageInfoDao(session.DbSession))
				{
					var info = dao.GetById(itemId);

					if (null == info) throw new FileNotFoundException();

					RolePermissionCheck.CheckDeleteBrokerage(session, info);

					info.MarkDeleted();
					dao.Update(info);

					resp.ResponseCode = HttpStatusCode.OK;
					resp.Data = new ClientData();
					resp.Data.Add("deleted", 1);
				}
				tran.Commit();
			}
		}

		private static void deletePricingPolicy(ClientSession session, int itemId, IResponseData resp)
		{
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
			{
				using (var dao = new PricingPolicyDao(session.DbSession))
				{
					var info = dao.GetById(itemId);

					if (null == info) throw new FileNotFoundException();

					RolePermissionCheck.CheckDeletePricingPolicy(session, info);

					info.MarkDeleted();
					dao.Update(info);

					resp.ResponseCode = HttpStatusCode.OK;
					resp.Data = new ClientData();
					resp.Data.Add("deleted", 1);
				}
				tran.Commit();
			}
		}
		#endregion

        #region extra methods which should go to manager class (?)
        private static EstateDeveloper extractDeveloperFromViewOrder(ClientSession session, ViewOrder vo)
        {
            EstateDeveloper result = null;

			try
			{
				switch (vo.TargetObjectType)
				{
					case ViewOrder.SubjectType.Building:
						{
							Building b;
							using (BuildingDao dao = new BuildingDao(session.DbSession))
								b = dao.GetById(vo.TargetObjectId);
							result = b.ConstructionSite.Developer;
						}
						break;

					case ViewOrder.SubjectType.Suite:
						{
							Suite s;
							using (SuiteDao dao = new SuiteDao(session.DbSession))
								s = dao.GetById(vo.TargetObjectId);
							result = s.Building.ConstructionSite.Developer;
						}
						break;
				}
			}
			catch (Exception ex)
			{
				ServiceInstances.Logger.Error("EDfVO: {0}, {1}, {2}, {3}",
					(session != null) ? session.ToString() : "N/A",
					(vo != null) ? vo.AutoID.ToString() : "N/A",
					(vo != null) ? vo.TargetObjectType.ToString() : "N/A",
					(vo != null) ? vo.TargetObjectId.ToString() : "N/A",
					ex.Message, ex.StackTrace);
				result = new EstateDeveloper(EstateDeveloper.Configuration.Online);
			}

            return result;
        }

		internal static ViewOrder RetrieveViewOrder(ISession session, string id, bool canBeDeleted)
		{
			if (null == id) throw new ArgumentException("Object ID missing.");

			Guid rqid;
			switch (UniversalId.TypeInUrlId(id))
			{
				case UniversalId.IdType.ViewOrder:
					rqid = UniversalId.ExtractAsGuid(id);
					break;

				case UniversalId.IdType.Unknown:
					if (!Guid.TryParseExact(id, "N", out rqid))
						throw new ArgumentException();
					break;

				default:
					throw new ArgumentException();
			}

			ViewOrder viewOrder;

			using (ViewOrderDao dao = new ViewOrderDao(session))
				viewOrder = dao.GetById(rqid);

			if ((null == viewOrder) || (viewOrder.Deleted && !canBeDeleted))
				throw new FileNotFoundException("Undefined view order");

			return viewOrder;
		}

        internal static ViewOrder RetrieveViewOrder(IServiceRequest request, bool canBeDeleted)
        {
			var sqry = request.Request.Query;
			var dbSession = request.UserInfo.Session.DbSession;

			var result = RetrieveViewOrder(dbSession, sqry["id"], canBeDeleted);

			if ((result != null) && (result.Product == ViewOrder.ViewOrderProduct.Building3DLayout))
			{
				string debugServerEndpoint = sqry["gwt.codesvr"];
				bool debugMode = 
					((debugServerEndpoint != null) 
					&& debugServerEndpoint.StartsWith("127.0.0.1"));

				if (debugMode)
					debugMode = _debugClientFilter.IsInRange(request.Request.EndPoint.Address);

				if (!debugMode && (request.UserInfo.Session.User.UserRole == User.Role.Visitor))
				{
					User u;
					using (var dao = new UserDao(dbSession)) u = dao.GetById(result.OwnerId);
					if (u.RefererRestriction.Length != 0)
					{
						var referer = request.Request.Referer;
						if (null == referer)
						{
							ServiceInstances.Logger.Error("RQZ-0");
							throw new FileNotFoundException("Undefined view order");
						}

						string testReferer = referer.ToString();
						foreach (string suri in u.RefererRestriction)
						{
							if (suri.StartsWith("^"))
							{
								var test = new Regex(suri);
								if (test.IsMatch(testReferer)) return result;
							}
							else if (testReferer.StartsWith(suri))
							{
								return result;
							}
							//if (0 == Uri.Compare(request.Referer, new Uri(suri), UriComponents.HostAndPort, UriFormat.Unescaped, StringComparison.InvariantCultureIgnoreCase))
							//    return result;
						}

						ServiceInstances.Logger.Error("RQZ-1");
						throw new FileNotFoundException("Undefined view order");
					}
				}
			}
			return result;
        }

        internal static bool ReflectViewOrderStatusInTarget(ViewOrder viewOrder, ISession dbSession)
        {
            bool result = false;
            bool isActive = !viewOrder.Deleted && viewOrder.Enabled && (viewOrder.ExpiresOn > DateTime.UtcNow);

            switch (viewOrder.TargetObjectType)
            {
                case ViewOrder.SubjectType.Suite:
                    using (SuiteDao dao = new SuiteDao(dbSession))
                    {
                        Suite s = dao.GetById(viewOrder.TargetObjectId);
                        switch (viewOrder.Product)
                        {
                            case ViewOrder.ViewOrderProduct.PublicListing:
                                if (isActive && (s.Status != Suite.SalesStatus.ResaleAvailable))
                                {
                                    s.Status = Suite.SalesStatus.ResaleAvailable;
                                    result = true;
                                }
                                else if (!isActive && (s.Status == Suite.SalesStatus.ResaleAvailable))
                                {
                                    s.Status = Suite.SalesStatus.Sold;
                                    result = true;
                                }
                                break;

                            case ViewOrder.ViewOrderProduct.PrivateListing:
                                // NO-OP
                                break;

                            default:
                                break;
                        }
                        if (result) dao.SafeUpdate(s);
                    }
                    break;

                case ViewOrder.SubjectType.Building:
                    // TODO: ???
                    break;
            }

            return result;
        }

        private static void TempReconcileViewOrdersNow(IEnumerable<Suite> suites, ISession dbSession)
        {
            System.Collections.Generic.HashSet<int> ids = new HashSet<int>();
            foreach (Suite s in suites) ids.Add(s.AutoID);

            int ccnt = 0;

            using (SuiteDao dao = new SuiteDao(dbSession))
            {
                using (ViewOrderDao vodao = new ViewOrderDao(dbSession))
                {
                    foreach (ViewOrder vo in vodao.GetAllExpiredStillActive(ViewOrder.SubjectType.Suite))
                    {
                        if (!ids.Contains(vo.TargetObjectId)) continue;

                        Suite suite = suites.First(s => s.AutoID == vo.TargetObjectId);
                        if (suite.Status == Suite.SalesStatus.Sold) continue;

                        suite.Status = Suite.SalesStatus.Sold;
                        dao.SafeUpdate(suite);

                        ccnt++;
                    }
                }
            }

            if (ccnt > 0)
                ServiceInstances.Logger.Info("On-the-fly ViewOrder reconcile adjusted {0} suite states.", ccnt);
        }

        /// <summary>
        /// Returns -1 if ID string passed cannot be resolved to a Estate Developer ID
        /// </summary>
        internal static int ResolveDeveloperId(ISession session, string id)
        {
            int result = -1;
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (!int.TryParse(id, out result))
                {
                    using (EstateDeveloperDao dao = new EstateDeveloperDao(session))
                    {
                        EstateDeveloper ed = dao.GetById(id);
                        if (ed != null) result = ed.AutoID;
                    }
                }
            }
            return result;
        }
        #endregion

        #region change subscription
        enum ChangeSubscriptionRequest { None, Reset, Set }

        private static ChangeSubscriptionRequest retrieveChangeSubscriptionRequest(ServiceQuery query)
        {
            string raw = query.GetParam("track", string.Empty);
            if (string.IsNullOrEmpty(raw))
            {
                return ChangeSubscriptionRequest.None;
            }
            else
            {
                if ("false".Equals(raw)) return ChangeSubscriptionRequest.Reset;
                else if ("true".Equals(raw)) return ChangeSubscriptionRequest.Set;
                else throw new ArgumentException("Change tracking argument invalid");
            }
        }

        private static void setChangeSubscription(ClientSession cs, Building b, ChangeSubscriptionRequest csrq)
        {
            switch (csrq)
            {
                case ChangeSubscriptionRequest.Set:
                    cs.Subscribe(new [] { b });
                    break;

                case ChangeSubscriptionRequest.Reset:
                    cs.UnsubscribeAll();
                    break;
            }
        }

        private static void setChangeSubscription(ClientSession cs, Suite s, ChangeSubscriptionRequest csrq)
        {
            switch (csrq)
            {
                case ChangeSubscriptionRequest.Set:
                    cs.Subscribe(new[] { s });
                    break;

                case ChangeSubscriptionRequest.Reset:
                    cs.UnsubscribeAll();
                    break;
            }
        }

        private static void setChangeSubscription(ClientSession cs, IEnumerable<Building> bl, ChangeSubscriptionRequest csrq)
        {
            setChangeSubscription(cs, bl, null, csrq);
        }

        private static void setChangeSubscription(ClientSession cs, IEnumerable<Building> bl, IEnumerable<Suite> sl, ChangeSubscriptionRequest csrq)
        {
            switch (csrq)
            {
                case ChangeSubscriptionRequest.Set:
                    cs.Subscribe(bl);
                    if (null == sl)
                    {
                        List<Suite> sll = new List<Suite>();
                        foreach (Building b in bl) sll.AddRange(b.Suites);
                        sl = sll;
                    }
                    cs.Subscribe(sl);
                    break;

                case ChangeSubscriptionRequest.Reset:
                    cs.UnsubscribeAll();
                    break;
            }
        }

        private static void setChangeSubscription(ClientSession cs, IEnumerable<Suite> sl, ChangeSubscriptionRequest csrq)
        {
            switch (csrq)
            {
                case ChangeSubscriptionRequest.Set:
                    cs.Subscribe(sl);
                    break;

                case ChangeSubscriptionRequest.Reset:
                    cs.UnsubscribeAll();
                    break;
            }
        }
        #endregion

		#region Geo Proximity lookup
		public static IList<Building> BuildingsByGeoProximity(ISession dbSession, Building center, double quadradiusM)
		{
			return BuildingsByGeoProximity(dbSession,
				center.Location.Longitude, center.Location.Latitude, quadradiusM);
		}

		public static IList<Building> BuildingsByGeoProximity(ISession dbSession,
			double longitude, double latitude, double quadradiusM)
		{
			double dLon = quadradiusM / GeoUtilities.LongitudeDegreeInM(latitude);
			double dLat = quadradiusM / GeoUtilities.LatitudeDegreeInM(latitude);

			IList<Building> result;

			using (var dao = new BuildingDao(dbSession))
				result = dao.SearchByProximity(longitude, latitude, dLon, dLat);

			ServiceInstances.Logger.Debug("BbGP: dLon={0}, dLat={1}, lon={2}, lat={3}, cnt={4}.",
				dLon, dLat, longitude, latitude, result.Count);

			return result;
		}

		public static IList<Structure> StructuresByGeoProximity(ISession dbSession,
			double longitude, double latitude, double quadradiusM)
		{
			double dLon = quadradiusM / GeoUtilities.LongitudeDegreeInM(latitude);
			double dLat = quadradiusM / GeoUtilities.LatitudeDegreeInM(latitude);

			IList<Structure> result;

			using (var dao = new StructureDao(dbSession))
				result = dao.SearchByProximity(longitude, latitude, dLon, dLat);

			ServiceInstances.Logger.Debug("SbGP: dLon={0}, dLat={1}, lon={2}, lat={3}, cnt={4}.",
				dLon, dLat, longitude, latitude, result.Count);

			return result;
		}
		#endregion
	}
}
