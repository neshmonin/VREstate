using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;

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

        public FinancialTransaction[] Get(string systemRefId)
        {
            return NHibernateHelper.IListToArray<FinancialTransaction>(_session.CreateQuery(
                "FROM Vre.Server.BusinessLogic.FinancialTransaction WHERE SystemRefId=:rid "
                + "ORDER BY Created DESC")
                .SetString("rid", systemRefId)
                .List<FinancialTransaction>());
        }
    }
}