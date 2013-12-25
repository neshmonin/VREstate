using System;
using NHibernate;
using Vre.Server.Dao;
using System.IO;
using System.Diagnostics;
using Vre.Server.RemoteService;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    internal class DeveloperManager : GenericManager
    {
        public DeveloperManager(ClientSession clientSession) : base(clientSession) { }

        public EstateDeveloper Create(EstateDeveloper.Configuration configuration, string name)
        {
            RolePermissionCheck.CheckCreateEstateDeveloper(_session, configuration, name);

            using (EstateDeveloperDao dao = new EstateDeveloperDao(_session.DbSession))
            {
                EstateDeveloper ed = new EstateDeveloper(configuration);
                ed.Name = name;
                dao.Create(ed);
                //dao.Flush();
                ServiceInstances.Logger.Info("Estate developer type <{0}> created (ID={1}).", configuration, ed.AutoID);
                return ed;
            }
        }

        public EstateDeveloper[] List(bool includeDeleted)
        {
            RolePermissionCheck.CheckListEstateDevelopers(_session);

            using (EstateDeveloperDao dao = new EstateDeveloperDao(_session.DbSession))
            {
                IList<EstateDeveloper> info = dao.GetAll();
                List<EstateDeveloper> result = new List<EstateDeveloper>(info.Count);

                foreach (EstateDeveloper ed in info) 
                    if (!ed.Deleted || includeDeleted) result.Add(ed);

                return result.ToArray();
            }
        }

        public void Delete(EstateDeveloper estateDeveloper)
        {
            RolePermissionCheck.CheckDeleteEstateDeveloper(_session, estateDeveloper);

            using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                int users = 0, buildings = 0;
                using (EstateDeveloperDao dao = new EstateDeveloperDao(_session.DbSession))
                {
                    dao.Delete(estateDeveloper);
                }
                using (UserDao dao = new UserDao(_session.DbSession))
                {
                    using (UserManager um = new UserManager(_session))
                    {
                        foreach (User.Role r in Enum.GetValues(typeof(User.Role)))
                        {
                            if (!User.IsEstateDeveloperTied(r)) continue;
                            //string errorReason;
                            foreach (User u in dao.ListUsers(r, estateDeveloper.AutoID, null, null, false))
                            {
                                um.Delete(u);
                            }
                            //if (Delete(u, out errorReason)) users++;

                            //else throw new Exception("Failed to delete user: " + errorReason);
                        }
                    }
                    //users = dao.DeleteUsers(estateDeveloper.AutoID);
                }
                // no need to delete these: when estate developer is marked as deleted these became invisible for browsing
                //foreach (Site s in estateDeveloper.Sites)
                //    using (BuildingDao dao = new BuildingDao(_session))
                //{
                //    buildings = dao.DeleteBuildings(estateDeveloper.AutoID);
                //}
                tran.Commit();
                ServiceInstances.Logger.Info("Estate developer ID={0} deleted; {1} users and {2} buildings affected.", 
                    estateDeveloper.AutoID, users, buildings);
            }
        }
    }
}