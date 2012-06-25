using System;
using NHibernate;
using Vre.Server.BusinessLogic;
using System.Collections.Generic;
using NHibernate.Criterion;

namespace Vre.Server.Dao
{
    public class EstateDeveloperDao : UpdateableBaseDao<EstateDeveloper>
    {
        public EstateDeveloperDao(ISession session) : base(session) { }

        public EstateDeveloper GetById(string id)
        {
            lock (_session)
            {
                int key;
                if (int.TryParse(id, out key)) return GetById(key);

                return _session.CreateCriteria<EstateDeveloper>()
                    .Add(Restrictions.Eq("Name", id))
                    .Add(Restrictions.Eq("Deleted", false))
                    .UniqueResult<EstateDeveloper>();
            }
        }
    }
}