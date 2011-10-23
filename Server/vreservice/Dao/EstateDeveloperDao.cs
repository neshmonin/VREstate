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
    }
}