using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Vre.Server.BusinessLogic;
using Vre.Server.BusinessLogic.Client;

namespace Vre.Server.RemoteService
{
    internal class DataService
    {
        private static bool _configured = false;
        private static bool _allowUnsecureService = true;

        public const string ServicePathPrefix = ServicePathElement0 + "/";
        private const string ServicePathElement0 = "data";

        enum ModelObject { User, EstateDeveloper, Site, Building, Suite, SuiteType }

        private static void configure()
        {
            _allowUnsecureService = ServiceInstances.Configuration.GetValue("AllowSensitiveDataOverNonSecureConnection", false);
            _configured = true;
        }

        public static void ProcessGetRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;
            long generation;
            bool includeDeleted;

            if (!_configured) configure();

            if (!_allowUnsecureService && !request.UserInfo.Session.TrustedConnection) 
                throw new PermissionException("Service available only over secure connection.");

            getPathElements(request.Request.Path, out mo, out objectId);

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
                        int siteId = request.Request.Query.GetParam("site", -1);
                        if (-1 == siteId) throw new ArgumentException("Site ID is missing.");
                        getBuildingList(request.UserInfo.Session, siteId, request.Response, generation, includeDeleted);
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
                        getSuiteList(request.UserInfo.Session, buildingId, request.Response, generation);
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
            }

