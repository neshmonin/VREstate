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

        public static void ConvertUrlsToAbsolute(Building target)
        {
            string url = target.DisplayModelUrl;
            if (!string.IsNullOrEmpty(url)) target.DisplayModelUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.OverlayModelUrl;
            if (!string.IsNullOrEmpty(url)) target.OverlayModelUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.BubbleTemplateUrl;
            if (!string.IsNullOrEmpty(url)) target.BubbleTemplateUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.PoiModelUrl;
            if (!string.IsNullOrEmpty(url)) target.PoiModelUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
        }

        public static void ConvertUrlsToAbsolute(Site target)
        {
            string url = target.DisplayModelUrl;
            if (!string.IsNullOrEmpty(url)) target.DisplayModelUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.OverlayModelUrl;
            if (!string.IsNullOrEmpty(url)) target.OverlayModelUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.BubbleTemplateUrl;
            if (!string.IsNullOrEmpty(url)) target.BubbleTemplateUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
        }

        private const string _ffChromeSpeedTestAltRoot = "https://static.3dcondox.com/vre/";
        private static string ffChromeSpeedTestPathConvert(string relativePath)
        {
            if (!relativePath.StartsWith(_ffChromeSpeedTestAltRoot))
                return _ffChromeSpeedTestAltRoot + relativePath;
            else
                return relativePath;
        }
    }
}
