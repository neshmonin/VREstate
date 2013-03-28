using System.Collections.Generic;
using NHibernate;
using NHibernate.Type;
using Vre.Server.BusinessLogic;

namespace Vre.Server
{
    internal class ObjectChangeInterceptor : EmptyInterceptor
    {
        private readonly List<UpdateableBase> _modifiedList = new List<UpdateableBase>();

        public override bool OnFlushDirty(object entity, object id, 
            object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            UpdateableBase ub = entity as UpdateableBase;
            if (ub != null)
            {
                if (!_modifiedList.Contains(ub)) _modifiedList.Add(ub);
            }
            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        public override bool OnSave(object entity, object id, 
            object[] state, string[] propertyNames, IType[] types)
        {
            UpdateableBase ub = entity as UpdateableBase;
            if (ub != null)
            {
                if (!_modifiedList.Contains(ub)) _modifiedList.Add(ub);
            }
            return base.OnSave(entity, id, state, propertyNames, types);
        }

        public override void AfterTransactionCompletion(ITransaction tx)
        {
            base.AfterTransactionCompletion(tx);

            if (_modifiedList.Count > 0)
            {
                ServiceInstances.EntityUpdateTracker.NotifyModified(_modifiedList);
                _modifiedList.Clear();
            }
        }
    }
}