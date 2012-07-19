using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public abstract class UpdateableBase : IClientDataProvider
    {
        public virtual int AutoID { get; protected set; }
        protected virtual byte[] Version { get; set; }
        public virtual DateTime Created { get; protected set; }
        public virtual DateTime Updated { get; protected set; }
        public virtual bool Deleted { get; protected set; }

        public UpdateableBase()
        {
        }

        protected UpdateableBase(UpdateableBase copy)
        {
            AutoID = copy.AutoID;
            Version = copy.Version;
            Created = copy.Created;
            Updated = copy.Updated;
            Deleted = copy.Deleted;
        }

        public override bool Equals(object obj)
        {
            UpdateableBase other = obj as UpdateableBase;
            if (other != null) return other.AutoID == AutoID;
            else return false;
        }

        protected void InitializeNew()
        {
            AutoID = 0;
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
            result.Add("deleted", Deleted);
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