using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;
using NHibernate.Criterion;

namespace Vre.Server.Dao
{
    public class OptionTypeDao : GenericDisposableDao<OptionType, int>
    {
        private static int _suiteOptionId = -1;

        public OptionTypeDao() : base() 
        {
            setupOptions();
        }

        public OptionTypeDao(ISession session) : base(session, true)
        {
            setupOptions();
        }

        /// <summary>
        /// Checks if option represents the suite itself as opposed to suite option.
        /// </summary>
        public bool IsSuiteOption(OptionType opType)
        {
            return (opType.AutoID == _suiteOptionId);
        }

        /// <summary>
        /// Returns suite option type
        /// </summary>
        public OptionType GetSuiteOption()
        {
            return GetById(_suiteOptionId);
        }

        private void setupOptions()
        {
            if (_suiteOptionId >= 0) return;

            OptionType o;
            lock (_session)
            {
                o = _session.CreateCriteria<OptionType>()
                    .Add(Restrictions.Eq("Description", OptionType.SuiteOptionDescription))
                    .UniqueResult<OptionType>();

                if (null == o)
                {
                    o = new OptionType(OptionType.SuiteOptionDescription);
                    Create(o);
                }
            }

            _suiteOptionId = o.AutoID;
        }
    }
}