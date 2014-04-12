using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;
using System;

namespace Vre.Server.Dao
{
    internal class FinancialTransactionDao : GenericDisposableDao<FinancialTransaction, int>
    {
        public FinancialTransactionDao(ISession session) : base(session) { }

        public FinancialTransaction[] Get(FinancialTransaction.AccountType type, int accountId)
        {
            return NHibernateHelper.IListToArray<FinancialTransaction>(_session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.FinancialTransaction WHERE Account=:at AND AccountId=:aid "
                + "ORDER BY Created DESC")
                .SetEnum("at", type)
                .SetInt32("aid", accountId)
                .List<FinancialTransaction>());
        }

        public IEnumerable<FinancialTransaction> Get(FinancialTransaction.AccountType type, int accountId,
            DateTime from, DateTime to)
        {
            return _session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.FinancialTransaction WHERE Account=:at AND AccountId=:aid AND Created>=:from AND Created<:to "
                + "ORDER BY Created ASC")
                .SetEnum("at", type)
                .SetInt32("aid", accountId)
                .SetDateTime("from", from)
                .SetDateTime("to", to)
                .Enumerable<FinancialTransaction>();
        }

        public FinancialTransaction[] Get(string systemRefId)
        {
            return NHibernateHelper.IListToArray<FinancialTransaction>(_session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.FinancialTransaction WHERE SystemRefId=:rid "
                + "ORDER BY Created DESC")
                .SetString("rid", systemRefId)
                .List<FinancialTransaction>());
        }

		public FinancialTransaction[] Get(FinancialTransaction.PaymentSystemType type, string paymentRefId)
		{
			return NHibernateHelper.IListToArray<FinancialTransaction>(_session.CreateQuery(
				"FROM Vre.Server.BusinessLogic.FinancialTransaction WHERE PaymentSystem=:type AND PaymentRefId=:pid "
				+ "ORDER BY Created DESC")
				.SetEnum("type", type)
				.SetString("pid", paymentRefId)
				.List<FinancialTransaction>());
		}
	}
}