using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    internal class AccountDao : GenericDao<Account, int>
    {      
        public IList<Account> GetBySuiteId(int SuiteID)
        {
            IList<Account> AccoutList = null;

            using (ISession session = getSession())
            {
                NHibernate.IQuery oQuery = session.CreateQuery(
                    "SELECT AccoutList FROM Vre.Server.BusinessLogic.Account AccoutList WHERE SuiteID = :SuiteID");
                oQuery = oQuery.SetInt32("SuiteID", SuiteID);
                AccoutList = oQuery.List<Account>();
                return AccoutList;
            }
        }
    }
}

