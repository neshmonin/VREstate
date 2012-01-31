using System;
using System.Collections.Generic;
using System.IO;
using NHibernate;
using NHibernate.Cfg;
using System.Text;
using Vre.Server.RemoteService;

namespace Vre.Server
{
    public interface INonNestedTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }

    public class NHibernateHelper
    {
        private const string NHibernateConfigurationFileName = "nhibernate.config";

        public static readonly DateTime DateTimeMinValue = new DateTime(1753, 1, 1);
        public static readonly DateTime DateTimeMaxValue = new DateTime(9999, 12, 31);

        private static readonly object _lock = new object();
        private static ISessionFactory _sessionFactory;
        private static System.Data.IsolationLevel _transactionIsolationLevel = System.Data.IsolationLevel.Unspecified;

        private static string connectionString
        {
            get
            {
                return ServiceInstances.Configuration.GetValue("connectionString", string.Empty);
            }
        }

        private static System.Data.IsolationLevel transactionIsolationLevel
        {
            get
            {
                if (System.Data.IsolationLevel.Unspecified == _transactionIsolationLevel)
                {
                    string til = ServiceInstances.Configuration.GetValue("DbTransactionIsolationLevel", "serializable").ToLower();
                    if (til.Equals("chaos")) _transactionIsolationLevel = System.Data.IsolationLevel.Chaos;
                    else if (til.Equals("readcommitted")) _transactionIsolationLevel = System.Data.IsolationLevel.ReadCommitted;
                    else if (til.Equals("readuncommitted")) _transactionIsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
                    else if (til.Equals("repeatableread")) _transactionIsolationLevel = System.Data.IsolationLevel.RepeatableRead;
                    else if (til.Equals("snapshot")) _transactionIsolationLevel = System.Data.IsolationLevel.Snapshot;
                    else _transactionIsolationLevel = System.Data.IsolationLevel.Serializable;

                    ServiceInstances.Logger.Info("Database transaction isolation level is {0}.", _transactionIsolationLevel);
                }
                return _transactionIsolationLevel;
            }
        }

        public static string DisplayableConnectionString
        {
            get
            {
                bool start = true;
                StringBuilder result = new StringBuilder();
                foreach (string p in connectionString.Split(';'))
                {
                    if (p.StartsWith("user", StringComparison.InvariantCultureIgnoreCase) ||
                        p.StartsWith("uid", StringComparison.InvariantCultureIgnoreCase) ||
                        p.StartsWith("password", StringComparison.InvariantCultureIgnoreCase) ||
                        p.StartsWith("pwd", StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (!start) result.Append(";");
                    result.Append(p);
                    start = false;
                }
                return result.ToString();
            }
        }

        private static ISessionFactory factory
        {
            get
            {
                lock (_lock)
                {
                    if (_sessionFactory == null) 
                    {
                        Configuration nHconfiguration = new Configuration();
                        string nHibernateConfigPath = Path.Combine(
                            Path.GetDirectoryName(ServiceInstances.Configuration.FilePath), 
                            NHibernateConfigurationFileName);
                        if (System.IO.File.Exists(nHibernateConfigPath) == false)
                        {
                            throw new ApplicationException(
                                string.Format("NHibernate configuration file {0} does not exist.",
                                nHibernateConfigPath));
                        }

                        nHconfiguration.Configure(nHibernateConfigPath);
                        nHconfiguration.SetProperty("connection.connection_string", connectionString);

                        nHconfiguration.AddAssembly(typeof(DatabaseSettings).Assembly);
                        //nHconfiguration.AddAssembly(typeof(Vre.Server.BusinessLogic.User).Assembly);
                        // TODO: Add more asssemblies here as required.

                        _sessionFactory = nHconfiguration.BuildSessionFactory();
                    }
                }
                return _sessionFactory;
            }
        }

        public static ISession GetSession()
        {
            return factory.OpenSession();
        }

        public static DateTime DateTimeToDb(DateTime value)
        {
            if (value < DateTimeMinValue) value = DateTimeMinValue;
            else if (value > DateTimeMaxValue) value = DateTimeMaxValue;
            return value;
        }

        public static DateTime DateTimeFromDb(DateTime value)
        {
            if (value.Equals(DateTimeMinValue)) value = DateTime.MinValue;
            else if (value.Equals(DateTimeMaxValue)) value = DateTime.MaxValue;
            return value;
        }

        /// <summary>
        /// Standard T[] List&lt;T&gt;.ToArray() method against IList interface.
        /// </summary>
        public static T[] IListToArray<T>(IList<T> list)
        {
            if (null == list) return null;
            T[] result = new T[list.Count];
            list.CopyTo(result, 0);
            return result;
        }

        public static INonNestedTransaction OpenNonNestedTransaction(ISession session)
        {
            ISession adHocSession = null;
            if (null == session)
            {
                session = NHibernateHelper.GetSession();
                adHocSession = session;
            }

            ITransaction tran = session.Transaction;

            if ((null == tran) || (!tran.IsActive)) 
                return new NonNestedTransaction(adHocSession, session.BeginTransaction(transactionIsolationLevel), false);
            else
                return new NonNestedTransaction(adHocSession, tran, true);
        }

        public static INonNestedTransaction OpenNonNestedTransaction(ClientSession session)
        {
            ISession adHocSession = null;
            if (session.Resume()) adHocSession = session.DbSession;

            ITransaction tran = session.DbSession.Transaction;

            if ((null == tran) || (!tran.IsActive))
                return new NonNestedTransaction(adHocSession, session.DbSession.BeginTransaction(transactionIsolationLevel), false);
            else
                return new NonNestedTransaction(adHocSession, tran, true);
        }

        private class NonNestedTransaction : INonNestedTransaction
        {
            private ClientSession _clientSession;
            private ISession _session;
            private ITransaction _tran;
            private bool _inherited, _committed;

            public NonNestedTransaction(ISession session, ITransaction tran, bool inherited)
            {
                _clientSession = null;
                _session = session;
                _tran = tran;
                _inherited = inherited;
                _committed = false;
            }

            public NonNestedTransaction(ClientSession session, ITransaction tran, bool inherited)
            {
                _clientSession = session;
                _session = null;
                _tran = tran;
                _inherited = inherited;
                _committed = false;
            }

            public void Commit()
            {
                if (!_inherited) _tran.Commit();
                _committed = true;
            }

            public void Rollback()
            {
                if (_tran.IsActive) _tran.Rollback();  // do immediate rollback 
                //if (!_inherited) _tran.Rollback();
            }

            public void Dispose()
            {
                if (!_committed && _tran.IsActive) _tran.Dispose();
                if (_session != null) _session.Dispose();
                if (_clientSession != null) _clientSession.Disconnect();
            }
        }
    }
}