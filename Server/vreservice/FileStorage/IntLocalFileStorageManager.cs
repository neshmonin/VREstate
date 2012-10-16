using System.IO;

namespace Vre.Server.FileStorage
{
    internal class IntLocalFileStorageManager : FsFileStorageManagerBase
    {
        public IntLocalFileStorageManager() : base("InternalFileStoreRoot") { }

        public override StorageType Type { get { return StorageType.Internal; } }

        public override string StoreFile(string namespaceHint, string typeHint, string extension, string idHint, Stream data)
        {
            return storeFile(namespaceHint, typeHint, extension, idHint, data);
        }

        public override string ConvertToFullPath(string relativePath)
        {
            return convertToLocalPath(relativePath);
        }

        public override void RemoveFile(string relativePath)
        {
            removeFile(relativePath);
        }
    }
}