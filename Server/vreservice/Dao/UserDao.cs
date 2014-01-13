using System.Collections.Generic;
using NHibernate;
using System;
using NHibernate.Criterion;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    internal class UserDao : UpdateableBaseDao<User>
    {
        public UserDao(ISession session) : base(session) { }

        public bool HasActiveSuperAdmin()
        {
            lock (_session) return (_session.CreateCriteria<User>()
                .Add(Restrictions.Eq("UserRole", User.Role.SuperAdmin) && Restrictions.Eq("Deleted", false))
                .List<User>().Count > 0);
        }

        internal User GetAnyActiveSuperAdmin()
        {
            IList<User> result;
            lock (_session) result = _session.CreateCriteria<User>()
                .Add(Restrictions.Eq("UserRole", User.Role.SuperAdmin) && Restrictions.Eq("Deleted", false))
                .List<User>();
            if (result.Count > 0) return result[0];
            else return null;
        }

        //public User[] ListSuperAdmins(string nameLookup, bool includeDeleted)
        //{
        //    lock (_session)
        //    {
        //        ICriteria c = _session.CreateCriteria<User>().Add(Restrictions.Eq("UserRole", User.Role.SuperAdmin));

        //        if (!includeDeleted)
        //            c = c.Add(Restrictions.Eq("Deleted", false));

        //        if (!string.IsNullOrEmpty(nameLookup))
        //            c.Add(Restrictions.Like("PersonalInfo.FirstName", nameLookup, MatchMode.Anywhere) &&
        //                Restrictions.Like("PersonalInfo.LastName", nameLookup, MatchMode.Anywhere) &&
        //                Restrictions.Like("PersonalInfo.MiddleName", nameLookup, MatchMode.Anywhere));

        //        return NHibernateHelper.IListToArray<User>(c.List<User>());
        //    }
        //}

        //public User[] ListUsers(int estateDeveloperId, string nameLookup, bool includeDeleted)
        //{
        //    lock (_session)
        //    {
        //        ICriteria c = _session.CreateCriteria<User>().Add(Restrictions.Eq("EstateDeveloperID", estateDeveloperId));

        //        if (!includeDeleted)
        //            c = c.Add(Restrictions.Eq("Deleted", false));

        //        if (!string.IsNullOrEmpty(nameLookup))
        //            c = c.CreateCriteria("PersonalInfo", NHibernate.SqlCommand.JoinType.InnerJoin).
        //                Add(Restrictions.Like("FirstName", nameLookup, MatchMode.Anywhere) &&
        //                Restrictions.Like("LastName", nameLookup, MatchMode.Anywhere) &&
        //                Restrictions.Like("MiddleName", nameLookup, MatchMode.Anywhere));

        //        return NHibernateHelper.IListToArray<User>(c.List<User>());
        //    }
        //}

        public IList<User> ListUsers(User.Role role, 
			int? estateDeveloperId, int? brokerageId,
			string nameLookup, 
			bool includeDeleted)
        {
            lock (_session)
            {
                ICriteria c = _session.CreateCriteria<User>().Add(Restrictions.Eq("UserRole", role));
                if (estateDeveloperId.HasValue) c = c.Add(Restrictions.Eq("EstateDeveloperID", estateDeveloperId));
				if (brokerageId.HasValue) c = c.Add(Restrictions.Eq("BrokerInfo.AutoID", brokerageId));

                if (!includeDeleted)
                    c = c.Add(Restrictions.Eq("Deleted", false));

                if (!string.IsNullOrEmpty(nameLookup))
                    c = c.CreateAlias("PersonalInfo", "PersonalInfo").
                        Add(Restrictions.Like("PersonalInfo.FirstName", nameLookup, MatchMode.Anywhere) ||
                        Restrictions.Like("PersonalInfo.LastName", nameLookup, MatchMode.Anywhere) ||
                        Restrictions.Like("PersonalInfo.MiddleName", nameLookup, MatchMode.Anywhere));

                //return NHibernateHelper.IListToArray<User>(c.List<User>());
                return c.List<User>();
            }
        }

        //public int DeleteUsers(int estateDeveloperId)
        //{
        //    int result;
        //    lock (_session)
        //    {
        //        IQuery q = _session.CreateQuery("UPDATE Vre.Server.BusinessLogic.User u SET u.Updated = GETUTCDATE(), u.Deleted = 1 WHERE u.EstateDeveloperID = :id");
        //        q = q.SetInt32("id", estateDeveloperId);
        //        result = q.ExecuteUpdate();

        //        if (_forcedFlush) _session.Flush();
        //    }
        //    return result;
        //}

		public IList<User> GetByEmailAndRole(User.Role role, string primaryEmail)
		{
			lock (_session) return _session.CreateCriteria<User>()
				.Add(Restrictions.Eq("UserRole", role))
				.Add(Restrictions.Eq("PrimaryEmailAddress", primaryEmail))
				.Add(Restrictions.Eq("Deleted", false))
				.List<User>();
		}

		public IList<User> GetForBrokerage(BrokerageInfo brokerage, User.Role role)
		{
			lock (_session) return _session.CreateCriteria<User>()
				.Add(Restrictions.Eq("UserRole", role))
				.Add(Restrictions.Eq("BrokerInfo", brokerage))
				.Add(Restrictions.Eq("Deleted", false))
				.List<User>();
		}
	}

    //internal class BuyerDao : GenericDisposableDao<Buyer, int> 
    //{
    //    public BuyerDao() : base() { }
    //    public BuyerDao(ISession session) : base(session) { }
    //    public void Update(Buyer entity, int id)
    //    {
    //        lock (_session)
    //        {
    //            _session.
    //        }
    //    }
    //}
}