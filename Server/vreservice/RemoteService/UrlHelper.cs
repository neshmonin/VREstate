using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vre.Server.BusinessLogic;

namespace Vre.Server.RemoteService
{
    internal class UrlHelper
    {
        public static void ConvertUrlsToAbsolute(SuiteType target)
        {
            string url = target.FloorPlanUrl;
            if (!string.IsNullOrEmpty(url)) target.FloorPlanUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
        }
    }
}
