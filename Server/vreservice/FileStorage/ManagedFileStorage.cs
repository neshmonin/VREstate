using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.FileStorage
{
	internal class ManagedFileStorage : IFileStorageManager
	{
		private IFileStorageManager _manager;
		private FileStorageItem.LocationType _locationType;
		private HashAlgorithm _hash;

		public ManagedFileStorage(FileStorageItem.LocationType locationType, IFileStorageManager manager)
		{
			_manager = manager;
			_locationType = locationType;
			_hash = MD5.Create();
		}

		public string StoreFile(string namespaceHint, string typeHint, 
			string extension, string idHint, Stream data)
		{
			string result = null;

			var pos = data.Position;
			var newHash = _hash.ComputeHash(data);
			data.Position = pos;

			using (var session = NHibernateHelper.GetSession())
				result = storeFileInt(namespaceHint, typeHint, extension, idHint, data, newHash, session);

			return result;
		}

		public string ReplaceFile(string currentRelativePath, 
			string namespaceHint, string typeHint, string extension, string idHint, Stream data)
		{
			var pos = data.Position;
			var newHash = _hash.ComputeHash(data);
			data.Position = pos;

			using (var session = NHibernateHelper.GetSession())
			{
				bool possibleMatch = false;
				using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
				{
					bool changed = false;
					using (var dao = new FileStorageItemDao(session))
					{
						// Try checking if file already exists and hash maches
						var fsi = dao.Get(_locationType, currentRelativePath);
						if (null != fsi)
						{
							if (fsi.Hash.Compare(newHash))
							{
								possibleMatch = true;  // need to compare actual data anyway
							}
							else
							{
								removeFileInt(currentRelativePath, session);
								changed = true;
							}
						}
					}
					if (changed) tran.Commit();
				}
				if (possibleMatch)
				{
					using (var storedFile = _manager.OpenFile(currentRelativePath))
					{
						if (matchFiles(storedFile, data))
						{
							ServiceInstances.Logger.Info("Replace for managed file resulted in noop ({0}): {1}",
								_locationType, currentRelativePath);
							return currentRelativePath;
						}
					}
					removeFileInt(currentRelativePath, session);
				}
				var result = storeFileInt(namespaceHint, typeHint, extension, idHint, data, newHash, session);
				ServiceInstances.Logger.Info("Replace for managed file resulted in new file ({0}): {1}",
					_locationType, result);
				return result;
			}
		}

		public void RemoveFile(string relativePath)
		{
			using (var session = NHibernateHelper.GetSession())
			{
				removeFileInt(relativePath, session);
			}
		}

		public string ConvertToFullPath(string relativePath)
		{
			return _manager.ConvertToFullPath(relativePath);
		}

		public StorageType Type { get { return _manager.Type; } }

		public Stream OpenFile(string relativePath)
		{
			return _manager.OpenFile(relativePath);
		}

		public void Dispose()
		{
			_manager.Dispose();
			// TODO
		}

		private string storeFileInt(string namespaceHint, string typeHint, string extension, string idHint,
			Stream data, byte[] newHash, NHibernate.ISession session)
		{
			string result = null;
			var candidates = new List<FileStorageItem>();

			// gather candidates; preoccupy them all to ensure no accidental removal by parallel operation
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
			{
				using (var dao = new FileStorageItemDao(session))
				{
					foreach (var fsi in dao.Match(_locationType, newHash))
					{
						candidates.Add(fsi);
						fsi.IncrementUseCount();
						session.Update(fsi);
					}
				}
				tran.Commit();
			}

			// check full match to select proper candidate (SLOW!)
			foreach (var fsi in candidates)
			{
				using (var storedFile = _manager.OpenFile(fsi.RelativePath))
				{
					if (matchFiles(storedFile, data))
					{
						candidates.Remove(fsi);
						result = fsi.RelativePath;
						break;
					}
				}
			}

			// release unused candidates; possibly remove
			if (candidates.Count > 0)
			{
				var toRemove = new List<string>(candidates.Count);
				using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
				{
					using (var dao = new FileStorageItemDao(session))
					{
						foreach (var fsi in candidates)
						{
							if (!fsi.DecrementUseCount()) toRemove.Add(fsi.RelativePath);
							session.Update(fsi);
						}
					}
					tran.Commit();
				}
				foreach (var rp in toRemove) _manager.RemoveFile(rp);
			}

			if (null == result)
			{
				result = _manager.StoreFile(namespaceHint, typeHint, extension, idHint, data);
				var fsi = new FileStorageItem(_locationType, result, newHash);
				using (var dao = new FileStorageItemDao(session)) dao.Create(fsi);
				ServiceInstances.Logger.Info("Added managed file to store ({0}): {1}",
					_locationType, result);
			}
			else
			{
				ServiceInstances.Logger.Info("Incremented use count for managed file ({0}): {1}",
					_locationType, result);
			}

			return result;
		}

		private bool removeFileInt(string relativePath, NHibernate.ISession session)
		{
			var remove = false;
			using (var tran = NHibernateHelper.OpenNonNestedTransaction(session))
			{
				bool changed = false;
				using (var dao = new FileStorageItemDao(session))
				{
					var fsi = dao.Get(_locationType, relativePath);
					if (null == fsi)
					{
						ServiceInstances.Logger.Error("Managed file entry was not found ({0}; {1}).",
							_locationType, relativePath);
						remove = true;
					}
					else
					{
						remove = !fsi.DecrementUseCount();
						session.Update(fsi);
						changed = true;
					}
				}
				if (changed) tran.Commit();
			}
			if (remove)
			{
				_manager.RemoveFile(relativePath);
				ServiceInstances.Logger.Info("Removed managed file from store ({0}): {1}",
					_locationType, relativePath);
			}
			else
			{
				ServiceInstances.Logger.Info("Decremended use count for managed file ({0}): {1}",
					_locationType, relativePath);
			}
			return remove;
		}

		private static bool matchFiles(Stream left, Stream right)
		{
			var posLeft = left.Position;
			var posRight = right.Position;

			var len = left.Length - posLeft;
			if ((right.Length - posRight) != len) return false;

			var result = true;
			var bufferL = new byte[65536];
			var bufferR = new byte[65536];

			while (left.Position < left.Length)
			{
				var rd = left.Read(bufferL, 0, bufferL.Length);
				if (right.Read(bufferR, 0, rd) != rd) { result = false; break; }

				var idx = rd - 1;
				for (; idx >= 0; idx--)
					if (bufferR[idx] != bufferL[idx]) break;

				if (idx >= 0) { result = false; break; }
			}

			left.Position = posLeft;
			right.Position = posRight;
			return result;
		}
	}
}