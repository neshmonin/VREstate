using NHibernate;
using Vre.Server.Command;
using Vre.Server.Dao;

namespace Vre.Server.Task
{
    internal class RemoveStaleReverseRequests : BaseTask
    {
        public override string Name { get { return "RemoveStaleReverseRequests"; } }

        public override void Execute(Parameters param)
        {
            int deleted;

            using (ISession session = NHibernateHelper.GetSession())
            {
                DatabaseSettingsDao.VerifyDatabase();

                using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(session))
                {
                    using (ReverseRequestDao dao = new ReverseRequestDao(session))
                        deleted = dao.DeleteStale();
                    tran.Commit();
                }
            }

            ServiceInstances.Logger.Info("Deleted {0} stale Reverse Requests.", deleted);
        }
    }
}