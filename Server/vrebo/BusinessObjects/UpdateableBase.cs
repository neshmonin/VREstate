using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    public abstract class UpdateableBase : UpdateableBaseGen<int> 
    {
        public UpdateableBase() : base() { }
        public UpdateableBase(UpdateableBase copy) : base(copy) { }
    }

    public abstract class UpdateableGuidBase : UpdateableBaseGen<Guid> 
    {
        public UpdateableGuidBase() : base() { }
        public UpdateableGuidBase(UpdateableGuidBase copy) : base(copy) { }
    }

    public abstract class UpdateableBaseGen<T> : IClientDataProvider
    {
        public virtual T AutoID { get; protected set; }
        protected virtual byte[] Version { get; set; }
        public virtual DateTime Created { get; protected set; }
        public virtual DateTime Updated { get; protected set; }
        public virtual bool Deleted { get; protected set; }

        public UpdateableBaseGen()
        {
        }

        protected UpdateableBaseGen(UpdateableBaseGen<T> copy)
        {
            AutoID = copy.AutoID;
            Version = copy.Version;
            Created = copy.Created;
            Updated = copy.Updated;
            Deleted = copy.Deleted;
        }

        public override bool Equals(object obj)
        {
            UpdateableBaseGen<T> other = obj as UpdateableBaseGen<T>;
            if (other != null) return other.AutoID.Equals(AutoID);
            else return false;
        }

        protected void InitializeNew()
        {
            AutoID = default(T);
            Version = null;
            Created = DateTime.UtcNow;
            Updated = Created;
            Deleted = false;
        }

        public virtual bool Validate() { return (Updated >= Created); }

        public virtual void MarkDeleted() 
        { 
            Deleted = true; 
            MarkUpdated(); 
        }

        public virtual void Undelete()
        {
            Deleted = false;
            MarkUpdated();
        }

        public virtual void MarkUpdated() 
        { 
            Updated = DateTime.UtcNow;  
        }

        #region IClientDataProvide members and related methods
        public virtual ClientData GetClientData()
        {
            ClientData result = new ClientData();
            result.Add("id", AutoID);  
            if (Deleted) result.Add("deleted", "true");
            result.Add("version", Version);
            return result;
        }

        public virtual bool UpdateFromClient(ClientData data)
        {
            bool result;
            Version = data.GetProperty("version", new byte[0], out result);
            return false;  // possible version value rollback should not count as an object value change
        }
        #endregion
    }
}