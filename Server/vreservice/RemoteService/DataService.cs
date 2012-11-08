using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Vre.Server.BusinessLogic;
using Vre.Server.BusinessLogic.Client;
using System.Diagnostics;
using Vre.Server.Dao;

namespace Vre.Server.RemoteService
{
    internal class DataService
    {
        private static bool _configured = false;
        private static bool _allowUnsecureService = true;

        public const string ServicePathPrefix = ServicePathElement0 + "/";
        private const string ServicePathElement0 = "data";

        enum ModelObject { User, EstateDeveloper, Site, Building, Suite, SuiteType, ViewOrder, View, FinancialTransaction }

        private static void configure()
        {
            _allowUnsecureService = ServiceInstances.Configuration.GetValue("AllowSensitiveDataOverNonSecureConnection", false);

            _configured = true;
        }

        public static void ProcessGetRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;
            string strObjectId;
            long generation;
            bool includeDeleted;

            if (!_configured) configure();

            if (!_allowUnsecureService && !request.UserInfo.Session.TrustedConnection) 
                throw new PermissionException("Service available only over secure connection.");

            getPathElements(request.Request.Path, out mo, out objectId, out strObjectId);

            if (!long.TryParse(request.Request.Query.GetParam("genval", "0"), out generation)) generation = 0;
            includeDeleted = request.Request.Query.GetParam("withdeleted", "false").Equals("true");

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
                        int estateDeveloperId = request.Request.Query.GetParam("ed", -1);
                        if (-1 == estateDeveloperId) throw new ArgumentException("Developer ID is missing.");
                        getSiteList(request.UserInfo.Session, estateDeveloperId, request.Response, generation, includeDeleted);
                    }
                    else
                    {
                        getBuilding(request.UserInfo.Session, objectId, request.Response, generation);
                    }
                    return;

                case ModelObject.Building:
                    if (-1 == objectId)
                    {
                        getBuildingList(request.UserInfo.Session, request.Request.Query, request.Response, generation, includeDeleted);
                    }
                    else
                    {
                        getBuilding(request.UserInfo.Session, objectId, request.Response, generation);
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
                            getSuiteList(request.UserInfo.Session, buildingId, request.Response, generation, filter);
                        else
                            getSuiteList(request.UserInfo.Session, buildingId, request.Response, generation, null);
                    }
                    else
                    {
                        getSuite(request.UserInfo.Session, objectId, request.Response, generation);
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
                        getViewOrder(request.UserInfo.Session, strObjectId, request.Response);
                    }
                    return;

                case ModelObject.View:
                    getView(request.UserInfo.Session, request.Request.Query, request.Response);
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
                    updateViewOrder(request.UserInfo.Session, strObjectId, request.Request.Data, request.Response);
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

