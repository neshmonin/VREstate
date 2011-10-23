using System;
using System.Diagnostics;
using NHibernate;
using Vre.Server.Dao;

namespace Vre.Server.BusinessLogic
{
    internal class ChangeLogDao : AbstractDisposableDao
    {
        public ChangeLogDao() : base() {}
        public ChangeLogDao(ISession session) : base(session) { }

        public void AddItem(EstateDeveloper developer, User user,
            object objectChanged, string fieldName, string oldValue)
        {
            ChangeLogItem item = new ChangeLogItem(developer, user, objectChanged, fieldName, oldValue);
        }

        private void create(ChangeLogItem item)
        {
            lock (_session)
            {
                _session.Save(item);
                if (_forcedFlush) _session.Flush();
            }
        }
    }

    internal class ChangeLogItem
    {
        public enum ObjectType : byte
        {
            Account = 1,
            Price = 2,
            Transaction = 3,
            User = 4,
            Building = 5,
            ContactInfo = 6,
            Option = 7,
            Suite = 11,
            Developer = 12,
            Tour = 13
        }

        private int Id { get; set; }
        public DateTime Time { get; private set; }
        public int DeveloperId { get; private set; }
        public int UserId { get; private set; }
        public ObjectType ObjectIdSource { get; private set; }
        public int ObjectId { get; private set; }
        public string Field { get; private set; }
        public string OldValue { get; private set; }

        public ChangeLogItem() { }

        //public ChangeLogItem(SuperAdmin admin,
        //    object objectChanged, string fieldName, object oldValue)
        //    : this(null, admin, objectChanged, fieldName, oldValue.ToString()) { }

        //public ChangeLogItem(SuperAdmin admin,
        //    object objectChanged, string fieldName, string oldValue)
        //    : this(null, admin, objectChanged, fieldName, oldValue) { }

        public ChangeLogItem(EstateDeveloper developer, User user,
            object objectChanged, string fieldName, object oldValue)
            : this(developer, user, objectChanged, fieldName, oldValue.ToString()) { }

        public ChangeLogItem(EstateDeveloper developer, User user,
            object objectChanged, string fieldName, string oldValue)
        {
            Debug.Assert(user != null, "User object passed is null!");
            Debug.Assert(objectChanged != null, "'Object changed' is null!");

            Time = DateTime.UtcNow;
            UserId = user.AutoID;
            
            if (developer != null)
            {
                DeveloperId = developer.AutoID;
            }
            else
            {
                if (user.UserRole != User.Role.SuperAdmin) throw new ApplicationException("Developer object not passed and user is not a SA.");
                DeveloperId = 0;
            }

            if (objectChanged is Account) ObjectIdSource = ChangeLogItem.ObjectType.Account;
            else if (objectChanged is Price) ObjectIdSource = ChangeLogItem.ObjectType.Price;
            else if (objectChanged is Transaction) ObjectIdSource = ChangeLogItem.ObjectType.Transaction;
            else if (objectChanged is User) ObjectIdSource = ChangeLogItem.ObjectType.User;
            else if (objectChanged is Building) ObjectIdSource = ChangeLogItem.ObjectType.Building;
            else if (objectChanged is ContactInfo) ObjectIdSource = ChangeLogItem.ObjectType.ContactInfo;
            else if (objectChanged is Option) ObjectIdSource = ChangeLogItem.ObjectType.Option;
            else if (objectChanged is Suite) ObjectIdSource = ChangeLogItem.ObjectType.Suite;
            else if (objectChanged is EstateDeveloper) ObjectIdSource = ChangeLogItem.ObjectType.Developer;
            else if (objectChanged is VRTour) ObjectIdSource = ChangeLogItem.ObjectType.Tour;
            else throw new ApplicationException("'Object changed' is not known!");            

            Field = fieldName;
            OldValue = oldValue;
        }
    }
}