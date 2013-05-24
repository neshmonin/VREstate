using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.RemoteService
{
    internal class DataService
    {
        private static bool _configured = false;
        private static bool _allowUnsecureService = true;
        private static string _disabledViewOrderRecoverUrl = null;

        public const string ServicePathPrefix = ServicePathElement0 + "/";
        private const string ServicePathElement0 = "data";

        enum ModelObject { User, EstateDeveloper, Site, Building, Suite, SuiteType, ViewOrder, View, FinancialTransaction, Inventory }

        private static void configure()
        {
            _allowUnsecureService = ServiceInstances.Configuration.GetValue("AllowSensitiveDataOverNonSecureConnection", false);
            _disabledViewOrderRecoverUrl = ServiceInstances.Configuration.GetValue("DisabledViewOrderRecoverUrl", "http://3dcondox.com/order?recoverId={0}");

            _configured = true;
        }

        public static void ProcessGetRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;
            string strObjectId;
            bool includeDeleted;
            ChangeSubscriptionRequest csrq;

            if (!_configured) configure();

            if (!_allowUnsecureService && !request.UserInfo.Session.TrustedConnection) 
                throw new PermissionException("Service available only over secure connection.");

            getPathElements(request.Request.Path, out mo, out objectId, out strObjectId);

            csrq = retrieveChangeSubscriptionRequest(request.Request.Query);
            includeDeleted = request.Request.Query.GetParam("withdeleted", false);

            if (includeDeleted) RolePermissionCheck.CheckReadDeletedObjects(request.UserInfo.Session);

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
                        int buildingId = request.Request.Query.GetParam("building", -1);
                        if (-1 == buildingId) throw new ArgumentException("Building ID is missing.");

                        Suite.SalesStatus filter;
                        string filterStr = request.Request.Query.GetParam("statusFilter", "");
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
                        int siteId = request.Request.Query.GetParam("site", -1);
                        if (-1 == siteId) throw new ArgumentException("Site ID is missing.");
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
                    getView(request.UserInfo.Session, request.Request.Query, csrq, request.Response);
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
                        int buildingId = request.Request.Query.GetParam("building", -1);
                        if (-1 == buildingId) throw new ArgumentException("Building ID is missing.");

                        getInventoryList(request.UserInfo.Session, buildingId, csrq, request.Response);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    return;
            }

            throw new NotImplementedException();
        }

        public static void ProcessReplaceRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;
            string strObjectId;

            if (!_configured) configure();

            if (!request.UserInfo.Session.TrustedConnection) throw new PermissionException("Service available only over secure connection.");

            getPathElements(request.Request.Path, out mo, out objectId, out strObjectId);

            if (null == request.Request.Data) throw new ArgumentException("Object data not passed.");

            switch (mo)
            {
                case ModelObject.Building:
                    if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
                    updateBuilding(request.UserInfo.Session, objectId, request.Request.Data, request.Response);
                    return;

                case ModelObject.User:
                    if (-1 == objectId) throw new ArgumentException("Object ID is missing.");
                    updateUser(request.UserInfo.Session, objectId, request.Request.Data, request.Response);
                    return;

                case ModelObject.ViewOrder:
                    if (null == strObjectId) throw new ArgumentException("Object ID is missing.");
                    updateViewOrder(request.UserInfo.Session, request.Request.Query, strObjectId, request.Request.Data, request.Response);
                    return;
            }

            throw new NotImplementedException();
        }

        public static void ProcessCreateRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;
            string strObjectId;

            if (!_configured) configure();

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
            }

            throw new NotImplementedException();
        }

        public static void ProcessDeleteRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;
            string strObjectId;

            if (!_configured) configure();

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
            else if (elements[1].Equals("viewOrder")) mo = ModelObject.ViewOrder;
            else if (elements[1].Equals("view")) mo = ModelObject.View;
            else if (elements[1].Equals("ft")) mo = ModelObject.FinancialTransaction;
            else if (elements[1].Equals("inventory")) mo = ModelObject.Inventory;
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
                    ServiceInstances.ModelCache.FillWithModelInfo(s, false);
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

            string scopeType = query.GetParam("scopeType", "site");

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
                    ServiceInstances.ModelCache.FillWithModelInfo(b, false);
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

                ServiceInstances.ModelCache.FillWithModelInfo(building, false);
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
                ServiceInstances.ModelCache.FillWithModelInfo(s, false);
                result.Add(s.GetClientData());
            }
        }

        private static void getSuite(ClientSession session, int suiteId, IResponseData resp, ChangeSubscriptionRequest csrq)
        {
            ClientData result = null;

            using (var dao = new SuiteDao(session.DbSession))
            {
                var suite = dao.GetById(suiteId);

                ServiceInstances.ModelCache.FillWithModelInfo(suite, false);
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
                ServiceInstances.ModelCache.FillWithModelInfo(st, false);
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

            using (UserManager manager = new UserManager(session))
            {
                user = manager.Get(userId);
            }
            
            ClientData result = user.GetClientData();
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
                string nameLookup = query.GetParam("nameFilter", string.Empty);// data.GetProperty("nameFilter", string.Empty);
                User[] list;

                using (UserManager manager = new UserManager(session))
                    list = manager.List(role, estateDeveloperId, nameLookup, includeDeleted);

                // produce output
                //
                int cnt = list.Length;
                result = new ClientData[cnt];
                using (IAuthentication auth = new Authentication(session.DbSession))
                {
                    for (int idx = 0; idx < cnt; idx++)
                    {
                        result[idx] = list[idx].GetClientData();
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

        private static void getInventoryList(ClientSession session, int buildingId, ChangeSubscriptionRequest csrq, IResponseData resp)
        {
            Building b;

            using (BuildingDao dao = new BuildingDao(session.DbSession))
                b = dao.GetById(buildingId);
            if (null == b) throw new FileNotFoundException("Building does not exist");

            List<ClientData> result = new List<ClientData>(b.Suites.Count);
            foreach (Suite s in b.Suites)
            {
                if ((s.SuiteType != null)
                    // a bit of too deep inspection to avoid unnecessary calls; maybe not required
                    //&& (!string.IsNullOrWhiteSpace(s.SuiteType.FloorPlanUrl)) && (!s.SuiteType.FloorPlanUrl.StartsWith("http://"))
                    )
                    ReferencedFileHelper.ConvertUrlsToAbsolute(s.SuiteType);
                    
				result.Add(s.GetInventoryClientData(null, false));
            }

            if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, b.Suites, csrq);

            resp.Data = new ClientData();
            resp.Data.Add("inventory", result);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getFinancialTransactionList(ClientSession session, ServiceQuery query, IResponseData resp)
        {
            ClientData[] result;

            int userId = query.GetParam("userId", -1);
            User user;
            FinancialTransaction[] list;

            // TODO: implement paging
            query.GetParam("pgStartIdx", -1);
            query.GetParam("pgMaxCount", -1);

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

            resp.Data = convertViewOrderdata(session, viewOrder, query.GetParam("verbose", false));

            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static ClientData convertViewOrderdata(ClientSession session, ViewOrder viewOrder, bool verbose)
        {
            viewOrder.ViewOrderURL = ReverseRequestService.ConstructViewOrderUrl(viewOrder);
            ClientData result = viewOrder.GetClientData();

            if (verbose)
            {
                string label = null;
                switch (viewOrder.TargetObjectType)
                {
                    case ViewOrder.SubjectType.Building:
                        {
                            Building b;
                            using (BuildingDao dao = new BuildingDao(session.DbSession))
                                b = dao.GetById(viewOrder.TargetObjectId);
                            label = AddressHelper.ConvertToReadableAddress(b, null);
                        }
                        break;

                    case ViewOrder.SubjectType.Suite:
                        {
                            Suite s;
                            using (SuiteDao dao = new SuiteDao(session.DbSession))
                                s = dao.GetById(viewOrder.TargetObjectId);
                            label = AddressHelper.ConvertToReadableAddress(s.Building, s);
                        }
                        break;
                }
                if (label != null) result.Add("label", label);
            }

            return result;
        }

        private static void getViewOrderList(ClientSession session, ServiceQuery query, IResponseData resp, bool includeDeleted)
        {
            int userId = query.GetParam("userId", -1);
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
                    result.Add(convertViewOrderdata(session, vo, verbose));
                }
            }

            resp.Data = new ClientData();
            resp.Data.Add("viewOrders", result.ToArray());
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getView(ClientSession session, ServiceQuery query, ChangeSubscriptionRequest csrq, IResponseData resp)
        {
            var type = query.GetParam("type", "viewOrder");

			var vs = new ViewSettings(query, type);

            if (type.Equals("viewOrder")) getViewViewOrder(session, query, vs, csrq, resp);
            else if (type.Equals("site")) getViewSite(session, query, vs, csrq, resp);
            else if (type.Equals("building")) getViewBuilding(session, query, vs, csrq, resp);
            else if (type.Equals("suite")) getViewSuite(session, query, vs, csrq, resp);
            else if (type.Equals("geo")) getViewGeo(session, query, vs, csrq, resp);
            else throw new NotImplementedException();
        }

        private static void getViewViewOrder(ClientSession session, ServiceQuery query, 
			ViewSettings vs, ChangeSubscriptionRequest csrq, IResponseData resp)
        {
            ViewOrder viewOrder;
            var viewOrderValid = false;

            using (var tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
            {
                viewOrder = RetrieveViewOrder(session.DbSession, query["id"], false);

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
                    getViewSuiteViewOrder(session, viewOrder, vs, csrq, resp);
                    break;

                case ViewOrder.SubjectType.Building:
                    getViewBuildingViewOrder(session, viewOrder, vs, csrq, resp);
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

            generateViewResponse(session.DbSession,
                null, new Building[] { building }, null, building.Suites, viewOrders, viewOrder.AutoID,
                resp, vs, false);
        }

        private static void getViewSuiteViewOrder(ClientSession session, ViewOrder viewOrder, 
			ViewSettings vs, ChangeSubscriptionRequest csrq, IResponseData resp)
        {
            // TODO: make this variable
            const double defaultProximityQuadradiusM = 1000.0;

            IList<Building> buildings = null;
            IList<ViewOrder> viewOrders;
            IList<Suite> suites = null;
            Suite suite;

            using (var dao = new SuiteDao(session.DbSession))
                suite = dao.GetById(viewOrder.TargetObjectId);

            if (null == suite) throw new FileNotFoundException("Unknown object listed");

            if (viewOrder.Product == ViewOrder.ViewOrderProduct.PublicListing)
            {
                var buildingIds = ServiceInstances.ModelCache.BuildingsByGeoProximity(suite.Building, defaultProximityQuadradiusM);

                ServiceInstances.Logger.Debug("dSPVO: got {0} buildings in proximity.", buildingIds.Length);

                var devLock = suite.Building.ConstructionSite.Developer;
                buildings = new List<Building>();
                using (var dao = new BuildingDao(session.DbSession))
                {
                    foreach (var id in buildingIds)
                    {
                        var b = dao.GetById(id);
                        if (b != null)  // should never happen?
                            if (b.ConstructionSite.Developer.Equals(devLock))
                                buildings.Add(b);
                    }
                }

                ServiceInstances.Logger.Debug("dSPVO: got {0} live buildings.", buildings.Count);

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

            generateViewResponse(session.DbSession,
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
            string voId = query["relatedVoId"];
            if (voId != null)
            {
                ViewOrder vo = RetrieveViewOrder(session.DbSession, voId, false);
                if (vo.Enabled && (vo.ExpiresOn > DateTime.UtcNow))
                    devLock = extractDeveloperFromViewOrder(session, vo);
            }

            List<Building> buildings = new List<Building>();
            using (BuildingDao dao = new BuildingDao(session.DbSession))
            {
                foreach (int id in ServiceInstances.ModelCache.BuildingsByGeoProximity(cLon, cLat, sqRadM))
                {
                    Building b = dao.GetById(id);
                    if (!b.Deleted && ((null == devLock) || (b.ConstructionSite.Developer.Equals(devLock))))
                        buildings.Add(b);
                }
            }

            var suites = (from b in buildings from s in b.Suites where vs.SuiteStatusMatches(s) select s).ToList();

	        if (csrq != ChangeSubscriptionRequest.None) setChangeSubscription(session, buildings, suites, csrq);

            generateViewResponse(session.DbSession,
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

            generateViewResponse(session.DbSession,
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

            generateViewResponse(session.DbSession, 
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

            generateViewResponse(session.DbSession, new Suite[] { suite }, resp, vs, true);
        }

        private static void generateViewResponse(ISession dbSession,
			ICollection<Suite> suites,
            IResponseData resp,
            ViewSettings vs, bool minimizeOutput)
        {
            generateViewResponse(dbSession, null, null, null, suites, null, Guid.Empty, resp, vs, minimizeOutput);
        }

        private static void generateViewResponse(ISession dbSession,
			ICollection<Suite> suites,
			ICollection<ViewOrder> viewOrders,
            Guid primaryListingId,
            IResponseData resp,
            ViewSettings vs, bool minimizeOutput)
        {
            generateViewResponse(dbSession, null, null, null, suites, viewOrders, primaryListingId, resp, vs, minimizeOutput);
        }

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
			ICollection<Site> sites, ICollection<Building> buildings,
			ICollection<SuiteType> suiteTypes, ICollection<Suite> suites,
			ICollection<ViewOrder> viewOrders,
            Guid primaryListingId,
            IResponseData resp,
            ViewSettings vs, bool minimizeOutput)
        {
            generateViewResponse(dbSession, sites, buildings, suiteTypes, suites, viewOrders, primaryListingId, resp, vs, minimizeOutput,
                false);
        }

        private static void generateViewResponse(ISession dbSession,
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

			    ServiceInstances.ModelCache.FillWithModelInfo(s, false);
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
                    ServiceInstances.ModelCache.FillWithModelInfo(st, false);
                    ReferencedFileHelper.ConvertUrlsToAbsolute(st);
                    elements.Add(st.GetClientData());
                }
            }
            else
            {
                elements = new List<ClientData>(usedSuiteTypes.Count());
                foreach (SuiteType st in usedSuiteTypes)
                {
                    ServiceInstances.ModelCache.FillWithModelInfo(st, false);
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
                    ServiceInstances.ModelCache.FillWithModelInfo(b, false);
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
                    ServiceInstances.ModelCache.FillWithModelInfo(b, false);
                    ReferencedFileHelper.ConvertUrlsToAbsolute(b);
                    ClientData cd = b.GetClientData();
                    //if (maskSaleStatus) cd["status"] = "Selected";
                    cd.Add("address", AddressHelper.ConvertToReadableAddress(b, null));
                    elements.Add(cd);
                    usedSites.Add(b.ConstructionSite);
                }
            }
            resp.Data.Add("buildings", elements.ToArray());

            if (!tempEventMode)
            {
                if (sites != null)
                {
                    elements = new List<ClientData>(sites.Count());
                    foreach (Site s in sites)
                    {
                        if (!usedSites.Contains(s)) continue;
                        ServiceInstances.ModelCache.FillWithModelInfo(s, false);
                        ReferencedFileHelper.ConvertUrlsToAbsolute(s);
                        elements.Add(s.GetClientData());
                    }
                }
                else
                {
                    elements = new List<ClientData>(usedSites.Count());
                    foreach (Site s in usedSites)
                    {
                        ServiceInstances.ModelCache.FillWithModelInfo(s, false);
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

                        if (!vo.Enabled || (vo.ExpiresOn < now))
                        {
	                        cd.Add("reason", !vo.Enabled ? "disabled" : "expired");
	                        cd.Add("recoverUrl", string.Format(_disabledViewOrderRecoverUrl, vo.AutoID.ToString("N")));
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
            generateViewResponse(session, null, null, null, suites, null, Guid.Empty, response,
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

				if (type.Equals("viewOrder"))
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

				ShowSold = query.GetParam("showSold", ShowSold);
				ShowSold = query.GetParam("sp_s", ShowSold);
				ShowAvailable = query.GetParam("sp_sa", ShowAvailable);
				ShowOnHold = query.GetParam("sp_sh", ShowOnHold);
				ShowResaleAvailable = query.GetParam("sp_ra", ShowResaleAvailable);
				ShowRental = query.GetParam("sp_rr", ShowRental);
				ShowImported = query.GetParam("includeImported", ShowImported);
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
                            }

                        if (null == suite) continue;
                        if (suite.Building.AutoID != building.AutoID)
                        {
                            throw new InvalidDataException("The suite ID=" + suite.AutoID + 
                                " does not belong to building ID=" + building.AutoID);
                        }

	                    var price = suite.CurrentPrice;
	                    if (!suite.UpdateFromClient(suiteData)) continue;
	                    try
	                    {
		                    if (manager.UpdateSuite(suite))
		                    {
			                    updatedCnt++;

			                    if (price.HasValue && (price.Value.CompareTo(suite.CurrentPrice.Value) != 0))
				                    manager.LogNewSuitePrice(suite, (float)Convert.ToDouble(suite.CurrentPrice));
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

        private static void updateUser(ClientSession session, int userId, ClientData data, IResponseData resp)
        {
            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
            {
                using (UserManager manager = new UserManager(session))
                {
                    User user = manager.Get(userId);

                    if (user.UpdateFromClient(data))
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

        private static void updateViewOrder(ClientSession session, ServiceQuery query, string strObjectId, ClientData data, IResponseData resp)
        {
            string paymentSystemRefId = query["pr"];

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
            {
                ViewOrder viewOrder = RetrieveViewOrder(session.DbSession, strObjectId, true);

                User owner;
                using (UserDao dao = new UserDao(session.DbSession))
                    owner = dao.GetById(viewOrder.OwnerId);

                RolePermissionCheck.CheckUpdateViewOrder(session, owner);

                if (viewOrder.UpdateFromClient(data))
                {
                    viewOrder.MarkUpdated();

                    using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                        dao.Update(viewOrder);

                    // Generate financial transaction
                    //
                    FinancialTransaction.TranTarget tt = FinancialTransaction.TranTarget.Suite;
                    switch (viewOrder.TargetObjectType)
                    {
                        case ViewOrder.SubjectType.Building: tt = FinancialTransaction.TranTarget.Building; break;
                        case ViewOrder.SubjectType.Suite: tt = FinancialTransaction.TranTarget.Suite; break;
                    }
                    FinancialTransaction ft = new FinancialTransaction(session.User.AutoID,
                        FinancialTransaction.AccountType.User, viewOrder.OwnerId,
                        FinancialTransaction.OperationType.Debit, 0m,
                        FinancialTransaction.TranSubject.View,
                        tt, viewOrder.TargetObjectId, viewOrder.AutoID.ToString());

                    if (!string.IsNullOrWhiteSpace(paymentSystemRefId))
                        ft.SetPaymentSystemReference(FinancialTransaction.PaymentSystemType.CondoExplorer, paymentSystemRefId);

                    using (FinancialTransactionDao dao = new FinancialTransactionDao(session.DbSession))
                    {
                        dao.Create(ft);
                        ft.SetAutoSystemReferenceId();
                        dao.Update(ft);
                    }

                    ReflectViewOrderStatusInTarget(viewOrder, session.DbSession);

                    resp.ResponseCode = HttpStatusCode.OK;
                    resp.Data = new ClientData();
                    resp.Data.Add("ref", ft.SystemRefId);
                    resp.Data.Add("updated", 1);
                }
                else
                {
                    resp.ResponseCode = HttpStatusCode.NotModified;
                    resp.Data = new ClientData();
                    resp.Data.Add("updated", 0);
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
            int estateDeveloperId = ResolveDeveloperId(session.DbSession, data.GetProperty("ed", string.Empty));
            string login = data.GetProperty("uid", string.Empty);
            string password = data.GetProperty("pwd", string.Empty);

            using (UserManager manager = new UserManager(session))
            {
                manager.Create(role, estateDeveloperId, type, login, password);
                try
                {
                    // create contact info block with any added fields from inbound JSON
                    User u = manager.Get(type, role, estateDeveloperId, login);
                    u.UpdateFromClient(data);
                    resp.ResponseCode = HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    resp.ResponseCode = HttpStatusCode.Created;
                    resp.ResponseCodeDescription = "Contact information was not stored.";
                    ServiceInstances.Logger.Error("Contact information for created user {0}[{1}] was not saved: {2}", type, login, ex);
                }
            }
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
                        foreach (ViewOrder vo in dao.Get(session.User.AutoID))
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
        #endregion

        #region extra methods which should go to manager class (?)
        private static EstateDeveloper extractDeveloperFromViewOrder(ClientSession session, ViewOrder vo)
        {
            EstateDeveloper result = null;

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

            if ((null == viewOrder) || (viewOrder.Deleted && !canBeDeleted)) throw new FileNotFoundException("Undefined view order");

            return viewOrder;
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
    }
}
