using System;
using Vre.Server.BusinessLogic;

namespace Vre.Server.ManagementConsole
{
    public class PropertyPageFactory
    {
        public static IPropertyPage GetPropertyPage(Type businessObjectType)
        {
            if (businessObjectType.Equals(typeof(EstateDeveloper))) return new EstateDeveloperProps();
            else if (businessObjectType.Equals(typeof(Site))) return new SiteProps();
            else if (businessObjectType.Equals(typeof(Building))) return new BuildingProps();
            else return null;
        }
    }
}