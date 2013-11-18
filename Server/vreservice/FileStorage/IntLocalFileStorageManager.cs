namespace Vre.Server.FileStorage
{
    internal class IntLocalFileStorageManager : FsFileStorageManagerBase
    {
        public IntLocalFileStorageManager() : base(Configuration.InternalFileStore.RootPath) { }

        public override StorageType Type { get { return StorageType.Internal; } }

		public override string ConvertToFullPath(string relativePath)
		{
			return convertToLocalPath(relativePath);
		}
    }
}