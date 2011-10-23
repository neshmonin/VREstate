using NHibernate;
using Vre.Server.BusinessLogic;
using System.Collections.Generic;
using NHibernate.Criterion;

namespace Vre.Server.Dao
{
    public class RoomCategoryDao : GenericDisposableDao<RoomCategory, int>
    {
        public RoomCategoryDao(ISession session) : base(session) { }
        public RoomCategory CreateOrGetByName(string categoryName)
        {
            RoomCategory result;
            bool changed = false;

            lock (_session)
            {
                using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_session))
                {
                    ICriteria c = _session.CreateCriteria<RoomCategory>().Add(Restrictions.Eq("Name", categoryName));
                    IList<RoomCategory> rcl = c.List<RoomCategory>();

                    if (rcl.Count > 0)
                    {
                        result = rcl[0];
                    }
                    else
                    {
                        result = new RoomCategory(categoryName);
                        _session.Save(result);
                        changed = true;
                    }

                    if (changed)
                    {
                        if (_forcedFlush) _session.Flush();
                        tran.Commit();
                    }
                }
            }

            return result;
        }
    }
}