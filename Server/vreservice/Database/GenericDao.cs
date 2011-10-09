using System;
using NHibernate;
using System.Collections.Generic;

namespace Vre.Server.Dao
{
    public interface IGenericDao<TEntity, TId> : IDisposable
    {
        bool Exists(TId entityId);
        TEntity GetById(TId entityId);
        IList<TEntity> GetAll();
        void Create(TEntity entity);
        void Update(TEntity entity);
        void CreateOrUpdate(TEntity entity);
        void Delete(TEntity entity);
    }

    public class AbstractDisposableDao : IDisposable
    {
        /// <summary>
        /// A session opened for current DAO object. The session is exclusive and persistent while object exists (until Disposed).
        /// </summary>
        protected readonly ISession _session;
        /// <summary>
        /// Flag to control if write operations must be followed by synchronous flushing.
        /// </summary>
        protected bool _forcedFlush;

        protected readonly bool _externalSession;

        /// <summary>
        /// Default c'tor.  Uses default database connection string and no forced Flush().
        /// </summary>
        protected AbstractDisposableDao() : this(null, false) { }
        /// <summary>
        /// C'tor with forced Flush() control operations.
        /// </summary>
        /// <param name="forcedFlush">True to enforce synchronous changes flushing back to database.</param>
        protected AbstractDisposableDao(bool forcedFlush) : this(null, forcedFlush) { }
        /// <summary>
        /// C'tor with external session.  Session is NOT closed by this object.
        /// </summary>
        /// <param name="session">External session to use for all operations.</param>
        protected AbstractDisposableDao(ISession session) : this(session, false) { }
        /// <summary>
        /// C'tor with external session.  Session is NOT closed by this object.
        /// </summary>
        /// <param name="session">External session to use for all operations.</param>
        /// <param name="forcedFlush">True to enforce synchronous changes flushing back to database.</param>
        protected AbstractDisposableDao(ISession session, bool forcedFlush)
        {
            if (session != null)
            {
                _externalSession = true;
                _session = session;
            }
            else
            {
                _externalSession = false;
                _session = NHibernateHelper.GetSession();
            }

            _forcedFlush = forcedFlush;
        }

        public void Flush()
        {
            if (!_forcedFlush) _session.Flush();
        }

