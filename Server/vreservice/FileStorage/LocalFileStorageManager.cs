namespace Vre.Server.FileStorage
{
    internal class LocalFileStorageManager : FsFileStorageManagerBase
    {
        private string _accessRoot;

        public LocalFileStorageManager() : base(Configuration.PublicFileStore.RootPath)
        {
			_accessRoot = Configuration.PublicFileStore.AccessRoot.Value;

            if (string.IsNullOrWhiteSpace(_accessRoot))
            {
                ServiceInstances.Logger.Error("File Store settings (URL) not defined; access is disabled.");
                _accessRoot = null;
            }
            else
            {
                if (!_accessRoot.EndsWith("/")) _accessRoot += "/";
            }
        }

        public override StorageType Type { get { return StorageType.Public; } }

        public override string ConvertToFullPath(string relativePath)
        {
            if (!relativePath.Contains("://"))//.StartsWith(_accessRoot))
                return _accessRoot + relativePath;
            else
                return relativePath;
        }

		public override string ConvertToRelativePath(string fullPath)
		{
			if (fullPath.StartsWith(_accessRoot))
				return fullPath.Substring(_accessRoot.Length);
			else
				return null;
		}
    }
}