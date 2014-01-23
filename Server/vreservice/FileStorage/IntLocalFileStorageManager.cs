using System.IO;
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

		public override string ConvertToRelativePath(string fullPath)
		{
			if (fullPath.StartsWith(_storeRoot.Value))
			{
				var p = fullPath.Substring(_storeRoot.Value.Length).Replace(Path.DirectorySeparatorChar, '/');
				if ((p.Length > 0) && (p[0] == Path.DirectorySeparatorChar)) p = p.Substring(1);
				return p;
			}
			else
			{
				return null;
			}
		}
    }
}