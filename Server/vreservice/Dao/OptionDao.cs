using System;
using Vre.Server.BusinessLogic;
using NHibernate;

namespace Vre.Server.Dao
{
    public class OptionDao : UpdateableBaseDao<Option>
    {
        public OptionDao(ISession session) : base(session) { }
    }
}