﻿using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    /// <summary>
    /// Realization specific to UpdateableBase class
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class UpdateableBaseDao<TEntity> : GenericDisposableDao<TEntity, int> where TEntity : UpdateableBase
    {
        public UpdateableBaseDao(ISession session) : base(session) { }

        public override IList<TEntity> GetAll()
        {
            lock (_session) return _session.CreateCriteria<TEntity>()
                .Add(Restrictions.Eq("Deleted", false))
                .List<TEntity>();
        }

		public IList<TEntity> GetAll(bool includeDeleted)
		{
			if (!includeDeleted)
				return GetAll();
			else
				lock (_session) return _session.CreateCriteria<TEntity>()
					.List<TEntity>();
		}

		public override TEntity GetById(int entityId)
        {
            lock (_session)
            {
                TEntity u = base.GetById(entityId);
                if ((u != null) && u.Deleted) u = null;
                return u;
            }
        }

        public TEntity GetById(int entityId, bool getDeleted)
        {
            lock (_session)
            {
                TEntity u = base.GetById(entityId);
                if ((u != null) && u.Deleted && !getDeleted) u = null;
                return u;
            }
        }

        public override void Create(TEntity entity)
        {
            lock (_session)
            {
                entity.MarkUpdated();
                base.Create(entity);
                //_session.Refresh(entity);
            }
        }

        [Obsolete("Use SafeUpdate method instead.", true)]
        public override void Update(TEntity entity) { throw new NotImplementedException(); }

        public bool SafeUpdate(TEntity entity)
        {
            if (entity.Deleted) throw new InvalidOperationException("Cannot modify deleted item.");
            entity.MarkUpdated();
            lock (_session)
            {
                try
                {
                    base.Update(entity);
                    //_session.Refresh(entity);
                    return true;
                }
                catch (StaleObjectStateException)
                {
                    _session.Refresh(entity);
                    return false;
                }
            }
        }

        [Obsolete("Use SafeCreateOrUpdate method instead.", true)]
        public override void CreateOrUpdate(TEntity entity) { throw new NotImplementedException(); }

        public bool SafeCreateOrUpdate(TEntity entity)
        {
            if (entity.Deleted) throw new InvalidOperationException("Cannot modify deleted item.");
            entity.MarkUpdated();
            lock (_session)
            {
                try
                {
                    base.CreateOrUpdate(entity);
                    //_session.Refresh(entity);
                    return true;
                }
                catch (StaleObjectStateException)
                {
                    _session.Refresh(entity);
                    return false;
                }
            }
        }

        [Obsolete("Use SafeDelete method instead.", true)]
        public override void Delete(TEntity entity) { throw new NotImplementedException(); }

        public bool SafeDelete(TEntity entity)
        {
            entity.MarkDeleted();
            lock (_session)
            {
                try
                {
                    base.Update(entity);
                    //_session.Refresh(entity);
                    return true;
                }
                catch (StaleObjectStateException)
                {
                    _session.Refresh(entity);
                    return false;
                }
            }
        }
    }
}