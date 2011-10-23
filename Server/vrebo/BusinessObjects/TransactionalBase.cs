using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public class TransactionalBase
    {
        public virtual int AutoID { get; set; }
        public virtual DateTime Created { get; private set; }

        public TransactionalBase()
        {
        }

        protected void InitializeNew()
        {
            AutoID = 0;
            Created = DateTime.UtcNow;
        }
    }
}