﻿using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    public abstract class UpdateableBase : UpdateableBaseGen<int> 
    {
        protected UpdateableBase() : base() { }
        protected UpdateableBase(ClientData data) : base(data)
        {
            AutoID = data.GetProperty("id", 0);
        }
        public UpdateableBase(UpdateableBase copy) : base(copy) { }
        public virtual bool UpdateFromClient(ClientData data)
        {
            if (AutoID == 0) AutoID = data.GetProperty("id", 0);
            return base.UpdateFromClient(data);
        }
    }

    public abstract class UpdateableGuidBase : UpdateableBaseGen<Guid> 
    {
		private static readonly string _defaultId = Guid.Empty.ToString();

        protected UpdateableGuidBase() : base() { }
        protected UpdateableGuidBase(ClientData data) : base(data)
        {
            AutoID = Guid.Parse(data.GetProperty("id", _defaultId));
        }
        public UpdateableGuidBase(UpdateableGuidBase copy) : base(copy) { }
        public virtual bool UpdateFromClient(ClientData data)
        {
            if (AutoID == Guid.Empty)
                AutoID = Guid.Parse(data.GetProperty("id", _defaultId));
            return base.UpdateFromClient(data);
        }
    }

    public abstract class UpdateableBaseGen<T> : IClientDataProvider where T : struct
    {
        public virtual T AutoID { get; protected set; }
        protected virtual byte[] Version { get; set; }
        public virtual DateTime Created { get; protected set; }
        public virtual DateTime Updated { get; protected set; }
        public virtual bool Deleted { get; protected set; }

        protected UpdateableBaseGen()
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

        protected UpdateableBaseGen(ClientData data)
        {
            Deleted = data.GetProperty<bool>("deleted", false);
            UpdateFromClient(data);
            Created = DateTime.MinValue;
            Updated = DateTime.MinValue;
        }

        #region IClientDataProvider members and related methods
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
            byte[] replacement = data.GetProperty("version", new byte[0], out result);
            if (replacement.Length > 0) Version = replacement;
            return false;  // possible version value rollback should not count as an object value change; if all other fields remain intact - we consider object unchanged
        }
        #endregion

        public override int GetHashCode()
        {
            return AutoID.GetHashCode();
        }
    }
}