using System.IO;
using Vre.Server.Dao;
using Vre.Server.RemoteService;

namespace Vre.Server.BusinessLogic
{
    internal class BrokerageManager : GenericManager
    {
        public BrokerageManager(ClientSession clientSession) : base(clientSession) { }

        public decimal Credit(int brokerageId, decimal amount)
        {
            decimal result;

            using (var tran = NHibernateHelper.OpenNonNestedTransaction(_session.DbSession))
            {
                BrokerageInfo b;

                using (var dao = new BrokerageInfoDao(_session.DbSession)) b = dao.GetById(brokerageId);
                if (null == b) throw new FileNotFoundException("Brokerage ID is not known");

                if (User.Role.SuperAdmin != _session.User.UserRole)
                    throw new PermissionException("This operation is not allowed.");
                //RolePermissionCheck.CheckUserAccess(_session, u,
                //    RolePermissionCheck.UserInfoAccessLevel.Administrative);

                b.Credit(amount);
                result = b.CreditUnits;

                _session.DbSession.Update(b);


                var ft = new FinancialTransaction(0, FinancialTransaction.AccountType.Brokerage, b.AutoID,
                    FinancialTransaction.OperationType.Credit, amount,
                    FinancialTransaction.TranSubject.DirectOperation, FinancialTransaction.TranTarget.None, -1,
                    "Credited by " + _session.User.ToString());

                _session.DbSession.Save(ft);


                tran.Commit();

                ServiceInstances.Logger.Info("Superadmin {0} credited brokerage {1} for {2} units; current brokerage's Credit Units value is {3}",
                    _session.User, b, amount, result);
            }
            return result;
        }
    }
}