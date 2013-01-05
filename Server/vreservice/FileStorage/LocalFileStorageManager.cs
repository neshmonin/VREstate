using System.IO;

namespace Vre.Server.FileStorage
{
    internal class LocalFileStorageManager : FsFileStorageManagerBase
    {
        private string _accessRoot;

        public LocalFileStorageManager() : base("FileStoreRoot")
        {
            _accessRoot = ServiceInstances.Configuration.GetValue("FileStoreAccessRoot", "");

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

        public override string StoreFile(string namespaceHint, string typeHint, string extension, string idHint, Stream data)
        {
            string rp = storeFile(namespaceHint, typeHint, extension, idHint, data);

            return rp.Replace(Path.DirectorySeparatorChar, '/');
        }

        public override string ConvertToFullPath(string relativePath)
        {
            if (!relativePath.StartsWith(_accessRoot))
                return _accessRoot + relativePath;
            else
                return relativePath;
        }

        public override void RemoveFile(string relativePath)
        {
            removeFile(relativePath.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}