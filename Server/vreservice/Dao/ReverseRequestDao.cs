﻿using System;
using Vre.Server.BusinessLogic;
using NHibernate;
using NHibernate.Criterion;

namespace Vre.Server.Dao
{
    internal class ReverseRequestDao : GenericDisposableDao<ReverseRequest, Guid>
    {
        public ReverseRequestDao(ISession session) : base(session) { }

        public ReverseRequest GetByLoginAndType(string login, ReverseRequest.RequestType type)
        {
            lock (_session)
            {
                IQuery oQuery = _session.CreateQuery(
                    "FROM Vre.Server.BusinessLogic.ReverseRequest WHERE Login = :login AND Request = :request");
                oQuery = oQuery.SetString("login", login);
                oQuery = oQuery.SetEnum("request", type);
                return oQuery.UniqueResult<ReverseRequest>();
            }
        }

        public ReverseRequest GetBySubjectAndType(string subject, ReverseRequest.RequestType type)
        {
            lock (_session)
            {
                IQuery oQuery = _session.CreateQuery(
                    "FROM Vre.Server.BusinessLogic.ReverseRequest WHERE Subject = :subject AND Request = :request");
                oQuery = oQuery.SetString("subject", subject);
                oQuery = oQuery.SetEnum("request", type);
                return oQuery.UniqueResult<ReverseRequest>();
            }
        }

        public void DeleteByUserIdAndType(int userId, ReverseRequest.RequestType type)
        {
            lock (_session)
            {
                IQuery oQuery = _session.CreateQuery(
                    "DELETE FROM Vre.Server.BusinessLogic.ReverseRequest WHERE UserId = :id AND Request = :request");
                oQuery = oQuery.SetInt32("id", userId);
                oQuery = oQuery.SetEnum("request", type);
                oQuery.ExecuteUpdate();
            }
        }

        public void DeleteByLoginAndType(string login, ReverseRequest.RequestType type)
        {
            lock (_session)
            {
                IQuery oQuery = _session.CreateQuery(
                    "DELETE FROM Vre.Server.BusinessLogic.ReverseRequest WHERE Login = :login AND Request = :request");
                oQuery = oQuery.SetString("login", login);
                oQuery = oQuery.SetEnum("request", type);
                oQuery.ExecuteUpdate();
            }
        }

        public int DeleteStale()
        {
            lock (_session)
            {
                IQuery oQuery = _session.CreateQuery(
                    "DELETE FROM Vre.Server.BusinessLogic.ReverseRequest WHERE ExpiresOn < GETUTCDATE()");
                return oQuery.ExecuteUpdate();
            }
        }
    }
}