        public virtual void Dispose()
        {
            if (!_externalSession)
            {
                lock (_session)
                {
                    if (_session.IsOpen)
                    {
                        if (!_forcedFlush) _session.Flush();
                        _session.Close();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Generic for NHibernate CRUD operations in atomic mode (i.e. session is opened and
    /// closed for every operation). This class should
    /// not be used for those database classes that rely on NHibernate lazy loading! 
    /// Alternative is <see cref="GenericDisposableDao{TEntity, TId}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Persistable type.</typeparam>
    /// <typeparam name="TId">Persistable type Primary Key type.</typeparam>
    /// <remarks>
    /// <para>
    /// The protected <paramref name="getSession"/> method opens a new NHibernate 
    /// session on every call.
    /// When making extensions, follow the pattern below:
    /// <code>
    /// public virtual void Create(TEntity entity)
    /// {
    ///     using (ISession oSession = getSession())
    ///     {
    ///         oSession.Save(entity);
    ///     }
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    internal class GenericDao<TEntity, TId> : IGenericDao<TEntity, TId>
    {
        /// <summary>
        /// Opens a new NHibernate session.  Prefer using(){} pattern for handling session.
        /// </summary>
        protected ISession getSession()
        {
            return NHibernateHelper.GetSession();
        }

        public virtual void Dispose()
        {
        }

        #region IGenericDao<TEntity,TId> Members

        public virtual bool Exists(TId entityId)
        {
            using (ISession oSession = getSession())
            {
                return oSession.Get<TEntity>(entityId) != null;
            }
        }

        public virtual TEntity GetById(TId entityId)
        {
            using (ISession oSession = getSession())
            {
                return oSession.Get<TEntity>(entityId);
            }
        }

        public virtual IList<TEntity> GetAll()
        {
            using (ISession oSession = getSession())
            {
                return oSession.CreateCriteria(typeof(TEntity)).List<TEntity>();
            }
        }

        public virtual void Create(TEntity entity)
        {
            using (ISession oSession = getSession())
            {
                oSession.Save(entity);
                oSession.Flush();
                //oSession.Close();
            }
        }

        public virtual void Update(TEntity entity)
        {
            using (ISession oSession = getSession())
            {
                oSession.Update(entity);
                oSession.Flush();
                //oSession.Close();
            }
        }

        public virtual void CreateOrUpdate(TEntity entity)
        {
            using (ISession oSession = getSession())
            {
                oSession.SaveOrUpdate(entity);
                oSession.Flush();
                //oSession.Close();
            }
        }

        public virtual void Delete(TEntity entity)
        {
            using (ISession oSession = getSession())
            {
                oSession.Delete(entity);
                oSession.Flush();
                //oSession.Close();
            }
        }

        #endregion
    }


    /// <summary>
    /// Generic for NHibernate CRUD operations with a session held open throughout the object lifetime.
    /// Alternative is <see cref="GenericDao{TEntity, TId}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Persistable type.</typeparam>
    /// <typeparam name="TId">Persistable type Primary Key type.</typeparam>
    /// <remarks>
    /// <para>
    /// There are two possible use cases for this object:
    /// <list type="number">
    /// <item>Create object with no ISession passed. An NHibernate session is
    /// opened by object; this session remains open until the class instance is disposed. 
    /// Useful for lazy-loaded collections:
    /// <code>
    /// using (DisposableDao1 d1 = new DisposableDao1())
    /// {
    ///     // do some work with d1
    /// } // DAO and NHibernate session disposed here
    /// </code>
    /// <para>
    /// In this use case it is important to EXPLICITLY DISPOSE
    /// the class instance when the NHibernate instance is no longer needed!
    /// Otherwise, the contained NHibernate session will remain open until the
    /// class instance and the NHibernate session are garbage collected. 
    /// If left for Garbage Collector, in case of
    /// big number of calls this may result in open database 
    /// connections buildup, resulting in DB server fault.
    /// </para>
    /// </item>
    /// <item>
    /// Create object passing an open session as a parameter.
    /// In this case object disposal shall not result in session closure; this is useful for
    /// creating long transactions spanning several DAO objects usage:
    /// <code>
    /// using (ISession session = nHibernateHelper.OpenSession())
    /// {
    ///     using (ITransaction tx = session.BeginTransaction())
    ///     {
    ///         DisposableDao1 d1 = new DisposableDao1(session);
    ///         // do some work with d1
    ///         DisposableDao2 d2 = new DisposableDao2(session);
    ///         // do some work with d2
    ///         tx.Commit(); 
    ///     } // transaction disposed here (rolled back on exception)
    /// } // NHibernate session disposed here
    /// </code>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// The <paramref name="_forcedFlush"/> member may be used to
    /// enforce synchronous changes flushing back to database.  Default is off.
    /// </para>
    /// <para>
    /// The protected <paramref name="_session"/> member refers to open NHibernate session.
    /// When making extensions, follow the pattern below:
    /// <code>
    /// public virtual void Create(TEntity entity)
    /// {
    ///     lock (_session) // session is not thread-safe
    ///     {
    ///         _session.Save(entity);
    ///         if (_forcedFlush) _session.Flush();
    ///     }
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    public class GenericDisposableDao<TEntity, TId> : AbstractDisposableDao, IGenericDao<TEntity, TId> where TEntity : class
    {
        /*
        /// <summary>
        /// A session opened for current DAO object. The session is exclusive and persistent while object exists (until Disposed).
        /// </summary>
        protected ISession _session;
        /// <summary>
        /// Flag to control if write operations must be followed by synchronous flushing.
        /// </summary>
        protected bool _forcedFlush;

        private bool _externalSession;
        */
        /// <summary>
        /// Default c'tor.  Uses default database connection string and no forced Flush().
        /// </summary>
        protected GenericDisposableDao() : base(null, true) { }
        /// <summary>
        /// C'tor with forced Flush() control operations.
        /// </summary>
        /// <param name="forcedFlush">True to enforce synchronous changes flushing back to database.</param>
        protected GenericDisposableDao(bool forcedFlush) : base(null, forcedFlush) { }
        /// <summary>
        /// C'tor with external session.  Session is NOT closed by this object.
        /// </summary>
        /// <param name="session">External session to use for all operations.</param>
        protected GenericDisposableDao(ISession session) : base(session, true) { }
        /// <summary>
        /// C'tor with external session.  Session is NOT closed by this object.
        /// </summary>
        /// <param name="session">External session to use for all operations.</param>
        /// <param name="forcedFlush">True to enforce synchronous changes flushing back to database.</param>
        protected GenericDisposableDao(ISession session, bool forcedFlush) : base(session, forcedFlush) { }
        /*
        protected GenericDisposableDao(ISession session, bool forcedFlush)
        {
            if (session != null)
            {
                _externalSession = true;
                _session = session;
            }
            else
            {
                _externalSession = false;
                _session = NHibernateHelper.GetSession();
            }

            _forcedFlush = forcedFlush;
        }

        public virtual void Dispose()
        {
            if (!_externalSession)
            {
                lock (_session)
                {
                    if (_session.IsOpen) _session.Close();
                }
            }
        }
        */

        #region IGenericDao<TEntity,TId> Members

        public virtual bool Exists(TId entityId)
        {
            lock (_session)
            {
                return _session.Get<TEntity>(entityId) != null;
            }
        }

        public virtual TEntity GetById(TId entityId)
        {
            lock (_session)
            {
                return _session.Get<TEntity>(entityId);
            }
        }

        public virtual void Create(TEntity entity)
        {
            lock (_session)
            {
                _session.Save(entity);
                if (_forcedFlush) _session.Flush();
            }
        }

        public virtual void Update(TEntity entity)
        {
            lock (_session)
            {
                _session.Update(entity);
                if (_forcedFlush) _session.Flush();
            }
        }

        public virtual void CreateOrUpdate(TEntity entity)
        {
            lock (_session)
            {
                _session.SaveOrUpdate(entity);
                if (_forcedFlush) _session.Flush();
            }
        }

        public virtual void Delete(TEntity entity)
        {
            lock (_session)
            {
                _session.Delete(entity);
                if (_forcedFlush) _session.Flush();
            }
        }

        public virtual IList<TEntity> GetAll()
        {
            lock (_session) return _session.CreateCriteria<TEntity>().List<TEntity>();
        }

        #endregion
    }

    /*
    internal class GenericDao<Tid, T> : IDisposable
    {
        protected bool _externalSession;
        protected ISession _session;

        protected GenericDao()
        {
            _session = NHibernateHelper.GetSession();
            _externalSession = false;
        }

        protected GenericDao(ISession session)
        {
            _session = session;
            _externalSession = true;
        }

        public void Dispose()
        {
            if (!_externalSession && (_session != null)) 
            {
                _session.Flush();
                _session.Dispose(); 
                _session = null; 
            }
        }

        public virtual T GetById(Tid id)
        {
            return _session.Get<T>(id);
        }

        public virtual IList<T> GetAll()
        {
            return _session.CreateCriteria(typeof(T)).List<T>();
        }

        public virtual void Create(T item)
        {
            _session.Save(item);
        }

        public virtual void Update(T item)
        {
            _session.Update(item);
        }

        public virtual void CreateOrUpdate(T item)
        {
            _session.SaveOrUpdate(item);
        }

        public virtual void Delete(T item)
        {
            _session.Delete(item);
        }
    }
    */
}