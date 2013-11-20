using System.Collections.Generic;
using NHibernate;
using Vre.Server.BusinessLogic;
using NHibernate.Criterion;

namespace Vre.Server.Dao
{
	internal class FileStorageItemDao : UpdateableBaseDao<FileStorageItem>
	{
		public FileStorageItemDao(ISession session) : base(session) { }

		public IList<FileStorageItem> GetAll(FileStorageItem.LocationType type)
		{
			lock (_session) return _session.CreateCriteria<FileStorageItem>()
				.Add(Restrictions.Eq("Deleted", false))
				.Add(Restrictions.Eq("Store", type))
				.List<FileStorageItem>();
		}

		public IList<FileStorageItem> Match(FileStorageItem.LocationType type, byte[] hash)
		{
			lock (_session) return _session.CreateQuery(
				"FROM Vre.Server.BusinessLogic.FileStorageItem WHERE Store=:ty AND Hash=:ha AND Deleted=0")
				.SetEnum("ty", type)
				.SetBinary("ha", hash)
				.List<FileStorageItem>();
		}

		public FileStorageItem Get(FileStorageItem.LocationType type, string relativePath)
		{
			lock (_session) return _session.CreateQuery(
				"FROM Vre.Server.BusinessLogic.FileStorageItem WHERE Store=:ty AND RelativePath=:rp AND Deleted=0")
				.SetEnum("ty", type)
				.SetString("rp", relativePath)
				.UniqueResult<FileStorageItem>();
		}
	}
}