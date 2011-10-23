using NHibernate;
using Vre.Server.BusinessLogic;

namespace Vre.Server.Dao
{
    public class RoomDao : GenericDisposableDao<Room, int>
    {
        public RoomDao(ISession session) : base(session) { }
    }
}