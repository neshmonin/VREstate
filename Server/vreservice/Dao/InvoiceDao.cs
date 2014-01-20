using System;
using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    internal class InvoiceDao : GenericDisposableDao<Invoice, int>
    {
        public InvoiceDao(ISession session) : base(session) { }

        public IEnumerable<Invoice> Get(Invoice.SubjectType type, int accountId,
            DateTime from, DateTime to)
        {
            return _session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.Invoice WHERE TargetObjectType=:tt AND TargetObjectId=:tid AND Created>=:from AND Created<:to "
                + "ORDER BY Created ASC")
                .SetEnum("tt", type)
                .SetInt32("tid", accountId)
                .SetDateTime("from", from)
                .SetDateTime("to", to)
                .Enumerable<Invoice>();
        }
	}
}