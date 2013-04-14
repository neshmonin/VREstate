using Vre.Server.BusinessLogic;

namespace Vre.Server.RemoteService
{
    internal class UrlHelper
    {
        private static bool _initialized = false;
        private static bool _convertTemplateUrls;
        private static bool _speedTest;

        private static void initialize()
        {
            _convertTemplateUrls = ServiceInstances.Configuration.GetValue("DebugConvertRelativeTemplateUrls", true);
            _speedTest = ServiceInstances.Configuration.GetValue("DebugConvertRelativeUrlsFFCSpeedTest", false);
            _initialized = true;
        }

        public static void ConvertUrlsToAbsolute(SuiteType target)
        {
            if (!_initialized) initialize();

            string url = target.FloorPlanUrl;
            if (!string.IsNullOrEmpty(url)) target.FloorPlanUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
        }

        public static void ConvertUrlsToAbsolute(Building target)
        {
            if (!_initialized) initialize();

            string url = target.DisplayModelUrl;
            if (!string.IsNullOrEmpty(url)) target.DisplayModelUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.OverlayModelUrl;
            if (!string.IsNullOrEmpty(url)) target.OverlayModelUrl = _speedTest ?
                ffChromeSpeedTestPathConvert(url) : ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.BubbleTemplateUrl;
            if (_convertTemplateUrls && !string.IsNullOrEmpty(url)) target.BubbleTemplateUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.PoiModelUrl;
            if (!string.IsNullOrEmpty(url)) target.PoiModelUrl = _speedTest ?
                ffChromeSpeedTestPathConvert(url) : ServiceInstances.FileStorageManager.ConvertToFullPath(url);
        }

        public static void ConvertUrlsToAbsolute(Site target)
        {
            if (!_initialized) initialize();

            string url = target.DisplayModelUrl;
            if (!string.IsNullOrEmpty(url)) target.DisplayModelUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.OverlayModelUrl;
            if (!string.IsNullOrEmpty(url)) target.OverlayModelUrl = _speedTest ?
                ffChromeSpeedTestPathConvert(url) : ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.BubbleTemplateUrl;
            if (_convertTemplateUrls && !string.IsNullOrEmpty(url)) target.BubbleTemplateUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
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
