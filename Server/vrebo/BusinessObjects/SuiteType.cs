using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public partial class SuiteType : UpdateableBase
    {
        public virtual Site ConstructionSite { get; protected set; }
        public virtual string Name { get; set; }
        public virtual string Model { get; set; }
        public virtual IList<SuiteLevel> Levels { get; protected set; }

        protected SuiteType() { }
        public SuiteType(Site constructionSite, string name) : base()
        {
            InitializeNew();
            ConstructionSite = constructionSite;
            Name = name;
            Model = null;
            Levels = new List<SuiteLevel>();
        }
    }
}
