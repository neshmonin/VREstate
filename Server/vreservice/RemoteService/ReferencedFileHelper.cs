using Vre.Server.BusinessLogic;

namespace Vre.Server.RemoteService
{
    internal class ReferencedFileHelper
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
            if (!string.IsNullOrEmpty(url)) target.OverlayModelUrl = Configuration.Debug.ConvertRelativeUrlsFFCSpeedTest.Value ?
                ffChromeSpeedTestPathConvert(url) : ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.BubbleWebTemplateUrl;
            if (Configuration.Debug.ConvertRelativeTemplateUrls.Value && !string.IsNullOrEmpty(url)) target.BubbleWebTemplateUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.BubbleKioskTemplateUrl;
			if (Configuration.Debug.ConvertRelativeTemplateUrls.Value && !string.IsNullOrEmpty(url)) target.BubbleKioskTemplateUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
			//url = target.PoiModelUrl;
			//if (!string.IsNullOrEmpty(url)) target.PoiModelUrl = Configuration.Debug.ConvertRelativeUrlsFFCSpeedTest.Value ?
			//    ffChromeSpeedTestPathConvert(url) : ServiceInstances.FileStorageManager.ConvertToFullPath(url);
        }

        public static void ConvertUrlsToAbsolute(Site target)
        {
            string url = target.DisplayModelUrl;
            if (!string.IsNullOrEmpty(url)) target.DisplayModelUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.OverlayModelUrl;
			if (!string.IsNullOrEmpty(url)) target.OverlayModelUrl = Configuration.Debug.ConvertRelativeUrlsFFCSpeedTest.Value ?
                ffChromeSpeedTestPathConvert(url) : ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.BubbleWebTemplateUrl;
			if (Configuration.Debug.ConvertRelativeTemplateUrls.Value && !string.IsNullOrEmpty(url)) target.BubbleWebTemplateUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
            url = target.BubbleKioskTemplateUrl;
			if (Configuration.Debug.ConvertRelativeTemplateUrls.Value && !string.IsNullOrEmpty(url)) target.BubbleKioskTemplateUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
			url = target.PoiModelUrl;
			if (!string.IsNullOrEmpty(url)) target.PoiModelUrl = Configuration.Debug.ConvertRelativeUrlsFFCSpeedTest.Value ?
				ffChromeSpeedTestPathConvert(url) : ServiceInstances.FileStorageManager.ConvertToFullPath(url);
		}

		public static void ConvertUrlsToAbsolute(Structure target)
		{
			string url = target.DisplayModelUrl;
			if (!string.IsNullOrEmpty(url)) target.DisplayModelUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
		}

		public static void ConvertUrlsToAbsolute(User target)
		{
			string url = target.PhotoUrl;
			if (!string.IsNullOrEmpty(url)) target.PhotoUrl = ServiceInstances.FileStorageManager.ConvertToFullPath(url);
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
