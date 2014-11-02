using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;
using Vre.Server.FileStorage;

namespace Vre.Server.Task
{
	internal class ImportFilesIntoManagedStorage : ManagedStorageTaskBase
	{
		public override string Name { get { return "ImportFilesIntoManagedStorage"; } }

		protected override void processStorage(ISession dbSession,
			FileStorageItem.LocationType locationType,
			IFileStorageManager manager,
			string storageRoot, IEnumerable<string> namespaceHints,
			IEnumerable<string> dbFileList,
			ref StringBuilder report, ref List<string> unusedFileList)
		{
			var hashAlg = MD5.Create();

			var usedList = dbFileList.ToList();
			var cutLen = storageRoot.Length;
			using (var dao = new FileStorageItemDao(dbSession))
			{
				var added = new List<string>();
				foreach (var fn in Directory.EnumerateFiles(storageRoot, "*.*", SearchOption.AllDirectories))
				{
					var rfn = fn.Substring(cutLen).Replace(Path.DirectorySeparatorChar, '/');
					if (usedList.Contains(rfn, StringComparer.InvariantCultureIgnoreCase))
					{
						if (added.Contains(rfn, StringComparer.InvariantCultureIgnoreCase))
						{   // if we just added it in this session - update use counter
							transactOp(dbSession, (s, d) =>
							{
								var tfsi = d.Get(locationType, rfn);
								if (tfsi != null)
								{
									tfsi.IncrementUseCount();
									s.Update(tfsi);
									return true;
								}
								return false;
							});
						}
						else
						{
							byte[] hash;
							using (var fs = File.OpenRead(fn)) hash = hashAlg.ComputeHash(fs);
							var fsi = dao.Get(locationType, rfn);
							if (null == fsi)
							{
								fsi = new FileStorageItem(locationType, rfn, hash);
								dao.Create(fsi);
								report.AppendFormat("Added {0}\r\n", fn);
								added.Add(rfn);
							}
							else
							{
								if (!hash.Compare(fsi.Hash))
								{
									report.AppendFormat("ERROR: file {0} exists in managed xref and has invalid hash. Possibly manual file operation! Run 'ReconcileManagedStorage' task to resolve.\r\n", fn);
								}
							}
						}
					}
					else
					{
						if (transactOp(dbSession, (s, d) =>
						{
							var tfsi = d.Get(locationType, rfn);
							if (tfsi != null)
							{
								tfsi.MarkDeleted();
								s.Update(tfsi);
								return true;
							}
							return false;
						}))
							report.AppendFormat("Removed unused file from xref: {0}\r\n", fn);
						unusedFileList.Add(fn);
					}
				}
			}
		}
	}
}