            throw new NotImplementedException();
        }

        public static void ProcessReplaceRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;

            if (!_configured) configure();

            if (!request.UserInfo.Session.TrustedConnection) throw new PermissionException("Service available only over secure connection.");

            getPathElements(request.Request.Path, out mo, out objectId);
            if (-1 == objectId) throw new ArgumentException("Object ID is missing.");

            if (null == request.Request.Data) throw new ArgumentException("Object data not passed.");

            switch (mo)
            {
                case ModelObject.Building:
                    updateBuilding(request.UserInfo.Session, objectId, request.Request.Data, request.Response);
                    return;

                case ModelObject.User:
                    updateUser(request.UserInfo.Session, objectId, request.Request.Data, request.Response);
                    return;
            }

            throw new NotImplementedException();
        }

        public static void ProcessCreateRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;

            if (!_configured) configure();

            if (!request.UserInfo.Session.TrustedConnection) throw new PermissionException("Service available only over secure connection.");

            getPathElements(request.Request.Path, out mo, out objectId);

            if (null == request.Request.Data) throw new ArgumentException("Object data not passed.");

            switch (mo)
            {
                case ModelObject.User:
                    createUser(request.UserInfo.Session, request.Request.Data, request.Response);
                    return;
            }

            throw new NotImplementedException();
        }

        public static void ProcessDeleteRequest(IServiceRequest request)
        {
            ModelObject mo;
            int objectId;

            if (!_configured) configure();

            if (!request.UserInfo.Session.TrustedConnection) throw new PermissionException("Service available only over secure connection.");

            getPathElements(request.Request.Path, out mo, out objectId);
            if (-1 == objectId) throw new ArgumentException("Object ID is missing.");

            switch (mo)
            {
                case ModelObject.User:
                    deleteUser(request.UserInfo.Session, objectId, request.Response);
                    return;
            }

            throw new NotImplementedException();
        }

        private static void getPathElements(string path, out ModelObject mo, out int id)
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
            else throw new ArgumentException("Object path is invalid (2).");

            if (2 == elements.Length)
            {
                id = -1;
            }
            else
            {
                if (!int.TryParse(elements[2], out id)) throw new ArgumentException("Object path is invalid (3).");
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

        private static void getBuildingList(ClientSession session, int siteId, IResponseData resp, 
            long generation, bool includeDeleted)
        {
            ClientData[] result = null;

            Spikes.PullUpdateService.UpdateInfo updateInfo =
                ServiceInstances.UpdateService.GetUpdate(Spikes.PullUpdateService.EntityLevel.Site, siteId, generation);

            using (SiteManager manager = new SiteManager(session))
            {
                Building[] buildingList = manager.ListBuildings(siteId, includeDeleted);

                if (0 == generation)  // full request
                {
                    int cnt = buildingList.Length;
                    result = new ClientData[cnt];
                    for (int idx = 0; idx < cnt; idx++)
                    {
                        Building b = buildingList[idx];
                        ServiceInstances.ModelCache.FillWithModelInfo(b, false);
                        result[idx] = b.GetClientData();
                    }
                }
                else if (updateInfo.Buildings != null)  // changed item list
                {
                    int cnt = updateInfo.Buildings.Count;
                    List<ClientData> buildingDataList = new List<ClientData>(cnt);
                    foreach (Building b in buildingList)
                    {
                        if (updateInfo.Buildings.Contains(b.AutoID))
                        {
                            ServiceInstances.ModelCache.FillWithModelInfo(b, false);
                            buildingDataList.Add(b.GetClientData());
                        }
                    }
                    result = buildingDataList.ToArray();
                }
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Add("buildings", result);
            resp.Data.Add("generation", updateInfo.Generation);
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

        private static void getSuiteList(ClientSession session, int buildingId, IResponseData resp, long generation)
        {
            ClientData[] result = null;

            Spikes.PullUpdateService.UpdateInfo updateInfo =
                ServiceInstances.UpdateService.GetUpdate(Spikes.PullUpdateService.EntityLevel.Building, buildingId, generation);

            using (SiteManager manager = new SiteManager(session))
            {
                Suite[] suiteList = manager.ListSuitesByBuiding(buildingId);

                if (0 == generation)  // full request
                {
                    int cnt = suiteList.Length;
                    result = new ClientData[cnt];
                    for (int idx = 0; idx < cnt; idx++)
                    {
                        Suite s = suiteList[idx];
                        ServiceInstances.ModelCache.FillWithModelInfo(s, false);
                        result[idx] = SuiteEx.GetClientData(s, manager.GetCurrentSuitePrice(s));
                    }
                }
                else if (updateInfo.Suites != null)  // changed item list
                {
                    int cnt = updateInfo.Suites.Count;
                    List<ClientData> suiteDataList = new List<ClientData>(cnt);
                    foreach (Suite s in suiteList)
                    {
                        if (updateInfo.Suites.Contains(s.AutoID))
                        {
                            ServiceInstances.ModelCache.FillWithModelInfo(s, false);
                            suiteDataList.Add(SuiteEx.GetClientData(s, manager.GetCurrentSuitePrice(s)));
                        }
                    }
                    result = suiteDataList.ToArray();
                }
            }

            // produce output
            //
            resp.Data = new ClientData();
            if (result != null) resp.Data.Add("suites", result);
            resp.Data.Add("generation", updateInfo.Generation);
            resp.ResponseCode = HttpStatusCode.OK;
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

            // TODO ?!
            //ServiceInstances.ModelCache.FillWithModelInfo(suite, false);

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

            foreach (SuiteType st in list) ServiceInstances.ModelCache.FillWithModelInfo(st, false);

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
            User.Role role;
            if (!Enum.TryParse<User.Role>(query.GetParam("role", "buyer"), true, out role)) role = User.Role.Buyer;
            int estateDeveloperId = query.GetParam("ed", -1);// data.GetProperty("ed", -1);
            string nameLookup = query.GetParam("nameFilter", string.Empty);// data.GetProperty("nameFilter", string.Empty);
            User[] list;

            using (UserManager manager = new UserManager(session))
            {
                list = manager.List(role, estateDeveloperId, nameLookup, includeDeleted);
            }

            // produce output
            //
            int cnt = list.Length;
            ClientData[] result = new ClientData[cnt];
            using (IAuthentication auth = new Authentication(session.DbSession))
            {
                for (int idx = 0; idx < cnt; idx++)
                {
                    result[idx] = list[idx].GetClientData();

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

            resp.Data = new ClientData();
            resp.Data.Add("users", result);
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

                    // this shall throw out if user has no right to modify building info
                    manager.TestUserCanUpdate(building);

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
        #endregion

        #region create
        private static void createUser(ClientSession session, ClientData data, IResponseData resp)
        {
            User.Role role = data.GetProperty<User.Role>("role", User.Role.Buyer);
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
        #endregion
    }
}