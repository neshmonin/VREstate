using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;
using Vre.Server.FileStorage;

namespace Vre.Server.Task
{
	internal class ReconcileManagedStorage : ManagedStorageTaskBase
	{
		public override string Name { get { return "ReconcileManagedStorage"; } }

		protected override void processStorage(ISession dbSession,
			FileStorageItem.LocationType locationType,
			IFileStorageManager manager,
			string storageRoot, IEnumerable<string> namespaceHints,
			IEnumerable<string> dbFileList,
			ref StringBuilder report, ref List<string> unusedFileList)
		{
			var hashAlg = GetHasher();

			var usedList = dbFileList.ToList();
			var cutLen = storageRoot.Length;
			using (var dao = new FileStorageItemDao(dbSession))
			{
				var added = new List<string>();

				//
				// Step 1: go through all files in FS; check/fix xref records for used ones
				//
				foreach (var fn in Directory.EnumerateFiles(storageRoot, "*.*", SearchOption.AllDirectories))
				{
					var rfn = fn.Substring(cutLen).Replace(Path.DirectorySeparatorChar, '/');

					int hintLen = rfn.IndexOf('/');
					if (hintLen < 0) continue;
					var hint = rfn.Substring(0, hintLen);
					if (!namespaceHints.Contains(hint)) continue;

					if (usedList.Contains(rfn, StringComparer.InvariantCultureIgnoreCase))
					{
						if (added.Contains(rfn, StringComparer.InvariantCultureIgnoreCase)) continue;

						byte[] hash;
						using (var fs = File.OpenRead(fn)) hash = hashAlg.ComputeHash(fs);
						var fsi = dao.Get(locationType, rfn);

						if (null == fsi)
						{   // xref record is missing; add it
							fsi = new FileStorageItem(locationType, rfn, hash);
							dao.Create(fsi);
							report.AppendFormat("Added {0}\r\n", fn);
						}
						else
						{   // Check/update xref record
							if (!hash.Compare(fsi.Hash))
							{
								transactOp(dbSession, (s, d) =>
								{
									var tfsi = d.Get(locationType, rfn);
									tfsi.ReplaceHash(hash);
									s.Update(tfsi);
									return true;
								});

								report.AppendFormat("ERROR: file {0} exists in managed xref and has invalid hash; hash updated.\r\n", fn);
							}
						}
						added.Add(rfn);
					}
					else
					{   // this file is not referenced by anything in DB
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

				//
				// Step 2: go through used files, find ones missing if FS
				//
				foreach (var rfn in usedList)
				{
					if (!added.Contains(rfn))
						report.AppendFormat("ERROR: file {0} is used by DB object but is missing in FS.\r\n",
							Path.Combine(storageRoot, rfn.Replace('/', Path.DirectorySeparatorChar)));
				}

				//
				// Step 3: go through xref records; find ones referencing non-existent files
				//
				StringBuilder localReport = new StringBuilder();
				transactOp(dbSession, (s, d) =>
				{
					foreach (var fsi in d.GetAll(locationType))
					{
						var c = usedList.Count((n) => n.Equals(fsi.RelativePath, StringComparison.InvariantCultureIgnoreCase));
						if (fsi.SetUseCount(c))
						{
							s.Update(fsi);
							localReport.AppendFormat("Xref for {0} fix-up to {1}\r\n", fsi.RelativePath, c);
						}
					}
					return true;
				});
				report.Append(localReport);
			}
		}
	}
}