        private static void getSiteList(ClientSession session, int developerId, IResponseData resp, 
            long generation, bool includeDeleted)
        {
            ClientData[] result = null;

            Spikes.PullUpdateService.UpdateInfo updateInfo =
                ServiceInstances.UpdateService.GetUpdate(Spikes.PullUpdateService.EntityLevel.Developer, developerId, generation);

            using (SiteManager manager = new SiteManager(session))
            {
                Site[] siteList = manager.List(developerId, includeDeleted);

                if (0 == generation)  // full request
                {
                    int cnt = siteList.Length;
                    result = new ClientData[cnt];
                    for (int idx = 0; idx < cnt; idx++)
                    {
                        Site s = siteList[idx];
                        ServiceInstances.ModelCache.FillWithModelInfo(s, false);
                        result[idx] = s.GetClientData();
                    }
                }
                else if (updateInfo.Sites != null)  // changed item list
                {
                    int cnt = updateInfo.Sites.Count;
                    List<ClientData> siteDataList = new List<ClientData>(cnt);
                    foreach (Site s in siteList)
                    {
                        if (updateInfo.Sites.Contains(s.AutoID))
                        {
                            ServiceInstances.ModelCache.FillWithModelInfo(s, false);
                            siteDataList.Add(s.GetClientData());
                        }
                    }
                    result = siteDataList.ToArray();
                }
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Add("sites", result);
            resp.Data.Add("generation", updateInfo.Generation);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getBuildingList(ClientSession session, ServiceQuery query, IResponseData resp, 
            long generation, bool includeDeleted)
        {
            List<Building> toReturn = null;
            ClientData[] result = null;
            long resGeneration = -1;

            string scopeType = query.GetParam("scopeType", "site");

            if (scopeType.Equals("address"))
            {
                toReturn = new List<Building>();
                foreach (UpdateableBase u in AddressHelper.ParseGeographicalAddressToModel(query, session.DbSession))
                {
                    Building b = u as Building;
                    if (b != null) toReturn.Add(b);
                }
            }
            else if (scopeType.Equals("site"))
            {
                int siteId = query.GetParam("site", -1);
                if (-1 == siteId) throw new ArgumentException("Site ID is missing.");

                Spikes.PullUpdateService.UpdateInfo updateInfo =
                    ServiceInstances.UpdateService.GetUpdate(Spikes.PullUpdateService.EntityLevel.Site, siteId, generation);

                using (SiteManager manager = new SiteManager(session))
                {
                    Building[] buildingList = manager.ListBuildings(siteId, includeDeleted);

                    if (0 == generation)  // full request
                    {
                        toReturn = new List<Building>(buildingList);
                    }
                    else if (updateInfo.Buildings != null)  // changed item list
                    {
                        toReturn = new List<Building>(updateInfo.Buildings.Count);
                        foreach (Building b in buildingList)
                        {
                            if (updateInfo.Buildings.Contains(b.AutoID)) toReturn.Add(b);
                        }
                    }
                }

                resGeneration = updateInfo.Generation;
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
                    result[idx] = b.GetClientData();
                }
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Add("buildings", result);
            if (resGeneration >= 0) resp.Data.Add("generation", resGeneration);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getBuilding(ClientSession session, int buildingId, IResponseData resp, long generation)
        {
            ClientData result = null;

            Spikes.PullUpdateService.UpdateInfo updateInfo =
                ServiceInstances.UpdateService.GetUpdate(Spikes.PullUpdateService.EntityLevel.Building, buildingId, generation);

            using (SiteManager manager = new SiteManager(session))
            {
                Building building = manager.GetBuildingById(buildingId);

                if (0 == generation)  // full request
                {
                    ServiceInstances.ModelCache.FillWithModelInfo(building, false);
                    result = building.GetClientData();
                }
                else if (updateInfo.Buildings != null)  // changed item list
                {
                    if (updateInfo.Buildings.Contains(building.AutoID))
                    {
                        ServiceInstances.ModelCache.FillWithModelInfo(building, false);
                        result = building.GetClientData();
                    }
                }
            }            

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Merge(result);
            resp.Data.Add("generation", updateInfo.Generation);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getSuiteList(ClientSession session, int buildingId, IResponseData resp, long generation,
            Suite.SalesStatus? statusFilter)
        {
            List<ClientData> result = null;

            Spikes.PullUpdateService.UpdateInfo updateInfo =
                ServiceInstances.UpdateService.GetUpdate(Spikes.PullUpdateService.EntityLevel.Building, buildingId, generation);

            using (SiteManager manager = new SiteManager(session))
            {
                Suite[] suiteList = manager.ListSuitesByBuiding(buildingId);

                if (0 == generation)  // full request
                {
                    int cnt = suiteList.Length;
                    result = new List<ClientData>(cnt);
                    for (int idx = 0; idx < cnt; idx++)
                    {
                        Suite s = suiteList[idx];
                        insertSuiteIntoResult(statusFilter, result, manager, s);
                    }
                }
                else if (updateInfo.Suites != null)  // changed item list
                {
                    int cnt = updateInfo.Suites.Count;
                    result = new List<ClientData>(cnt);
                    foreach (Suite s in suiteList)
                    {
                        if (updateInfo.Suites.Contains(s.AutoID))
                            insertSuiteIntoResult(statusFilter, result, manager, s);
                    }
                }
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Add("suites", NHibernateHelper.IListToArray<ClientData>(result));
            resp.Data.Add("generation", updateInfo.Generation);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void insertSuiteIntoResult(Suite.SalesStatus? statusFilter, List<ClientData> result, 
            SiteManager manager, Suite s)
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
                result.Add(SuiteEx.GetClientData(s, manager.GetCurrentSuitePrice(s)));
            }
        }

        private static void getSuite(ClientSession session, int suiteId, IResponseData resp, long generation)
        {
            ClientData result = null;

            Spikes.PullUpdateService.UpdateInfo updateInfo =
                ServiceInstances.UpdateService.GetUpdate(Spikes.PullUpdateService.EntityLevel.Suite, suiteId, generation);

            using (SiteManager manager = new SiteManager(session))
            {
                Suite suite = manager.GetSuiteById(suiteId);

                if (0 == generation)  // full request
                {
                    ServiceInstances.ModelCache.FillWithModelInfo(suite, false);
                    result = SuiteEx.GetClientData(suite, manager.GetCurrentSuitePrice(suite));
                }
                else if (updateInfo.Suites != null)  // changed item list
                {
                    if (updateInfo.Suites.Contains(suite.AutoID))
                    {
                        ServiceInstances.ModelCache.FillWithModelInfo(suite, false);
                        result = SuiteEx.GetClientData(suite, manager.GetCurrentSuitePrice(suite));
                    }
                }
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Merge(result);
            resp.Data.Add("generation", updateInfo.Generation);
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getSuiteType(ClientSession session, int siteId, string name, IResponseData resp)
        {
            SuiteType st;

            using (SiteManager manager = new SiteManager(session))
            {
                st = manager.GetSuiteTypeByName(siteId, name);
            }

            ServiceInstances.ModelCache.FillWithModelInfo(st, false);
            UrlHelper.ConvertUrlsToAbsolute(st);

            // produce output
            //
            resp.Data = st.GetClientData();
            resp.ResponseCode = HttpStatusCode.OK;
        }

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
                UrlHelper.ConvertUrlsToAbsolute(st);
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
            
            resp.Data = user.GetClientData();
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getUserList(ClientSession session, ServiceQuery query, IResponseData resp, bool includeDeleted)
        {
            ClientData[] result;

            if (query.GetParam("sellerMode", "false").Equals("true"))
            {
                session.Resume();

                User[] list = session.User.CanView.ToArray();

                int cnt = list.Length;
                result = new ClientData[cnt];
                for (int idx = 0; idx < cnt; idx++)
                    result[idx] = list[idx].GetClientData();

                session.Disconnect();
            }
            else
            {
                User.Role role;
                if (!Enum.TryParse<User.Role>(query.GetParam("role", "buyer"), true, out role)) role = User.Role.Buyer;
                int estateDeveloperId = query.GetParam("ed", -1);// data.GetProperty("ed", -1);
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

                        // retrieve associated login information (which is not a part of user record)
                        //
                        LoginType lt;
                        User.Role ur;
                        int ed;
                        string login;
                        if (auth.LoginByUserId(list[idx].AutoID, out lt, out ur, out ed, out login))
                        {
                            result[idx].Add("loginType", lt);
                            result[idx].Add("login", login);
                        }
                    }
                }
            }

            resp.Data = new ClientData();
            resp.Data.Add("users", result);
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

        private static void getViewOrder(ClientSession session, string strObjectId, IResponseData resp)
        {
            Guid rqid;
            if (!Guid.TryParseExact(strObjectId, "N", out rqid))
                throw new ArgumentException();

            ViewOrder viewOrder;
            using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                viewOrder = dao.GetById(rqid);

            if (null == viewOrder) throw new FileNotFoundException("Listing does not exist");

            resp.Data = viewOrder.GetClientData();
            resp.Data.Add("viewOrder-url", ReverseRequestService.ConstructViewOrderUrl(viewOrder));
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getViewOrderList(ClientSession session, ServiceQuery query, IResponseData resp, bool includeDeleted)
        {
            int userId = query.GetParam("userId", -1);
            User user;
            ViewOrder[] list;

            using (UserDao dao = new UserDao(session.DbSession))
                user = dao.GetById(userId);

            if (null == user) throw new FileNotFoundException("User does not exist");

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
            int cnt = list.Length;
            List<ClientData> result = new List<ClientData>(cnt);
            for (int idx = 0; idx < cnt; idx++)
            {
                ViewOrder vo = list[idx];
                if (vo.ExpiresOn < timeLim)
                {
                    ClientData cd = vo.GetClientData();
                    cd.Add("viewOrder-url", ReverseRequestService.ConstructViewOrderUrl(vo));
                    result.Add(cd);
                }
            }

            resp.Data = new ClientData();
            resp.Data.Add("viewOrders", result.ToArray());
            resp.ResponseCode = HttpStatusCode.OK;
        }

        private static void getView(ClientSession session, ServiceQuery query, IResponseData resp)
        {
            string type = query.GetParam("type", "viewOrder");

            if (type.Equals("viewOrder")) getViewViewOrder(session, query, resp);
            else if (type.Equals("site")) getViewSite(session, query, resp);
            else if (type.Equals("building")) throw new NotImplementedException();
            else if (type.Equals("suite")) throw new NotImplementedException();
            else if (type.Equals("geo")) throw new NotImplementedException();
            else throw new NotImplementedException();
        }

        private static void getViewViewOrder(ClientSession session, ServiceQuery query, IResponseData resp)
        {
            string strObjectId = query["id"];
            if (null == strObjectId) throw new ArgumentException("Object ID missing.");

            Guid rqid;
            if (!Guid.TryParseExact(strObjectId, "N", out rqid))
                throw new ArgumentException();

            ViewOrder viewOrder;

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
            {
                using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                    viewOrder = dao.GetById(rqid);

                if ((null == viewOrder) || !viewOrder.Enabled || (viewOrder.ExpiresOn < DateTime.UtcNow)) throw new FileNotFoundException("Undefined or expired view order");

                viewOrder.Touch();
                using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                    dao.Update(viewOrder);

                tran.Commit();
            }

            Suite suite;
            using (SuiteDao dao = new SuiteDao(session.DbSession))
                suite = dao.GetById(viewOrder.TargetObjectId);

            if (null == suite) throw new FileNotFoundException("Unknown object listed");

            generateViewResponse(
                new Site[] { suite.Building.ConstructionSite },
                new Building[] { suite.Building },
                new SuiteType[] { suite.SuiteType },
                new Suite[] { suite },
                new ViewOrder[] { viewOrder },
                viewOrder.AutoID,
                resp);
        }

        private static void getViewSite(ClientSession session, ServiceQuery query, IResponseData resp)
        {
            int objectId = query.GetParam("id", -1);
            if (objectId < 0) throw new ArgumentException("Object ID missing.");

            Site site;
            using (SiteDao dao = new SiteDao(session.DbSession))
                site = dao.GetById(objectId);

            if (null == site) throw new FileNotFoundException("Unknown site");

            List<Suite> suites = new List<Suite>();
            foreach (Building b in site.Buildings) suites.AddRange(b.Suites);

            generateViewResponse(
                new Site[] { site },
                site.Buildings.ToArray(),  
                site.SuiteTypes.ToArray(),
                suites.ToArray(),
                new ViewOrder[0],
                Guid.Empty,
                resp);
        }

        private static void generateViewResponse(
            IEnumerable<Site> sites, IEnumerable<Building> buildings, 
            IEnumerable<SuiteType> suiteTypes, IEnumerable<Suite> suites,
            IEnumerable<ViewOrder> viewOrders,
            Guid primaryListingId,
            IResponseData resp)
        {
            resp.Data = new ClientData();

            List<ClientData> elements;
                
            elements = new List<ClientData>(suites.Count());
            foreach (Suite s in suites)
            {
                ServiceInstances.ModelCache.FillWithModelInfo(s, false);
                elements.Add(s.GetClientData());
            }
            resp.Data.Add("suites", elements.ToArray());

            elements = new List<ClientData>(suiteTypes.Count());
            foreach (SuiteType st in suiteTypes)
            {
                ServiceInstances.ModelCache.FillWithModelInfo(st, false);
                UrlHelper.ConvertUrlsToAbsolute(st);
                elements.Add(st.GetClientData());
            }
            resp.Data.Add("suiteTypes", elements.ToArray());

            elements = new List<ClientData>(buildings.Count());
            foreach (Building b in buildings)
            {
                ServiceInstances.ModelCache.FillWithModelInfo(b, false);
                ClientData cd = b.GetClientData();
                cd.Add("address", AddressHelper.ConvertToReadableAddress(b, null));
                elements.Add(cd);
            }
            resp.Data.Add("buildings", elements.ToArray());

            elements = new List<ClientData>(sites.Count());
            foreach (Site s in sites)
            {
                ServiceInstances.ModelCache.FillWithModelInfo(s, false);
                elements.Add(s.GetClientData());
            }
            resp.Data.Add("sites", elements.ToArray());
            
            // Cannot reuse viewOrder.GetClientData() here as it exposes too much information
            elements = new List<ClientData>(viewOrders.Count());
            foreach (ViewOrder l in viewOrders)
            {
                ClientData cd = new ClientData();
                cd.Add("id", l.AutoID);
                cd.Add("suiteId", l.TargetObjectId);  // TODO: now Suites only!!!
                cd.Add("product", ClientData.ConvertProperty<ViewOrder.ViewOrderType>(l.Product));
                cd.Add("mlsId", l.MlsId);
                cd.Add("productUrl", l.ProductUrl);
                elements.Add(cd);
            }
            resp.Data.Add("viewOrders", elements.ToArray());

            if (!primaryListingId.Equals(Guid.Empty)) resp.Data.Add("primaryViewOrderId", primaryListingId);
            resp.Data.Add("initialView", "");  // TODO

            resp.ResponseCode = HttpStatusCode.OK;
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

                        bool updated = false, canUpdate = true;
                        if (suite.UpdateFromClient(suiteData))
                        {
                            try
                            {
                                if (manager.UpdateSuite(suite))
                                {
                                    updatedCnt++;
                                    updated = true;
                                }
                                else
                                {
                                    staleIds.Add(suite.AutoID);
                                    error = "At least one object is stale.";
                                    canUpdate = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                error = string.Format("Cannot update suite {0} (ID={1}): {2}", suite.SuiteName, suite.AutoID, ex.Message);
                                ServiceInstances.Logger.Error("Cannot update suite ID={0}: {1}", suite.AutoID, Utilities.ExplodeException(ex));
                                break;
                            }
                        }

                        // update price
                        if (canUpdate)
                        {
                            SuiteEx suiteEx = new SuiteEx(suite, manager.GetCurrentSuitePrice(suite));
                            if (suiteEx.UpdateFromClient(suiteData))
                            {
                                if (manager.SetSuitePrice(suite, (float)suiteEx.CurrentPrice))
                                {
                                    if (!updated) updatedCnt++;  // increment counter if not done above
                                }
                                else
                                {
                                    staleIds.Add(suite.AutoID);
                                    error = "At least one object is stale.";
                                }
                            }
                        }
                        
                        //double pp = manager.GetCurrentSuitePrice(suite);
                        //double ip = suiteData.GetProperty("currentPrice", -1.0);
                        //if ((ip > 0.0) && (ip != pp) && canUpdate)
                        //{
                        //    manager.SetSuitePrice(session.User, suite, (float)ip);
                        //    if (!updated) updatedCnt++;
                        //}
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
                resp.Data = new ClientData();
                resp.Data.Add("updated", updatedCnt);
            }
            else
            {
                resp.ResponseCode = HttpStatusCode.Conflict;
                resp.ResponseCodeDescription = error;
                resp.Data = new ClientData();
                resp.Data.Add("updated", 0);
                if (staleIds.Count > 0) resp.Data.Add("staleIds", Utilities.ToCsv<int>(staleIds));
            }
        }

        private static void updateUser(ClientSession session, int userId, ClientData data, IResponseData resp)
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
        }

        private static void updateViewOrder(ClientSession session, string strObjectId, ClientData data, IResponseData resp)
        {
            Guid rqid;
            if (!Guid.TryParseExact(strObjectId, "N", out rqid))
                throw new ArgumentException();

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
            {
                ViewOrder viewOrder;
                using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                    viewOrder = dao.GetById(rqid);

                if (null == viewOrder) throw new FileNotFoundException("Undefined view order");

                User owner;
                using (UserDao dao = new UserDao(session.DbSession))
                    owner = dao.GetById(viewOrder.OwnerId);

                RolePermissionCheck.CheckUpdateViewOrder(session, owner);

                if (viewOrder.UpdateFromClient(data))
                {
                    viewOrder.MarkUpdated();

                    using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                        dao.Update(viewOrder);

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

                tran.Commit();
            }
        }
        #endregion

        #region create
        private static void createUser(ClientSession session, ClientData data, IResponseData resp)
        {
            User.Role role = data.GetProperty<User.Role>("role", User.Role.Visitor);
            LoginType type = data.GetProperty<LoginType>("type", LoginType.Plain);
            int estateDeveloperId = data.GetProperty("ed", -1);
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

            using (UserManager manager = new UserManager(session))
            {
                user = manager.Get(userId);

                manager.Delete(user);
                resp.ResponseCode = HttpStatusCode.OK;
                resp.Data = new ClientData();
                resp.Data.Add("deleted", 1);
            }
        }

        private static void deleteViewOrder(ClientSession session, string strObjectId, IResponseData resp)
        {
            Guid rqid;
            if (!Guid.TryParseExact(strObjectId, "N", out rqid))
                throw new ArgumentException();

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session.DbSession))
            {
                ViewOrder viewOrder;
                using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                    viewOrder = dao.GetById(rqid);

                if (null == viewOrder) throw new FileNotFoundException("Undefined view order");

                User owner;
                using (UserDao dao = new UserDao(session.DbSession))
                    owner = dao.GetById(viewOrder.OwnerId);

                RolePermissionCheck.CheckDeleteViewOrder(session, owner);

                viewOrder.MarkDeleted();
                using (ViewOrderDao dao = new ViewOrderDao(session.DbSession))
                    dao.Update(viewOrder);

                tran.Commit();
            }

            resp.ResponseCode = HttpStatusCode.OK;
            resp.Data = new ClientData();
            resp.Data.Add("deleted", 1);
        }
        #endregion
    }
}
